using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using HarmonyLib;
using MTM101BaldAPI;

namespace BaldisBasicsPlusAdvanced.Patches.Systems
{
    [HarmonyPatch(typeof(NPC))]
    internal class NpcPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void InitControllerSystem(NPC __instance)
        {
            if (BaseGameManager.Instance.Ec != null)
            {
                NpcControllerSystem controllerSystem = __instance.gameObject.GetOrAddComponent<NpcControllerSystem>();
                controllerSystem.Initialize(__instance, BaseGameManager.Instance.Ec);
            }
        }
    }
    // Legacy solution
    /*[HarmonyPatch(typeof(EnvironmentController))]
    internal class NpcPatch
    {
        [HarmonyPatch("SpawnNPC")]
        [HarmonyPostfix]
        private static void OnSpawnNPC(EnvironmentController __instance, ref List<NPC> ___npcs)
        {
            if (___npcs.Count - 1 >= 0)
            {
                NPC npc = ___npcs[___npcs.Count - 1];
                NpcControllerSystem controllerSystem = npc.gameObject.AddComponent<NpcControllerSystem>();
                controllerSystem.Initialize(npc, __instance);
            }
        }
    }*/
}
