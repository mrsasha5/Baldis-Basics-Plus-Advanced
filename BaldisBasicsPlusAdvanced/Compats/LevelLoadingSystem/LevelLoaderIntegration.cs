using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Activities;
using BaldisBasicsPlusAdvanced.Game.NPCs.CrissTheCrystal;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BepInEx.Bootstrap;
using MTM101BaldAPI;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelLoadingSystem
{
    internal class LevelLoaderIntegration
    {

        private const string minVersion = "1.6.0.0";

        public static void Initialize()
        {
            if (Chainloader.PluginInfos[IntegrationManager.levelLoaderId].Metadata.Version < new Version(minVersion))
            {
                ObjectCreator.CauseCrash(new Exception("Level Loading system is outdated, please update it!"));
            }

            foreach (string objectName in ObjectStorage.ItemObjects.Keys)
            {
                string key = "adv_" + objectName;
                LevelLoaderPlugin.Instance.itemObjects.Add(key, ObjectStorage.ItemObjects[objectName]);
            }

            /*foreach (string vendingMachineName in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + vendingMachineName;
                LevelLoaderPlugin.Instance.basicObjects.Add(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject);
            }*/

            LevelLoaderPlugin.Instance.basicObjects.Add("adv_good_machine", 
                ObjectStorage.SodaMachines["GoodMachine"].gameObject);

            foreach (string name in ObjectStorage.Objects.Keys)
            {
                if (ObjectStorage.Objects[name].TryGetComponent(out BasePlate plate))
                {
                    string key = "adv_" + name;
                    LevelLoaderPlugin.Instance.basicObjects.Add(key, plate.gameObject);
                }
            
            }

            InitializeEvents();
            InitializeNpcs();
            InitializePosters();
            InitializeDoors();
            InitializeLights();
            InitializeObjects();
            InitializeActivities();
            InitializeStructures();
            InitializeRoomSettings();
            InitializeRoomTextureAliases();
        }

        private static void InitializeEvents()
        {
            LevelLoaderPlugin.Instance.randomEventAliases.Add("adv_disappearing_characters", ObjectStorage.Events["DisappearingCharacters"]);
            LevelLoaderPlugin.Instance.randomEventAliases.Add("adv_cold_school", ObjectStorage.Events["ColdSchool"]);
            LevelLoaderPlugin.Instance.randomEventAliases.Add("adv_portal_chaos", ObjectStorage.Events["PortalChaos"]);
            LevelLoaderPlugin.Instance.randomEventAliases.Add("adv_voting", ObjectStorage.Events["Voting"]);
        }

        private static void InitializeNpcs()
        {
            LevelLoaderPlugin.Instance.npcAliases.Add("adv_criss_the_crystal", ObjectStorage.Npcs["CrissTheCrystal"]);
        }

        private static void InitializePosters()
        {
            foreach (PosterObject poster in ObjectStorage.Posters)
            {
                LevelLoaderPlugin.Instance.posterAliases.Add(poster.name.ToLower().ReplaceFirst("_poster", ""), poster);
            }
        }

        private static void InitializeDoors()
        {
            LevelLoaderPlugin.Instance.windowObjects.Add("adv_big_hole",
                ObjectStorage.Npcs["CrissTheCrystal"].GetComponent<CrissTheCrystal>().windowObjectPre);
        }

        private static void InitializeLights()
        {
            LevelLoaderPlugin.Instance.lightTransforms.Add("adv_advanced_education_lamp",
                AssetHelper.LoadAsset<Transform>("AdvancedClassLampLight"));
        }

        private static void InitializeObjects()
        {
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_symbol_machine", ObjectStorage.Objects["symbol_machine"]);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_voting_ballot", ObjectStorage.Objects["voting_ballot"]);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_farm_finish_flag", ObjectStorage.Objects["farm_flag"]);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_farm_finish_points_flag", ObjectStorage.Objects["farm_points_flag"]);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_farm_sign1", ObjectStorage.Objects["farm_sign1"]);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_voting_ceiling_screen", ObjectStorage.Objects["voting_screen"]);
        }

        private static void InitializeStructures()
        {
            LevelLoaderPlugin.Instance.structureAliases.Add("adv_zipline", 
                new LoaderStructureData(ObjectStorage.StructureBuilders["Structure_Zipline"], 
                    new Dictionary<string, GameObject>()
                    {
                        { "hanger_white", ObjectStorage.Objects["zipline_hanger"] },
                        { "hanger_black", ObjectStorage.Objects["zipline_black_hanger"] }
                    }
                )
            );

            LevelLoaderPlugin.Instance.structureAliases.Add("adv_gum_dispenser",
                new LoaderStructureData(ObjectStorage.StructureBuilders["Structure_GumDispenser"],
                    new Dictionary<string, GameObject>()
                    {
                        { "gum_dispenser", ObjectStorage.Objects["gum_dispenser"] },
                        { "button", AssetStorage.gameButton.gameObject }
                    }
                )
            );

            LevelLoaderPlugin.Instance.structureAliases.Add("adv_noisy_plate",
                new LoaderStructureData(ObjectStorage.StructureBuilders["Structure_NoisyPlate"],
                    new Dictionary<string, GameObject>()
                    {
                        { "noisy_plate", ObjectStorage.Objects["noisy_plate"] }
                    }
                )
            );

            LevelLoaderPlugin.Instance.structureAliases.Add("adv_generic_plate",
                new LoaderStructureData(ObjectStorage.StructureBuilders["Structure_GenericPlate"],
                    new Dictionary<string, GameObject>()
                    {
                        { "invisibility_plate", ObjectStorage.Objects["invisibility_plate"] },
                        { "stealing_plate", ObjectStorage.Objects["stealing_plate"] },
                        { "bully_plate", ObjectStorage.Objects["bully_plate"] },
                        { "present_plate", ObjectStorage.Objects["present_plate"] },
                        { "slowdown_plate", ObjectStorage.Objects["slowdown_plate"] },
                        { "sugar_addiction_plate", ObjectStorage.Objects["sugar_addiction_plate"] },
                        { "protection_plate", ObjectStorage.Objects["protection_plate"] },
                        { "teleportation_plate", ObjectStorage.Objects["teleportation_plate"] },
                        { "fake_plate", ObjectStorage.Objects["fake_plate"] }
                    }
                )
            );

            LevelLoaderPlugin.Instance.structureAliases.Add("adv_kitchen_stove",
                new LoaderStructureData(ObjectStorage.StructureBuilders["Structure_KitchenStove"],
                    new Dictionary<string, GameObject>()
                    {
                        { "kitchen_stove", ObjectStorage.Objects["kitchen_stove"] }
                    }
                )
            );

            LevelLoaderPlugin.Instance.structureAliases.Add("adv_acceleration_plate",
                new LoaderStructureData(ObjectStorage.StructureBuilders["Structure_AccelerationPlate"],
                    new Dictionary<string, GameObject>()
                    {
                        { "acceleration_plate", ObjectStorage.Objects["acceleration_plate"] }
                    }
                )
            );

            LevelLoaderPlugin.Instance.structureAliases.Add("adv_pulley",
                new LoaderStructureData(ObjectStorage.StructureBuilders["Structure_Pulley"],
                    new Dictionary<string, GameObject>()
                    {
                        { "pulley", ObjectStorage.Objects["pulley"] }
                    }
                )
            );
        }

        private static void InitializeActivities()
        {
            LevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine",
                ObjectStorage.Objects["advanced_math_machine"].GetComponent<AdvancedMathMachine>());
            LevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine_corner",
                ObjectStorage.Objects["advanced_math_machine_corner"].GetComponent<AdvancedMathMachine>());
        }

        private static void InitializeRoomSettings()
        {
            LevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                ObjectStorage.RoomColors["English"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("EnglishClassMapBG", AssetStorage.textures["adv_english_class_bg"])
                ));

            LevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class_timer", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                ObjectStorage.RoomColors["English"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("EnglishClassMapBG", AssetStorage.textures["adv_english_class_bg"])
                ));

            LevelLoaderPlugin.Instance.roomSettings.Add("adv_school_council_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("SchoolCouncil"),
                RoomType.Room,
                ObjectStorage.RoomColors["SchoolCouncil"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "SchoolCouncilDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("SchoolCouncilMapBG", AssetStorage.textures["adv_school_council_bg"])
                ));

            LevelLoaderPlugin.Instance.roomSettings.Add("adv_advanced_class", new RoomSettings(
                RoomCategory.Class,
                RoomType.Room,
                ObjectStorage.RoomColors["AdvancedClass"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "AdvancedClassDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("AdvancedClassMapBG", AssetStorage.textures["adv_advanced_class_bg"])
                ));

            LevelLoaderPlugin.Instance.roomSettings.Add("adv_corn_field", new RoomSettings(
                RoomCategory.Special,
                RoomType.Room,
                ObjectStorage.RoomColors["CornField"],
                AssetHelper.LoadAsset<StandardDoorMats>("ClassDoorSet")
                ));

            LevelLoaderPlugin.Instance.roomSettings["adv_advanced_class"].container =
                AssetHelper.LoadAsset<RoomFunctionContainer>("ClassRoomFunction");

            LevelLoaderPlugin.Instance.roomSettings["adv_school_council_class"].container =
                ObjectStorage.RoomFunctionsContainers["CorruptedLightsFunction"];
            LevelLoaderPlugin.Instance.roomSettings["adv_english_class_timer"].container =
                ObjectStorage.RoomFunctionsContainers["EnglishClassTimerFunction"];

            LevelLoaderPlugin.Instance.roomSettings["adv_school_council_class"].container =
                ObjectStorage.RoomFunctionsContainers["SchoolCouncilFunction"];
            LevelLoaderPlugin.Instance.roomSettings["adv_corn_field"].container =
                UnityEngine.Object.Instantiate(LevelLoaderPlugin.Instance.roomSettings["outside"].container);

            //Corn Field room function container
            RoomFunctionContainer cornContainer = LevelLoaderPlugin.Instance.roomSettings["adv_corn_field"].container;
            cornContainer.name = "CornFieldFunctionContainer";
            cornContainer.gameObject.ConvertToPrefab(true);

            SkyboxRoomFunction skyboxFunc = cornContainer.GetComponent<SkyboxRoomFunction>();

            cornContainer.gameObject.ConvertToPrefab(true);
            cornContainer.RemoveFunction(cornContainer.GetComponent<StaminaBoostRoomFunction>());

            MeshRenderer[] renderers = skyboxFunc.skybox.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < 4; i++)
            {
                Vector3 pos = renderers[i].transform.localPosition;
                pos.y = 35f;
                renderers[i].transform.localScale = new Vector3(10f, 100f, 1f);
                renderers[i].transform.localPosition = pos;
            }
            Vector3 _pos = renderers[4].transform.localPosition;
            _pos.y = 85f;
            renderers[4].transform.localPosition = _pos;
        }

        private static void InitializeRoomTextureAliases()
        {
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_english_ceiling", AssetStorage.textures["adv_english_ceiling"]);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_english_wall", AssetStorage.textures["adv_english_wall"]);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_english_floor", AssetStorage.textures["adv_english_floor"]);

            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_advanced_class_ceiling", AssetStorage.textures["adv_advanced_class_ceiling"]);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_advanced_class_floor", AssetStorage.textures["adv_advanced_class_floor"]);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_advanced_class_wall", AssetStorage.textures["adv_advanced_class_wall"]);

            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_school_council_wall", AssetStorage.textures["adv_school_council_wall"]);

            LevelLoaderPlugin.Instance.roomTextureAliases.Add("adv_corn_wall",
                AssetHelper.TextureFromFile("Textures/Rooms/CornField/Adv_Thick_Corn_Wall.png"));
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_corn_floor", AssetHelper.LoadAsset<Texture2D>("ground2"));
        }

    }
}
