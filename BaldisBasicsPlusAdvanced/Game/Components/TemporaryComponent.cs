using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class TemporaryComponent : MonoBehaviour
    {
        protected EnvironmentController ec;

        protected float time;

        public void Initialize(EnvironmentController ec, float time)
        {
            this.ec = ec;
            SetLivingTime(time);
        }

        public void SetLivingTime(float time)
        {
            this.time = time;
        }

        private void Update()
        {
            VirtualUpdate();
        }

        protected virtual void VirtualUpdate()
        {
            if (time == float.PositiveInfinity) return;
            time -= Time.deltaTime * ec.EnvironmentTimeScale;
            if (time < 0)
            {
                Destroy(this);
            }
        }

    }
}
