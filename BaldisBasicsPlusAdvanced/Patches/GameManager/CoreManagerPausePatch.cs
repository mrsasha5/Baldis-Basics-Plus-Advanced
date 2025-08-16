using System;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.GameManager
{
    [HarmonyPatch(typeof(CoreGameManager))]
    internal class CoreManagerPausePatch
    {
        private static int pauseDisables;

        public static Action onFailPress;

        [HarmonyPatch("Start")] //Game itself doesn't use Boom()...
        [HarmonyPostfix]
        private static void OnStart()
        {
            pauseDisables = 0;
            onFailPress = null;
        }

        [HarmonyPatch("Pause")]
        [HarmonyPrefix]
        private static bool OnPause(ref bool ___paused)
        {
            if (___paused) return true;

            if (pauseDisables < 1) return true;

            onFailPress?.Invoke();
            return false;
        }

        [HarmonyPatch("OpenMap")]
        [HarmonyPrefix]
        private static bool OnOpenMap(ref bool ___paused)
        {
            if (___paused) return true;

            if (pauseDisables < 1) return true;

            onFailPress?.Invoke();
            return false;
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

    }
}
