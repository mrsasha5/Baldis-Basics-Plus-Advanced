using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{

    [HarmonyPatch(typeof(FirstPrize_Active))]
    internal class FirstPrizePushesEveryone
    {

        [HarmonyPatch("OnStateTriggerEnter")]
        [HarmonyPrefix]
        private static void OnTriggerEnter(FirstPrize_Active __instance, Collider other, ref MoveModsManager ___moveModsMan)
        {
            if (other.CompareTag("NPC") &&
                OptionsDataManager.ExtraSettings.GetValue<bool>("first_prize_extensions"))
            {
                NPCMetadata meta = other.GetComponent<NPC>().GetMeta();

                if (meta == null || meta.tags.Contains(TagsStorage.firstPrizeImmunity))
                    return;

                ___moveModsMan.AddMoveMod(other.transform);
                other.transform.position = __instance.Npc.gameObject.transform.position 
                    + __instance.Npc.gameObject.transform.forward * 4f;
                
            }
        }

        [HarmonyPatch("OnStateTriggerExit")]
        [HarmonyPrefix]
        private static void OnTriggerExit(FirstPrize_Active __instance, Collider other, ref MoveModsManager ___moveModsMan)
        {
            if (other.CompareTag("NPC") && 
                OptionsDataManager.ExtraSettings.GetValue<bool>("first_prize_extensions"))
            {
                if (other.GetComponent<NPC>().GetMeta().tags.Contains(TagsStorage.firstPrizeImmunity))
                    return;

                ___moveModsMan.Remove(other.transform);
            }
        }
    }
}
