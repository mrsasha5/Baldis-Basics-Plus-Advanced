using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{
    [HarmonyPatch(typeof(ChalkFace))]
    internal class ChalklesPatch
    {
        //sprite base - flying renderer

        
        [HarmonyPatch("Teleport")]
        [HarmonyPostfix]
        private static void onTeleport(ChalkFace __instance)
        {
            if (DisappearingCharactersEvent.ActiveEvents > 0 && DisappearingCharactersEvent.ChalkFacesDisappeared.Contains(__instance))
            {
                SpriteRenderer chalkRenderer = ReflectionHelper.getValue<SpriteRenderer>(__instance, "chalkRenderer");
                chalkRenderer.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch("Teleport")]
        [HarmonyPostfix]
        private static void onActivate(ChalkFace __instance)
        {
            if (DisappearingCharactersEvent.ActiveEvents > 0 && DisappearingCharactersEvent.ChalkFacesDisappeared.Contains(__instance))
            {
                __instance.spriteBase.SetActive(false);
            }
        }

}
}
