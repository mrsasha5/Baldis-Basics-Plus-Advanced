using System.Collections;
using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using HarmonyLib;
using MTM101BaldAPI.UI;
using The3DElevator.MonoBehaviours.ElevatorCoreComponents;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Patches
{
    [HarmonyPatch(typeof(LobbyElevator))]
    [ConditionalPatchIntegrableMod(typeof(SpatialElevatorIntegration))]
    internal class LobbyElevatorPatch
    {

        private static TextMeshPro tmpText;

        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void OnInitialize(LobbyElevator __instance)
        {
            __instance.GetComponentInChildren<SpatialChalkboard>()
                .Initialize();
        }

    }
}