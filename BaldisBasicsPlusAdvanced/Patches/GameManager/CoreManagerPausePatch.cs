using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.GameManager
{
    [HarmonyPatch(typeof(CoreGameManager))]
    internal class CoreManagerPausePatch
    {
        private static int pauseDisables;

        [HarmonyPatch("Pause")]
        [HarmonyPrefix]
        private static bool OnPause()
        {
            return pauseDisables < 1;
        }

        public static void SetPauseDisable(bool state)
        {
            if (state)
            {
                pauseDisables++;
            }
            else pauseDisables--;

            if (pauseDisables < 0) pauseDisables = 0;
        }

        [HarmonyPatch("Boom")]
        [HarmonyPostfix]
        private static void OnBoom()
        {
            pauseDisables = 0;
        }

    }
}
