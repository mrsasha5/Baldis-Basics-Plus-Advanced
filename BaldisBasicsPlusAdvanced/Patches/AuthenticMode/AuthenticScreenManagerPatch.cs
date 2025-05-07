using BaldisBasicsPlusAdvanced.API;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.AuthenticMode
{
    [HarmonyPatch(typeof(AuthenticScreenManager))]
    internal class AuthenticScreenManagerPatch
    {

        [HarmonyPatch("UseItem")]
        [HarmonyPrefix]
        private static bool onUseItem()
        {
            if (!ApiManager.AuthenticModeExtended) return true;
            return !MapFix.MapIsOpened;
        }

    }
}
