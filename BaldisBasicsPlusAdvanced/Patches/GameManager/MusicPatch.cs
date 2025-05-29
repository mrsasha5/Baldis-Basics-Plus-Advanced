using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.GameManager
{
    [HarmonyPatch(typeof(MainGameManager))]
    internal class MusicPatch
    {

        public static Dictionary<LevelType, List<string>> musicNames = new Dictionary<LevelType, List<string>>();

        private static bool methodsLocked;

        public static bool IsMusicAvailable(BaseGameManager man)
        {
            return man.levelObject != null && musicNames.ContainsKey(man.levelObject.type) &&
                musicNames[man.levelObject.type].Count > 0;
        }

        [HarmonyPatch("BeginPlay")]
        [HarmonyPrefix]
        private static void OnBeginPlay(MainGameManager __instance)
        {
            methodsLocked = IsMusicAvailable(__instance);
        }

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        private static void OnBeginPlayPost(MainGameManager __instance)
        {
            methodsLocked = false;
            if (IsMusicAvailable(__instance))
            {
                ReflectionHelper.GetValue<AudioManager>(Singleton<CoreGameManager>.Instance, "musicMan")
                    .FlushQueue(true);
                Singleton<MusicManager>.Instance.PlayMidi(
                    musicNames[__instance.levelObject.type][
                        new System.Random(Singleton<CoreGameManager>.Instance.Seed())
                        .Next(0, musicNames[__instance.levelObject.type].Count)], loop: true);
                Singleton<MusicManager>.Instance.SetLoop(val: true);
            }
        }

        //it was the most easy way to provide compability with Custom Musics
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

        /*[HarmonyPatch("BeginPlay")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnBeginPlay(IEnumerable<CodeInstruction> instructions)
        {
            
            return new CodeMatcher()
                .Insert(instructions.ToArray())
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "school", "school"))
                .InstructionEnumeration();
        }*/


    }
}
