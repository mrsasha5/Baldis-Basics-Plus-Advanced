using HarmonyLib;
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
