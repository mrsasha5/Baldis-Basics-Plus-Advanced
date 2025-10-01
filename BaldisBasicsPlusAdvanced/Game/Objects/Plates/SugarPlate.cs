using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class SugarPlate : BasePlate
    {
        [SerializeField]
        private SoundObject audBoost;

        [SerializeField]
        private Sprite gaugeIcon;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            audBoost = AssetsStorage.sounds["adv_boost"];
            gaugeIcon = AssetsHelper.SpriteFromFile("Textures/Gauges/adv_gauge_sugar_addiction.png");
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_sugar_addiction_plate");
            SetEditorSprite("adv_editor_sugar_addiction_plate");
        }

        protected override void SetValues(PlateData data)
        {
            base.SetValues(data);
            data.MarkAsCooldownPlate();
            //plateData.hasLight = true;
            //plateData.lightColor = new Color(1f, 0.4f, 1f);
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            Entity entity = entities[0];
            if (entity != null)
            {
                if (entity.CompareTag("Player"))
                    entity.SetSpeedEffect(1.8f, 10f, gaugeIcon);
                else
                    entity.SetSpeedEffect(1.8f, 10f);
            }

            audMan.PlaySingle(audBoost);
            SetCooldown(70f);
        }

    }
}
