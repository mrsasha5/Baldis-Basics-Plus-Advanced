using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced
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
                if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))// && !(ObjectsStorage.GameButtons[name] is PressurePlate))
                {
                    string key = "adv_" + name;
                    PlusLevelLoaderPlugin.Instance.prefabAliases.Add(key, plate.gameObject);
                }
            }

            //PlusLevelLoaderPlugin.Instance.buttons.Add("adv_pressure_plate", ObjectsStorage.GameButtons["plate"]);

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

            PlusLevelLoaderPlugin.Instance.npcAliases.Add("adv_criss_the_crystal", ObjectsStorage.Npcs["CrissTheCrystal"]);

            //Symbol Machine
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_symbol_machine", ObjectsStorage.Objects["symbol_machine"]);

            //Advanced Math Machine
            PlusLevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine",
                ObjectsStorage.Objects["advanced_math_machine"].GetComponent<AdvancedMathMachine>());
            PlusLevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine_corner",
                ObjectsStorage.Objects["advanced_math_machine_corner"].GetComponent<AdvancedMathMachine>());

            //Voting Ballot
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_voting_ballot", ObjectsStorage.Objects["voting_ballot"]);
            //Finish flag
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_farm_finish_flag", ObjectsStorage.Objects["farm_flag"]);
            //Farm sign
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_farm_sign1", ObjectsStorage.Objects["farm_sign1"]);

            //Gum Dispenser
            //PlusLevelLoaderPlugin.Instance.tileAliases.Add("adv_gum_dispenser", ObjectsStorage.Objects["gum_dispenser"].GetComponent<TileBasedObject>());

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                ObjectsStorage.RoomColors["English"],
                Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("EnglishClassMapBG", AssetsStorage.textures["adv_english_class_bg"])
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class_timer", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                ObjectsStorage.RoomColors["English"],
                Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("EnglishClassMapBG", AssetsStorage.textures["adv_english_class_bg"])
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_school_council_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("SchoolCouncil"),
                RoomType.Room,
                ObjectsStorage.RoomColors["SchoolCouncil"],
                Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "SchoolCouncilDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("SchoolCouncilMapBG", AssetsStorage.textures["adv_school_council_bg"])
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_advanced_class", new RoomSettings(
                RoomCategory.Class,//EnumExtensions.GetFromExtendedName<RoomCategory>("AdvancedClass"),
                RoomType.Room,
                ObjectsStorage.RoomColors["AdvancedClass"],
                Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "AdvancedClassDoorSet"),
                mapMaterial: RoomHelper.CreateMapMaterial("AdvancedClassMapBG", AssetsStorage.textures["adv_advanced_class_bg"])
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_corn_field", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("CornField"),
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
            //PlusLevelLoaderPlugin.Instance.roomSettings["adv_corn_field"].container =
            //    GameObject.Instantiate(PlusLevelLoaderPlugin.Instance.roomSettings["outside"].container);

            //RoomFunctionContainer cornContainer = PlusLevelLoaderPlugin.Instance.roomSettings["adv_corn_field"].container;

            //cornContainer.gameObject.ConvertToPrefab(true);
            //cornContainer.RemoveFunction(cornContainer.GetComponent<SkyboxRoomFunction>());

            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_english_ceiling", AssetsStorage.textures["adv_english_ceiling"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_english_wall", AssetsStorage.textures["adv_english_wall"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_english_floor", AssetsStorage.textures["adv_english_floor"]);

            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_advanced_class_ceiling", AssetsStorage.textures["adv_advanced_class_ceiling"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_advanced_class_floor", AssetsStorage.textures["adv_advanced_class_floor"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_advanced_class_wall", AssetsStorage.textures["adv_advanced_class_wall"]);

            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_school_council_wall", AssetsStorage.textures["adv_school_council_wall"]);

            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_basic_floor", AssetsHelper.LoadAsset<Texture2D>("BasicFloor"));

            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_corn_wall", AssetsHelper.LoadAsset<Texture2D>("Corn"));
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_corn_floor", AssetsHelper.LoadAsset<Texture2D>("ground2"));
        }

        private static void CreateFakeMarker(string name)
        {
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add(name, new GameObject(name));
            PlusLevelLoaderPlugin.Instance.prefabAliases[name].ConvertToPrefab(true);
        }
    }
}
