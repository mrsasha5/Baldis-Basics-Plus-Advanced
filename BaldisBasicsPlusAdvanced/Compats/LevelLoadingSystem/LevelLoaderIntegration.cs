using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Activities;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelLoadingSystem
{
    internal class LevelLoaderIntegration
    {

        public static void Initialize()
        {
            foreach (string objectName in ObjectsStorage.ItemObjects.Keys)
            {
                string key = "adv_" + objectName;
                LevelLoaderPlugin.Instance.itemObjects.Add(key, ObjectsStorage.ItemObjects[objectName]);
            }
            foreach (string vendingMachineName in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + vendingMachineName;
                LevelLoaderPlugin.Instance.basicObjects.Add(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject);
            }

            foreach (string name in ObjectsStorage.Objects.Keys)
            {
                if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                {
                    string key = "adv_" + name;
                    LevelLoaderPlugin.Instance.basicObjects.Add(key, plate.gameObject);
                }
            }

            LevelLoaderPlugin.Instance.basicObjects.Add("adv_trigger_no_plate_cooldown",
                ObjectsStorage.Triggers["no_plate_cooldown"].gameObject);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_trigger_low_plate_unpress_time",
                ObjectsStorage.Triggers["low_plate_unpress_time"].gameObject);

            LevelLoaderPlugin.Instance.npcAliases.Add("adv_criss_the_crystal", ObjectsStorage.Npcs["CrissTheCrystal"]);

            //Symbol Machine
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_symbol_machine", ObjectsStorage.Objects["symbol_machine"]);

            LevelLoaderPlugin.Instance.basicObjects.Add("adv_voting_ballot", ObjectsStorage.Objects["voting_ballot"]);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_farm_finish_flag", ObjectsStorage.Objects["farm_flag"]);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_farm_finish_points_flag", ObjectsStorage.Objects["farm_points_flag"]);
            LevelLoaderPlugin.Instance.basicObjects.Add("adv_farm_sign1", ObjectsStorage.Objects["farm_sign1"]);

            InitializeActivities();

            InitializeStructures();

            InitializeRoomSettings();

            InitializeRoomTextureAliases();
        }

        private static void InitializeStructures()
        {
            LevelLoaderPlugin.Instance.structureAliases.Add("adv_zipline", 
                new LoaderStructureData(ObjectsStorage.StructureBuilders["Structure_Zipline"], 
                    new Dictionary<string, GameObject>()
                    {
                        { "hanger_white", ObjectsStorage.Objects["zipline_hanger"] },
                        { "hanger_black", ObjectsStorage.Objects["zipline_black_hanger"] }
                    }
                )
            );
        }

        private static void InitializeActivities()
        {
            LevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine",
                ObjectsStorage.Objects["advanced_math_machine"].GetComponent<AdvancedMathMachine>());
            LevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine_corner",
                ObjectsStorage.Objects["advanced_math_machine_corner"].GetComponent<AdvancedMathMachine>());
        }

        private static void InitializeRoomSettings()
        {
            LevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                ObjectsStorage.RoomColors["English"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("EnglishClassMapBG", AssetsStorage.textures["adv_english_class_bg"])
                ));

            LevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class_timer", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                ObjectsStorage.RoomColors["English"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("EnglishClassMapBG", AssetsStorage.textures["adv_english_class_bg"])
                ));

            LevelLoaderPlugin.Instance.roomSettings.Add("adv_school_council_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("SchoolCouncil"),
                RoomType.Room,
                ObjectsStorage.RoomColors["SchoolCouncil"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "SchoolCouncilDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("SchoolCouncilMapBG", AssetsStorage.textures["adv_school_council_bg"])
                ));

            LevelLoaderPlugin.Instance.roomSettings.Add("adv_advanced_class", new RoomSettings(
                RoomCategory.Class,
                RoomType.Room,
                ObjectsStorage.RoomColors["AdvancedClass"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "AdvancedClassDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("AdvancedClassMapBG", AssetsStorage.textures["adv_advanced_class_bg"])
                ));

            LevelLoaderPlugin.Instance.roomSettings.Add("adv_corn_field", new RoomSettings(
                RoomCategory.Special,
                RoomType.Room,
                ObjectsStorage.RoomColors["CornField"],
                AssetsHelper.LoadAsset<StandardDoorMats>("ClassDoorSet")
                ));

            LevelLoaderPlugin.Instance.roomSettings["adv_advanced_class"].container =
                AssetsHelper.LoadAsset<RoomFunctionContainer>("ClassRoomFunction");

            LevelLoaderPlugin.Instance.roomSettings["adv_school_council_class"].container =
                ObjectsStorage.RoomFunctionsContainers["CorruptedLightsFunction"];
            LevelLoaderPlugin.Instance.roomSettings["adv_english_class_timer"].container =
                ObjectsStorage.RoomFunctionsContainers["EnglishClassTimerFunction"];

            LevelLoaderPlugin.Instance.roomSettings["adv_school_council_class"].container =
                ObjectsStorage.RoomFunctionsContainers["SchoolCouncilFunction"];
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
                "adv_english_ceiling", AssetsStorage.textures["adv_english_ceiling"]);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_english_wall", AssetsStorage.textures["adv_english_wall"]);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_english_floor", AssetsStorage.textures["adv_english_floor"]);

            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_advanced_class_ceiling", AssetsStorage.textures["adv_advanced_class_ceiling"]);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_advanced_class_floor", AssetsStorage.textures["adv_advanced_class_floor"]);
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_advanced_class_wall", AssetsStorage.textures["adv_advanced_class_wall"]);

            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_school_council_wall", AssetsStorage.textures["adv_school_council_wall"]);

            LevelLoaderPlugin.Instance.roomTextureAliases.Add("adv_corn_wall",
                AssetsHelper.TextureFromFile("Textures/Rooms/CornField/adv_thick_corn_wall.png"));
            LevelLoaderPlugin.Instance.roomTextureAliases.Add(
                "adv_corn_floor", AssetsHelper.LoadAsset<Texture2D>("ground2"));
        }

    }
}
