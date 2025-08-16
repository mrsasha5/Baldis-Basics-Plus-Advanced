using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Events;
using HarmonyLib;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Patches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class EnvironmentControllerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(EnvironmentController __instance)
        {
            if (__instance.GetComponent<PlayerInteractionController>() == null) 
                __instance.gameObject.AddComponent<PlayerInteractionController>();
        }
    }
}
