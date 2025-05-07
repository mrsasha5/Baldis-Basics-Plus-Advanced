using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Game.GameItems;
using BBE;
using BBE.CustomClasses;
using HarmonyLib;
using MTM101BaldAPI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Patches.Game.FunSettings
{
    [HarmonyPatch(typeof(IceBootsItem))]
    [ConditionalPatchIntegratedMod(ModsIntegration.extraId, ModsIntegration.extraVersion)]
    internal class IceBootsPatch
    {
        [HarmonyPatch("Use")]
        [HarmonyPostfix]
        private static void OnUse(PlayerManager pm, ref bool __result)
        {
            if ("IceBoots".ToEnum<FunSettingsType>().IsActive() && pm.itm.selectedItem == 0)
            {
                __result = false;
            }
        }

    }
}
