using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.Player;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class InvisibilityPotionItem : Item
    {

        public override bool Use(PlayerManager pm)
        {
            bool created = pm.getControllerSystem().createController(out PlayerInvisibilityController effect);

            if (!created)
            {
                Destroy(gameObject);
                return false;
            }
            effect.postInit(1f, 20f);
            pm.plm.AddStamina(45f, false);
            AudioManager audMan = pm.ec.getAudMan();
            audMan.PlaySingle(AssetsStorage.sounds["water_slurp"]);
            Destroy(gameObject);
            return true;
        }
    }
}
