using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class InvisibilityPotionItem : Item
    {

        public override bool Use(PlayerManager pm)
        {
            bool created = pm.GetControllerSystem().CreateController(out InvisibilityController effect);

            if (!created)
            {
                Destroy(gameObject);
                return false;
            }
            effect.SetValuesToStart(0f, 1f, 20f);
            //effect.postInit(1f, 20f); //old class
            pm.plm.AddStamina(45f, false);

            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["water_slurp"]);
            Destroy(gameObject);
            return true;
        }
    }
}
