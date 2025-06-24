using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects.Interaction
{
    public class BaseInteractionObject<T> : MonoBehaviour, IClickable<int> where T : BaseInteractionObject<T>
    {

        public Action onClick;

        protected BoxCollider collider;


        public virtual T SetBoxCollider(Vector3 size)
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = size;
            return (T)this;
        }

        public virtual void Clicked(int player)
        {
            onClick?.Invoke();
        }

        public virtual bool ClickableHidden()
        {
            return false;
        }

        public virtual bool ClickableRequiresNormalHeight()
        {
            return true;
        }

        public virtual void ClickableSighted(int player)
        {

        }

        public virtual void ClickableUnsighted(int player)
        {

        }

    }
}
