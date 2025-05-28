using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class ProtectionPlate : BaseCooldownPlate
    {

        /*protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.hasLight = true;
            plateData.lightColor = Color.white;
        }*/

        protected override void SetTextures()
        {
            base.SetTextures();
            SetTexturesByBaseName("adv_protection_plate");
            SetEditorSprite("adv_editor_protection_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            if (entities[0] != null && entities[0].CompareTag("Player"))
            {
                PlayerControllerSystem controllerSystem = entities[0].GetComponent<PlayerControllerSystem>();

                controllerSystem.CreateController(out ShieldController controller);

                controller.SetTime(20f);

                ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_protection"]);
                SetCooldown(80f);
            }
        }

    }
}
