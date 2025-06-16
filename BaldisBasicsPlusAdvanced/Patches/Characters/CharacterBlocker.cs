using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class CharacterBlocker
    {
        [HarmonyPatch("BeginPlay")]
        [HarmonyPrefix]
        private static void OnBeginPlay(EnvironmentController __instance)
        {
            List<Cell> spawnTiles = __instance.npcSpawnTile.ToList();
            for (int i = 0; i < __instance.npcsToSpawn.Count; i++)
            {
                if (LevelDataManager.LevelData.bannedCharacters.Contains(__instance.npcsToSpawn[i].Character))
                {
                    __instance.npcsToSpawn.Remove(__instance.npcsToSpawn[i]);
                    spawnTiles.RemoveAt(i);
                    i--;
                }
            }
            __instance.npcSpawnTile = spawnTiles.ToArray();
        }
    }
}
