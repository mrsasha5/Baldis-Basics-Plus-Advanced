using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class ProtectionPlate : CooldownPlateBase
    {

        protected override void SetTextures()
        {
            base.SetTextures();
            SetTexturesByBaseName("adv_protection_plate");
            SetEditorSprite("adv_editor_protection_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            if (entities[0].TryGetComponent(out PlayerControllerSystem controllerSystem))
            {
                controllerSystem.CreateController(out InvulnerabilityController controller);

                controller.SetTime(20f);

                ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_protection"]);
                SetCooldown(80f);
            }
        }

    }
}
