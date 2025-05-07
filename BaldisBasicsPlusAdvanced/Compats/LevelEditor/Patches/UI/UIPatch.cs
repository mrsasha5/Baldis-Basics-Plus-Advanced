using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using HarmonyLib;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.Patches.UI
{
    [HarmonyPatch(typeof(PlusLevelEditor))]
    [ConditionalPatchIntegratedMod(ModsIntegration.levelEditorId)]
    internal class UIPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void AddNewObjects(PlusLevelEditor __instance)
        {
            List<EditorTool> halls = __instance.toolCats.Find(x => x.name == "halls").tools;
            List<EditorTool> items = __instance.toolCats.Find(x => x.name == "items").tools;
            List<EditorTool> objects = __instance.toolCats.Find(x => x.name == "objects").tools;
            //List<EditorTool> utilities = __instance.toolCats.Find(x => x.name == "utilities").tools;
            List<EditorTool> activities = __instance.toolCats.Find(x => x.name == "activities").tools;
            List<EditorTool> connectables = __instance.toolCats.Find(x => x.name == "connectables").tools;

            foreach (string name in ObjectsStorage.ItemsObjects.Keys)
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

            foreach (string name in ObjectsStorage.GameButtons.Keys)
            {
                if (ObjectsStorage.GameButtons[name] is PlateBase)
                {
                    string key = "adv_" + name;
                    PlateBase plate = (PlateBase)ObjectsStorage.GameButtons[name];

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

            ControlledRotatableTool ballotTool = new ControlledRotatableTool("adv_voting_ballot");
            ballotTool.SetSprite(AssetsStorage.sprites["adv_editor_voting_ballot"]);
            objects.Add(ballotTool);

            //adv_english_class
            //EnglishClass
            ControlledFloorTool englishFloorTool = new ControlledFloorTool("adv_english_class");
            englishFloorTool.SetSprite(AssetsStorage.sprites["adv_editor_english_floor"]);
            halls.Add(englishFloorTool);

            ControlledFloorTool englishFloorTimerTool = new ControlledFloorTool("adv_english_class_timer");
            englishFloorTimerTool.SetSprite(AssetsStorage.sprites["adv_editor_english_floor_timer"]);
            halls.Add(englishFloorTimerTool);

            ControlledTileBasedTool gumDispenserTool = new ControlledTileBasedTool("adv_gum_dispenser");
            gumDispenserTool.SetSprite(AssetsStorage.sprites["adv_editor_gum_dispenser"]);
            connectables.Add(gumDispenserTool);

            /*PlateTool buttonTool = new PlateTool("adv_pressure_plate");
            buttonTool.SetSprite(AssetsStorage.sprites["adv_editor_acceleration_plate"]);
            connectables.Add(buttonTool);*/
        }

        
    }
}
