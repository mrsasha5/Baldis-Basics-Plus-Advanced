using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches.Shop;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove
{
    public class JohnnyKitchenStove : KitchenStove
    {

        [SerializeField]
        protected int cookingPrice;

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
            bool result = allowed && isYTPsEnough;

            if (!allowed)
            {
                audMan.PlaySingle(AssetsStorage.sounds["buzz_elv"]);
            }

            if (!isYTPsEnough)
            {
#warning temp solution
                StoreRoomPatches.PlayJohnnyUnafforable();
            }

            if (result)
            {
                usedCount++;
                SetVisualUses(usedCount, plateData.uses);
                StoreRoomPatches.PlayJohnnyBuy();
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

        protected override void OnActivatingPre()
        {
            base.OnActivatingPre();
            textBase.gameObject.SetActive(false);
        }

        protected override void OnDeactivatingPost()
        {
            base.OnDeactivatingPost();
            if (pickups.Count == 0) textBase.gameObject.SetActive(true);
        }

        protected override void OnItemCollected(Pickup pickup, int player)
        {
            base.OnItemCollected(pickup, player);
            if (pickups.Count == 0) textBase.gameObject.SetActive(true);
        }

    }
}
