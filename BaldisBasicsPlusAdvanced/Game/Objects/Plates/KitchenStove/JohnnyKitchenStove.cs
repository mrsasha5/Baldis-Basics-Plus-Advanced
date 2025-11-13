using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches.Shop;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove
{
    public class JohnnyKitchenStove : KitchenStove
    {

        [SerializeField]
        protected int cookingPrice;

        private StoreRoomFunction func;

        public void Assign(StoreRoomFunction func) => this.func = func; 

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.SetUses(2);
            plateData.showsUses = true;
        }

        public override bool IsCookingAvailable()
        {
            bool allowed = currentRecipe != null;
            bool isYTPsEnough = CoreGameManager.Instance.GetPoints(0) >= cookingPrice;
            bool result = allowed && isYTPsEnough && base.IsCookingAvailable();

            if (!allowed)
            {
                audMan.PlaySingle(AssetsStorage.sounds["buzz_elv"]);
            }

            if (!isYTPsEnough)
            {
                StoreRoomPatches.PlayJohnnyUnafforable(func);
            }

            if (result)
            {
                StoreRoomPatches.PlayJohnnyBuy(func);
                CoreGameManager.Instance.AddPoints(-cookingPrice, 0, true);

            }

            return result;
        }

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            cookingPrice = 50;
            burningTime = 3f;
            coolingTime = 0f;
            colorTransitionSpeed = 2f;
        }

        protected override void OnPostActivating()
        {
            base.OnPostActivating();
            activeTime = burningTime; //Prevents from recipe's burning time overriding
        }

    }
}
