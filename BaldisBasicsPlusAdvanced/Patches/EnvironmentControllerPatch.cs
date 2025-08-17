using BaldisBasicsPlusAdvanced.Game.Components;
using HarmonyLib;

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
