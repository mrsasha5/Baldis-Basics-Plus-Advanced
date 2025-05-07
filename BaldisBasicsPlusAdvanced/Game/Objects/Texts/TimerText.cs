using System;
using System.Linq;
using TMPro;
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
            SetTime(0f);
            SetText("");
        }

        public void SetTime(float time)
        {
            this.time = time;
        }

        public void UpdateTime(float time)
        {
            this.time = time;
            if (time <= 0f)
            {
                Stop();
                onTimerEnds?.Invoke();
            }
            else
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
                    onTimerEnds?.Invoke();
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
