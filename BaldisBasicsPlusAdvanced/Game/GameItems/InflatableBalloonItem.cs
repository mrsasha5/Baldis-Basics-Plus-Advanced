using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class InflatableBalloonItem : Item
    {

        public override bool Use(PlayerManager pm)
        {
            BaseControllerSystem controller = pm.GetControllerSystem();

            if (controller.CreateController(out InflatableBalloonController balloonController))
            {
                balloonController.SetTime(20f);
                Destroy(gameObject);
                return true;
            }

            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["error_maybe"]);
            Destroy(gameObject);
            return false;
        }

    }
}
