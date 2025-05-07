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
        private static void onSpawnNPC(EnvironmentController __instance, ref List<NPC> ___npcs)
        {
            NPC npc = ___npcs[___npcs.Count - 1];
            NPCControllerSystem controllers = npc.gameObject.AddComponent<NPCControllerSystem>();
            controllers.initialize(npc, __instance);
        }
    }
}
