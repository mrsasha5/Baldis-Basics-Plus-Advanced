using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{

    [HarmonyPatch(typeof(FirstPrize_Active))]
    internal class FirstPrizePushesEveryone
    {

#warning update this code completely
        private static readonly string[] exceptionCharacters = new string[] 
        { "ChalkFace(Clone)", "Bully(Clone)", "Gotta Sweep(Clone)", "FirstPrize(Clone)" }; 
        //based on Mystman methods... but I think that's not good idea

        [HarmonyPatch("OnStateTriggerEnter")]
        [HarmonyPrefix]
        private static void OnTriggerEnter(FirstPrize_Active __instance, Collider other)
        {
            if (other.TryGetComponent(out ITM_AlarmClock _) || 
                (OptionsDataManager.ExtraSettings.GetValue<bool>("first_prize_extensions") && other.CompareTag("NPC")))
            {
                if (!exceptionCharacters.Contains(other.name))
                {
                    ReflectionHelper.GetValue<MoveModsManager>(__instance, "moveModsMan")
                        .AddMoveMod(other.transform);
                    other.transform.position = __instance.Npc.gameObject.transform.position 
                        + __instance.Npc.gameObject.transform.forward * 4f;
                }
                
            }
        }

        [HarmonyPatch("OnStateTriggerExit")]
        [HarmonyPrefix]
        private static void OnTriggerExit(FirstPrize_Active __instance, Collider other)
        {
            if (other.TryGetComponent(out ITM_AlarmClock _) || 
                (OptionsDataManager.ExtraSettings.GetValue<bool>("first_prize_extensions") && other.CompareTag("NPC")))
            {
                if (!exceptionCharacters.Contains(other.name))
                {
                    ReflectionHelper.GetValue<MoveModsManager>(__instance, "moveModsMan")
                        .Remove(other.transform);
                }
            }
        }
    }
}
