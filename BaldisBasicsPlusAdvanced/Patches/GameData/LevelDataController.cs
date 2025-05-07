using BaldisBasicsPlusAdvanced.SavedData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.GameData
{
    [HarmonyPatch(typeof(CoreGameManager))]
    internal class LevelDataController
    {
        private static bool saved;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void onStart(ref bool ___saveEnabled)
        {
            if (!___saveEnabled) DataManager.LevelDataLoaded.setDefaults();
            if (___saveEnabled) DataManager.setLastSave();
        }

        [HarmonyPatch("SaveAndQuit")]
        [HarmonyPrefix]
        private static void onSaveAndQuit(ref bool ___saveEnabled)
        {
            saved = true;
        }
        
        [HarmonyPatch("Quit")]
        [HarmonyPostfix]
        private static void onQuit(ref bool ___saveEnabled)
        {
            if (!saved && ___saveEnabled) DataManager.ResetAllData();
            saved = false;
        }

    }
}
