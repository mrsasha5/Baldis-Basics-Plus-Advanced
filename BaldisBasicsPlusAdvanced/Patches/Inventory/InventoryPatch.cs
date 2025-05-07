using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Menu;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Patches.Inventory
{
    [HarmonyPatch(typeof(StoreScreen))]
    internal class InventoryStoreScreenPatch
    {
        /*
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void onStart(ref ItemObject[] ___inventory, ref Image[] ___inventoryImage, ref ItemObject ___defaultItem)
        {
            
            if (Singleton<PlayerFileManager>.Instance.authenticMode && !ModsIntegration.EndlessInstalled)
            {
                if (Singleton<CoreGameManager>.Instance == null || Singleton<CoreGameManager>.Instance.GetPlayer(0) == null)
                {
                    for (int j = 0; j < ___inventory.Length; j++)
                    {
                        ___inventory[j] = ___defaultItem;
                        ___inventoryImage[j].sprite = ___defaultItem.itemSpriteSmall;
                    }
                }
                else
                {
                    ItemManager itemManager = Singleton<CoreGameManager>.Instance.GetPlayer(0).itm;
                    for (int k = 0; k < ___inventory.Length; k++)
                    {
                        if (k < InventoryPatch.maxItem)
                        {
                            ___inventory[k] = itemManager.items[k];
                            ___inventoryImage[k].sprite = ___inventory[k].itemSpriteSmall;
                        }
                        else
                        {
                            ___inventory[k] = ___defaultItem;
                            ___inventoryImage[k].sprite = ___defaultItem.itemSpriteSmall;
                        }
                    }
                }
            }
        }*/
    }

    [HarmonyPatch(typeof(ItemManager))]
    internal class InventoryPatch
    {
        public const int maxItem = 3;

        /*
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static void onUpdate(ItemManager __instance)
        {
            if (Singleton<PlayerFileManager>.Instance.authenticMode) __instance.maxItem = maxItem - 1;
        }*/
    }

}
