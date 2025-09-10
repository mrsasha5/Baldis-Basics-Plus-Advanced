using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusLevelStudio.Editor.Tools;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio
{
    public class LevelStudioIntegration : CompatibilityModule
    {

        public LevelStudioIntegration() : base()
        {
            guid = "mtm101.rulerp.baldiplus.levelstudio";
            priority = 127;
            versionInfo = new VersionInfo(this);

            CreateConfigValue("Level Studio",
                "Adds support for Level Studio like new objects, activities and other content which can be used on your levels!");
        }

        public override bool IsIntegrable()
        {
            return base.IsIntegrable() && IntegrationManager.LevelLoaderInstalled;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeVisuals();
            EditorInterfaceModes.AddModeCallback(InitializeTools);
            EditorLevelData.AddDefaultTextureAction(InitializeTextureContainers);
        }

        private static void InitializeVisuals()
        {
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_corn_wall");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_english_wall");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_english_ceiling");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_school_council_wall");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_advanced_class_floor");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_advanced_class_wall");
            LevelStudioPlugin.Instance.selectableTextures.Add("adv_advanced_class_ceiling");

            foreach (string name in ObjectsStorage.SodaMachines.Keys)
            {
                EditorInterface.AddObjectVisual("adv_" + name, ObjectsStorage.SodaMachines[name].gameObject, true);
            }

            foreach (string name in ObjectsStorage.Objects.Keys)
            {
                if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                {
                    EditorInterface.AddObjectVisual("adv_" + name, ObjectsStorage.Objects[name], true);
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

            EditorInterface.AddObjectVisual("adv_farm_finish_flag", ObjectsStorage.Objects["farm_flag"], true);
            EditorInterface.AddObjectVisual("adv_farm_finish_points_flag", ObjectsStorage.Objects["farm_points_flag"], true);

            InitializeBoxCollider(EditorInterface.AddObjectVisual("adv_farm_sign1", ObjectsStorage.Objects["farm_sign1"], false),
                Vector3.one * 0.75f,
                    Vector3.zero);

            InitializeBoxCollider(EditorInterface.AddObjectVisual("adv_trigger_no_plate_cooldown", 
                ObjectsStorage.Triggers["no_plate_cooldown"].gameObject, false),
                    Vector3.one * 0.5f,
                        Vector3.zero);

            InitializeBoxCollider(EditorInterface.AddObjectVisual("adv_trigger_low_plate_unpress_time", 
                ObjectsStorage.Triggers["low_plate_unpress_time"].gameObject, false),
                    Vector3.one * 0.5f,
                        Vector3.zero);

            EditorInterface.AddRoomVisualManager<OutsideRoomVisualManager>("adv_corn_field");
        }

        private static void InitializeTools(EditorMode mode, bool vanillaCompliant)
        {
            EditorInterfaceModes.AddToolToCategory(mode, "npcs",
                    new NPCTool("adv_criss_the_crystal", AssetsStorage.sprites["adv_editor_criss_the_crystal"]));

            foreach (string objectName in ObjectsStorage.ItemObjects.Keys)
            {
                string key = "adv_" + objectName;
                EditorInterfaceModes.AddToolToCategory(mode, "items", 
                    new ItemTool(key));
            }

            foreach (string name in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + name;
                EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                    new ObjectTool(key, ObjectsStorage.EditorSprites["vending_" + name]));
            }

            foreach (string name in ObjectsStorage.Objects.Keys)
            {
                if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                {
                    string key = "adv_" + name;

                    if (plate.EditorToolSprite == null) continue;

                    EditorInterfaceModes.AddToolToCategory(mode, "objects",
                        new ObjectTool(key, plate.EditorToolSprite));
                }
            }
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectTool("adv_symbol_machine", AssetsStorage.sprites["adv_editor_symbol_machine"]));

            EditorInterfaceModes.AddToolToCategory(mode, "activities",
                new ActivityTool("adv_advanced_math_machine", AssetsStorage.sprites["adv_editor_advanced_math_machine"], heightOffset: 0f));

            EditorInterfaceModes.AddToolToCategory(mode, "activities",
                new ActivityTool("adv_advanced_math_machine_corner", AssetsStorage.sprites["adv_editor_advanced_math_machine_corner"],
                    heightOffset: 0f));

            EditorInterfaceModes.AddToolToCategory(mode, "objects",
                new ObjectTool("adv_voting_ballot", AssetsStorage.sprites["adv_editor_voting_ballot"]));

            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectTool("adv_farm_finish_flag", AssetsStorage.sprites["adv_editor_finish_flag"]));
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectTool("adv_farm_finish_points_flag", AssetsStorage.sprites["adv_editor_finish_points_flag"]));
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectTool("adv_farm_sign1", AssetsStorage.sprites["adv_editor_corn_sign1"]));
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectTool("adv_trigger_no_plate_cooldown", AssetsStorage.sprites["adv_editor_no_cooldown_plate"]));
            EditorInterfaceModes.AddToolToCategory(mode, "objects", 
                new ObjectTool("adv_trigger_low_plate_unpress_time", AssetsStorage.sprites["adv_editor_low_unpress_time"]));

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
                new TextureContainer("adv_basic_floor", "adv_school_council_wall", "Ceiling"));
            containers.Add("adv_advanced_class", 
                new TextureContainer("adv_advanced_class_floor", "adv_advanced_class_wall", "adv_advanced_class_ceiling"));
            containers.Add("adv_corn_field", 
                new TextureContainer("adv_corn_floor", "adv_corn_wall", "None"));
        }

        private static EditorBasicObject InitializeBoxCollider(EditorBasicObject obj, Vector3 size, Vector3 offset)
        {
            BoxCollider collider = obj.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = size;
            collider.center = offset;
            obj.editorCollider = collider;

            return obj;
        }

        public static void LoadEditorAssets()
        {
            AssetsStorage.LoadModSprite("adv_editor_criss_the_crystal",
                "Compats/LevelEditor/NPCs/adv_editor_criss_the_crystal.png");

            AssetsStorage.LoadModSprite("adv_editor_corn_sign1",
                "Compats/LevelEditor/Objects/adv_editor_corn_sign1.png");
            AssetsStorage.LoadModSprite("adv_editor_finish_flag",
                "Compats/LevelEditor/Objects/adv_editor_finish_flag.png");
            AssetsStorage.LoadModSprite("adv_editor_finish_points_flag",
                "Compats/LevelEditor/Objects/adv_editor_finish_points_flag.png");

            AssetsStorage.LoadModSprite("adv_editor_invisibility_plate",
                "Compats/LevelEditor/Objects/adv_editor_invisibility_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_acceleration_plate",
                "Compats/LevelEditor/Objects/adv_editor_acceleration_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_fake_plate",
                "Compats/LevelEditor/Objects/adv_editor_fake_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_noisy_plate",
                "Compats/LevelEditor/Objects/adv_editor_noisy_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_noisy_faculty_plate",
                "Compats/LevelEditor/Objects/adv_editor_noisy_faculty_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_stealing_plate",
                "Compats/LevelEditor/Objects/adv_editor_stealing_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_bully_plate",
                "Compats/LevelEditor/Objects/adv_editor_bully_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_present_plate",
                "Compats/LevelEditor/Objects/adv_editor_present_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_sugar_addiction_plate",
                "Compats/LevelEditor/Objects/adv_editor_sugar_addiction_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_slowdown_plate",
                "Compats/LevelEditor/Objects/adv_editor_slowdown_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_protection_plate",
                "Compats/LevelEditor/Objects/adv_editor_protection_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_teleportation_plate",
                "Compats/LevelEditor/Objects/adv_editor_teleportation_plate.png");
            AssetsStorage.LoadModSprite("adv_editor_safety_trapdoor",
                "Compats/LevelEditor/Objects/adv_editor_safety_trapdoor.png");
            AssetsStorage.LoadModSprite("adv_editor_voting_ballot",
                "Compats/LevelEditor/Objects/adv_editor_voting_ballot.png");
            AssetsStorage.LoadModSprite("adv_editor_advanced_math_machine",
                "Compats/LevelEditor/Objects/adv_editor_activity_advanced_math_machine.png");
            AssetsStorage.LoadModSprite("adv_editor_advanced_math_machine_corner",
                "Compats/LevelEditor/Objects/adv_editor_activity_advanced_math_machine_corner.png");

            AssetsStorage.LoadModSprite("adv_editor_symbol_machine",
                "Compats/LevelEditor/Objects/adv_editor_symbol_machine.png");
            AssetsStorage.LoadModSprite("adv_editor_english_floor",
                "Compats/LevelEditor/Rooms/adv_room_english.png");
            AssetsStorage.LoadModSprite("adv_editor_school_council_floor",
                "Compats/LevelEditor/Rooms/adv_room_school_council.png");
            AssetsStorage.LoadModSprite("adv_editor_english_floor_timer",
                "Compats/LevelEditor/Rooms/adv_room_english_timer.png");
            AssetsStorage.LoadModSprite("adv_editor_advanced_class_floor",
                "Compats/LevelEditor/Rooms/adv_room_advanced.png");
            AssetsStorage.LoadModSprite("adv_editor_corn_field",
                "Compats/LevelEditor/Rooms/adv_room_corn_field.png");

            AssetsStorage.LoadModSprite("adv_editor_no_cooldown_plate",
                "Compats/LevelEditor/Objects/adv_editor_no_cooldown.png");
            AssetsStorage.LoadModSprite("adv_editor_low_unpress_time",
                "Compats/LevelEditor/Objects/adv_editor_low_unpress_time.png");
        }
    }
}
