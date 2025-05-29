using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class BaseController
    {
        public EnvironmentController ec;

        public PlayerManager pm;

        public Entity entity;

        public BaseControllerSystem controllerSystem;

        public NPC npc;

        public ControllerOwner owner;

        protected float time;

        protected bool infinityLivingTime;

        protected Action onPreDestroyingEvent;

        protected Action onPostDestroyingEvent;

        protected bool invincibleEffect;

        public virtual bool Invincibility => invincibleEffect;

        public virtual int MaxCount => -1;

        public virtual bool ToDestroy => time <= 0f && !infinityLivingTime;

        public virtual ControllerType Type => ControllerType.Controller;

        public virtual bool Initializable => true;

        public virtual void OnInitialize()
        {

        }

        public void RegisterOnDestroy(Action action, bool post)
        {
            if (post) onPostDestroyingEvent += action;
            else onPreDestroyingEvent += action;
        }

        public virtual void VirtualUpdate()
        {
            if (time > 0f)
            {
                time -= Time.deltaTime * ec.NpcTimeScale;
            }
        }

        public virtual void VirtualTriggerEnter(Collider other)
        {

        }

        public virtual void VirtualTriggerStay(Collider other)
        {

        }

        public virtual void VirtualTriggerExit(Collider other)
        {

        }

        public virtual void OnPreDestroying()
        {
            onPreDestroyingEvent?.Invoke();
        }

        public virtual void OnPostDestroying()
        {
            onPostDestroyingEvent?.Invoke();
        }

        public virtual void SetToDestroy()
        {
            time = -1f;
            infinityLivingTime = false;
        }

        public virtual void SetTime(float time)
        {
            this.time = time;
        }

        public virtual void SetInfinityLivingTime()
        {
            infinityLivingTime = true;
        }
    }
}
