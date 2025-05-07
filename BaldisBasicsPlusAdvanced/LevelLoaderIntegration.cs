using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
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
            foreach (string objectName in ObjectsStorage.ItemsObjects.Keys)
            {
                string key = "adv_" + objectName;
                PlusLevelLoaderPlugin.Instance.itemObjects.Add(key, ObjectsStorage.ItemsObjects[objectName]);
            }
            foreach (string vendingMachineName in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + vendingMachineName;
                PlusLevelLoaderPlugin.Instance.prefabAliases.Add(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject);
            }

            foreach (string name in ObjectsStorage.GameButtons.Keys)
            {
                if (ObjectsStorage.GameButtons[name] is PlateBase)// && !(ObjectsStorage.GameButtons[name] is PressurePlate))
                {
                    string key = "adv_" + name;
                    PlusLevelLoaderPlugin.Instance.prefabAliases.Add(key, ObjectsStorage.GameButtons[name].gameObject);
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

            //Symbol Machine
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_symbol_machine", ObjectsStorage.Objects["symbol_machine"]);

            //Advanced Math Machine
            PlusLevelLoaderPlugin.Instance.activityAliases.Add("adv_advanced_math_machine",
                ObjectsStorage.Objects["advanced_math_machine"].GetComponent<AdvancedMathMachine>());

            //Voting Ballot
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_voting_ballot", ObjectsStorage.Objects["voting_ballot"]);

            //Gum Dispenser
            PlusLevelLoaderPlugin.Instance.tileAliases.Add("adv_gum_dispenser", ObjectsStorage.Objects["gum_dispenser"].GetComponent<TileBasedObject>());

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                Color.white,
                Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet")
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings.Add("adv_english_class_timer", new RoomSettings(
                EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                RoomType.Room,
                Color.white,
                Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet")
                ));

            PlusLevelLoaderPlugin.Instance.roomSettings["adv_english_class_timer"].container = ObjectsStorage.RoomFunctionsContainers["EnglishClassTimerFunction"];

            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_english_ceiling", AssetsStorage.textures["adv_english_ceiling"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_english_wall", AssetsStorage.textures["adv_english_wall"]);
            PlusLevelLoaderPlugin.Instance.textureAliases.Add("adv_english_floor", AssetsStorage.textures["adv_english_floor"]);

            ModsIntegration.AddPluginAsIntegrated(PlusLevelLoaderPlugin.Instance);

        }

        private static void CreateFakeMarker(string name)
        {
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add(name, new GameObject(name));
            PlusLevelLoaderPlugin.Instance.prefabAliases[name].ConvertToPrefab(true);
        }
    }
}
