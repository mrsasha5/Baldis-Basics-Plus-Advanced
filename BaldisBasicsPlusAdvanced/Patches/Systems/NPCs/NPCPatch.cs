using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.Systems.NPCs
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class NPCPatch
    {

        [HarmonyPatch("SpawnNPC")]
        [HarmonyPostfix]
        private static void OnSpawnNPC(EnvironmentController __instance, ref List<NPC> ___npcs)
        {
            if (___npcs.Count - 1 >= 0)
            {
                NPC npc = ___npcs[___npcs.Count - 1];
                NPCControllerSystem controllerSystem = npc.gameObject.AddComponent<NPCControllerSystem>();
                controllerSystem.Initialize(npc, __instance);
            }
        }
    }
}
