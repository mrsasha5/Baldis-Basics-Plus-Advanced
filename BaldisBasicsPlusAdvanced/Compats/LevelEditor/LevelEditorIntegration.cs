using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using PlusLevelFormat;
using System;
using System.Xml.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor
{
    public class LevelEditorIntegration
    {

        public static bool IsIntegratable()
        {
            return ModsIntegration.LevelEditorInstalled && ModsIntegration.LevelLoaderInstalled && AdvancedCore.editorIntegrationEnabled;
        }

        public static void Initialize()
        {
            try
            {
                foreach (string objectName in ObjectsStorage.ItemsObjects.Keys)
                {
                    string key = "adv_" + objectName;
                    BaldiLevelEditorPlugin.itemObjects.Add(key, ObjectsStorage.ItemsObjects[objectName]);
                }
                foreach (string vendingMachineName in ObjectsStorage.SodaMachines.Keys)
                {
                    string key = "adv_" + vendingMachineName;
                    BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject, Vector3.zero));
                }

                //offset - addend vector for base position

                foreach (string name in ObjectsStorage.GameButtons.Keys)
                {
                    if (ObjectsStorage.GameButtons[name] is PlateBase)
                    {
                        string key = "adv_" + name;
                        BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    (key, ObjectsStorage.GameButtons[name].gameObject, Vector3.zero));
                    }
                }

                //BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                //("adv_plate", ObjectsStorage.GameButtons["plate"].gameObject, Vector3.up * 5f));
                //PlusLevelLoaderPlugin.Instance.buttons.Add("adv_plate", ObjectsStorage.GameButtons["plate"]);

                //Symbol Machine
                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                ("adv_symbol_machine", ObjectsStorage.Objects["symbol_machine"].gameObject, Vector3.zero));

                //Advanced Math Machine
                BaldiLevelEditorPlugin.editorActivities.Add(
                    EditorObjectType.CreateFromGameObject<ActivityPrefab, RoomActivity>("adv_advanced_math_machine",
                    ObjectsStorage.Objects["advanced_math_machine"].gameObject, Vector3.zero));

                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                ("adv_voting_ballot", ObjectsStorage.Objects["voting_ballot"].gameObject, Vector3.zero));

                //Gum Dispenser
                TiledEditorConnectable tiledEditorConnectable = BaldiLevelEditorPlugin.CreateTileVisualFromObject<TiledEditorConnectable, TiledPrefab>(ObjectsStorage.Objects["gum_dispenser"].gameObject);
                tiledEditorConnectable.positionOffset = Vector3.up * 5f; //it doesn't work even, fuck.
                tiledEditorConnectable.directionAddition = 0f;
                BaldiLevelEditorPlugin.tiledPrefabPrefabs.Add("adv_gum_dispenser", tiledEditorConnectable);

                ModsIntegration.AddPluginAsIntegrated(BaldiLevelEditorPlugin.Instance);
            }
            catch (Exception e)
            {
                ObjectsCreator.CauseCrash(e);
            }
        }

    }
}
