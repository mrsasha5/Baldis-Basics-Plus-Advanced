using System;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Texts
{
    public class TimerText : BaseText
    {
        protected float time;

        protected EnvironmentController ec;

        public Action onTimerEnds;

        public void Initialize(EnvironmentController ec)
        {
            this.ec = ec;
        }

        public void Stop()
        {
            time = 0f;
            SetText("");
            onTimerEnds?.Invoke();
        }

        public void UpdateTime(float time, bool updateVisual = true)
        {
            this.time = time;
            if (time <= 0f)
            {
                Stop();
            }
            else if (updateVisual)
                SetVisualTime((int)time + 1);
        }

        public void UpdateTime()
        {
            if (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (time <= 0f)
                {
                    Stop();
                }
                else 
                    SetVisualTime((int)time + 1);
            }
        }

        private void SetVisualTime(int time)
        {
            text.text = string.Join("", time.ToString().Select(num => "<sprite=" + num + ">"));
        }

    }
}
