using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.Patches.Shop;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.GameEventsProvider
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class BaseGameManagerPatch
    {
        private static List<IGameManagerEventsReceiver> receivers = new List<IGameManagerEventsReceiver>();

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInit(BaseGameManager __instance)
        {
            ElevatorExpelHammerPatch.OnGameManagerInit(__instance);
            /*try
            {
                onInit?.Invoke();
            } catch (Exception e)
            {
                AdvancedCore.Logging.LogError(e);
            }

            onInit = null;*/

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

        [HarmonyPatch("LoadSceneObject", new Type[] { typeof(SceneObject), typeof(bool) })]
        [HarmonyPostfix]
        private static void OnLoadSceneObject(SceneObject sceneObject, bool restarting)
        {
            LevelDataManager.LevelData.OnLoadSceneObject(sceneObject, restarting);
        }

        [HarmonyPatch("LoadNextLevel")]
        [HarmonyPostfix]
        private static void OnLoadNextLevel(BaseGameManager __instance)
        {
            LevelDataManager.LevelData.OnLoadNextLevel(__instance is PitstopGameManager);
            FixHud();
        }

        [HarmonyPatch("RestartLevel")]
        [HarmonyPrefix]
        private static void OnLevelRestart()
        {
            EmergencyOptionsMenu.SetButtonState(ButtonState.InGame);
        }

        private static void FixHud()
        {
            HudManager hud = GameObject.FindObjectOfType<HudManager>();
            hud.Darken(false); //fix
            TooltipController tooltipController = ReflectionHelper.GetValue<TooltipController>(hud, "tooltip");
            ReflectionHelper.SetValue<int>(tooltipController, "stayOnCount", 0);
            tooltipController.CloseTooltip();
        }

    }
}
