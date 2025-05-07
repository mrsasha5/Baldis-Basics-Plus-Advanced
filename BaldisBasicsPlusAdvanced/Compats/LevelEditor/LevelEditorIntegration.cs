using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using PlusLevelFormat;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor
{
    public class LevelEditorIntegration
    {

        public static void onModAssetsLoaded()
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
                    BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject, Vector3.zero));
                    PlusLevelLoaderPlugin.Instance.prefabAliases.Add(key, ObjectsStorage.SodaMachines[vendingMachineName].gameObject);
                }

                //offset - addend vector for base position

                foreach (string name in ObjectsStorage.GameButtons.Keys)
                {
                    if (ObjectsStorage.GameButtons[name] is BasePlate)
                    {
                        string key = "adv_" + name;
                        BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    (key, ObjectsStorage.GameButtons[name].gameObject, Vector3.zero));
                        PlusLevelLoaderPlugin.Instance.prefabAliases.Add(key, ObjectsStorage.GameButtons[name].gameObject);
                    }
                }


                //BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                //("adv_plate", ObjectsStorage.GameButtons["plate"].gameObject, Vector3.up * 5f));
                //PlusLevelLoaderPlugin.Instance.buttons.Add("adv_plate", ObjectsStorage.GameButtons["plate"]);

                GameObject authenticIcon = ObjectsCreator.createSpriteRendererBillboard(AssetsStorage.sprites["adv_authentic_label"]);
                authenticIcon.name = "Advanced Authentic Mode label";
                authenticIcon.ConvertToPrefab(true);
                BoxCollider authenticCollider = authenticIcon.AddComponent<BoxCollider>();
                authenticCollider.size = new Vector3(10f, 10f, 10f);
                authenticCollider.isTrigger = true;

                authenticIcon.AddComponent<AuthenticLabelController>().spriteRenderer = authenticIcon.GetComponentInChildren<SpriteRenderer>();

                ObjectsStorage.Objects.Add("authentic_mode_editor_label", authenticIcon);

                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                    ("adv_authentic_mode_label", ObjectsStorage.Objects["authentic_mode_editor_label"], Vector3.up * 5f));
                PlusLevelLoaderPlugin.Instance.prefabAliases.Add("adv_authentic_mode_label", ObjectsStorage.Objects["authentic_mode_editor_label"]);

            }
            catch (Exception e)
            {
                ObjectsCreator.causeCrash(e);
            }
        }

    }
}
