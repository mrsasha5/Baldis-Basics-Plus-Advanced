using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Texts
{
    public class TimerText : TextBase
    {
        protected float time;

        protected EnvironmentController ec;

        public Action onTimerEnd;

        public void Initialize(EnvironmentController ec)
        {
            this.ec = ec;
        }

        public void Stop()
        {
            SetTime(0f);
            SetText("");
        }

        public void SetTime(float time)
        {
            this.time = time;
        }

        private void Update()
        {
            if (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (time <= 0f)
                {
                    onTimerEnd?.Invoke();
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
