using System.Collections.Generic;
using HarmonyLib;
using MTM101BaldAPI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Objects
{
    [HarmonyPatch(typeof(BeltManager))]
    internal class BeltManagerPatch
    {
        public static List<BeltManager> connectedBelts = new List<BeltManager>();

        [HarmonyPatch("ConnectButton")]
        [HarmonyPrefix]
        private static void OnConnect(BeltManager __instance)
        {
            __instance.gameObject.GetOrAddComponent<BeltManagerTracker>().beltMan = __instance;
            connectedBelts.Add(__instance);
        }
    }

    internal class BeltManagerTracker : MonoBehaviour
    {
        public BeltManager beltMan;

        private void OnDestroy()
        {
            BeltManagerPatch.connectedBelts.Remove(beltMan);
        }
    }
}