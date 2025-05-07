using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Managers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaldisBasicsPlusAdvanced.Game;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Items
{

    [HarmonyPatch(typeof(PartyEvent))]
    internal class PartyEventItemsPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void addNewItems(PartyEvent __instance)
        {
            WeightedItemObject[] items = ReflectionHelper.getValue<WeightedItemObject[]>(__instance, "potentialItems");
            foreach (string itemName in ObjectsStorage.WeightedItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData[itemName];

                WeightedItemObject weightedItem = ObjectsStorage.WeightedItemObjects[itemName];

                if (itemSpawnData.SpawnsOnParty && !items.Contains(weightedItem))
                {
                    items = items.AddToArray(weightedItem);
                }

            }
            ReflectionHelper.setValue<WeightedItemObject[]>(__instance, "potentialItems", items);
        }
    }
    [HarmonyPatch(typeof(MysteryRoom))]
    internal class MysteryRoomItemsPatch
    {
        [HarmonyPatch("SpawnItem")]
        [HarmonyPrefix]
        private static void addNewItems(MysteryRoom __instance)
        {
            WeightedItemObject[] items = ReflectionHelper.getValue<WeightedItemObject[]>(__instance, "items");
            foreach (string itemName in ObjectsStorage.WeightedItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData[itemName];

                WeightedItemObject weightedItem = ObjectsStorage.WeightedItemObjects[itemName];

                if (itemSpawnData.SpawnsOnMysteryRooms && !items.Contains(weightedItem))
                {
                    items = items.AddToArray(weightedItem);
                }

            }
            
            ReflectionHelper.setValue<WeightedItemObject[]>(__instance, "items", items);
        }
    }
}
