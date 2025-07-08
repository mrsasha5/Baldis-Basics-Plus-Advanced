using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.UI;
using PlusLevelFormat;
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

        public void Initialize(string keyReason, float timeToReload, float notifSpawnWaitTime = 1f)
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
                    //Singleton<CoreGameManager>.Instance.GetHud(0).ShowEventText(reason, 5f);
                    Singleton<BaseGameManager>.Instance.Ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_emergency"]);
                    showNotification = false;
                }
                time -= Time.deltaTime;
                if (time < 0)
                {
                    reloadStarted = true;
                    EndGame();
                }
            }
        }

        private void EndGame()
        {
            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;

            Time.timeScale = 0f;
            Singleton<MusicManager>.Instance.StopMidi();
            coreGameManager.disablePause = true;
            coreGameManager.GetCamera(0).matchTargetRotation = false;
            coreGameManager.audMan.volumeModifier = 0.6f;

            //coreGameManager.StartCoroutine("EndSequence");
            EndSequence();
            Singleton<InputManager>.Instance.Rumble(1f, 2f);
        }

        private void EndSequence()
        {
            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;

            coreGameManager.audMan.FlushQueue(endCurrent: true);
            AudioListener.pause = true;

            int extraLives = ReflectionHelper.GetValue<int>(coreGameManager, "extraLives");

            if (coreGameManager.Lives < 1 && extraLives < 1)
            {
                Singleton<GlobalCam>.Instance.SetListener(val: true);
                coreGameManager.ReturnToMenu();
                return;
            }

            if (coreGameManager.Lives > 0)
            {
                ReflectionHelper.SetValue<int>(coreGameManager, "lives", coreGameManager.Lives - 1);
            }
            else
            {
                extraLives--;
                ReflectionHelper.SetValue<int>(coreGameManager, "extraLives", extraLives);
            }

            Singleton<BaseGameManager>.Instance.RestartLevel();
        }

    }
}
