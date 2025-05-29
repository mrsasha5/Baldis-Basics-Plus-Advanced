using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using HarmonyLib;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{
    [HarmonyPatch(typeof(Baldi_Chase))]
    internal class BaldiCatchingPatch
    {
        [HarmonyPatch("OnStateTriggerStay")]
        [HarmonyPrefix]
        private static bool OnStateTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && other.GetComponent<BaseControllerSystem>().Invincibility) return false;
            return true;
        }
    }
}
