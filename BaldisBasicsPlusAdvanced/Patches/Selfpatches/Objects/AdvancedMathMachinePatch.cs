using HarmonyLib;
using BaldisBasicsPlusAdvanced.Game.Objects;

namespace BaldisBasicsPlusAdvanced.Patches.Selfpatches.Objects
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


