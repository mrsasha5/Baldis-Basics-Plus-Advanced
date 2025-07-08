using System;
using System.Collections.Generic;
using System.IO;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.CustomMusics;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI.AssetTools;

namespace BaldisBasicsPlusAdvanced.Patches.GameManager
{
    [HarmonyPatch(typeof(MainGameManager))]
    [HarmonyPatch(typeof(EndlessGameManager))]
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

        //it was the most easy way to provide compatibility with Custom Musics (I mean to do not break its Transpiler)
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
