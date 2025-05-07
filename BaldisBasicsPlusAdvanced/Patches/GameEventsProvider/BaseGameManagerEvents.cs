using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.Patches.Shop;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BaldisBasicsPlusAdvanced.SavedData;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.GameEventsProvider
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class BaseGameManagerEvents
    {
        private static List<IGameManagerEventsReceiver> receivers = new List<IGameManagerEventsReceiver>();

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInit(BaseGameManager __instance)
        {
            ElevatorExpelHammerPatch.OnGameManagerInit(__instance);

            /*Stopwatch sw = new Stopwatch();
            sw.Start();

            UnityEngine.Object[] receivers = 
                Array.FindAll(GameObject.FindObjectsOfType<UnityEngine.Object>(),
                x => x is IGameManagerEventsReceiver);

            sw.Stop();
            AdvancedCore.Logging.LogWarning("Receivers found in: " + sw.ElapsedMilliseconds + "ms");*/


            List<IGameManagerEventsReceiver> _receivers = new List<IGameManagerEventsReceiver>(receivers);

            for (int i = 0; i < _receivers.Count; i++)
            {
                if (_receivers[i] != null)
                {
                    _receivers[i].OnManagerInitPost();
                } else
                {
                    receivers.RemoveAt(i);
                }
            }
        }

        public static void Register(IGameManagerEventsReceiver receiver)
        {
            receivers.Add(receiver);
        }

        public static void Unregister(IGameManagerEventsReceiver receiver)
        {
            receivers.Remove(receiver);
        }

        [HarmonyPatch("LoadNextLevel")]
        [HarmonyPostfix]
        private static void OnLoadNextLevel(BaseGameManager __instance)
        {
            LevelDataManager.LevelData.OnLoadNextLevel(__instance is PitstopGameManager);
            GameObject.FindObjectOfType<HudManager>()?.Darken(false); //fix
        }

        [HarmonyPatch("RestartLevel")]
        [HarmonyPrefix]
        private static void OnLevelRestart()
        {
            EmergencyButtonMenu.SetButtonState(ButtonState.InGame);
            GameObject.FindObjectOfType<HudManager>()?.Darken(false); //fix
        }

    }
}
