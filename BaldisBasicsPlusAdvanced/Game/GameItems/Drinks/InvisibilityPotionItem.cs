using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems.Drinks
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
