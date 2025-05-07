using BaldisBasicsPlusAdvanced.Enums;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class BaseRandomEvent : RandomEvent
    {
        protected string descriptionKey;

        protected bool cancelEventTextOnBegin;


        public override void Initialize(EnvironmentController controller, System.Random rng)
        {
            base.Initialize(controller, rng);
            prepareData();
            //if (Singleton<BaseGameManager>.Instance.levelObject != null) onFloor(FloorHelper.getFloorByLevelData(Singleton<BaseGameManager>.Instance.levelObject));
        }


        /// <summary>
        /// Calls after initialization event
        /// </summary>
        /*[Obsolete("Looks like i need to redo this!")]
        protected virtual void onFloor(Floor floor)
        {

        }*/


        public virtual void prepareData() 
        {
            descriptionKey = "unknownEventDescKey";
        }

        public override void Begin()
        {
            active = true;
            eventTimer = Timer(base.EventTime);
            StartCoroutine(eventTimer);
            if (!cancelEventTextOnBegin) setEventText(descriptionKey);
            /*if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
            {
                Singleton<PlayerFileManager>.Instance.Find(Singleton<PlayerFileManager>.Instance.foundEvnts, (int)eventType);
            }*/

        }

        protected void setEventText(string localizationKey)
        {
            Singleton<CoreGameManager>.Instance.GetHud(0).ShowEventText(Singleton<LocalizationManager>.Instance.GetLocalizedText(localizationKey), 5f);
        }

        private IEnumerator Timer(float time)
        {
            remainingTime = time;
            while (remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }

            End();
        }
    }

}
