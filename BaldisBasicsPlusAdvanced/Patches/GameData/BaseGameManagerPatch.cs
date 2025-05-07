using BaldisBasicsPlusAdvanced.SavedData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.GameData
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal partial class BaseGameManagerPatch
    {

        [HarmonyPatch("LoadNextLevel")]
        [HarmonyPostfix]
        private static void onLoadNextLevel()
        {
            DataManager.LevelDataLoaded.onLoadNextLevel();
        }

    }
}
