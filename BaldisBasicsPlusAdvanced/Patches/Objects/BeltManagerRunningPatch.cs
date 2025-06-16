using System.Collections.Generic;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.Objects
{
    [HarmonyPatch(typeof(BeltManager))]
    internal class BeltManagerRunningPatch
    {

        public static List<BeltManager> belts = new List<BeltManager>();

        [HarmonyPatch("SetRunning")]
        [HarmonyPrefix]
        private static bool OnSetRunning(BeltManager __instance)
        {
            for (int i = 0; i < belts.Count; i++)
            {
                if (belts[i] == __instance) return false;
            }
            return true;
        }
    }
}
