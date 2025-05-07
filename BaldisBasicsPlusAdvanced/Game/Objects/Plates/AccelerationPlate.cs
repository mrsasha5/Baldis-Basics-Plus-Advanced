using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class AccelerationPlate : BaseCooldownPlate
    {
        private float cooldown = 20f;

        private int layerForRay = ~(LayerMask.GetMask("NPCs", "Player", "Ignore Raycast", "StandardEntities", "ClickableEntities") | 1 << 18);

        private float ignoringTime;

        public void chooseBestRotation()
        {
            Vector3[] forwards = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            float lastDistance = float.NegativeInfinity;
            int chosen = 0;
            for (int i = 0; i < forwards.Length; i++)
            {
                Physics.Raycast(transform.position, forwards[i], out RaycastHit hit, float.PositiveInfinity, layerForRay, QueryTriggerInteraction.Ignore);
                if (hit.distance > lastDistance)
                {
                    chosen = i;
                    lastDistance = hit.distance;
                }
            }
            transform.forward = forwards[chosen];
        }

        protected override void setTextures()
        {
            setTexturesByBaseName("adv_acceleration_plate");
            setEditorSprite("adv_editor_acceleration_plate");
        }

        protected override void setValues(ref PlateData plateData)
        {
            base.setValues(ref plateData);
            plateData.showCooldown = true;
        }

        protected override void virtualUpdate()
        {
            if (ignoringTime >= 0) ignoringTime -= Time.deltaTime;

            if (toPress && !pressed && ignoringTime < 0)
            {
                onPress();
                pressed = true;
                time = plateData.timeToUnpress;
            }
            else if (pressed && (!locked || entities.Count <= 0))
            {
                time -= Time.deltaTime;
                if (time < 0)
                {
                    onUnpress();
                    pressed = false;
                }
            }

            if (locked && cooldownTime > 0)
            {
                cooldownTime -= Time.deltaTime;

                setVisualCooldown((int)cooldownTime + 1);

                if (cooldownTime < 0)
                {
                    onCooldownEnded();
                    locked = false;
                    text.text = "";
                }
            }
            toPress = false;
            entities.Clear();
        }

        protected override void virtualOnUnpress()
        {
            base.virtualOnUnpress();
            if (entities.Count <= 0) return;
            audMan.PlaySingle(AssetsStorage.sounds["adv_boing"]);
            float initialSpeed = 120f;
            float acceleration = -40f;
            float time = Math.Abs(initialSpeed / acceleration);
            foreach (Entity entity in entities)
            {
                Force force = new Force(transform.forward, initialSpeed, acceleration);
                entity.AddForce(force);

                ForcedEntityBehaviour behaviour = entity.gameObject.AddComponent<ForcedEntityBehaviour>();
                behaviour.initialize(Singleton<BaseGameManager>.Instance.Ec, time);
                behaviour.postInit(entity, force, transform.forward, 0.75f, 4f, time: time);
            }
            setCooldown(cooldown);
            ignoringTime = 1f;
        }

    }
}
