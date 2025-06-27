using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems.Food
{
    public class BreadItem : BaseMultipleUsableItem, IPrefab
    {
        [SerializeField]
        private float staminaModifier;

        [SerializeField]
        private float speedModifier;

        public void InitializePrefab(int variant)
        {
            staminaModifier = 1.75f;
            speedModifier = 1.5f;
        }

        public override bool Use(PlayerManager pm)
        {
            this.pm = pm;
            float value = pm.plm.staminaMax * staminaModifier;

            if (value > pm.plm.stamina) pm.plm.stamina = value;

            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["chip_crunch"]);

            if (Random.value > 0.5f)
            {
                pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_boost"]);
                pm.GetComponent<Entity>().SetSpeedEffect(speedModifier, 10f, ObjectsStorage.ItemObjects["Bread"].itemSpriteSmall);
            }

            Destroy(gameObject);
            return ReturnOnUse();
        }
    }
}
