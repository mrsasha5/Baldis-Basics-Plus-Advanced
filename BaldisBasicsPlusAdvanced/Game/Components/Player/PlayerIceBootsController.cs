using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.NPCs;
using BaldisBasicsPlusAdvanced.Game.GameItems;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.PlusExtensions;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.Player
{
    public class PlayerIceBootsController : PlayerController
    {
        private MovementModifier moveMod = new MovementModifier(Vector3.zero, 1f);

        private ValueModifier staminaModifier = new ValueModifier(0f, 0f);

        private PlayerMovementStatModifier movementStatModifier;

        private IceBootsItem iceBoots;

        private float speed = 50f;

        private float acceleration = -15f;

        private float minSpeedToBreak = 15f; //to break boots or break window

        private float distanceToBreak = 2.5f; //to break boots or break window

        private Vector3 forward;

        private bool broken;

        private RaycastHit hit;

        private float pushSpeed = 50f;

        private float pushAcceleration = -40f;


        public override int MaxCount => 1;

        public override bool ToDestroy => speed <= 0 || broken;

        public override void initialize(EnvironmentController ec, PlayerManager pm, PlayerControllerSystem pc)
        {
            base.initialize(ec, pm, pc);
            movementStatModifier = pm.plm.GetModifier();

            entity = pm.GetComponent<Entity>();
            entity.ExternalActivity.moveMods.Add(moveMod);

            invincibleEffect = true; //add immunity!

            movementStatModifier.AddModifier("walkSpeed", staminaModifier);
            movementStatModifier.AddModifier("runSpeed", staminaModifier);

            forward = pm.transform.forward;
        }

        public virtual void postInitialize(IceBootsItem iceBoots)
        {
            this.iceBoots = iceBoots;
        }

        public override void virtualTriggerEnter(Collider other)
        {
            base.virtualTriggerEnter(other);

            if (other.tag == "NPC" && other.TryGetComponent(out Entity entity))
            {
                NPCControllerSystem controllerSystem = other.GetComponent<NPC>().getControllerSystem();
                controllerSystem.createController(out FrozennessController frozennessController);
                frozennessController.setTime(10f);

                entity.AddForce(new Force(forward, pushSpeed, pushAcceleration));
                AudioManager audioManager = ec.getAudMan();
                audioManager.PlaySingle(AssetsStorage.sounds["bang"]);
                ec.MakeNoise(pm.transform.position, 64); //like First Prize
            }
        }

        public override void virtualUpdate()
        {
            base.virtualUpdate();

            updateSpeed();
            moveMod.movementAddend = forward * speed;

            if (pm.hidden || ReflectionHelper.getValue<bool>(entity, "ignoreAddend"))
            {
                breakBoots();
                return;
            }

            if (speed > minSpeedToBreak && Physics.Raycast(pm.transform.position, forward, out hit, distanceToBreak, ObjectsStorage.RaycastMaskIgnorableObjects))
            {
                //ray don't ignores any colliders
                if (hit.transform.tag == "Window" && hit.transform.TryGetComponent(out Window window) && !window.IsOpen)
                {
                    window.Break(true);
                }
            }

            if (speed > minSpeedToBreak && Physics.Raycast(pm.transform.position, forward, out hit, distanceToBreak, ObjectsStorage.RaycastMaskIgnorableObjects, QueryTriggerInteraction.Ignore)
            && !hit.collider.isTrigger)
            {
                breakBoots(AssetsStorage.sounds["bang"]);
                ec.MakeNoise(pm.transform.position, 64); //like First Prize
                //return;
            }
            
        }

        private void updateSpeed()
        {
            speed += Time.deltaTime * acceleration;
            if (speed < 0) speed = 0;
        }

        public override void onDestroying()
        {
            base.onDestroying();
            breakBoots();
            GameObject.Destroy(iceBoots.gameObject);
        }

        public void breakBoots(SoundObject soundToPlay = null)
        {
            if (!broken)
            {
                movementStatModifier.RemoveModifier(staminaModifier);

                if (soundToPlay == null) soundToPlay = AssetsStorage.sounds["bal_break"];

                AudioManager audMan = ec.getAudMan();
                audMan.PlaySingle(soundToPlay);
                entity.ExternalActivity.moveMods.Remove(moveMod);

                broken = true;
            }
        }

        public override void setToDestroy()
        {
            base.setToDestroy();
            breakBoots();
        }

    }
}
