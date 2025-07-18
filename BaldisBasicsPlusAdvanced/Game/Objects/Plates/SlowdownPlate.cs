﻿using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class SlowdownPlate : BaseCooldownPlate
    {
        /*protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.hasLight = true;
            plateData.lightColor = new Color(0.59f, 0.29f, 0f); //brown
        }*/

        [SerializeField]
        private Sprite gaugeIcon;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            gaugeIcon = AssetsHelper.SpriteFromFile("Textures/Gauges/adv_gauge_slowness.png");
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_slowdown_plate");
            SetEditorSprite("adv_editor_slowdown_plate");
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
            audMan.PlaySingle(AssetsStorage.sounds["adv_metal_blow"]);
            SetCooldown(60f);
        }

    }
}
