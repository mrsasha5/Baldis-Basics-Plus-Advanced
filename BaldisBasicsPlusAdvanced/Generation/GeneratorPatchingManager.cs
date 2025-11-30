using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BaldisBasicsPlusAdvanced.Game.Spawning;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SerializableData;
using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Generation
{
    //Migrating gradually into the new system
    internal class GeneratorPatchingManager
    {

        private static bool recipesLoaded;

        private static void RegisterLevelData(string name, int floor, CustomLevelObject levelObject)
        {
            //Cannot remind why I did put it here, lol
            if (!recipesLoaded)
            {
                ApiManager.LoadKitchenStoveRecipesFromFolder(AdvancedCore.Instance.Info,
                    AssetHelper.modPath + "Data/Recipes/KitchenStove/", true);
                recipesLoaded = true;
            }

            if (!levelObject.IsModifiedByMod(AdvancedCore.Instance.Info))
            {
                levelObject.MarkAsModifiedByMod(AdvancedCore.Instance.Info);
            }
            else return;

            foreach (CellTextureSerializableData cellTexData in ObjectStorage.CellTextureData)
            {
                if (name == "END" && !cellTexData.endlessMode) continue;

                if (name != "END" && cellTexData.bannedFloors != null && cellTexData.bannedFloors.Contains(floor)) continue;

                if (cellTexData.levelTypes != null && cellTexData.levelTypes.Length > 0 &&
                    !cellTexData.levelTypes.Contains(levelObject.type.ToString()) &&
                        !cellTexData.levelTypes.Contains(levelObject.type.ToStringExtended())) continue;

                WeightedTexture2D weightedTex = new WeightedTexture2D()
                {
                    selection = cellTexData.tex,
                    weight = cellTexData.weights.GetWeight(floor)
                };

                if (cellTexData.replacementWall && levelObject.hallWallTexs.Length >= 1)
                {
                    levelObject.hallWallTexs[0] = weightedTex;
                }
                else if (cellTexData.types.Contains("Wall"))
                    levelObject.hallWallTexs = levelObject.hallWallTexs.AddToArray(weightedTex);

                if (cellTexData.types.Contains("Ceiling"))
                    levelObject.hallCeilingTexs = levelObject.hallCeilingTexs.AddToArray(weightedTex);

                if (cellTexData.types.Contains("Floor"))
                    levelObject.hallFloorTexs = levelObject.hallFloorTexs.AddToArray(weightedTex);
            }

            foreach (string builderName in ObjectStorage.StructureBuilders.Keys)
            {
                StructureBuilderSpawningData spawningData = (StructureBuilderSpawningData)ObjectStorage.SpawningData["builder_" + builderName];

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

            foreach (BaseSpawningData data in ObjectStorage.SpawningData.Values) 
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

            foreach (WeightedPosterObject weightedPosterObject in ObjectStorage.WeightedPosterObjects)
            {
                levelObject.posters = levelObject.posters.AddToArray(weightedPosterObject);
            }

            foreach (string group in ObjectStorage.RoomGroups.Keys)
            {
                BaseSpawningData spawningData = ObjectStorage.SpawningData["room_group_" + group];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floor)) continue;

                RoomGroup groupToClone = ObjectStorage.RoomGroups[group];

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

            foreach (CustomRoomData roomData in ObjectStorage.CustomRoomData)
            {
                if (roomData.isAHallway != null && (bool)roomData.isAHallway) continue;

                if (name == "END" && !(bool)roomData.endlessMode) continue;

                if (name != "END" && roomData.bannedFloors != null && roomData.bannedFloors.Contains(floor)) continue;

                if (roomData.levelTypes != null && roomData.levelTypes.Length > 0 &&
                    !roomData.levelTypes.Contains(levelObject.type.ToString()) &&
                    !roomData.levelTypes.Contains(levelObject.type.ToStringExtended())) continue;

                WeightedRoomAsset weightedRoom = new WeightedRoomAsset()
                {
                    selection = roomData.roomAsset,
                    weight = roomData.weights.GetWeight(floor)
                };

                RoomCategory category = roomData.roomAsset.category;

                if (category == RoomCategory.Special)
                {
                    levelObject.potentialSpecialRooms = levelObject.potentialSpecialRooms.AddToArray(weightedRoom);
                    continue;
                }

                foreach (RoomGroup group in levelObject.roomGroup)
                {
                    if (group.name == category.ToString() || group.name == category.ToStringExtended())
                    {
                        group.potentialRooms = group.potentialRooms.AddToArray(weightedRoom);
                        break;
                    }
                }
            }

            //For halls
            foreach (CustomRoomData roomData in ObjectStorage.CustomRoomData)
            {
                if (roomData.isAHallway != null && !(bool)roomData.isAHallway) continue;

                if (name == "END" && !(bool)roomData.endlessMode) continue;

                if (name != "END" && roomData.bannedFloors != null && roomData.bannedFloors.Contains(floor)) continue;

                if (roomData.levelTypes != null && roomData.levelTypes.Length > 0 &&
                        !roomData.levelTypes.Contains(levelObject.type.ToStringExtended())) continue;

                RoomCategory category = roomData.roomAsset.category;

                if (category == RoomCategory.Hall)
                {
                    if ((bool)roomData.isPotentialPostPlotSpecialHall)
                        levelObject.potentialPostPlotSpecialHalls =
                            levelObject.potentialPostPlotSpecialHalls.AddToArray(
                                new WeightedRoomAsset()
                                    {
                                        selection = roomData.roomAsset,
                                        weight = roomData.weights.GetWeight(floor)
                                    }
                            );
                }
            }

            InitializeKitchenStovePosters(levelObject);
        }

        public static void RegisterMainLevelData(string name, int floor, SceneObject mainLevel)
        {
            GenerationManager.RegisterSceneObjectData(name, floor, mainLevel); //New system (WiP)
            floor++;

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

#if DEBUG
            DebugWeights(mainLevel);
#endif
        }

        private static void InitializeKitchenStovePosters(LevelObject obj)
        {
            List<FoodRecipeData> data = ApiManager.GetAllKitchenStoveRecipes();
            List<PosterObject> posters = new List<PosterObject>();

            for (int i = 0; i < data.Count; i++)
            {
                posters.Add(data[i].Poster);
            }

            data.Clear();

            int weight = 100 / posters.Count;

            for (int i = 0; i < posters.Count; i++)
            {
                obj.posters = obj.posters.AddToArray(
                new WeightedPosterObject()
                {
                    selection = posters[i],
                    weight = weight
                });
            }

            posters.Clear();
        }

#if DEBUG

        private static void DebugWeights(SceneObject mainLevel)
        {
            if (mainLevel.levelObject != null)
            {
                DebugLevel(mainLevel.levelObject);
            }

            if (mainLevel.randomizedLevelObject.Length > 0)
            {
                foreach (WeightedLevelObject weightedLevelObj in mainLevel.randomizedLevelObject)
                {
                    DebugLevel(weightedLevelObj.selection);
                }
            }
        }

        private static void DebugLevel(LevelObject level)
        {
            AdvancedCore.Logging.LogInfo("-------------------------------------");
            AdvancedCore.Logging.LogInfo($"Level name: {level.name}");

            AdvancedCore.Logging.LogInfo("Potential Items:");

            foreach (WeightedItemObject item in level.potentialItems)
            {
                AdvancedCore.Logging.LogInfo($"{item.selection.itemType.ToStringExtended()}: {item.weight}");
            }

            AdvancedCore.Logging.LogInfo("Random Events:");

            foreach (WeightedRandomEvent @event in level.randomEvents)
            {
                AdvancedCore.Logging.LogInfo($"{@event.selection.Type.ToStringExtended()}: {@event.weight}");
            }

            AdvancedCore.Logging.LogInfo("Potential Structures:");

            foreach (WeightedStructureWithParameters structure in level.potentialStructures)
            {
                AdvancedCore.Logging.LogInfo($"{structure.selection.prefab.name}: {structure.weight}");
            }

            AdvancedCore.Logging.LogInfo("-------------------------------------");
        }

#endif

        }
}
