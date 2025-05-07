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

namespace BaldisBasicsPlusAdvanced.Patches.UI
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class ElevatorPatch
    {
        private static TextMeshProUGUI tipText;

        private static string overrideText;

        private static bool overrideEvenTipsDisabled;

        //public static bool patched => tipText != null;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void onStart(ElevatorScreen __instance)
        {
            //(ApiManager.ElevatorTopText && (DataManager.ExtraSettings.tipsEnabled || (overrideText != null && overrideEvenTipsDisabled)))
            if (DataManager.ExtraSettings.tipsEnabled || (overrideText != null && overrideEvenTipsDisabled))
            {
                LocalizationManager localizationManager = Singleton<LocalizationManager>.Instance;

                bool loadTip = !(ObjectsStorage.TipKeys.Count == 0);

                string tip = "";

                int modNum = UnityEngine.Random.Range(0, ObjectsStorage.TipKeys.Count);

                if (loadTip)
                {
                    PluginInfo key = ObjectsStorage.TipKeys.Keys.ToList()[modNum];

                    int tipNum = UnityEngine.Random.Range(0, ObjectsStorage.TipKeys[key].Count);

                    tip = localizationManager.GetLocalizedText(ObjectsStorage.TipKeys[key][tipNum]);
                }

                string text = localizationManager.GetLocalizedText("Adv_Elv_Tip_Base") + "\n" + tip;

                if (overrideText != null)
                {
                    text = localizationManager.GetLocalizedText(overrideText);
                    overrideText = null;
                    overrideEvenTipsDisabled = false;
                }

                Canvas elevatorCanvas = ReflectionHelper.getValue<Canvas>(__instance, "canvas");

                tipText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, text, elevatorCanvas.transform, new Vector3(0, 150, 0), false);
                tipText.fontSize = 12;
                tipText.alignment = TextAlignmentOptions.Top;

                tipText.GetComponent<RectTransform>().sizeDelta = new Vector2(325, 50);

                tipText.transform.SetSiblingIndex(__instance.GetComponentInChildren<BigScreen>().transform.GetSiblingIndex());
            }
            
        }

        public static void setOverride(string key, bool overrideWorksEvenTipsDisabled = true)
        {
            ElevatorPatch.overrideText = key;
            ElevatorPatch.overrideEvenTipsDisabled = overrideWorksEvenTipsDisabled;
        }

    }
}
