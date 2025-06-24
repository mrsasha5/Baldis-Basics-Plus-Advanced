using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Pickups
{

#warning REMOVE IN 0.11

    public class RefreshPickup : BasePickup
    {
        public StoreRoomFunction storeRoomFunc;

        private float currentDegrees;

        public override bool RaiseAlarmDuringRobbery => false;

        protected override void OnCreationPost()
        {
            renderer.sprite = AssetsStorage.sprites["adv_arrows"];

            SetSaleState(purchasable);
            desc = "Adv_Item_Refresh_Desc";
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (nonClickableTime < 0f && currentDegrees != 0f)
            {
                currentDegrees = 0f;
                renderer.SetSpriteRotation(0f);
            } else if (nonClickableTime > 0f)
            {
                currentDegrees += Time.deltaTime * -720f;
                renderer.SetSpriteRotation(currentDegrees);
            }
        }

        public override void OnPurchasing(int spentYTPs)
        {
            //don't inherit
            SetPrice(price * 2);
            ReflectionHelper.UseRequiredMethod(storeRoomFunc, "Restock");
            nonClickableTime = 1f;
            
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(AssetsStorage.sounds["adv_refresh"]);
        }

    }
}
