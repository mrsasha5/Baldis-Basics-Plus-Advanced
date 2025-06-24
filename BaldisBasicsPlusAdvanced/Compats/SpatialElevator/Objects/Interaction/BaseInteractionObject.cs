using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects.Interaction
{
    public class BaseInteractionObject<T> : MonoBehaviour, IInteractionObject, IClickable<int> where T : BaseInteractionObject<T>
    {

        public Action onClick;

        protected bool interactionEnabled;

        protected BoxCollider collider;

        protected bool sighted;

        public virtual void Hide(bool state, bool animation)
        {

        }

        public virtual T SetBoxCollider(Vector3 size)
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = size;
            return (T)this;
        }

        public virtual void Clicked(int player)
        {
            if (!ClickableHidden()) 
                onClick?.Invoke();
        }

        public virtual bool ClickableHidden()
        {
            return interactionEnabled;
        }

        public virtual bool ClickableRequiresNormalHeight()
        {
            return true;
        }

        public virtual void ClickableSighted(int player)
        {
            sighted = true;
        }

        public virtual void ClickableUnsighted(int player)
        {
            sighted = false;
        }
    }
}
