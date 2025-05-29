using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;

namespace BaldisBasicsPlusAdvanced.Patches.Systems.Player
{
    [HarmonyPatch(typeof(PlayerManager))]
    internal class PlayerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void OnStart(PlayerManager __instance)
        {
            PlayerControllerSystem controllerSystem = __instance.gameObject.AddComponent<PlayerControllerSystem>();
            controllerSystem.Initialize(__instance);
        }
    }
}
