using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class SlowdownPlate : BasePlate
    {
        /*protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.hasLight = true;
            plateData.lightColor = new Color(0.59f, 0.29f, 0f); //brown
        }*/

        [SerializeField]
        private SoundObject audHit;

        [SerializeField]
        private Sprite gaugeIcon;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            audHit = AssetStorage.sounds["adv_metal_blow"];
            gaugeIcon = AssetHelper.SpriteFromFile("Textures/Gauges/adv_gauge_slowness.png");
        }

        protected override void SetValues(PlateData data)
        {
            base.SetValues(data);
            data.MarkAsCooldownPlate();
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_slowdown_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            Entity entity = entities[0];
            if (entity != null) {
                if (entity.CompareTag("Player"))
                    entity.SetSpeedEffect(0.25f, 10f, gaugeIcon);
                else
                    entity?.SetSpeedEffect(0.25f, 10f);
            }
            audMan.PlaySingle(audHit);
            SetCooldown(60f);
        }

    }
}
