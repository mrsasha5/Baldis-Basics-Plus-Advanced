using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.PlusExtensions;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class IceBootsController : ControllerBase
    {
        private MovementModifier moveMod = new MovementModifier(Vector3.zero, 1f);

        private MovementModifier zeroMoveMod = new MovementModifier(Vector3.zero, 0f);

        private ValueModifier staminaModifier = new ValueModifier(0f, 0f);

        private PlayerMovementStatModifier movementStatModifier;

        private float speed = 60f;

        private float acceleration = -20f;

        private float minSpeedToBreak = 15f; //to break boots or break window

        private float distanceToBreak = 2.5f; //to break boots or break window

        private Vector3 forward;

        private bool broken;

        private RaycastHit hit;

        private float pushSpeed = 50f;

        private float pushAcceleration = -40f;

        public override int MaxCount => 1;

        public override bool ToDestroy => speed <= 0 || broken;

        public override void OnInitialize()
        {
            entity.ExternalActivity.moveMods.Add(moveMod);

            invincibleEffect = true; //add immunity!

            if (owner == ControllerOwner.Player)
            {
                movementStatModifier = pm.plm.GetModifier();
                movementStatModifier.AddModifier("walkSpeed", staminaModifier);
                movementStatModifier.AddModifier("runSpeed", staminaModifier);
            } else
            {
                entity.ExternalActivity.moveMods.Add(zeroMoveMod);
            }

            forward = entity.transform.forward;
        }

        public override void VirtualTriggerEnter(Collider other)
        {
            base.VirtualTriggerEnter(other);

            if (other.tag == "NPC" && other.TryGetComponent(out Entity entity))
            {
                NPC npc = other.GetComponent<NPC>();
                if (Refl_IsFreezable(npc))
                {
                    NPCControllerSystem controllerSystem = npc.GetControllerSystem();
                    controllerSystem.CreateController(out FrozennessController frozennessController);
                    frozennessController.SetTime(10f);
                }

                if (Refl_IsPushable(npc))
                {
                    entity.AddForce(new Force(forward, pushSpeed, pushAcceleration));
                    ec.GetAudMan().PlaySingle(AssetsStorage.sounds["bang"]);
                    ec.MakeNoise(entity.transform.position, 64); //like First Prize
                    Refl_OnIceBootsHit(npc, pm);
                }
            }
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();

            UpdateSpeed();
            moveMod.movementAddend = forward * speed;

            if (entity.Frozen || entity.InteractionDisabled || ReflectionHelper.GetValue<bool>(entity, "ignoreAddend"))
            {
                BreakBoots();
                return;
            }

            if (speed > minSpeedToBreak && Physics.Raycast(entity.transform.position, forward, out hit, distanceToBreak, ObjectsStorage.RaycastMaskIgnorableObjects))
            {
                //ray don't ignores any colliders
                if (hit.transform.tag == "Window" && hit.transform.TryGetComponent(out Window window) && !window.IsOpen)
                {
                    if (Refl_IsPushable(window))
                    {
                        window.Break(true);
                        Refl_OnIceBootsHit(window, pm);
                    }
                }
            }

            if (speed > minSpeedToBreak && Physics.Raycast(entity.transform.position, forward, out hit, distanceToBreak, ObjectsStorage.RaycastMaskIgnorableObjects, QueryTriggerInteraction.Ignore)
            && !hit.collider.isTrigger)
            {
                BreakBoots(AssetsStorage.sounds["bang"]);
                ec.MakeNoise(entity.transform.position, 64); //like First Prize
                //return;
            }

        }

        private void UpdateSpeed()
        {
            speed += Time.deltaTime * acceleration;
            if (speed < 0) speed = 0;
        }

        public override void OnPreDestroying()
        {
            base.OnPreDestroying();
            BreakBoots();
        }

        public void BreakBoots(SoundObject soundToPlay = null)
        {
            if (!broken)
            {
                if (owner == ControllerOwner.Player)
                {
                    movementStatModifier.RemoveModifier(staminaModifier);
                }
                else
                {
                    entity.ExternalActivity.moveMods.Remove(zeroMoveMod);
                }

                if (soundToPlay == null) soundToPlay = AssetsStorage.sounds["bal_break"];

                ec.GetAudMan().PlaySingle(soundToPlay);
                entity.ExternalActivity.moveMods.Remove(moveMod);

                broken = true;
            }
        }

        public override void SetToDestroy()
        {
            base.SetToDestroy();
            BreakBoots();
        }

        private void Refl_OnIceBootsHit(object @object, PlayerManager pm)
        {
            ReflectionHelper.UseMethod(@object, "Adv_OnIceBootsHit", pm);
        }

        private bool Refl_IsFreezable(object @object)
        {
            object isFreezable = ReflectionHelper.UseMethod(@object, "Adv_IsFreezable");
            return isFreezable == null || ((bool)isFreezable);
        }

        private bool Refl_IsPushable(object @object)
        {
            object isPushable = ReflectionHelper.UseMethod(@object, "Adv_IsPushable");
            return isPushable == null || ((bool)isPushable);
        }

    }
}
