using BaldisBasicsPlusAdvanced.Patches.GameData;
using BaldisBasicsPlusAdvanced.SavedData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class CharacterBlocker
    {
        [HarmonyPatch("BeginPlay")]
        [HarmonyPrefix]
        private static void onBeginPlay(EnvironmentController __instance)
        {
            List<NPC> npcsToRemove = new List<NPC>();
            foreach (NPC npc in __instance.npcsToSpawn)
            {
                if (DataManager.LevelDataLoaded.bannedCharacters.Contains(npc.Character))
                {
                    npcsToRemove.Add(npc);
                }
            }

            foreach (NPC npc in npcsToRemove)
            {
                __instance.npcsToSpawn.Remove(npc);
            }
        }
    }
}
