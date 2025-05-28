using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using System;

namespace BaldisBasicsPlusAdvanced.Patches.GameData
{
    [HarmonyPatch(typeof(PlayerFileManager))]
    internal class DataLoader
    {
        [HarmonyPatch("Load")]
        [HarmonyPostfix]
        private static void OnLoad()
        {
            OptionsDataManager.Load();
            PlayerDataManager.Load();
        }

        [HarmonyPatch("Save", new Type[] { typeof(float) })]
        [HarmonyPostfix]
        private static void OnSave()
        {
            PlayerDataManager.Save();
        }
    }
}
