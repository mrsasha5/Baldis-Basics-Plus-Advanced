using BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects;
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

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInit(BaseGameManager __instance)
        {
            ElevatorExpelHammerPatch.OnGameManagerInit(__instance);

            //Part of compatibility
            ExpelHammerInteractionObject.OnGameManagerInit(__instance);
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
