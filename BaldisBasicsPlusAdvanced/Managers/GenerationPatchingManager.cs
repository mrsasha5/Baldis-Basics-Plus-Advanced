using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Spawning;
using BaldisBasicsPlusAdvanced.SerializableData.Rooms;
using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Managers
{
    internal class GenerationPatchingManager
    {

        private static void RegisterLevelData(string name, int floor, CustomLevelObject levelObject)
        {
            
            foreach (string itemName in ObjectsStorage.ItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData["item_" + itemName];

                if (name == "END" && !itemSpawnData.EndlessMode) continue;

                if (name != "END" && itemSpawnData.BannedFloors.Contains(floor)) continue;

                if (!itemSpawnData.LevelTypes.Contains(levelObject.type)) continue;

                if (itemSpawnData.SpawnsOnRooms)
                {
                    if (itemSpawnData.Forced)
                    {
                        levelObject.forcedItems.Add(itemSpawnData.ItemObject);
                    } else
                    {
                        levelObject.potentialItems = levelObject.potentialItems.AddToArray(new WeightedItemObject()
                        {
                            selection = itemSpawnData.ItemObject,
                            weight = itemSpawnData.GetWeight(floor)
                        });
                    }
                }
            }

            foreach (string eventName in ObjectsStorage.Events.Keys)
            {
                EventSpawningData spawningData = (EventSpawningData)ObjectsStorage.SpawningData["random_event_" + eventName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floor)) continue;

                if (spawningData.GetWeight(floor) <= 0) return; //don't count forced because there's no forced events in the base game

                if (!spawningData.LevelTypes.Contains(levelObject.type)) continue;

                levelObject.randomEvents.Add(new WeightedRandomEvent()
                {
                    selection = spawningData.RandomEvent,
                    weight = spawningData.GetWeight(floor)
                });
            }

            foreach (string builderName in ObjectsStorage.StructureBuilders.Keys)
            {
                StructureBuilderSpawningData spawningData = (StructureBuilderSpawningData)ObjectsStorage.SpawningData["builder_" + builderName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floor)) continue;

                if (spawningData.GetWeight(floor) <= 0 && !spawningData.Forced) continue;

                if (!spawningData.LevelTypes.Contains(levelObject.type)) continue;

                if (spawningData.Forced)
                {
                    levelObject.forcedStructures = levelObject.forcedStructures.AddToArray(new StructureWithParameters()
                    {
                        prefab = spawningData.StructureBuilder,
                        parameters = spawningData.GetStructureParameters(floor)
                    });
                } else
                {
                    levelObject.potentialStructures = levelObject.potentialStructures.AddToArray(new WeightedStructureWithParameters()
                    {
                        selection = new StructureWithParameters()
                        {
                            prefab = spawningData.StructureBuilder,
                            parameters = spawningData.GetStructureParameters(floor)
                        },
                        weight = spawningData.GetWeight(floor)
                    });
                }
            }

            foreach (BaseSpawningData data in ObjectsStorage.SpawningData.Values) 
            {
                if (data is StructureBuilderExtensionsSpawningData)
                {
                    StructureBuilderExtensionsSpawningData spawningData = (StructureBuilderExtensionsSpawningData)data;

                    if (name == "END" && !spawningData.EndlessMode) continue;

                    if (name != "END" && spawningData.BannedFloors.Contains(floor)) continue;

                    if (!data.LevelTypes.Contains(levelObject.type)) continue;

                    StructureParameters structureParametersData = spawningData.GetStructureParameters(floor);

                    StructureWithParameters parameters = null;

                    if (spawningData.Forced)
                        parameters = Array.Find(levelObject.forcedStructures, x => x.prefab == spawningData.StructureBuilder);
                    else parameters = Array.Find(levelObject.potentialStructures, x => x.selection.prefab == spawningData.StructureBuilder)
                            ?.selection;

                    if (parameters != null) {
                        if (structureParametersData.chance != null) parameters.parameters.chance =
                            parameters.parameters.chance.AddRangeToArray(structureParametersData.chance);
                        if (structureParametersData.prefab != null) parameters.parameters.prefab =
                            parameters.parameters.prefab.AddRangeToArray(structureParametersData.prefab);
                        if (structureParametersData.minMax != null) parameters.parameters.minMax =
                            parameters.parameters.minMax.AddRangeToArray(structureParametersData.minMax);
                    }
                }
                
            }


            foreach (WeightedPosterObject weightedPosterObject in ObjectsStorage.WeightedPosterObjects)
            {
                levelObject.posters = levelObject.posters.AddToArray(weightedPosterObject);
            }

            foreach (string group in ObjectsStorage.RoomGroups.Keys)
            {
                BaseSpawningData spawningData = ObjectsStorage.SpawningData["room_group_" + group];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floor)) continue;

                RoomGroup groupToClone = ObjectsStorage.RoomGroups[group];

                RoomGroup roomGroup = new RoomGroup();
                roomGroup.name = groupToClone.name;
                roomGroup.minRooms = groupToClone.minRooms;
                roomGroup.maxRooms = groupToClone.maxRooms;
                roomGroup.ceilingTexture = groupToClone.ceilingTexture;
                roomGroup.wallTexture = groupToClone.wallTexture;
                roomGroup.floorTexture = groupToClone.floorTexture;
                roomGroup.light = groupToClone.light;

                levelObject.roomGroup = levelObject.roomGroup.AddToArray(roomGroup);
            }

            foreach (RoomGroup group in levelObject.roomGroup)
            {
                foreach (CustomRoomData roomData in ObjectsStorage.RoomDatas)
                {
                    if (name == "END" && !(bool)roomData.endlessMode) continue;

                    if (name != "END" && roomData.bannedFloors.Contains(floor)) continue;

                    if (roomData.levelTypes != null && roomData.levelTypes.Length > 0 &&
                        !roomData.levelTypes.Contains(levelObject.type.ToString()) &&
                        !roomData.levelTypes.Contains(levelObject.type.ToStringExtended())) continue;

                    RoomCategory category = roomData.weightedRoomAsset.selection.category;

                    if (category == RoomCategory.Special && !levelObject.potentialSpecialRooms.Contains(roomData.weightedRoomAsset))
                    {
                        levelObject.potentialSpecialRooms = levelObject.potentialSpecialRooms.AddToArray(roomData.weightedRoomAsset);
                    }
                    else if ((group.name == category.ToString() || group.name == category.ToStringExtended()) 
                        && !group.potentialRooms.Contains(roomData.weightedRoomAsset))
                    {
                        group.potentialRooms = group.potentialRooms.AddToArray(roomData.weightedRoomAsset);
                    }
                }
            }

            //for halls
            foreach (CustomRoomData roomData in ObjectsStorage.RoomDatas)
            {
                if (name == "END" && !(bool)roomData.endlessMode) continue;

                if (name != "END" && roomData.bannedFloors.Contains(floor)) continue;

                if (roomData.levelTypes != null && roomData.levelTypes.Length > 0 &&
                        !roomData.levelTypes.Contains(levelObject.type.ToStringExtended())) continue;

                RoomCategory category = roomData.weightedRoomAsset.selection.category;

                if (category == RoomCategory.Hall)
                {
                    if ((bool)roomData.isPotentialPostPlotSpecialHall)
                        levelObject.potentialPostPlotSpecialHalls = 
                            levelObject.potentialPostPlotSpecialHalls.AddToArray(roomData.weightedRoomAsset);
                }
            }

        }

        public static void RegisterMainLevelData(string name, int floor, SceneObject mainLevel)
        {
            floor++;

            foreach (string itemName in ObjectsStorage.ItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData["item_" + itemName];

                if (itemSpawnData.BannedFloors.Contains(floor)) continue;

                if (!itemSpawnData.SpawnsOnShop) continue;

                if (itemSpawnData.SpawnsOnShop)
                {
                    mainLevel.shopItems = mainLevel.shopItems.AddToArray(new WeightedItemObject()
                    {
                        selection = itemSpawnData.ItemObject,
                        weight = itemSpawnData.GetWeight(floor)
                    });
                }
            }

            foreach (string npcName in ObjectsStorage.Npcs.Keys)
            {
                NPCSpawningData spawningData = (NPCSpawningData)ObjectsStorage.SpawningData["npc_" + npcName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floor)) continue;

                if (spawningData.Forced && floor == 1)
                {
                    mainLevel.forcedNpcs = mainLevel.forcedNpcs.AddToArray(spawningData.Npc);
                }
                else if (spawningData.GetWeight(floor) > 0)
                {
                    mainLevel.potentialNPCs.Add(new WeightedNPC()
                    {
                        selection = spawningData.Npc,
                        weight = spawningData.GetWeight(floor)
                    });
                }
            }

            if (name == "END") return;

            if (mainLevel.levelObject != null)
            {
                RegisterLevelData(name, floor, (CustomLevelObject)mainLevel.levelObject);
            }

            if (mainLevel.randomizedLevelObject.Length > 0)
            {
                foreach (WeightedLevelObject weightedLevelObj in mainLevel.randomizedLevelObject)
                {
                    RegisterLevelData(name, floor, (CustomLevelObject)weightedLevelObj.selection);
                }
            }
        }

    }
}
