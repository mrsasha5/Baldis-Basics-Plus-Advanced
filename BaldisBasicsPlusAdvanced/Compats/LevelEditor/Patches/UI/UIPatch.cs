using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.Patches.UI
{
    [HarmonyPatch(typeof(PlusLevelEditor))]
    [ConditionalPatchMod(ModsIntegration.LevelEditorId)]
    internal class UIPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void addNewObjects(PlusLevelEditor __instance)
        {
            List<EditorTool> items = __instance.toolCats.Find(x => x.name == "items").tools;
            List<EditorTool> objects = __instance.toolCats.Find(x => x.name == "objects").tools;
            List<EditorTool> utilities = __instance.toolCats.Find(x => x.name == "utilities").tools;
            //List<EditorTool> connectables = __instance.toolCats.Find(x => x.name == "connectables").tools;

            foreach (string name in ObjectsStorage.ItemsObjects.Keys)
            {
                if (ObjectsStorage.SpawningData[name].Editor)
                {
                    string key = "adv_" + name;
                    ControlledItemTool itemTool = new ControlledItemTool(key);
                    itemTool.setSprite(ObjectsStorage.EditorSprites["item_" + name]);
                    items.Add(itemTool);
                }
            }
            
            foreach (string name in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + name;
                ControlledRotatableTool objectTool = new ControlledRotatableTool(key);
                objectTool.setSprite(ObjectsStorage.EditorSprites["vending_" + name]);
                objects.Add(objectTool);
            }

            foreach (string name in ObjectsStorage.GameButtons.Keys)
            {
                if (ObjectsStorage.GameButtons[name] is BasePlate)
                {
                    string key = "adv_" + name;
                    BasePlate plate = (BasePlate)ObjectsStorage.GameButtons[name];

                    if (plate.EditorToolSprite == null) continue;

                    ControlledRotatableTool plateTool = new ControlledRotatableTool(key);
                    plateTool.setSprite(plate.EditorToolSprite);
                    objects.Add(plateTool);
                }
            }

            ControlledRotatableTool authenticTool = new ControlledRotatableTool("adv_authentic_mode_label");
            authenticTool.setSprite(AssetsStorage.sprites["adv_authentic_label_slot"]);
            utilities.Add(authenticTool);

            //PlateTool buttonTool = new PlateTool("adv_plate");
            //buttonTool.setSprite(AssetsStorage.sprites["adv_editor_plate"]);
            //connectables.Add(buttonTool);
        }
    }
}
