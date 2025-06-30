
//I never forget you
//You have been added in 0.4 by me in the 1st release as well as another items like Hammer, Wind Blower and Mysterious Teleporter...
//And you are supposed to die in 0.11
//Since now you are officially official item
//Anyways it was a good time and you was a part of main basics collection of items from the mod


/*using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems.Drinks
{
    public class InvisibilityPotionItem : Item, IPrefab
    {
        [SerializeField]
        private float beginsIn;

        [SerializeField]
        private float endsIn;

        [SerializeField]
        private float extraStamina;

        public void InitializePrefab(int variant)
        {
            beginsIn = 1f;
            endsIn = 20f;
            extraStamina = 45f;
        }

        public override bool Use(PlayerManager pm)
        {
            bool created = pm.GetControllerSystem().CreateController(out InvisibilityController effect);

            if (!created)
            {
                Destroy(gameObject);
                return false;
            }
            effect.SetValuesToStart(0f, beginsIn, endsIn);
            pm.plm.AddStamina(extraStamina, false);

            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["water_slurp"]);
            Destroy(gameObject);
            return true;
        }
    }
}
*/