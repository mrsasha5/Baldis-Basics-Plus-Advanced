using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio;
using MTM101BaldAPI;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class VotingCeilingScreen : MonoBehaviour, IPrefab
    {

        [SerializeField]
        private TextMeshPro[] tmp;

        public void InitializePrefab(int variant)
        {
            float height = 9.5f;
            BaldiFonts screenFont = BaldiFonts.ComicSans12;

            GameObject cubeGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeGameObject.name = "Model";
            cubeGameObject.transform.SetParent(transform, false);
            cubeGameObject.transform.localPosition = Vector3.up * height;
            cubeGameObject.transform.localScale = Vector3.one + Vector3.right * 7f;

            MeshRenderer renderer = cubeGameObject.GetComponent<MeshRenderer>();
            renderer.material = new Material(AssetStorage.materials["belt"]);
            renderer.material.SetMainTexture(AssetStorage.textures["white"]);
            renderer.material.SetColor(Color.black);

            RectTransform rect1 = new GameObject().AddComponent<RectTransform>();
            rect1.name = "ForwardText";
            TextMeshPro text1 = rect1.gameObject.AddComponent<TextMeshPro>();
            rect1.localScale = Vector3.one * 0.5f;
            rect1.rotation = Quaternion.Euler(0f, 180f, 0f);
            text1.text = "OH NO!";
            text1.alignment = TextAlignmentOptions.Center;
            text1.font = screenFont.FontAsset();
            text1.fontSize = screenFont.FontSize();
            text1.color = Color.green;
            rect1.SetParent(transform, false);
            rect1.localPosition = Vector3.forward * 0.52f + Vector3.up * height;

            RectTransform rect2 = new GameObject().AddComponent<RectTransform>();
            rect2.name = "BackText";
            TextMeshPro text2 = rect2.gameObject.AddComponent<TextMeshPro>();
            rect2.localScale = Vector3.one * 0.5f;
            text2.text = "OH NO!";
            text2.alignment = TextAlignmentOptions.Center;
            text2.font = screenFont.FontAsset();
            text2.fontSize = screenFont.FontSize();
            text2.color = Color.green;
            rect2.SetParent(transform, false);
            rect2.localPosition = Vector3.back * 0.52f + Vector3.up * height;

            //If Level Studio is installed, it will be destroyed after defining visual
            if (!IntegrationManager.IsActive<LevelStudioIntegration>())
                GameObject.Destroy(cubeGameObject.GetComponent<Collider>());
            else
            {
                BoxCollider collider = cubeGameObject.GetComponent<BoxCollider>();
                collider.center = Vector3.up * height;
                collider.size = Vector3.one + Vector3.right * 7f;
            }

            tmp = new TextMeshPro[] { text1, text2 };
        }

        public void UpdateTexts(string text)
        {
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i].text = text;
            }
        }

    }
}
