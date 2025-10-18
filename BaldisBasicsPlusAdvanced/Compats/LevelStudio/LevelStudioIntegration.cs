using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GenericPlate;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.KitchenStove;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyFacultyPlate;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Builders;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusLevelStudio.Editor.Tools;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio
{
    internal class LevelStudioIntegration : CompatibilityModule
    {

        internal const string standardMsg_StructureVersionException = 
            "Incompatible structure format: saved maps on new format version can't be loaded on previous ones.";

        private static List<StructureBuilderPrefabReference> visualPrefabs;

        public static List<GameObject> GetVisualPrefabsFrom(string type)
        {
            List<GameObject> prefabs = new List<GameObject>();

            for (int i = 0; i < visualPrefabs.Count; i++)
            {
                if (visualPrefabs[i].builder == type)
                {
                    prefabs.Add(visualPrefabs[i].visualPrefab);
                }
            }

            return prefabs;
        }

        public static GameObject GetVisualPrefab(string type, string prefab)
        {
            for (int i = 0; i < visualPrefabs.Count; i++)
            {
                if (visualPrefabs[i].builder == type && visualPrefabs[i].prefab == prefab)
                {
                    return visualPrefabs[i].visualPrefab;
                }
            }

            throw new Exception("Visual prefab reference is not in the list.");
        }

        public static bool ContainsStructureVisualPrefab(string type, string prefab)
        {
            for (int i = 0; i < visualPrefabs.Count; i++)
            {
                if (visualPrefabs[i].builder == type && visualPrefabs[i].prefab == prefab)
                {
                    return true;
                }
            }

            return false;
        }

        private static GameObject AddStructureVisualPrefab(string type, string prefab, GameObject obj)
        {
            GameObject instance = EditorInterface.AddStructureGenericVisual("_99_seconds", obj);
            LevelStudioPlugin.Instance.genericStructureDisplays.Remove("_99_seconds");

            visualPrefabs.Add(new StructureBuilderPrefabReference()
            {
                builder = type,
                prefab = prefab,
                visualPrefab = instance
            });

            return instance;
        }

        public LevelStudioIntegration() : base()
        {
            guid = "mtm101.rulerp.baldiplus.levelstudio";
            priority = 127;
            versionInfo = new VersionInfo(this);

            CreateConfigValue("Level Studio",
                "Adds support for Level Studio like new objects, structures and other content which can be used on your levels!");
        }

        public override bool IsIntegrable()
        {
            return base.IsIntegrable() && IntegrationManager.LevelLoaderInstalled;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeVisuals();
            InitializeStructureLocs();
            EditorInterfaceModes.AddModeCallback(InitializeTools);
            EditorLevelData.AddDefaultTextureAction(InitializeTextureContainers);
        }

        private static void InitializeStructureLocs()
        {
            LevelStudioPlugin.Instance.structureTypes.Add("adv_zipline", typeof(ZiplineStructureLocation));
            LevelStudioPlugin.Instance.structureTypes.Add("adv_gum_dispenser", typeof(GumDispenserStructureLocation));
            LevelStudioPlugin.Instance.structureTypes.Add("adv_noisy_plate", typeof(NoisyPlateStructureLocation));
            LevelStudioPlugin.Instance.structureTypes.Add("adv_generic_plate", typeof(GenericPlateStructureLocation));
            LevelStudioPlugin.Instance.structureTypes.Add("adv_kitchen_stove", typeof(KitchenStoveStructureLocation));
        }

        private static void InitializeVisuals()
        {
            #region Selectable Textures
            
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_english_wall");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_english_ceiling");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_english_floor");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_school_council_wall");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_advanced_class_floor");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_advanced_class_wall");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_advanced_class_ceiling");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_corn_wall");

            #endregion

            visualPrefabs = new List<StructureBuilderPrefabReference>();

            #region Zipline Visuals

            EditorInterface.AddStructureGenericVisual("adv_zipline_pillar", Structure_Zipline.ceilingPillarPre);
            AddStructureVisualPrefab("adv_zipline", "hanger_white", ObjectsStorage.Objects["zipline_hanger"]);
            AddStructureVisualPrefab("adv_zipline", "hanger_black", ObjectsStorage.Objects["zipline_black_hanger"]);

            foreach (GameObject zipline in GetVisualPrefabsFrom("adv_zipline"))
            {
                zipline.GetComponent<SphereCollider>().center = Vector3.up * 2.5f;

                SettingsComponent comp = zipline.AddComponent<SettingsComponent>();
                comp.offset = Vector3.up * 10f;

                zipline.AddComponent<EditorSettingsableComponent>();
            }

            #endregion

            #region Gum Dispenser Visual

            BoxCollider gumDispCollider = AddStructureVisualPrefab("adv_gum_dispenser", "gum_dispenser", ObjectsStorage.Objects["gum_dispenser"])
                .AddComponent<BoxCollider>();
            gumDispCollider.size = new Vector3(8f, 8f, 1f);
            gumDispCollider.center = (Vector3.forward + Vector3.up) * 5f;
            gumDispCollider.gameObject.AddComponent<SettingsComponent>().offset = Vector3.up * 15f;

            #endregion

            #region Noisy Plate Visual

            AddStructureVisualPrefab("adv_noisy_plate", "noisy_plate", ObjectsStorage.Objects["noisy_plate"])
                .gameObject.AddComponent<SettingsComponent>().offset = Vector3.up * 15f;

            #endregion

            #region Kitchen Stove Visual

            BoxCollider stoveCollider = 
                AddStructureVisualPrefab("adv_kitchen_stove", "kitchen_stove", ObjectsStorage.Objects["kitchen_stove"])
                    .GetComponent<BoxCollider>();

            stoveCollider.size = new Vector3(10f, 1f, 10f);
            stoveCollider.center = Vector3.zero;

            stoveCollider.gameObject.AddComponent<SettingsComponent>().offset = Vector3.up * 15f;

            #endregion

            #region Generic Object Visuals

            foreach (string name in ObjectsStorage.SodaMachines.Keys)
            {
                EditorInterface.AddObjectVisual("adv_" + name, ObjectsStorage.SodaMachines[name].gameObject, true);
            }

            foreach (string name in ObjectsStorage.Objects.Keys)
            {
                if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                {
                    EditorInterface.AddObjectVisual("adv_" + name, ObjectsStorage.Objects[name], true); //This one for rooms only
                    if (!ContainsStructureVisualPrefab("adv_generic_plate", name))
                    {
                        GameObject plateVisual = AddStructureVisualPrefab("adv_generic_plate", name, plate.gameObject);

                        BoxCollider boxColl = plateVisual.GetComponent<BoxCollider>();
                        boxColl.size = new Vector3(10f, 1f, 10f);
                        boxColl.center = Vector3.zero;

                        plateVisual.AddComponent<SettingsComponent>().offset = Vector3.up * 15f;   
                    }
                }
            }

            EditorInterface.AddNPCVisual("adv_criss_the_crystal", ObjectsStorage.Npcs["CrissTheCrystal"]);

            EditorInterface.AddObjectVisual("adv_symbol_machine", ObjectsStorage.Objects["symbol_machine"], true);

            EditorInterface.AddObjectVisual("adv_voting_ballot", ObjectsStorage.Objects["voting_ballot"], true);

            GameObject advancedMMVisual = 
                EditorInterface.AddActivityVisual("adv_advanced_math_machine", ObjectsStorage.Objects["advanced_math_machine"]);
            BoxCollider mmCollider = ObjectsStorage.Objects["advanced_math_machine"].transform.Find("Model").GetComponent<BoxCollider>();

            GameObject advancedCornerMMVisual =
                EditorInterface.AddActivityVisual("adv_advanced_math_machine_corner", ObjectsStorage.Objects["advanced_math_machine_corner"]);
            BoxCollider cornerMmCollider = 
                ObjectsStorage.Objects["advanced_math_machine_corner"].transform.Find("Model").GetComponent<BoxCollider>();

            BoxCollider ammCollider = advancedMMVisual.AddComponent<BoxCollider>();
            ammCollider.size = mmCollider.size;
            ammCollider.center = mmCollider.center;

            BoxCollider cornerAmmCollider = advancedCornerMMVisual.AddComponent<BoxCollider>();
            cornerAmmCollider.size = cornerMmCollider.size;
            cornerAmmCollider.center = cornerMmCollider.center;

            EditorInterface.AddObjectVisual("adv_farm_finish_flag", ObjectsStorage.Objects["farm_flag"], true)
                .GetComponentInChildren<SpriteRenderer>().RemoveBillboard();
            EditorInterface.AddObjectVisual("adv_farm_finish_points_flag", ObjectsStorage.Objects["farm_points_flag"], true)
                .GetComponentInChildren<SpriteRenderer>().RemoveBillboard();

            EditorInterface.AddObjectVisualWithCustomBoxCollider("adv_farm_sign1", ObjectsStorage.Objects["farm_sign1"],
                Vector3.one * 7.5f, Vector3.zero);

            EditorInterface.AddObjectVisualWithCustomBoxCollider("adv_trigger_no_plate_cooldown",
                ObjectsStorage.Triggers["no_plate_cooldown"].gameObject, Vector3.one * 5f, Vector3.zero);

            EditorInterface.AddObjectVisualWithCustomBoxCollider("adv_trigger_low_plate_unpress_time", 
                ObjectsStorage.Triggers["low_plate_unpress_time"].gameObject, Vector3.one * 5f, Vector3.zero);

            #endregion

            EditorInterface.AddRoomVisualManager<OutsideRoomVisualManager>("adv_corn_field");
        }

        private static void InitializeTools(EditorMode mode, bool vanillaCompliant)
        {
            #region Tools Info

            Dictionary<string, string> platesInfo = new Dictionary<string, string>()
            {
                { "invisibility_plate", "adv_editor_invisibility_plate.png" },
                { "stealing_plate", "adv_editor_stealing_plate.png" },
                { "bully_plate", "adv_editor_bully_plate.png" },
                { "present_plate", "adv_editor_present_plate.png" },
                { "slowdown_plate", "adv_editor_slowdown_plate.png" },
                { "sugar_addiction_plate", "adv_editor_sugar_addiction_plate.png" },
                { "protection_plate", "adv_editor_protection_plate.png" },
                { "teleportation_plate", "adv_editor_teleportation_plate.png" },
                { "fake_plate", "adv_editor_fake_plate.png" },
            };

            #endregion

            EditorInterfaceModes.AddToolToCategory(mode, "npcs",
                    new NPCTool("adv_criss_the_crystal", AssetsStorage.sprites["adv_editor_criss_the_crystal"]));

            foreach (string objectName in ObjectsStorage.ItemObjects.Keys)
            {
                string key = "adv_" + objectName;
                EditorInterfaceModes.AddToolToCategory(mode, "items", 
                    new ItemTool(key));
            }

#warning Rename this one in dictionary and include in filter fix
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectTool("adv_GoodMachine", 
                    AssetsHelper.SpriteFromFile("Compats/LevelStudio/Textures/Objects/adv_editor_GoodMachine.png")));

            //Plates are supposed to be used in some premades, so they still exist as objects in Level Studio as well
            if (mode.id == "rooms")
            {
                foreach (string name in ObjectsStorage.Objects.Keys)
                {
                    if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                    {
                        string key = "adv_" + name;

                        if (!platesInfo.ContainsKey(name)) continue;

                        EditorInterfaceModes.AddToolToCategory(mode, "objects",
                            new ObjectTool(key, 
                                AssetsHelper.SpriteFromFile("Compats/LevelStudio/Textures/Structures/" + platesInfo[name])));
                    }
                }
            }
            
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectTool("adv_symbol_machine", AssetsStorage.sprites["adv_editor_symbol_machine"]));

            EditorInterfaceModes.AddToolToCategory(mode, "activities",
                new ActivityTool("adv_advanced_math_machine", AssetsStorage.sprites["adv_editor_advanced_math_machine"], 0f));

            EditorInterfaceModes.AddToolToCategory(mode, "activities",
                new ActivityTool("adv_advanced_math_machine_corner", AssetsStorage.sprites["adv_editor_advanced_math_machine_corner"], 0f));

            EditorInterfaceModes.AddToolToCategory(mode, "objects",
                new ObjectTool("adv_voting_ballot", AssetsStorage.sprites["adv_editor_voting_ballot"]));

            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectToolNoRotation("adv_farm_finish_flag", AssetsStorage.sprites["adv_editor_finish_flag"], 5f));
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectToolNoRotation("adv_farm_finish_points_flag", AssetsStorage.sprites["adv_editor_finish_points_flag"], 5f));
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectToolNoRotation("adv_farm_sign1", AssetsStorage.sprites["adv_editor_corn_sign1"], 5f));
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectToolNoRotation("adv_trigger_no_plate_cooldown", AssetsStorage.sprites["adv_editor_no_cooldown_plate"], 5f));
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectToolNoRotation("adv_trigger_low_plate_unpress_time", AssetsStorage.sprites["adv_editor_low_unpress_time"], 5f));

            EditorInterfaceModes.AddToolToCategory(mode, "structures", 
                new ZiplineTool("adv_zipline", "hanger_white", 
                    AssetsStorage.sprites["adv_editor_zipline_white"]));
            EditorInterfaceModes.AddToolToCategory(mode, "structures",
                new ZiplineTool("adv_zipline", "hanger_black", 
                    AssetsStorage.sprites["adv_editor_zipline_black"]));
            EditorInterfaceModes.AddToolToCategory(mode, "structures", 
                new GumDispenserTool("adv_gum_dispenser", "gum_dispenser", 
                    AssetsStorage.sprites["adv_editor_gum_dispenser"]));
            EditorInterfaceModes.AddToolToCategory(mode, "structures",
                new NoisyPlateTool("adv_noisy_plate", "noisy_plate",
                    AssetsStorage.sprites["adv_editor_noisy_plate"]));

            foreach (KeyValuePair<string, string> pair in platesInfo)
            {
                EditorInterfaceModes.AddToolToCategory(
                    mode, "structures", new GenericPlateTool("adv_generic_plate", pair.Key,
                        AssetsHelper.SpriteFromFile("Compats/LevelStudio/Textures/Structures/" + pair.Value)));
            }

            EditorInterfaceModes.AddToolToCategory(mode, "structures", 
                new KitchenStoveTool("adv_kitchen_stove", "kitchen_stove",
                    AssetsHelper.SpriteFromFile("Compats/LevelStudio/Textures/Structures/adv_editor_stove.png")));

            EditorInterfaceModes.AddToolToCategory(mode, "rooms", 
                new RoomTool("adv_english_class", AssetsStorage.sprites["adv_editor_english_floor"]));
            EditorInterfaceModes.AddToolToCategory(mode, "rooms", 
                new RoomTool("adv_english_class_timer", AssetsStorage.sprites["adv_editor_english_floor_timer"]));
            EditorInterfaceModes.AddToolToCategory(mode, "rooms", 
                new RoomTool("adv_school_council_class", AssetsStorage.sprites["adv_editor_school_council_floor"]));
            EditorInterfaceModes.AddToolToCategory(mode, "rooms", 
                new RoomTool("adv_advanced_class", AssetsStorage.sprites["adv_editor_advanced_class_floor"]));
            EditorInterfaceModes.AddToolToCategory(mode, "rooms", 
                new RoomTool("adv_corn_field", AssetsStorage.sprites["adv_editor_corn_field"]));
        }

        private static void InitializeTextureContainers(Dictionary<string, TextureContainer> containers)
        {
            containers.Add("adv_english_class", 
                new TextureContainer("adv_english_floor", "adv_english_wall", "adv_english_ceiling"));
            containers.Add("adv_english_class_timer",
                new TextureContainer(containers["adv_english_class"]));
            containers.Add("adv_school_council_class", 
                new TextureContainer("BasicFloor", "adv_school_council_wall", "Ceiling"));
            containers.Add("adv_advanced_class", 
                new TextureContainer("adv_advanced_class_floor", "adv_advanced_class_wall", "adv_advanced_class_ceiling"));
            containers.Add("adv_corn_field", 
                new TextureContainer("adv_corn_floor", "adv_corn_wall", "None"));
        }

        public static void LoadEditorAssets()
        {
            string texBase = "Compats/LevelStudio/Textures/";

            AssetsStorage.LoadModSprite("adv_editor_zipline_white",
                texBase + "Structures/adv_editor_zipline_white.png");
            AssetsStorage.LoadModSprite("adv_editor_zipline_black",
                texBase + "Structures/adv_editor_zipline_black.png");
            AssetsStorage.LoadModSprite("adv_editor_gum_dispenser",
                texBase + "Structures/adv_editor_gum_dispenser.png");

            AssetsStorage.LoadModSprite("adv_editor_criss_the_crystal",
                texBase + "NPCs/adv_editor_criss_the_crystal.png");

            AssetsStorage.LoadModSprite("adv_editor_corn_sign1",
                texBase + "Objects/adv_editor_corn_sign1.png");
            AssetsStorage.LoadModSprite("adv_editor_finish_flag",
                texBase + "Objects/adv_editor_finish_flag.png");
            AssetsStorage.LoadModSprite("adv_editor_finish_points_flag",
                texBase + "Objects/adv_editor_finish_points_flag.png");

            AssetsStorage.LoadModSprite("adv_editor_acceleration_plate",
                texBase + "Structures/adv_editor_acceleration_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_noisy_plate",
                texBase + "Structures/adv_editor_noisy_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_safety_trapdoor",
                texBase + "Structures/adv_editor_safety_trapdoor.png");
            AssetsStorage.LoadModSprite("adv_editor_voting_ballot",
                texBase + "Objects/adv_editor_voting_ballot.png");
            AssetsStorage.LoadModSprite("adv_editor_advanced_math_machine",
                texBase + "Objects/adv_editor_activity_advanced_math_machine.png");
            AssetsStorage.LoadModSprite("adv_editor_advanced_math_machine_corner",
                texBase + "Objects/adv_editor_activity_advanced_math_machine_corner.png");

            AssetsStorage.LoadModSprite("adv_editor_symbol_machine",
                texBase + "Objects/adv_editor_symbol_machine.png");
            AssetsStorage.LoadModSprite("adv_editor_english_floor",
                texBase + "Rooms/adv_room_english.png");
            AssetsStorage.LoadModSprite("adv_editor_school_council_floor",
                texBase + "Rooms/adv_room_school_council.png");
            AssetsStorage.LoadModSprite("adv_editor_english_floor_timer",
                texBase + "Rooms/adv_room_english_timer.png");
            AssetsStorage.LoadModSprite("adv_editor_advanced_class_floor",
                texBase + "Rooms/adv_room_advanced.png");
            AssetsStorage.LoadModSprite("adv_editor_corn_field",
                texBase + "Rooms/adv_room_corn_field.png");

            AssetsStorage.LoadModSprite("adv_editor_no_cooldown_plate",
                texBase + "Objects/adv_editor_no_cooldown.png");
            AssetsStorage.LoadModSprite("adv_editor_low_unpress_time",
                texBase + "Objects/adv_editor_low_unpress_time.png");
        }
    }
}
