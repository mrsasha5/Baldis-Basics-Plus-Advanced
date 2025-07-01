using System.Collections;
using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using HarmonyLib;
using MTM101BaldAPI.UI;
using The3DElevator.MonoBehaviours.ElevatorCoreComponents;
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
        [HarmonyPrefix]
        private static void OnInitialize(LobbyElevator __instance)
        {

            if (ElevatorTipsPatch.LoadTip)
            {
                Transform mainParent = new GameObject("Screen").transform;
                mainParent.SetParent(__instance.transform, false);
                mainParent.localPosition = new Vector3(0f, 24f, 18f);
                mainParent.localRotation = Quaternion.Euler(0f, 180f, 0f);

                GameObject screenObj = GameObject.Instantiate(SpatialElevatorIntegration.monitorPre);
                screenObj.name = "Model";
                screenObj.transform.SetParent(mainParent, false);

                Collider collider = screenObj.GetComponent<Collider>();
                GameObject.Destroy(collider);

                tmpText = ObjectsCreator.CreateTextMesh(
                    BaldiFonts.ComicSans12, new Vector2(35f, 100f), mainParent, new Vector3(0f, 0f, 0.3f));
                tmpText.transform.localScale = Vector3.one * 0.5f;
                tmpText.color = Color.green;

                screenObj.transform.localScale = new Vector3(5f, 6f, 1f);

                __instance.StartCoroutine(TextLoader());
            }

            SpatialChalkboard board = 
                new GameObject("Spatial Chalkboard").AddComponent<SpatialChalkboard>();
            board.transform.SetParent(__instance.transform, false);
            board.transform.localPosition = new Vector3(-22.3f, 17f, 11f);
            board.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            board.Initialize();
            
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