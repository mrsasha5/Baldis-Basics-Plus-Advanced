using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.NPCs;
using BaldisBasicsPlusAdvanced.Game.Components.Player;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class InvisibilityPlate : BaseCooldownPlate
    {
        private float effectTime = 15f;

        private float cooldown = 40f;

        protected override void setTextures()
        {
            setTexturesByBaseName("adv_invisibility_plate");
            setEditorSprite("adv_editor_invisibility_plate");
        }

        protected override void virtualOnPress()
        {
            base.virtualOnPress();
            Entity entity = entities[0];
            if (entity.tag == "NPC")
            {
                NPCControllerSystem controllerSystem = entity.GetComponent<NPC>().getControllerSystem();

                if (controllerSystem.createController(out NpcInvisibilityController controller))
                {
                    controller.postInit(chalkEffectTime: 0f, beginsIn: 1f, endsIn: effectTime);
                }

                setCooldown(cooldown);
            }

            if (entity.tag == "Player")
            {
                PlayerControllerSystem controllerSystem = entity.GetComponent<PlayerManager>().getControllerSystem();

                if (controllerSystem.createController(out PlayerInvisibilityController controller))
                {
                    controller.postInit(beginsIn: 1f, endsIn: effectTime);
                }

                setCooldown(cooldown);
            }
        }

    }
}
