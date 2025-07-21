using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using PlusLevelFormat;
using PlusLevelLoader;
using System;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor
{
    public class LevelEditorIntegration : CompatibilityModule
    {

        public LevelEditorIntegration() : base()
        {
            guid = "mtm101.rulerp.baldiplus.leveleditor";
            priority = 127;
        }

        protected override void InitializePre()
        {
            base.InitializePre();
            CreateConfigValue("Legacy Level Editor", 
                "Adds support for Legacy Level Editor like new objects, activities and other content which can be used on your levels!\n" +
                "Please note: backward compatibility of maps on old versions is not guaranteed.");
        }

        public override bool IsIntegrable()
        {
            return AssetsHelper.ModInstalled(guid) && IntegrationManager.LevelLoaderInstalled;
        }

        protected override void Initialize()
        {
            try
            {
                foreach (string objectName in ObjectsStorage.ItemObjects.Keys)
                {
                    string key = "adv_" + objectName;
                    BaldiLevelEditorPlugin.itemObjects.Add(key, ObjectsStorage.ItemObjects[objectName]);
                }
                foreach (string vendingMachineName in ObjectsStorage.SodaMachines.Keys)
                {
                    string key = "adv_" + vendingMachineName;
                    BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject, Vector3.zero));
                }

                foreach (string name in ObjectsStorage.Objects.Keys)
                {
                    if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                    {
                        string key = "adv_" + name;

                        BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                                (key, plate.gameObject, Vector3.zero));
                    }
                }

                //Triggers
                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    ("adv_trigger_no_plate_cooldown", 
                    ObjectsStorage.Triggers["no_plate_cooldown"].gameObject, Vector3.up * 5f));

                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    ("adv_trigger_low_plate_unpress_time", 
                    ObjectsStorage.Triggers["low_plate_unpress_time"].gameObject, Vector3.up * 5f));

                //Symbol Machine
                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    ("adv_symbol_machine", 
                    ObjectsStorage.Objects["symbol_machine"].gameObject, Vector3.zero));

                //Farm flags
                EditorObjectType flagEditorRef = EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    ("adv_farm_finish_flag",
                    ObjectsStorage.Objects["farm_flag"].gameObject, Vector3.up * 5f);
                flagEditorRef.prefab.gameObject.GetComponentInChildren<SpriteRenderer>()
                    .material = AssetsStorage.materials["sprite_standard_no_billboard"];

                BaldiLevelEditorPlugin.editorObjects.Add(flagEditorRef);

                flagEditorRef = EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    ("adv_farm_finish_points_flag",
                    ObjectsStorage.Objects["farm_points_flag"].gameObject, Vector3.up * 5f);
                flagEditorRef.prefab.gameObject.GetComponentInChildren<SpriteRenderer>()
                    .material = AssetsStorage.materials["sprite_standard_no_billboard"];

                BaldiLevelEditorPlugin.editorObjects.Add(flagEditorRef);

                //Farm sign
                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    ("adv_farm_sign1", 
                    ObjectsStorage.Objects["farm_sign1"].gameObject, Vector3.up * 5f));

                //Criss the Crystal
                BaldiLevelEditorPlugin.characterObjects.Add(
                    "adv_criss_the_crystal", ObjectsStorage.Npcs["CrissTheCrystal"].gameObject);

                //Advanced Math Machine
                BaldiLevelEditorPlugin.editorActivities.Add(
                    EditorObjectType.CreateFromGameObject<ActivityPrefab, RoomActivity>("adv_advanced_math_machine",
                    ObjectsStorage.Objects["advanced_math_machine"].gameObject, Vector3.zero));

                //Advanced Math Machine corner
                BaldiLevelEditorPlugin.editorActivities.Add(
                    EditorObjectType.CreateFromGameObject<ActivityPrefab, RoomActivity>("adv_advanced_math_machine_corner",
                    ObjectsStorage.Objects["advanced_math_machine_corner"].gameObject, Vector3.zero));
                EditorObjectType editorObjectType = BaldiLevelEditorPlugin.editorActivities.Last();
                GameObject obj2 = UnityEngine.Object.Instantiate(editorObjectType.prefab.gameObject);
                obj2.GetComponents<MonoBehaviour>().Do(delegate (MonoBehaviour x)
                {
                    UnityEngine.Object.Destroy(x);
                });
                obj2.GetComponentsInChildren<Collider>().Do(delegate (Collider x)
                {
                    UnityEngine.Object.Destroy(x);
                });
                obj2.transform.SetParent(editorObjectType.prefab.transform, worldPositionStays: false);
                obj2.transform.localPosition = Vector3.zero;
                editorObjectType.prefab.gameObject.transform.eulerAngles = Vector3.zero;
                obj2.transform.eulerAngles = new Vector3(0f, 45f, 0f);
                UnityEngine.Object.Destroy(editorObjectType.prefab.gameObject.GetComponent<MeshRenderer>());
                obj2.SetActive(value: true);
                obj2.name = "CornerRenderer";

                //BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    //("adv_voting_ballot",
                    //ObjectsStorage.Objects["voting_ballot"].gameObject, Vector3.zero));
            }
            catch (Exception e)
            {
                ObjectsCreator.CauseCrash(e);
            }
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
