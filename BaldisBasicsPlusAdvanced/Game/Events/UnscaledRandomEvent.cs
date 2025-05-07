using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class UnscaledRandomEvent : BaseRandomEvent
    {

        public override void Begin()
        {
            active = true;
            eventTimer = Timer(base.EventTime);
            StartCoroutine(eventTimer);
            Singleton<CoreGameManager>.Instance.GetHud(0).ShowEventText(Singleton<LocalizationManager>.Instance.GetLocalizedText(descriptionKey), 5f);
            /*if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
            {
                Singleton<PlayerFileManager>.Instance.Find(Singleton<PlayerFileManager>.Instance.foundEvnts, (int)eventType);
            }*/

        }

        private IEnumerator Timer(float time)
        {
            remainingTime = time;
            while (remainingTime > 0f)
            {
                remainingTime -= Time.unscaledDeltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }

            End();
        }

    }
}
