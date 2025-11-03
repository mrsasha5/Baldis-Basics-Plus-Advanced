using HarmonyLib;
using BaldisBasicsPlusAdvanced.Game.Activities;

namespace BaldisBasicsPlusAdvanced.Patches.Selfpatches
{
    [HarmonyPatch(typeof(MathMachine))]
    internal class AdvancedMathMachinePatch
    {
        [HarmonyPatch("NewProblem")]
        [HarmonyPrefix]
        private static bool GenNewProblem(MathMachine __instance)
        {
            if (__instance is AdvancedMathMachine)
            {
                ((AdvancedMathMachine)__instance).GenerateProblem();
                return false;
            }
            return true;
        }
    }
}


