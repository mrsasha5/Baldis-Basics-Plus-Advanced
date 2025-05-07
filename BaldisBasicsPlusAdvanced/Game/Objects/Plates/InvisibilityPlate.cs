using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class InvisibilityPlate : CooldownPlateBase
    {
        private float effectTime = 15f;

        private float cooldown = 40f;

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_invisibility_plate");
            SetEditorSprite("adv_editor_invisibility_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            Entity entity = entities[0];
            if (entity.tag == "NPC" || entity.tag == "Player")
            {
                ControllerSystemBase controllerSystem = entity.GetComponent<ControllerSystemBase>();

                if (controllerSystem.CreateController(out InvisibilityController controller))
                    controller.SetValuesToStart(chalkEffectTime: 0f, beginsIn: 1f, endsIn: effectTime);

                SetCooldown(cooldown);
            }
        }

    }
}
