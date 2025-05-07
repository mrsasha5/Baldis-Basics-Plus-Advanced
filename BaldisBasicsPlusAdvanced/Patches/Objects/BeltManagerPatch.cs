/*using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Objects
{
    [HarmonyPatch(typeof(BeltManager))]
    internal class BeltManagerPatch
    {
        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPostfix]
        private static void onTriggerEnter(BeltManager __instance, Collider other, ref List<ActivityModifier> ___currentActMods, ref MovementModifier ___moveMod)
        {
            //may be not better implementation, but i think that's normal idea
            if (__instance.name == "Adv_WindManager" && other.name == "CloudyCopter(Clone)" && other.TryGetComponent(out Entity entity))
            {
                ActivityModifier externalActivity = entity.ExternalActivity;
                ___currentActMods.Add(externalActivity);
                externalActivity.moveMods.Add(___moveMod);
            }
        }

        [HarmonyPatch("OnTriggerExit")]
        [HarmonyPostfix]
        private static void onTriggerExit(BeltManager __instance, Collider other, ref List<ActivityModifier> ___currentActMods, ref MovementModifier ___moveMod)
        {
            //may be not better implementation, but i think that's normal idea
            if (__instance.name == "Adv_WindManager" && other.name == "CloudyCopter(Clone)" && other.TryGetComponent(out Entity entity))
            {
                ActivityModifier externalActivity = entity.ExternalActivity;
                ___currentActMods.Remove(externalActivity);
                externalActivity.moveMods.Remove(___moveMod);
            }
        }
    }
}
*/