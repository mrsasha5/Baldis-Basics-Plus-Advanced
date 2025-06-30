using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class InvisibilityPlate : BaseCooldownPlate
    {
        private float effectTime = 15f;

        private float cooldown = 40f;

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            //plateData.hasLight = true;
            //plateData.lightColor = new Color(0f, 0.5f, 1f); //light blue
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_invisibility_plate");
            SetEditorSprite("adv_editor_invisibility_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            Entity entity = entities[0];
            if (entity != null && entity.CompareTag("NPC") || entity.CompareTag("Player"))
            {
                BaseControllerSystem controllerSystem = entity.GetComponent<BaseControllerSystem>();

                if (controllerSystem.CreateController(out InvisibilityController controller))
                    controller.SetValuesToStart(chalkEffectTime: 0f, beginsIn: 0f, endsIn: effectTime);

                SetCooldown(cooldown);
            }
        }

    }
}
