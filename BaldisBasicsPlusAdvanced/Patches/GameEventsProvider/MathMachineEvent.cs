using BaldisBasicsPlusAdvanced.GameEventSystem;
using HarmonyLib;
using System;

namespace BaldisBasicsPlusAdvanced.Patches.GameEventsProvider
{
    [HarmonyPatch(typeof(MathMachine))]
    internal class MathMachineEvent
    {
        [HarmonyPatch("Completed", new Type[] { typeof(int) })]
        [HarmonyPostfix]
        private static void OnLearn(bool ___givePoints)
        {
            EventsManager.OnMathMachineLearn(___givePoints);
        }
    }
}
