using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BBPlusCustomMusics.MonoBehaviours;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Compats.CustomMusics.Patches
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    [ConditionalPatchIntegrableMod(typeof(CustomMusicsIntegration))]
    
    internal class BoomBoxButtonPatch
    {

        private static StandardMenuButton button;

        private static BoomBox boomBox;

        private static Image boomBoxImage;

        private static TipsMonitor.MonitorOverrider overrider;

        private static string text;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(ElevatorScreen __instance)
        {
            boomBox = __instance.GetComponentInChildren<BoomBox>();
            boomBoxImage = boomBox.GetComponent<Image>();
            if (button == null && ElevatorTipsPatch.LoadTip)
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
                button.OnRelease.AddListener(OnReleaseButton);
            }
        }

        private static void OnPress()
        {
            boomBoxImage.color = Color.red;
            if (overrider == null)
            {
                OverrideTips();
            }
            else
            {
                ResetOverride();
            }
        }

        private static void OnReleaseButton()
        {
            boomBoxImage.color = Color.green;
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
            overrider.UpdateText(GetTip());
        }

        private static string GetTip()
        {
            return string.Format(text, GetTrackName(),
                MusicManager.Instance.MidiPlayer.MPTK_PlayTime.ToString("mm':'ss"), 
                MusicManager.Instance.MidiPlayer.MPTK_Duration.ToString("mm':'ss"));
        }

        private static string GetTrackName()
        {
            string trackName = MusicManager.Instance.MidiPlayer.MPTK_MidiName;

            if (MusicManager.Instance.MidiPlayer.MPTK_MidiName.StartsWith("custom_CustomMusicsMIDI_"))
            {
                trackName = trackName.ReplaceFirst("custom_CustomMusicsMIDI_", "");
            }
            else if (MusicManager.Instance.MidiPlayer.MPTK_MidiName.StartsWith("custom_"))
            {
                trackName = trackName.ReplaceFirst("custom_", "");
            }

            if (trackName.Length > 50)
            {
                trackName = trackName.Substring(0, 47);
                trackName += "...";
            }

            return trackName;
        }

        private static void OverrideTips()
        {
            overrider = ElevatorTipsPatch.SetOverride(GetTip(), priority: 0);
            if (overrider != null) overrider.onUpdate += Update;
        }

        private static void ResetOverride()
        {
            overrider?.Release();
            overrider = null;
        }

    }
}
