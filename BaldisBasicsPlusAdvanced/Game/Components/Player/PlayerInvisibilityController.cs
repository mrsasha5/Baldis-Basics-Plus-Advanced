using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.Player
{
    public class PlayerInvisibilityController : PlayerController
    {
        private bool ended;

        public override bool ToDestroy => ended;

        public void postInit(float beginsIn, float endsIn)
        {
            pm.StartCoroutine(effect(beginsIn, endsIn));
        }

        private IEnumerator effect(float beginsIn, float endsIn)
        {
            //ObjectsCreator.addChalkCloudEffect(pm, beginsIn, ec);
            float waitingTime = beginsIn;
            while (waitingTime >= 0)
            {
                waitingTime -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }

            pm.SetInvisible(true);
            //Singleton<CoreGameManager>.Instance.GetHud(0).ShowEventText(Singleton<LocalizationManager>.Instance.GetLocalizedText("InvisibilityPotion_onActivated"), 5f);
            AudioManager audMan = ec.getAudMan();
            audMan.PlaySingle(AssetsStorage.sounds["adv_disappearing"]);

            waitingTime = endsIn;
            while (waitingTime >= 0)
            {
                waitingTime -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }

            pm.SetInvisible(false);

            if (pc.getControllersCount<PlayerInvisibilityController>() < 2)
            {
                Singleton<CoreGameManager>.Instance.GetHud(0).ShowEventText(Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Invisibility_Deactivated"), 5f);
                audMan.PlaySingle(AssetsStorage.sounds["adv_appearing"]);
            }
            ended = true;
            yield break;
        }

    }
}
