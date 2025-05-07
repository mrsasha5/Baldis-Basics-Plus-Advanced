using BaldisBasicsPlusAdvanced.Cache;
using HarmonyLib;
using BaldisBasicsPlusAdvanced.Game.Spawning;

namespace BaldisBasicsPlusAdvanced.Patches.Items
{

    [HarmonyPatch(typeof(PartyEvent))]
    internal class PartyEventItemsPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        private static void AddNewItems(ref WeightedItemObject[] ___potentialItems)
        {
            foreach (string itemName in ObjectsStorage.ItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData["item_" + itemName];

                if (itemSpawnData.SpawnsOnParty)
                {
                    ___potentialItems = ___potentialItems.AddToArray(new WeightedItemObject()
                    {
                        selection = itemSpawnData.ItemObject,
                        weight = itemSpawnData.GetWeight(3),
                    });
                }

            }
        }
    }
    [HarmonyPatch(typeof(MysteryRoom))]
    internal class MysteryRoomItemsPatch
    {
        [HarmonyPatch("SpawnItem")]
        [HarmonyPrefix]
        private static void AddNewItems(ref WeightedItemObject[] ___items)
        {
            foreach (string itemName in ObjectsStorage.ItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData["item_" + itemName];

                if (itemSpawnData.SpawnsOnMysteryRooms)
                {
                    ___items = ___items.AddToArray(new WeightedItemObject()
                    {
                        selection = itemSpawnData.ItemObject,
                        weight = itemSpawnData.GetWeight(3),
                    });
                }

            }
        }
    }

    [HarmonyPatch(typeof(FieldTripBaseRoomFunction))]
    internal class FieldTripItemsPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void AddNewItems(ref WeightedItemObject[] ___potentialItems)
        {
            foreach (string itemName in ObjectsStorage.ItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData["item_" + itemName];

                if (itemSpawnData.SpawnsOnFieldTrips)
                {
                    ___potentialItems = ___potentialItems.AddToArray(new WeightedItemObject()
                    {
                        selection = itemSpawnData.ItemObject,
                        weight = itemSpawnData.GetWeight(3),
                    });
                }

            }
        }
    }
    
}
