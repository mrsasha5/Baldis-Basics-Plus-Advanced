using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.Patches.UI
{
    [HarmonyPatch(typeof(PlusLevelEditor))]
    [ConditionalPatchIntegrableMod(typeof(LevelEditorIntegration))]
    internal class UIPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void AddNewObjects(PlusLevelEditor __instance)
        {
            void CreateTool<T>(List<EditorTool> categoryList, string key, Sprite sprite) where T : EditorTool
            {
                T instance = (T)Activator.CreateInstance(typeof(T), key);
                typeof(T).GetMethod("SetSprite").Invoke(instance, new object[] { sprite });
                categoryList.Add(instance);
            }

            List<EditorTool> halls = __instance.toolCats.Find(x => x.name == "halls").tools;
            List<EditorTool> items = __instance.toolCats.Find(x => x.name == "items").tools;
            List<EditorTool> objects = __instance.toolCats.Find(x => x.name == "objects").tools;
            List<EditorTool> npcs = __instance.toolCats.Find(x => x.name == "characters").tools;
            List<EditorTool> activities = __instance.toolCats.Find(x => x.name == "activities").tools;
            List<EditorTool> connectables = __instance.toolCats.Find(x => x.name == "connectables").tools;

            CreateTool<ControlledNpcTool>(npcs, 
                "adv_criss_the_crystal", AssetsStorage.sprites["adv_editor_criss_the_crystal"]);

            foreach (string name in ObjectsStorage.ItemObjects.Keys)
            {
                if (ObjectsStorage.SpawningData["item_" + name].Editor)
                {
                    string key = "adv_" + name;
                    CreateTool<ControlledItemTool>(items, key, ObjectsStorage.EditorSprites["item_" + name]);
                }
            }
            
            foreach (string name in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + name;
                CreateTool<ControlledRotatableTool>(objects, key, ObjectsStorage.EditorSprites["vending_" + name]);
            }

            foreach (string name in ObjectsStorage.Objects.Keys)
            {
                if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                {
                    string key = "adv_" + name;

                    if (plate.EditorToolSprite == null) continue;

                    CreateTool<ControlledRotatableTool>(objects, key, plate.EditorToolSprite);
                }
            }

            CreateTool<ControlledObjectTool>(objects, "adv_symbol_machine", 
                AssetsStorage.sprites["adv_editor_symbol_machine"]);

            CreateTool<ControlledActivityTool>(activities, "adv_advanced_math_machine", 
                AssetsStorage.sprites["adv_editor_advanced_math_machine"]);

            CreateTool<ControlledActivityTool>(activities, "adv_advanced_math_machine_corner",
                AssetsStorage.sprites["adv_editor_advanced_math_machine_corner"]);

            //ControlledRotatableTool ballotTool = new ControlledRotatableTool("adv_voting_ballot");
            //ballotTool.SetSprite(AssetsStorage.sprites["adv_editor_voting_ballot"]);
            //objects.Add(ballotTool);

            CreateTool<ControlledObjectTool>(objects, "adv_farm_finish_flag",
                AssetsStorage.sprites["adv_editor_finish_flag"]);

            CreateTool<ControlledObjectTool>(objects, "adv_farm_finish_points_flag",
                AssetsStorage.sprites["adv_editor_finish_points_flag"]);

            CreateTool<ControlledObjectTool>(objects, "adv_farm_sign1",
                AssetsStorage.sprites["adv_editor_corn_sign1"]);

            CreateTool<ControlledObjectTool>(objects, "adv_trigger_no_plate_cooldown",
                AssetsStorage.sprites["adv_editor_no_cooldown_plate"]);

            CreateTool<ControlledObjectTool>(objects, "adv_trigger_low_plate_unpress_time",
                AssetsStorage.sprites["adv_editor_low_unpress_time"]);

            CreateTool<ControlledFloorTool>(halls, "adv_english_class",
                AssetsStorage.sprites["adv_editor_english_floor"]);

            CreateTool<ControlledFloorTool>(halls, "adv_english_class_timer",
                AssetsStorage.sprites["adv_editor_english_floor_timer"]);

            CreateTool<ControlledFloorTool>(halls, "adv_school_council_class",
                AssetsStorage.sprites["adv_editor_school_council_floor"]);

            CreateTool<ControlledFloorTool>(halls, "adv_advanced_class",
                AssetsStorage.sprites["adv_editor_advanced_class_floor"]);

            CreateTool<ControlledFloorTool>(halls, "adv_corn_field",
                AssetsStorage.sprites["adv_editor_corn_field"]);
        }

        
    }
}
