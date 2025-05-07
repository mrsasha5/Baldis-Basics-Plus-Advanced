using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Managers;
using BaldisBasicsPlusAdvanced.SavedData;
using BaldisBasicsPlusAdvanced;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using System;
using BaldiLevelEditor;
using UnityEngine;
using PlusLevelFormat;
using PlusLevelLoader;
using Steamworks;

namespace BaldiBasicsPlusAdvancedEditor
{
    [BepInPlugin(modId, modName, version)]
    [BepInDependency("baldi.basics.plus.advanced.mod")]
    [BepInDependency("mtm101.rulerp.baldiplus.leveleditor")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class AdvancedEditorCore : BaseUnityPlugin
    {
        public const string modId = "baldi.basics.plus.advanced.editor.mod";

        public const string modName = "Baldi's Basics Plus Advanced Edition Editor Integration";

        public const string version = "0.1.5.2";

        private static AdvancedEditorCore instance;

        public static AdvancedEditorCore Instance => instance;

        private void Awake()
        {
            Harmony harmony = new Harmony(modId);
            harmony.PatchAll();
            instance = this;
            AdvancedCore.registerOnModAssetsLoaded(onAssetsLoaded);
        }

        private static void onAssetsLoaded()
        {
            try
            {
                foreach (string objectName in ObjectsStorage.ItemsObjects.Keys)
                {
                    string key = "adv_" + objectName;
                    BaldiLevelEditorPlugin.itemObjects.Add(key, ObjectsStorage.ItemsObjects[objectName]);
                    PlusLevelLoaderPlugin.Instance.itemObjects.Add(key, ObjectsStorage.ItemsObjects[objectName]);
                }
                foreach (string vendingMachineName in ObjectsStorage.SodaMachines.Keys)
                {
                    string key = "adv_" + vendingMachineName;
                    BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject, new Vector3()));
                    PlusLevelLoaderPlugin.Instance.prefabAliases.Add(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject);
                }
            }
            catch (Exception e)
            {
                ObjectsCreator.causeCrash(Instance.Info, e);
            }
        }
    }
}
