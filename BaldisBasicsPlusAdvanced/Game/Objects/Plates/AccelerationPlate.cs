using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.Movement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class AccelerationPlate : CooldownPlateBase
    {
        [SerializeField]
        internal float initialSpeed;

        [SerializeField]
        internal float acceleration;

        [SerializeField]
        internal float timeToUnpress;

        private float cooldown = 20f;

        //private int layerForRay = ~(LayerMask.GetMask("NPCs", "Player", "Ignore Raycast", "StandardEntities", "ClickableEntities") | 1 << 18);

        private float ignoringTime;

        public void ChooseBestRotation()
        {
            Vector3[] forwards = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            float lastDistance = float.NegativeInfinity;
            int chosen = 0;
            for (int i = 0; i < forwards.Length; i++)
            {
                Physics.Raycast(transform.position, forwards[i], out RaycastHit hit, float.PositiveInfinity, ObjectsStorage.RaycastMaskIgnorableObjects, QueryTriggerInteraction.Ignore);
                if (hit.distance > lastDistance)
                {
                    chosen = i;
                    lastDistance = hit.distance;
                }
            }
            transform.forward = forwards[chosen];
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_acceleration_plate");
            SetEditorSprite("adv_editor_acceleration_plate");
        }

        protected override void SetValues(ref PlateData plateData)
        {
            base.SetValues(ref plateData);
            plateData.showCooldown = true;

            initialSpeed = 120f;
            acceleration = -40f;
            timeToUnpress = plateData.timeToUnpress;
        }

        protected override void VirtualUpdate()
        {
            if (ignoringTime >= 0) ignoringTime -= Time.deltaTime;

            if (toPress && !pressed && ignoringTime < 0)
            {
                OnPress();
                pressed = true;
                time = timeToUnpress;
            }
            else if (pressed && (!locked || entities.Count <= 0))
            {
                time -= Time.deltaTime;
                if (time < 0)
                {
                    OnUnpress();
                    pressed = false;
                }
            }

            if (locked && cooldownTime > 0)
            {
                cooldownTime -= Time.deltaTime;

                SetVisualCooldown((int)cooldownTime + 1);

                if (cooldownTime < 0)
                {
                    OnCooldownEnded();
                    locked = false;
                    text.text = "";
                }
            }
            toPress = false;
            entities.Clear();
        }

        protected override void VirtualOnUnpress()
        {
            base.VirtualOnUnpress();
            if (entities.Count <= 0) return;
            audMan.PlaySingle(AssetsStorage.sounds["adv_boing"]);
            float time = Math.Abs(initialSpeed / acceleration);
            foreach (Entity entity in entities)
            {
                Force force = new Force(transform.forward, initialSpeed, acceleration);
                entity.AddForce(force);

                ForcedEntityBehaviour behaviour = entity.gameObject.AddComponent<ForcedEntityBehaviour>();
                behaviour.Initialize(Singleton<BaseGameManager>.Instance.Ec, time);
                behaviour.PostInit(entity, force, transform.forward, 0.75f, behaviour.DefaultSlamDistance, makesNoises: entity is PlayerEntity, time: time);
            }
            SetCooldown(cooldown);
            ignoringTime = 1f;
        }

    }
}
