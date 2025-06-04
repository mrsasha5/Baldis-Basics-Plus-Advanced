//Decided to cancel because objects like Vending Machines are still working
/*using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using HarmonyLib;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Objects
{
    [HarmonyPatch(typeof(BreakerController))]
    internal class ElectricityBreakerPatch
    {

        [HarmonyPatch("BlowFuse")]
        [HarmonyPostfix]
        private static void TurnOffObjects()
        {
            foreach (BasePlate plate in GameObject.FindObjectsOfType<BasePlate>())
            {
                plate.SetTurnOff(true);
            }
        }

        [HarmonyPatch("ResetFuse")]
        [HarmonyPostfix]
        private static void TurnOnObjects()
        {
            foreach (BasePlate plate in GameObject.FindObjectsOfType<BasePlate>())
            {
                plate.SetTurnOff(false);
            }
        }

    }
}*/