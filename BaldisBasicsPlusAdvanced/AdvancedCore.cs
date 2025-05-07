using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Game;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Managers;
using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.SavedData;
using BaldisBasicsPlusAdvanced.SerializableData;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace BaldisBasicsPlusAdvanced
{
    [BepInPlugin(modId, modName, version)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", DependencyFlags.HardDependency)]
    [BepInDependency(ModsIntegration.levelLoaderId, DependencyFlags.HardDependency)]
    [BepInDependency(ModsIntegration.extraId, DependencyFlags.SoftDependency)]
    public class AdvancedCore : BaseUnityPlugin
    {
        public const string modId = "baldi.basics.plus.advanced.mod";

        public const string modName = "Baldi's Basics Plus Advanced Edition";

        public const string version = "0.2.1.3";

        internal static bool editorIntegrationEnabled;

        internal static bool extraIntegrationEnabled;

        private static AdvancedCore instance;

        public static AdvancedCore Instance => instance;

        internal static ManualLogSource Logging => Instance.Logger;

        private static Harmony harmony;

        private void Awake()
        {
            harmony = new Harmony(modId);
            //harmony.PatchAllConditionals(); //"direct replacement for PatchAll"
            //moved to assets loading
            instance = this;
            editorIntegrationEnabled = Config.Bind("Integration", "Editor", defaultValue: true, "If disabled, then items and other things from this mod will not load in the editor!").Value;
            extraIntegrationEnabled = Config.Bind("Integration", "Extra", defaultValue: true, "If disabled, then integration (Fun Settings and etc) will be disabled.").Value;
            PrepareSettingsMenu();
            ModdedSaveGame.AddSaveHandler(LevelDataManager.Instance);
            GeneratorManagement.Register(this, GenerationModType.Addend, RegisterLevelData);
            LoadingEvents.RegisterOnAssetsLoaded(Info, ModLoader(), false);
        }

        private static IEnumerator ModLoader()
        {
            ModsIntegration.CheckPotentialModIntegrations();

            harmony.PatchAllConditionals(); //I can't check installed some mods, because priority is different and this is solution of problem

            IEnumerator assetsLoading = OnAssetsLoaded();
            bool move = true;
            while (move)
            {
                try
                {
                    move = assetsLoading.MoveNext();
                } catch (Exception e)
                {
                    ObjectsCreator.CauseCrash(e);
                    move = false;
                }
                yield return assetsLoading.Current;
            }
            yield break;
        }

        private static IEnumerator OnAssetsLoaded()
        {
            ApiManager.onAssetsPreLoading?.Invoke();

            int count = 16; //+1 ?
            yield return count;
            yield return "Checking installed mods...";
            ModsIntegration.CheckCompabilities();
            yield return "Caching game assets...";
            AssetsStorage.Cache();
            if (AssetsStorage.exception != null) throw AssetsStorage.exception;
            yield return "Initializing vending machines...";
            GameRegisterManager.InitializeVendingMachines();
            yield return "Initializing UI...";
            GameRegisterManager.InitializeUI();
            yield return "Initializing objects...";
            GameRegisterManager.InitializeObjects();
            yield return "Initializing entities...";
            GameRegisterManager.InitializeEntities();
            yield return "Initializing items...";
            GameRegisterManager.InitializeGameItems();
            GameRegisterManager.InitializeMultipleUsableItems();
            yield return "Initializing room basics...";
            GameRegisterManager.InitializeRoomBasics();
            yield return "Loading extensions for the Level Loader...";
            LevelLoaderIntegration.Initialize();
            yield return "Initializing events...";
            GameRegisterManager.InitializeGameEvents();
            yield return "Initializing builders...";
            GameRegisterManager.InitializeObjectBuilders();
            yield return "Initializing posters...";
            GameRegisterManager.InitializePosters();
            yield return "Initializing tips and words...";
            GameRegisterManager.InitializeTipsAndWords();
            yield return "Adding some tags...";
            GameRegisterManager.SetTags();
            
            yield return "Initializing room assets...";
            GameRegisterManager.InitializeRoomAssets();
            //+3 to count
            //for (int i = 0; i < 3; i++)
            //{
            //    yield return "Invoking changes for MainLevel" + (i + 1);
            //    RegisterMainLevelData(AssetsHelper.loadAsset<SceneObject>("MainLevel_" + (i + 1)), i);
            //}
            yield return "Overriding game prefabs...";
            AssetsStorage.OverrideAssetsProperties();
            yield return "Integrating with other mods...";
            ModsIntegration.MakeIntegration();

            ApiManager.onAssetsPostLoading?.Invoke();
            GC.Collect();

            yield break;
        }

        private static void RegisterLevelData(string name, int floorNum, CustomLevelObject levelObject)
        {
            floorNum++; //because texture man gives it as index!
            foreach (string itemName in ObjectsStorage.WeightedItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData["item_" + itemName];

                //name = "INF" - infinity floors!
                //but... i allow to appear for objects on every floor and i don't need to use it...
                
                if (name == "END" && !itemSpawnData.EndlessMode) continue;

                if (name != "END" && itemSpawnData.BannedFloors.Contains(floorNum)) continue;

                WeightedItemObject weightedItemObject = ObjectsStorage.WeightedItemObjects[itemName];

                //WeightedItem weightedItem = ObjectsStorage.WeightedItems[itemName];

                // not obsolete
                // but is works only for endless floor
                // I also should to patch MainLevel, not only Main (but API does it and I don't need)
                
                if (itemSpawnData.SpawnsOnShop)
                {
                    levelObject.shopItems = levelObject.shopItems.AddToArray(weightedItemObject);
                }

                if (itemSpawnData.SpawnsOnRooms)
                {
                    levelObject.potentialItems = levelObject.potentialItems.AddToArray(weightedItemObject);
                }

                /*if (itemSpawnData.SpawnsOnFieldTrips && !levelObject.fieldTripItems.Contains(weightedItem))
                {
                    levelObject.fieldTripItems.Add(weightedItem);
                }*/
            }

            foreach (string eventName in ObjectsStorage.Events.Keys)
            {
                BaseSpawningData spawningData = ObjectsStorage.SpawningData["random_event_" + eventName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floorNum)) continue;

                levelObject.randomEvents.Add(ObjectsStorage.WeightedEvents[eventName]);
            }

            foreach (string builderName in ObjectsStorage.WeightedObjectBuilders.Keys)
            {
                BaseSpawningData spawningData = ObjectsStorage.SpawningData["builder_" + builderName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floorNum)) continue;

                levelObject.specialHallBuilders = levelObject.specialHallBuilders.AddToArray(ObjectsStorage.WeightedObjectBuilders[builderName]);
            }

            foreach (string forcedBuilderName in ObjectsStorage.ForcedSpecialObjectBuilders.Keys)
            {
                BaseSpawningData spawningData = ObjectsStorage.SpawningData["builder_" + forcedBuilderName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floorNum)) continue;

                levelObject.forcedSpecialHallBuilders = levelObject.forcedSpecialHallBuilders.AddToArray(ObjectsStorage.ForcedSpecialObjectBuilders[forcedBuilderName]);
            }

            foreach (WeightedPosterObject weightedPosterObject in ObjectsStorage.WeightedPosterObjects)
            {
                levelObject.posters = levelObject.posters.AddToArray(weightedPosterObject);
            }

            foreach (string group in ObjectsStorage.RoomGroups.Keys)
            {
                BaseSpawningData spawningData = ObjectsStorage.SpawningData["room_group_" + group];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floorNum)) continue;

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
                    if (name == "END" && !roomData.endlessMode) continue;

                    if (name != "END" && roomData.bannedFloors.Contains(floorNum)) continue;

                    RoomCategory category = roomData.weightedRoomAsset.selection.category;

                    if ((group.name == category.ToString() || group.name == category.ToStringExtended()) && !group.potentialRooms.Contains(roomData.weightedRoomAsset))
                    {
                        group.potentialRooms = group.potentialRooms.AddToArray(roomData.weightedRoomAsset);
                    }
                }
            }

            //for halls
            foreach (CustomRoomData roomData in ObjectsStorage.RoomDatas)
            {
                if (name == "END" && !roomData.endlessMode) continue;

                if (name != "END" && roomData.bannedFloors.Contains(floorNum)) continue;

                RoomCategory category = roomData.weightedRoomAsset.selection.category;

                if (category == RoomCategory.Hall)
                {
                    if (roomData.isPotentialPrePlotSpecialHall)
                        levelObject.potentialPrePlotSpecialHalls = levelObject.potentialPrePlotSpecialHalls.AddToArray(roomData.weightedRoomAsset);
                    if (roomData.isPotentialPostPlotSpecialHall)
                        levelObject.potentialPostPlotSpecialHalls = levelObject.potentialPostPlotSpecialHalls.AddToArray(roomData.weightedRoomAsset);

                }
            }

        }


        /*private static void RegisterMainLevelData(SceneObject mainLevel, int floorNum)
        {
            floorNum++;

            foreach (string itemName in ObjectsStorage.WeightedItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData[itemName];

                if (itemSpawnData.BannedFloors.Contains(floorNum)) continue;

                WeightedItemObject weightedItemObject = ObjectsStorage.WeightedItemObjects[itemName];

                if (itemSpawnData.SpawnsOnShop)
                {
                    mainLevel.shopItems = mainLevel.shopItems.AddToArray(weightedItemObject);
                }
            }
        }*/

        private void PrepareSettingsMenu()
        {
            CustomOptionsCore.OnMenuInitialize += ModSettingsMenu.OnMenuInit;
            CustomOptionsCore.OnMenuClose += ModSettingsMenu.OnMenuClose;

            CustomOptionsCore.OnMenuInitialize += EmergencyButtonMenu.OnMenuInit;
            CustomOptionsCore.OnMenuClose += EmergencyButtonMenu.OnMenuClose;
        }
    }
}
