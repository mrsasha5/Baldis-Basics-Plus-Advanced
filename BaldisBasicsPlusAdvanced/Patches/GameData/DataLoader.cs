using BaldisBasicsPlusAdvanced.SavedData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.GameData
{
    [HarmonyPatch(typeof(PlayerFileManager))]
    internal class DataLoader
    {
        [HarmonyPatch("Load")]
        [HarmonyPostfix]
        private static void OnLoad()
        {
            DataManager.Initialize();
        }
    }
}
