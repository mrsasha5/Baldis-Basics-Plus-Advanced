using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.GameSpecialEvents
{
    [HarmonyPatch(typeof(PlaceholderWinManager))]
    internal class WinManagerPatch
    {

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInitialize()
        {
            if (Singleton<CoreGameManager>.Instance.currentMode != Mode.Free)
            {
                PlayerDataManager.Data.gameWins++;
                PlayerDataManager.Save();
            }
        }
    }
}
