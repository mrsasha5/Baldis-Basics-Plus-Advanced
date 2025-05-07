using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.UI;
using Rewired;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Controllers
{
    public class LevelReloader : MonoBehaviour
    {
        private float time;

        private float notifSpawningTime;

        private bool reloadStarted;

        private bool showNotification;

        private string reason;

        public void initialize(string keyReason, float timeToReload, float notifSpawnWaitTime = 1f)
        {
            reason = Singleton<LocalizationManager>.Instance.GetLocalizedText(keyReason);
            notifSpawningTime = notifSpawnWaitTime;
            time = timeToReload;
            showNotification = true;
        }

        private void Update()
        {
            if (Time.deltaTime != 0 && !reloadStarted)
            {
                if (showNotification) notifSpawningTime -= Time.deltaTime;
                if (showNotification && Singleton<BaseGameManager>.Instance != null && notifSpawningTime < 0)
                {
                    Singleton<CoreGameManager>.Instance.GetHud(0).ShowEventText(reason, 5f);
                    AudioManager audMan = Singleton<BaseGameManager>.Instance.Ec.getAudMan();
                    audMan.PlaySingle(AssetsStorage.sounds["adv_emergency"]);
                    showNotification = false;
                }
                time -= Time.deltaTime;
                if (time < 0)
                {
                    reloadStarted = true;
                    endGame();
                }
            }
        }

        private void endGame()
        {
            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;

            Time.timeScale = 0f;
            Singleton<MusicManager>.Instance.StopMidi();
            coreGameManager.disablePause = true;
            coreGameManager.GetCamera(0).matchTargetRotation = false;
            coreGameManager.audMan.volumeModifier = 0.6f;

            //coreGameManager.StartCoroutine("EndSequence");
            endSequence();
            Singleton<InputManager>.Instance.Rumble(1f, 2f);
        }

        private void endSequence()
        {
            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;

            coreGameManager.audMan.FlushQueue(endCurrent: true);
            AudioListener.pause = true;

            int extraLives = ReflectionHelper.getValue<int>(coreGameManager, "extraLives");

            if (coreGameManager.Lives < 1 && extraLives < 1)
            {
                Singleton<GlobalCam>.Instance.SetListener(val: true);
                coreGameManager.ReturnToMenu();
                return;
            }

            if (coreGameManager.Lives > 0)
            {
                ReflectionHelper.setValue<int>(coreGameManager, "lives", coreGameManager.Lives - 1);
            }
            else
            {
                ReflectionHelper.setValue<int>(coreGameManager, "extraLives", extraLives - 1);
            }

            Singleton<BaseGameManager>.Instance.RestartLevel();
        }

    }
}
