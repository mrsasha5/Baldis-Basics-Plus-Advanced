using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class TeleportationPlate : BaseCooldownPlate
    {
        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            //plateData.hasLight = true;
            //plateData.lightColor = new Color(0.6f, 0.2f, 1f); //violet
        }

        protected override void SetTextures()
        {
            base.SetTextures();
            SetTexturesByBaseName("adv_teleportation_plate");
            SetEditorSprite("adv_editor_teleportation_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            entities[0]?.RandomTeleport();
            SetCooldown(60f);
        }

    }
}
