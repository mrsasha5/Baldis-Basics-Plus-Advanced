using BaldisBasicsPlusAdvanced.Game.Rooms;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Events
{
    [HarmonyPatch(typeof(PartyEvent))]
    internal class PartyEventPatch
    {

        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        private static void OnBegin(ref RoomController ___office)
        {
            if (___office.gameObject.GetComponent<PartyLightColors>() != null) return;
            PartyLightColors lightColors = ___office.gameObject.AddComponent<PartyLightColors>();
            lightColors.Initialize(___office);
        }

        [HarmonyPatch("End")]
        [HarmonyPostfix]
        private static void OnEnd(ref RoomController ___office)
        {
            if (___office.gameObject.TryGetComponent(out PartyLightColors lightColors))
            {
                lightColors.NormalizeLights();
                GameObject.Destroy(lightColors);
            }
        }

    }
}
