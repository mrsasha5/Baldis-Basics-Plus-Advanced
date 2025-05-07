using BaldisBasicsPlusAdvanced.GameEventSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.GameEventsProvider
{
    [HarmonyPatch(typeof(MathMachine))]
    internal class MathMachineEvent
    {
        //(int player, bool correct, Activity activity)
        [HarmonyPatch("Completed")]
        [HarmonyPostfix]
        private static void onLearn(bool ___givePoints)
        {
            EventsManager.onMathMachineLearn(___givePoints);
        }
    }
}
