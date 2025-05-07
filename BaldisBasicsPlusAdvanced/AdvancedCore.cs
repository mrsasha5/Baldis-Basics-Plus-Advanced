using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor;
using BaldisBasicsPlusAdvanced.Exceptions;
using BaldisBasicsPlusAdvanced.Game;
using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Managers;
using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.SavedData;
using BepInEx;
using BepInEx.Bootstrap;
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

namespace BaldisBasicsPlusAdvanced
{
    [BepInPlugin(modId, modName, version)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class AdvancedCore : BaseUnityPlugin
    {
        public const string modId = "baldi.basics.plus.advanced.mod";

        public const string modName = "Baldi's Basics Plus Advanced Edition";

        public const string version = "0.1.6.91";

        private static bool editorIntegrationEnabled;

        private static AdvancedCore instance;

        public static AdvancedCore Instance => instance;

        private static Harmony harmony;

        private void Awake()
        {
            harmony = new Harmony(modId);
            //harmony.PatchAllConditionals(); //"direct replacement for PatchAll"
            //moved to assets loading
            instance = this;
            editorIntegrationEnabled = Config.Bind("Integration", "Editor", defaultValue: true, "If disabled, then items and other things from this mod will not load in the editor!").Value;
            prepareSettingsMenu();
            ModdedSaveGame.AddSaveHandler(new DataManager());
            GeneratorManagement.Register(this, GenerationModType.Addend, registerLevelData);
            LoadingEvents.RegisterOnAssetsLoaded(Info, modLoader(), false);
        }

        private static IEnumerator modLoader()
        {
            harmony.PatchAllConditionals(); //i cannot check installed some mods, because priority is different and this is solution of problem

            IEnumerator assetsLoading = onAssetsLoaded();
            bool move = true;
            while (move)
            {
                try
                {
                    move = assetsLoading.MoveNext();
                } catch (Exception e)
                {
                    ObjectsCreator.causeCrash(e);
                    move = false;
                }
                yield return assetsLoading.Current;
            }
            yield break;
        }

        private static IEnumerator onAssetsLoaded()
        {
            ApiManager.onAssetsLoadingPre?.Invoke();

            int count = 15; //+1 ?
            yield return count;
            yield return "Checking installed mods...";
            checkCompabilities();
            yield return "Caching game assets...";
            AssetsStorage.cache();
            if (AssetsStorage.exception != null) throw AssetsStorage.exception;
            yield return "Initializing items...";
            GameRegisterManager.initializeGameItems();
            GameRegisterManager.initializeMultipleUsableItems();
            yield return "Initializing events...";
            GameRegisterManager.initializeGameEvents();
            yield return "Initializing vending machines...";
            GameRegisterManager.initializeVendingMachines();
            yield return "Initializing overlays...";
            GameRegisterManager.initializeOverlays();
            yield return "Initializing objects...";
            GameRegisterManager.initializeObjects();
            yield return "Initializing entities...";
            GameRegisterManager.initializeEntities();
            yield return "Initializing rooms...";
            GameRegisterManager.initializeRooms();
            yield return "Initializing builders...";
            GameRegisterManager.initializeObjectBuilders();
            yield return "Initializing posters...";
            GameRegisterManager.initializePosters();
            yield return "Initializing tips and words...";
            GameRegisterManager.initializeTipsAndWords();
            yield return "Adding characters tags...";
            GameRegisterManager.setCharactersTags();
            yield return "Overriding game prefabs...";
            AssetsStorage.overrideAssetsProperties();
            yield return "Integrating with other mods...";
            makeIntegration();
            yield return "All done!";

            ApiManager.onAssetsLoadingPost?.Invoke();

            yield break;
        }

        private static void registerLevelData(string name, int floorNum, CustomLevelObject levelObject)
        {
            floorNum++; //because texture man gives it as fucking index!
            foreach (string itemName in ObjectsStorage.WeightedItemObjects.Keys)
            {
                ItemSpawningData itemSpawnData = (ItemSpawningData)ObjectsStorage.SpawningData[itemName];

                //name = "INF" - infinity floors!
                //but... i allow to appear for objects on every floor and i don't need to use it...
                
                if (name == "END" && !itemSpawnData.EndlessMode) continue;

                if (name != "END" && itemSpawnData.BannedFloors.Contains(floorNum)) continue;

                WeightedItemObject weightedItemObject = ObjectsStorage.WeightedItemObjects[itemName];

                WeightedItem weightedItem = ObjectsStorage.WeightedItems[itemName];

                if (itemSpawnData.SpawnsOnShop && !levelObject.shopItems.Contains(weightedItemObject))
                {
                    levelObject.shopItems = levelObject.shopItems.AddToArray(weightedItemObject);
                }

                if (itemSpawnData.SpawnsOnRooms && !levelObject.potentialItems.Contains(weightedItemObject))
                {
                    //levelObject.items not longer using...
                    levelObject.potentialItems = levelObject.potentialItems.AddToArray(weightedItemObject);
                }

                if (itemSpawnData.SpawnsOnFieldTrips && !levelObject.fieldTripItems.Contains(weightedItem))
                {
                    levelObject.fieldTripItems.Add(weightedItem);
                }
            }

            foreach (string eventName in ObjectsStorage.Events.Keys)
            {
                BaseSpawningData spawningData = ObjectsStorage.SpawningData[eventName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floorNum)) continue;

                if (!levelObject.randomEvents.Contains(ObjectsStorage.WeightedEvents[eventName]))
                {
                    levelObject.randomEvents.Add(ObjectsStorage.WeightedEvents[eventName]);
                }
            }

            foreach (string builderName in ObjectsStorage.WeightedObjectBuilders.Keys)
            {
                BaseSpawningData spawningData = ObjectsStorage.SpawningData[builderName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floorNum)) continue;

                if (!levelObject.specialHallBuilders.Contains(ObjectsStorage.WeightedObjectBuilders[builderName]))
                {
                    levelObject.specialHallBuilders = levelObject.specialHallBuilders.AddToArray(ObjectsStorage.WeightedObjectBuilders[builderName]);
                }
            }

            foreach (string forcedBuilderName in ObjectsStorage.ForcedSpecialObjectBuilders.Keys)
            {
                BaseSpawningData spawningData = ObjectsStorage.SpawningData[forcedBuilderName];

                if (name == "END" && !spawningData.EndlessMode) continue;

                if (name != "END" && spawningData.BannedFloors.Contains(floorNum)) continue;

                levelObject.forcedSpecialHallBuilders = levelObject.forcedSpecialHallBuilders.AddToArray(ObjectsStorage.ForcedSpecialObjectBuilders[forcedBuilderName]);
            }

            foreach (WeightedPosterObject weightedPosterObject in ObjectsStorage.WeightedPosterObjects)
            {
                levelObject.posters = levelObject.posters.AddToArray(weightedPosterObject);
            }

            foreach (WeightedRoomAsset roomAsset in ObjectsStorage.WeightedRoomAssets)
            {
                levelObject.potentialFacultyRooms = levelObject.potentialExtraRooms.AddToArray(roomAsset);
            }
            
        }

        private void prepareSettingsMenu()
        {
            CustomOptionsCore.OnMenuInitialize += ModSettingsMenu.onMenuInit;
            CustomOptionsCore.OnMenuClose += ModSettingsMenu.onMenuClose;

            CustomOptionsCore.OnMenuInitialize += EmergencyButtonMenu.onMenuInit;
            CustomOptionsCore.OnMenuClose += EmergencyButtonMenu.onMenuClose;
        }

        private static void checkCompabilities()
        {
            if (AssetsHelper.modInstalled("baldi.basics.plus.advanced.endless.mod"))
            {
                throw new MessageException("Please uninstall baldi.basics.plus.advanced.endless.mod! This is obsolete.");
            }

            if (AssetsHelper.modInstalled("baldi.basics.plus.advanced.editor.mod"))
            {
                throw new MessageException("Please uninstall baldi.basics.plus.advanced.editor.mod! This is obsolete.");
            }

            if (MTM101BaldiDevAPI.Instance.Info.Metadata.Version < new Version("4.2.0.0"))
            {
                throw new MessageException("NO! USE API VERSION 4.2+!!!");
            }
        }

        private static void makeIntegration()
        {
            if (ModsIntegration.LevelEditorInstalled && ModsIntegration.LevelLoaderInstalled && editorIntegrationEnabled)
            {
                LevelEditorIntegration.onModAssetsLoaded();
            }
        }
    }
}
