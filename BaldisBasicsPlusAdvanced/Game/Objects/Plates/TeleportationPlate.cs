using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class TeleportationPlate : CooldownPlateBase
    {

        protected override void SetTextures()
        {
            base.SetTextures();
            SetTexturesByBaseName("adv_teleportation_plate");
            SetEditorSprite("adv_editor_teleportation_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            entities[0].SpectacularTeleport(
                ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true)
                        .FloorWorldPosition + Vector3.up * 5f);
            SetCooldown(60f);
        }

    }
}
