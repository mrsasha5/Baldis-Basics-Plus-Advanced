using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Patches.Game.Player
{
    [HarmonyPatch(typeof(ItemManager))]
    internal class ItemManagerPatch
    {
        public static List<Items> items = new List<Items>();

        [HarmonyPatch("UseItem")]
        [HarmonyPostfix]
        private static void OnUseItem(ItemManager __instance, ref bool ___disabled)
        {
            if (___disabled && items.Contains(__instance.items[__instance.selectedItem].itemType) && Object.Instantiate(__instance.items[__instance.selectedItem].item).Use(__instance.pm))
            {
                __instance.RemoveItem(__instance.selectedItem);
            }
        }

    }
}
