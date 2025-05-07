using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{
    [HarmonyPatch(typeof(Baldi))]
    internal class BaldiPatch
    {
        [HarmonyPatch("CaughtPlayer")]
        [HarmonyPrefix]
        private static bool OnCaughtPlayer(Baldi __instance, PlayerManager player)
        {
            if (player.GetControllerSystem().Invincibility) return false;
            return true;
        }
    }
}
