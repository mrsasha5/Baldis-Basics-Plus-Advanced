/*using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.Visuals
{
    internal class PlateEditorVisual : ButtonEditorVisual
    {
        public void InitMesh()
        {
            GameObject childObj = new GameObject("ButtonVisual");
            childObj.transform.localScale = new Vector3(10f, 10f, 1f);
            childObj.transform.SetParent(transform, true);

            Quaternion rotation = childObj.transform.rotation;
            rotation.eulerAngles = new Vector3(90f, 270f, 0f);
            childObj.transform.rotation = rotation;

            MeshRenderer meshRenderer = childObj.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = childObj.AddComponent<MeshFilter>();
            meshFilter.mesh = AssetsStorage.meshes["quad"];

            ReflectionHelper.SetValue<MeshRenderer>(this, "hackRenderer", meshRenderer);
            meshRenderer.materials = new Material[2]
            {
                new Material(BaldiLevelEditorPlugin.tileMaskedShader),
                new Material(BaldiLevelEditorPlugin.tileMaskedShader)
            };
            meshRenderer.materials[0].SetMainTexture(AssetsStorage.textures["adv_pressure_plate_deactivated"]);
            //meshRenderer.materials[1].SetMainTexture(AssetsStorage.textures["adv_plate_deactivated"]);

            childObj.AddComponent<MeshCollider>();

            wallParts[0] = childObj;
            meshRenders[0] = meshRenderer;
            wallParts[1] = childObj;
            meshRenders[1] = meshRenderer;
        }

        public override void Setup(EditorButtonPlacement prefab)
        {
            typedPrefab = prefab;
            InitMesh();
        }
    }
}*/