using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.CustomMusics;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.GameManager
{
    [HarmonyPatch]
    internal class MusicPatch
    {

        public static Dictionary<LevelType, List<string>> musicNames = new Dictionary<LevelType, List<string>>();

        private static bool methodsLocked;

        public static void Insert(string name, LevelType type)
        {
            if (!musicNames.ContainsKey(type)) musicNames.Add(type, new List<string>());
            musicNames[type].Add(name);
        }

        public static bool IsMusicAvailable(BaseGameManager man)
        {
            return !IntegrationManager.IsActive<CustomMusicsIntegration>() &&
                man.levelObject != null && musicNames.ContainsKey(man.levelObject.type) &&
                musicNames[man.levelObject.type].Count > 0;
        }

        [HarmonyPatch(typeof(MainGameManager), "BeginPlay")]
        [HarmonyPatch(typeof(EndlessGameManager), "BeginPlay")]
        [HarmonyPrefix]
        private static void OnBeginPlay(BaseGameManager __instance)
        {
            methodsLocked = IsMusicAvailable(__instance);
        }

        [HarmonyPatch(typeof(MainGameManager), "BeginPlay")]
        [HarmonyPatch(typeof(EndlessGameManager), "BeginPlay")]
        [HarmonyPostfix]
        private static void OnBeginPlayPost(BaseGameManager __instance)
        {
            methodsLocked = false;
            if (IsMusicAvailable(__instance))
            {
                ReflectionHelper.GetValue<AudioManager>(CoreGameManager.Instance, "musicMan")
                    .FlushQueue(true);
                MusicManager.Instance.PlayMidi(
                    musicNames[__instance.levelObject.type][
                        new System.Random(Singleton<CoreGameManager>.Instance.Seed())
                        .Next(0, musicNames[__instance.levelObject.type].Count)], loop: true);
                MusicManager.Instance.SetLoop(val: true);
            }
        }

        [HarmonyPatch(typeof(MusicManager))]
        private class MusicManagerSpecialPatch
        {

            [HarmonyPatch("PlayMidi", new Type[] { typeof(string), typeof(float), typeof(bool) })]
            [HarmonyPrefix]
            private static bool OnPlayMidi()
            {
                return !methodsLocked;
            }

            [HarmonyPatch("SetLoop")]
            [HarmonyPrefix]
            private static bool OnSetLoop()
            {
                return !methodsLocked;
            }

        }


    }
}
