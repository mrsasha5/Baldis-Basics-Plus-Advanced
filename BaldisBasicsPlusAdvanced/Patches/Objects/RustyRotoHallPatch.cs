using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects;
using HarmonyLib;
using System.Collections;

namespace BaldisBasicsPlusAdvanced.Patches.Objects
{
    [HarmonyPatch(typeof(RotoHall))]
    internal class RustyRotoHallPatch
    {

        [HarmonyPatch("Rotate")]
        [HarmonyPrefix]
        private static void OnRotate(RotoHall __instance, ref bool ___moving, ref AudioManager ___audMan, ref float ___speed,
            ref SoundObject ___audTurn)
        {
            if (___moving || !(__instance is RustyRotohall)) return;

            ___audMan.FlushQueue(endCurrent: true);
            __instance.StartCoroutine(WaitToOverrideAudMan(___audMan, ___audTurn));
        }

        private static IEnumerator WaitToOverrideAudMan(AudioManager audMan, SoundObject soundObj)
        {
            while (!audMan.QueuedAudioIsPlaying)
            {
                yield return null;
            }

            audMan.QueueAudio(AssetsStorage.sounds["adv_turning_loop"]);
            audMan.SetLoop(true);

            yield break;
        }


        [HarmonyPatch("Unblock")]
        [HarmonyPostfix]
        private static void OnUnblock(RotoHall __instance, ref AudioManager ___audMan)
        {
            if (!(__instance is RustyRotohall)) return;

            ___audMan.FlushQueue(endCurrent: true);
            ___audMan.QueueAudio(AssetsStorage.sounds["adv_turning_end"]);
        }

    }
}
