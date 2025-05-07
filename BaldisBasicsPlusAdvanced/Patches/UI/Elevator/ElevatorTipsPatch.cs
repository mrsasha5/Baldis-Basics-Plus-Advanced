using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SavedData;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.UI.Elevator
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class ElevatorTipsPatch
    {
        private static ElevatorScreen elvScreen;

        private static Canvas canvas;

        private static TextMeshProUGUI tipText;

        private static string originalText;

        //public static bool patched => tipText != null;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void OnStart(ElevatorScreen __instance, ref Canvas ___canvas)
        {
            elvScreen = __instance;
            canvas = ___canvas;
            //(ApiManager.ElevatorTopText && (DataManager.ExtraSettings.tipsEnabled || (overrideText != null && overrideEvenTipsDisabled)))
            LocalizationManager localization = Singleton<LocalizationManager>.Instance;

            bool loadTip = !(ObjectsStorage.TipKeys.Count == 0) &&
                (DataManager.ExtraSettings.tipsEnabled);

            string tip = "";

            if (loadTip)
            {
                List<string> tipKeys = ApiManager.GetAllTips();

                int tipNum = UnityEngine.Random.Range(0, tipKeys.Count);

                tip = localization.GetLocalizedText(tipKeys[tipNum]);
            }

            string text = localization.GetLocalizedText("Adv_Elv_Tip_Base") + "\n" + tip;

            originalText = loadTip ? text : "";

            CreateTipText(originalText);
        }

        private static void CreateTipText(string text)
        {
            tipText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, text, canvas.transform, new Vector3(0, 150, 0), false);
            tipText.fontSize = 12;
            tipText.alignment = TextAlignmentOptions.Top;

            tipText.GetComponent<RectTransform>().sizeDelta = new Vector2(325, 50);

            tipText.transform.SetSiblingIndex(elvScreen.GetComponentInChildren<BigScreen>().transform.GetSiblingIndex());
        }

        public static void SetOverride(bool state, string key, bool asTip, bool overrideWorksEvenTipsDisabled = false)
        {
            if (state)
            {
                if (tipText != null && (DataManager.ExtraSettings.tipsEnabled || overrideWorksEvenTipsDisabled))
                {
                    string text = Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
                    tipText.text = asTip ? Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Elv_Tip_Base") + "\n" + text : text;
                }
            } else
            {
                if (tipText != null)
                {
                    tipText.text = originalText;
                } 
            }
        }

    }
}
