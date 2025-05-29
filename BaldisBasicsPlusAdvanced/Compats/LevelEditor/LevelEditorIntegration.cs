using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using PlusLevelFormat;
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

        public override bool IsIntegrable()
        {
            return AssetsHelper.ModInstalled(guid) && IntegrationManager.LevelLoaderInstalled && AdvancedCore.editorIntegrationEnabled;
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

                //offset - addend vector for base position

                foreach (string name in ObjectsStorage.Objects.Keys)
                {
                    if (ObjectsStorage.Objects[name].TryGetComponent(out BasePlate plate))
                    {
                        string key = "adv_" + name;

                        BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                                (key, plate.gameObject, Vector3.zero));
                    }
                }

                //BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                //("adv_plate", ObjectsStorage.GameButtons["plate"].gameObject, Vector3.up * 5f));
                //PlusLevelLoaderPlugin.Instance.buttons.Add("adv_plate", ObjectsStorage.GameButtons["plate"]);

                //Symbol Machine
                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                ("adv_symbol_machine", ObjectsStorage.Objects["symbol_machine"].gameObject, Vector3.zero));
                //Farm flag
                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                ("adv_farm_finish_flag", ObjectsStorage.Objects["farm_flag"].gameObject, Vector3.up * 5f));
                //Farm sign
                BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                ("adv_farm_sign1", ObjectsStorage.Objects["farm_sign1"].gameObject, Vector3.up * 5f));

                BaldiLevelEditorPlugin.characterObjects.Add("adv_criss_the_crystal", ObjectsStorage.Npcs["CrissTheCrystal"].gameObject);

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
                //end

                //BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>
                //("adv_voting_ballot", ObjectsStorage.Objects["voting_ballot"].gameObject, Vector3.zero));

                //Gum Dispenser
                /*TiledEditorConnectable tiledEditorConnectable = BaldiLevelEditorPlugin.CreateTileVisualFromObject<TiledEditorConnectable, TiledPrefab>(ObjectsStorage.Objects["gum_dispenser"].gameObject);
                tiledEditorConnectable.positionOffset = Vector3.up * 5f; //it doesn't work even, fuck.
                tiledEditorConnectable.directionAddition = 0f;
                BaldiLevelEditorPlugin.tiledPrefabPrefabs.Add("adv_gum_dispenser", tiledEditorConnectable);*/
            }
            catch (Exception e)
            {
                ObjectsCreator.CauseCrash(e);
            }
        }

    }
}
