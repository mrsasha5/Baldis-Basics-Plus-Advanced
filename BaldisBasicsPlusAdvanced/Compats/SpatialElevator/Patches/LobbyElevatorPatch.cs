using System.Collections;
using System.Text;
using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Compats.SpatialElevator;
using BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.UI;
using The3DElevator.MonoBehaviours;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Patches
{
    [HarmonyPatch(typeof(LobbyElevator))]
    [ConditionalPatchIntegrableMod(typeof(SpatialElevatorIntegration))]
    internal class LobbyElevatorPatch
    {

        private static TextMeshPro tmpText;

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInitialize(LobbyElevator __instance)
        {

            if (ElevatorTipsPatch.LoadTip)
            {
                Transform mainParent = new GameObject("Screen").transform;
                mainParent.SetParent(__instance.transform, false);
                mainParent.localPosition = new Vector3(0f, 24f, 0f);

                GameObject screenObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                screenObj.name = "Renderer";
                screenObj.transform.SetParent(mainParent, false);

                MeshRenderer renderer = screenObj.GetComponent<MeshRenderer>();
                renderer.material = new Material(AssetsStorage.graphsStandardShader);
                renderer.material.SetColor(Color.black);

                BoxCollider collider = screenObj.GetComponent<BoxCollider>();
                GameObject.Destroy(collider);

                tmpText = ObjectsCreator.CreateSpatialText(
                    BaldiFonts.ComicSans12, new Vector2(35f, 100f), mainParent, new Vector3(0f, 1f, 0.55f));
                tmpText.transform.localScale = Vector3.one * 0.5f;
                tmpText.color = Color.green;

                screenObj.transform.localScale = new Vector3(18f, 6f, 1f);

                __instance.StartCoroutine(TextLoader());
            }

            if (ElevatorExpelHammerPatch.ShouldInitialize)
            {
                ExpelHammerInteractionObject hammer = 
                    new GameObject("Expel Hammer").AddComponent<ExpelHammerInteractionObject>();
                hammer.transform.SetParent(__instance.transform, false);
                hammer.transform.localPosition = new Vector3(10f, 6f, -10f);
                hammer.Initialize();
            }
            
        }

        private static IEnumerator TextLoader()
        {
            const float symbolCooldown = 0.01f;

            float time = 4f;
            while (time > 0f)
            {
                time -= Time.unscaledDeltaTime;
                yield return null;
            }

            tmpText.text = LocalizationManager.Instance.GetLocalizedText("Adv_Text_Loading");

            int counter = 0;

            time = 0.5f;

            while (time > 0f)
            {
                time -= Time.unscaledDeltaTime;

                if (time <= 0f)
                {
                    if (counter == 3) break;
                    counter++;
                    time = 0.5f;
                    tmpText.text += ".";
                }

                yield return null;
            }

            tmpText.text = "";

            string text = ElevatorTipsPatch.GetTipText();

            int index = 0;

            time = symbolCooldown;

            while (index < text.Length)
            {

                while (time > 0f)
                {
                    time -= Time.unscaledDeltaTime;
                    yield return null;
                }

                time = symbolCooldown;

                tmpText.text += text[index];
                index++;
            }
        }

    }
}
