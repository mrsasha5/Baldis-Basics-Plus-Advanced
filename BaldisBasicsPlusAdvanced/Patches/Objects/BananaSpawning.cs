using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Objects
{
    [HarmonyPatch(typeof(BananaTree))]
    internal class BananaTreeSpawning
    {
        [HarmonyPatch("LoadingFinished")]
        [HarmonyPrefix]
        private static bool onLoadingFinished(BananaTree __instance, ref int ___minBananas, ref int ___maxBananas, ref float ___radius,
            ref ITM_NanaPeel ___bananaPrefab)
        {
            int num = Random.Range(___minBananas, ___maxBananas + 1);
            for (int i = 0; i < num; i++)
            {
                ITM_NanaPeel iTM_NanaPeel = Object.Instantiate(___bananaPrefab);
                Vector2 insideUnitCircle = Random.insideUnitCircle;
                Vector3 vector = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
                float num2 = Random.Range(5f, ___radius);
                if (__instance.Ec.ContainsCoordinates(__instance.transform.position + vector * num2) && __instance.Ec.CellFromPosition(__instance.transform.position + vector * num2).room == __instance.Ec.CellFromPosition(__instance.transform.position).room)
                {
                    //iTM_NanaPeel.alternateSpawn(__instance.Ec, __instance.transform.position + vector * num2, vector, 0f);
                    iTM_NanaPeel.Spawn(__instance.Ec, __instance.transform.position + vector * num2, vector, 0f);
                    iTM_NanaPeel.setGrounded();
                }
                else if (__instance.Ec.ContainsCoordinates(__instance.transform.position - vector * num2) && __instance.Ec.CellFromPosition(__instance.transform.position - vector * num2).room == __instance.Ec.CellFromPosition(__instance.transform.position).room)
                {
                    //iTM_NanaPeel.alternateSpawn(__instance.Ec, __instance.transform.position + vector * num2, vector, 0f);
                    iTM_NanaPeel.Spawn(__instance.Ec, __instance.transform.position - vector * num2, vector, 0f);
                    iTM_NanaPeel.setGrounded();
                }
            }
            return false;
        }

    }

    [HarmonyPatch(typeof(NanaPeelRoomFunction))]
    internal class BananaRoomsSpawning
    {
        [HarmonyPatch("OnGenerationFinished")]
        [HarmonyPrefix]
        private static bool onGenerationFinished(NanaPeelRoomFunction __instance, int ___minBananas, int ___maxBananas, ITM_NanaPeel ___bananaPrefab)
        {
            int num = Random.Range(___minBananas, ___maxBananas + 1);
            for (int i = 0; i < num; i++)
            {
                ITM_NanaPeel banana = Object.Instantiate(___bananaPrefab);
                banana.Spawn(__instance.Room.ec, __instance.Room.ec.RealRoomRand(__instance.Room), Vector3.forward, 0f);
                banana.setGrounded();
            }
            return false;
        }

    }
}
