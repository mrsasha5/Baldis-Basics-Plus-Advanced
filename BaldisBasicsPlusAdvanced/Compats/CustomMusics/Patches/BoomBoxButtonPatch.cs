using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BBPlusCustomMusics.MonoBehaviours;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Compats.CustomMusics.Patches
{
    [ConditionalPatchIntegrableMod(typeof(CustomMusicsIntegration))]
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class BoomBoxButtonPatch
    {

        private static StandardMenuButton button;

        private static BoomBox boomBox;

        private static Image boomBoxImage;

        private static TipsMonitor monitor;

        private static string text;

        private static bool overridden;


        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(ElevatorScreen __instance)
        {
            overridden = false;
            boomBox = __instance.GetComponentInChildren<BoomBox>();
            boomBoxImage = boomBox.GetComponent<Image>();
            if (button == null)
            {
                text = "Adv_CustomMusics_Elv_Tip".Localize();

                button = ObjectsCreator.CreateSpriteButton(AssetsStorage.sprites["transparent"],
                    boomBox.transform.localPosition, __instance.Canvas.transform);
                button.name = "BoomBoxCursorChecker";
                button.transform.SetSiblingIndex(boomBox.transform.GetSiblingIndex() + 1);
                button.image.rectTransform.sizeDelta = boomBox.GetComponent<Image>().rectTransform.sizeDelta;
                button.eventOnHigh = true;
                button.OnPress.AddListener(OnPress);
                button.OnHighlight.AddListener(OnHighlightButton);
                button.OffHighlight.AddListener(OnUnhighlightButton);
            }
        }

        private static void OnPress()
        {
            if (OverrideTips())
            {
                overridden = true;
            }
            else if (ResetOverride())
            {
                overridden = false;
            }
        }

        private static void OnHighlightButton()
        {
            boomBoxImage.color = Color.green;
        }

        private static void OnUnhighlightButton()
        {
            boomBoxImage.color = Color.white;
        }

        private static void Update()
        {
            monitor.Tmp.text = GetTip();
        }

        private static string GetTip()
        {
            return string.Format(text, MusicManager.Instance.MidiPlayer.MPTK_MidiName,
                MusicManager.Instance.MidiPlayer.MPTK_PlayTime.ToString("mm':'ss"), 
                MusicManager.Instance.MidiPlayer.MPTK_Duration.ToString("mm':'ss"));
        }

        private static bool OverrideTips()
        {
            return ElevatorTipsPatch.SetOverride(true, GetTip(), out monitor, Update);
        }

        private static bool ResetOverride()
        {
            return ElevatorTipsPatch.SetOverride(false, null, out _);
        }

    }
}
