using BaldisBasicsPlusAdvanced.Menu;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.UI
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal partial class BaseGameManagerPatch
    {
        [HarmonyPatch("RestartLevel")]
        [HarmonyPrefix]
        private static void onLevelRestart()
        {
            EmergencyButtonMenu.setButtonState(ButtonState.InGame);
        }
    }
}