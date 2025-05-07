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

        public void initialize(EnvironmentController ec, float time)
        {
            this.ec = ec;
            setLivingTime(time);
        }

        public void setLivingTime(float time)
        {
            this.time = time;
        }

        private void Update()
        {
            virtualUpdate();
        }

        protected virtual void virtualUpdate()
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
