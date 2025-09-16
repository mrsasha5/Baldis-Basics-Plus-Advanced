using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class SugarPlate : BaseCooldownPlate
    {

        [SerializeField]
        private Sprite gaugeIcon;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            gaugeIcon = AssetsHelper.SpriteFromFile("Textures/Gauges/adv_gauge_sugar_addiction.png");
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_sugar_addiction_plate");
            SetEditorSprite("adv_editor_sugar_addiction_plate");
        }

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            //plateData.hasLight = true;
            //plateData.lightColor = new Color(1f, 0.4f, 1f);
        }

        protected override void OnFirstTouch(Entity entity)
        {
            base.OnFirstTouch(entity);
            if (IsUsable)
            {
                if (entity != null)
                {
                    if (entity.CompareTag("Player"))
                        entity.SetSpeedEffect(1.8f, 10f, gaugeIcon);
                    else
                        entity.SetSpeedEffect(1.8f, 10f);
                }
                
                audMan.PlaySingle(AssetsStorage.sounds["adv_boost"]);
                SetCooldown(70f);
            }
        }

    }
}
