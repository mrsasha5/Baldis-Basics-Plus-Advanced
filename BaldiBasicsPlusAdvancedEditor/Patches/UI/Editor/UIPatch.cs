using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Cache;
using HarmonyLib;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldiBasicsPlusAdvancedEditor.Patches.UI.Editor
{
    [HarmonyPatch(typeof(PlusLevelEditor))]
    internal class UIPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void addNewObjects(PlusLevelEditor __instance)
        {
            List<EditorTool> items = __instance.toolCats.Find(x => x.name == "items").tools;
            foreach (string objectName in ObjectsStorage.ItemsObjects.Keys)
            {
                if (ObjectsStorage.spawningItemsData[objectName].availableInEditor)
                {
                    string key = "adv_" + objectName;
                    items.Add(new ItemTool(key));
                }
            }
            List<EditorTool> objects = __instance.toolCats.Find(x => x.name == "objects").tools;
            foreach (string objectName in ObjectsStorage.SodaMachines.Keys)
            {
                string key = "adv_" + objectName;
                objects.Add(new ObjectTool(key));
            }
        }
    }
}
