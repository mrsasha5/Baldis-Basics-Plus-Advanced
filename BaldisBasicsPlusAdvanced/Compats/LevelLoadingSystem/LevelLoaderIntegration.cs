﻿using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Rooms.Functions;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelLoadingSystem
{
    public class LevelLoaderIntegration
    {

        public static void Initialize()
        {
            foreach (string objectName in ObjectsStorage.ItemObjects.Keys)
            {
                string key = "adv_" + objectName;
                PlusLevelLoaderPlugin.Instance.itemObjects.Add(key, ObjectsStorage.ItemObjects[objectName]);
            }
            foreach (string vendingMachineName in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + vendingMachineName;
                PlusLevelLoaderPlugin.Instance.prefabAliases.Add(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject);
            }

            foreach (string name in ObjectsStorage.Objects.Keys)
            {
                if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                {
                    string key = "adv_" + name;
                    PlusLevelLoaderPlugin.Instance.prefabAliases.Add(key, plate.gameObject);
                }
            }

            List<string> markers = new List<string>()
            {
                "nonSafeCellMarker",
                "potentialDoorMarker",
                "forcedDoorMarker",
                "lightSpotMarker",
                "itemSpawnMarker"
            };

            if (!AssetsHelper.ModInstalled("pixelguy.pixelmodding.baldiplus.editorcustomrooms"))
            {
                foreach (string marker in markers)
                {
                    if (!PlusLevelLoaderPlugin.Instance.prefabAliases.ContainsKey(marker))
                    {
                        CreateFakeMarker(marker);
                    }
                }
            }

            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_trigger_no_plate_cooldown",
                ObjectsStorage.Triggers["no_plate_cooldown"].gameObject);
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_trigger_low_plate_unpress_time", 
                ObjectsStorage.Triggers["low_plate_unpress_time"].gameObject);

            PlusLevelLoaderPlugin.Instance.npcAliases.Add("adv_criss_the_crystal", ObjectsStorage.Npcs["CrissTheCrystal"]);

            //Symbol Machine
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_symbol_machine", ObjectsStorage.Objects["symbol_machine"]);

            //Advanced Math Machine
            PlusLevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine",
                ObjectsStorage.Objects["advanced_math_machine"].GetComponent<AdvancedMathMachine>());
            PlusLevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine_corner",
                ObjectsStorage.Objects["advanced_math_machine_corner"].GetComponent<AdvancedMathMachine>());

            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_voting_ballot", ObjectsStorage.Objects["voting_ballot"]);
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_farm_finish_flag", ObjectsStorage.Objects["farm_flag"]);
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_farm_finish_points_flag", ObjectsStorage.Objects["farm_points_flag"]);
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_farm_sign1", ObjectsStorage.Objects["farm_sign1"]);

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                ObjectsStorage.RoomColors["English"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("EnglishClassMapBG", AssetsStorage.textures["adv_english_class_bg"])
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class_timer", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                ObjectsStorage.RoomColors["English"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("EnglishClassMapBG", AssetsStorage.textures["adv_english_class_bg"])
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_school_council_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("SchoolCouncil"),
                RoomType.Room,
                ObjectsStorage.RoomColors["SchoolCouncil"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "SchoolCouncilDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("SchoolCouncilMapBG", AssetsStorage.textures["adv_school_council_bg"])
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_advanced_class", new RoomSettings(
                RoomCategory.Class,//EnumExtensions.GetFromExtendedName<RoomCategory>("AdvancedClass"),
                RoomType.Room,
                ObjectsStorage.RoomColors["AdvancedClass"],
                Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "AdvancedClassDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("AdvancedClassMapBG", AssetsStorage.textures["adv_advanced_class_bg"])
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_corn_field", new RoomSettings(
                RoomCategory.Special,
                RoomType.Room,
                ObjectsStorage.RoomColors["CornField"],
                AssetsHelper.LoadAsset<StandardDoorMats>("ClassDoorSet")
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings["adv_advanced_class"].container = 
                AssetsHelper.LoadAsset<RoomFunctionContainer>("ClassRoomFunction");

            PlusLevelLoaderPlugin.Instance.roomSettings["adv_school_council_class"].container = 
                ObjectsStorage.RoomFunctionsContainers["CorruptedLightsFunction"];
            PlusLevelLoaderPlugin.Instance.roomSettings["adv_english_class_timer"].container = 
                ObjectsStorage.RoomFunctionsContainers["EnglishClassTimerFunction"];

            
            PlusLevelLoaderPlugin.Instance.roomSettings["adv_school_council_class"].container =
                ObjectsStorage.RoomFunctionsContainers["SchoolCouncilFunction"];
            PlusLevelLoaderPlugin.Instance.roomSettings["adv_corn_field"].container =
                GameObject.Instantiate(PlusLevelLoaderPlugin.Instance.roomSettings["outside"].container);

            //Corn Field room function container
            RoomFunctionContainer cornContainer = PlusLevelLoaderPlugin.Instance.roomSettings["adv_corn_field"].container;
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

            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_english_ceiling", AssetsStorage.textures["adv_english_ceiling"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_english_wall", AssetsStorage.textures["adv_english_wall"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_english_floor", AssetsStorage.textures["adv_english_floor"]);

            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_advanced_class_ceiling", AssetsStorage.textures["adv_advanced_class_ceiling"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_advanced_class_floor", AssetsStorage.textures["adv_advanced_class_floor"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_advanced_class_wall", AssetsStorage.textures["adv_advanced_class_wall"]);

            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_school_council_wall", AssetsStorage.textures["adv_school_council_wall"]);

            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_basic_floor", AssetsHelper.LoadAsset<Texture2D>("BasicFloor"));

            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_corn_wall", 
                AssetsHelper.TextureFromFile("Textures/Rooms/CornField/adv_thick_corn_wall.png"));
            PlusLevelLoaderPlugin.Instance.textureAliases.Add(
                "adv_corn_floor", AssetsHelper.LoadAsset<Texture2D>("ground2"));
        }

        private static void CreateFakeMarker(string name)
        {
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add(name, new GameObject(name));
            PlusLevelLoaderPlugin.Instance.prefabAliases[name].ConvertToPrefab(true);
        }
    }
}
