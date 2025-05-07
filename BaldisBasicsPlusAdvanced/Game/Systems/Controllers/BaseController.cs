using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class BaseController
    {
        protected EnvironmentController ec;

        protected PlayerManager pm;

        protected Entity entity;

        protected PlayerControllerSystem pc;

        protected float time;

        protected bool infinityLivingTime;

        public virtual int MaxCount => -1;

        public virtual bool ToDestroy => time < 0 && !infinityLivingTime;

        public virtual void virtualUpdate()
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
        }

        public virtual void virtualTriggerEnter(Collider other)
        {

        }

        public virtual void virtualTriggerStay(Collider other)
        {

        }

        public virtual void virtualTriggerExit(Collider other)
        {

        }

        public virtual void onDestroying()
        {

        }

        public virtual void setToDestroy()
        {
            time = -1f;
            infinityLivingTime = false;
        }

        public virtual void setTime(float time)
        {
            this.time = time;
        }

        public virtual void setInfinityLivingTime()
        {
            infinityLivingTime = true;
        }
    }
}
