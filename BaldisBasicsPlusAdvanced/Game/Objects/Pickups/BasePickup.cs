using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Pickups
{
    public class BasePickup : MonoBehaviour, IClickable<int>
    {
        protected SpriteRenderer renderer;

        protected PriceTag priceTag;

        protected int price;

        protected float nonClickableTime;

        protected bool purchasable;

        protected string desc;

        public virtual bool RaiseAlarmDuringRobbery => true; //Invokes SetOffAlarm()

        public bool Purchasable => purchasable;

        public int Price => price;

        public Action onPickupClick;

        public Action onPickupPurchasing;

        public void Initialize(SpriteRenderer renderer, PriceTag priceTag, int price)
        {
            this.renderer = renderer;
            this.priceTag = priceTag;
            this.price = price;
            purchasable = true;
            OnInitializationPost();
        }

        public virtual void OnStealing()
        {

        }

        public virtual void OnPurchasing(int spentYTPs)
        {
            SetSaleState(false);
        }

        protected virtual void OnInitializationPost()
        {

        }

        private void Update()
        {
            if (nonClickableTime > 0f) nonClickableTime -= Time.deltaTime;
            VirtualUpdate();
        }

        protected virtual void VirtualUpdate()
        {

        }

        public void Clicked(int player)
        {
            if (!ClickableHidden())
            {
                VirtualClicked(player);
                onPickupClick?.Invoke();
                if (Purchasable) onPickupPurchasing?.Invoke();
            }
        }

        protected virtual void VirtualClicked(int player)
        {

        }

        public void SetSaleState(bool active, bool markAsSoldIfNot = true)
        {
            if (active)
            {
                purchasable = true;
                if (priceTag != null) SetPrice(price);
                renderer.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                purchasable = false;
                if (priceTag != null) priceTag.SetText(Singleton<LocalizationManager>.Instance.GetLocalizedText(
                    markAsSoldIfNot ? "TAG_Sold" : "Adv_Tag_Out"));
                if (renderer != null) renderer.color = new Color(1f, 1f, 1f, 0.25f);
            }
        }

        protected void SetPrice(int price)
        {
            this.price = price;
            if (priceTag != null) priceTag.SetText(price.ToString());
        }

        public virtual bool ClickableHidden()
        {
            return nonClickableTime > 0f;
        }

        public virtual bool ClickableRequiresNormalHeight()
        {
            return false;
        }

        public virtual void ClickableSighted(int player)
        {
            Singleton<CoreGameManager>.Instance.GetHud(player).SetTooltip(desc);
        }

        public virtual void ClickableUnsighted(int player)
        {
            Singleton<CoreGameManager>.Instance.GetHud(player).CloseTooltip();
        }

    }
}
