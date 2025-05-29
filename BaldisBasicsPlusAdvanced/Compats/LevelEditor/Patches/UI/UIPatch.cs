using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using HarmonyLib;
using System.Collections.Generic;

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
            List<EditorTool> halls = __instance.toolCats.Find(x => x.name == "halls").tools;
            List<EditorTool> items = __instance.toolCats.Find(x => x.name == "items").tools;
            List<EditorTool> objects = __instance.toolCats.Find(x => x.name == "objects").tools;
            List<EditorTool> npcs = __instance.toolCats.Find(x => x.name == "characters").tools;
            List<EditorTool> activities = __instance.toolCats.Find(x => x.name == "activities").tools;
            List<EditorTool> connectables = __instance.toolCats.Find(x => x.name == "connectables").tools;

            ControlledNpcTool crissTool = new ControlledNpcTool("adv_criss_the_crystal");
            crissTool.SetSprite(AssetsStorage.sprites["adv_editor_criss_the_crystal"]);
            npcs.Add(crissTool);

            foreach (string name in ObjectsStorage.ItemObjects.Keys)
            {
                if (ObjectsStorage.SpawningData["item_" + name].Editor)
                {
                    string key = "adv_" + name;
                    ControlledItemTool itemTool = new ControlledItemTool(key);
                    itemTool.SetSprite(ObjectsStorage.EditorSprites["item_" + name]);
                    items.Add(itemTool);
                }
            }
            
            foreach (string name in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + name;
                ControlledRotatableTool objectTool = new ControlledRotatableTool(key);
                objectTool.SetSprite(ObjectsStorage.EditorSprites["vending_" + name]);
                objects.Add(objectTool);
            }

            foreach (string name in ObjectsStorage.Objects.Keys)
            {
                if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                {
                    string key = "adv_" + name;

                    if (plate.EditorToolSprite == null) continue;

                    ControlledRotatableTool plateTool = new ControlledRotatableTool(key);
                    plateTool.SetSprite(plate.EditorToolSprite);
                    objects.Add(plateTool);
                }
            }

            ControlledRotatableTool symbolMachineTool = new ControlledRotatableTool("adv_symbol_machine");
            symbolMachineTool.SetSprite(AssetsStorage.sprites["adv_editor_symbol_machine"]);
            objects.Add(symbolMachineTool);

            ControlledActivityTool advancedMathMachine = new ControlledActivityTool("adv_advanced_math_machine");
            advancedMathMachine.SetSprite(AssetsStorage.sprites["adv_editor_advanced_math_machine"]);
            activities.Add(advancedMathMachine);

            ControlledActivityTool advancedMathMachine2 = new ControlledActivityTool("adv_advanced_math_machine_corner");
            advancedMathMachine2.SetSprite(AssetsStorage.sprites["adv_editor_advanced_math_machine_corner"]);
            activities.Add(advancedMathMachine2);

            //ControlledRotatableTool ballotTool = new ControlledRotatableTool("adv_voting_ballot");
            //ballotTool.SetSprite(AssetsStorage.sprites["adv_editor_voting_ballot"]);
            //objects.Add(ballotTool);

            ControlledObjectTool farmFlagTool = new ControlledObjectTool("adv_farm_finish_flag");
            farmFlagTool.SetSprite(AssetsStorage.sprites["adv_editor_finish_flag"]);
            objects.Add(farmFlagTool);

            ControlledObjectTool farmSignTool = new ControlledObjectTool("adv_farm_sign1");
            farmSignTool.SetSprite(AssetsStorage.sprites["adv_editor_corn_sign1"]);
            objects.Add(farmSignTool);

            //adv_english_class
            //EnglishClass
            ControlledFloorTool englishFloorTool = new ControlledFloorTool("adv_english_class");
            englishFloorTool.SetSprite(AssetsStorage.sprites["adv_editor_english_floor"]);
            halls.Add(englishFloorTool);

            ControlledFloorTool englishFloorTimerTool = new ControlledFloorTool("adv_english_class_timer");
            englishFloorTimerTool.SetSprite(AssetsStorage.sprites["adv_editor_english_floor_timer"]);
            halls.Add(englishFloorTimerTool);

            ControlledFloorTool councilFloor = new ControlledFloorTool("adv_school_council_class");
            councilFloor.SetSprite(AssetsStorage.sprites["adv_editor_school_council_floor"]);
            halls.Add(councilFloor);

            ControlledFloorTool advancedFloor = new ControlledFloorTool("adv_advanced_class");
            advancedFloor.SetSprite(AssetsStorage.sprites["adv_editor_advanced_class_floor"]);
            halls.Add(advancedFloor);

            ControlledFloorTool cornFloor = new ControlledFloorTool("adv_corn_field");
            cornFloor.SetSprite(AssetsStorage.sprites["adv_editor_corn_field"]);
            halls.Add(cornFloor);

            //ControlledTileBasedTool gumDispenserTool = new ControlledTileBasedTool("adv_gum_dispenser");
            //gumDispenserTool.SetSprite(AssetsStorage.sprites["adv_editor_gum_dispenser"]);
            //connectables.Add(gumDispenserTool);

            /*PlateTool buttonTool = new PlateTool("adv_pressure_plate");
            buttonTool.SetSprite(AssetsStorage.sprites["adv_editor_acceleration_plate"]);
            connectables.Add(buttonTool);*/
        }

        
    }
}
