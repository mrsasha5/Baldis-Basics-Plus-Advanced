using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.FieldTrips
{
    [HarmonyPatch(typeof(MinigameBase))]
    internal class MinigameBasePatch
    {

        [HarmonyPatch("Win")]
        [HarmonyPrefix]
        private static void OnWin(MinigameBase __instance)
        {
            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<MusicManager>.Instance.SetSpeed(1f);
            __instance.StartCoroutine(PlaySound(__instance.AudioManager, 2f, AssetsStorage.sounds["adv_mus_win"]));
        }

        [HarmonyPatch("Lose")]
        [HarmonyPrefix]
        private static void OnLose(MinigameBase __instance)
        {
            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<MusicManager>.Instance.SetSpeed(1f);
            //__instance.StartCoroutine(PlaySound(__instance.AudioManager, 1f, AssetsStorage.sounds["adv_mus_game_over"]));
        }

        private static IEnumerator PlaySound(AudioManager audMan, float beginsIn, SoundObject sound)
        {
            float time = beginsIn;
            while (time > 0f)
            {
                time -= Time.deltaTime;
                if (time <= 0f)
                {
                    audMan.PlaySingle(sound);
                }
                yield return null;
            }
            yield break;
        }


    }
}
