using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SavedData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{

    [HarmonyPatch(typeof(FirstPrize_Active))]
    internal class FirstPrizePushesEveryone
    {

        private static readonly string[] exceptionCharacters = new string[] { "ChalkFace(Clone)", "Bully(Clone)", "Gotta Sweep(Clone)", "FirstPrize(Clone)" }; //based on Mystman methods... but i think that's not good idea

        [HarmonyPatch("OnStateTriggerEnter")]
        [HarmonyPrefix]
        private static void onTriggerEnter(FirstPrize_Active __instance, Collider other)
        {
            if (other.TryGetComponent(out ITM_AlarmClock _) || (DataManager.ExtraSettings.firstPrizeFeaturesEnabled && other.tag == "NPC"))
            {
                if (!exceptionCharacters.Contains(other.name))
                {
                    MoveModsManager moveModsMan = ReflectionHelper.getValue<MoveModsManager>(__instance, "moveModsMan");
                    moveModsMan.AddMoveMod(other.transform);
                    other.transform.position = __instance.Npc.gameObject.transform.position + __instance.Npc.gameObject.transform.forward * 4f;
                }
                
            }
        }

        [HarmonyPatch("OnStateTriggerExit")]
        [HarmonyPrefix]
        private static void onTriggerExit(FirstPrize_Active __instance, Collider other)
        {
            if (other.TryGetComponent(out ITM_AlarmClock _) || (DataManager.ExtraSettings.firstPrizeFeaturesEnabled && other.tag == "NPC"))
            {
                if (!exceptionCharacters.Contains(other.name))
                {
                    MoveModsManager moveModsMan = ReflectionHelper.getValue<MoveModsManager>(__instance, "moveModsMan");
                    moveModsMan.Remove(other.transform);
                }
            }
        }
    }
}
