using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
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

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_slowdown_plate");
            SetEditorSprite("adv_editor_slowdown_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            Entity entity = entities[0];
            entity?.SetSpeedEffect(0.25f, 10f);
            audMan.PlaySingle(AssetsStorage.sounds["adv_metal_blow"]);
            SetCooldown(60f);
        }

    }
}
