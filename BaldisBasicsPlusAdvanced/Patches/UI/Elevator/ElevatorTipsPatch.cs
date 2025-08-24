using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BBPlusCustomMusics.MonoBehaviours;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Patches.UI.Elevator
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class ElevatorTipsPatch
    {
        public static ElevatorScreen elvScreen;

        private static TipsMonitor monitor;

        private static string originalText;

        private static Dictionary<string, int> tipUsesDatas = new Dictionary<string, int>();

        private static int tipMaxUses = 3;

        private static TipsMonitor.MonitorOverrider tubesOverrider;

        public static bool LoadTip => !(ObjectsStorage.TipKeys.Count == 0) &&
                (OptionsDataManager.ExtraSettings.GetValue<bool>("tips"));

        public static string GetTipText()
        {
            if (!LoadTip) return "";
            return LocalizationManager.Instance.GetLocalizedText("Adv_Elv_Tip_Base") + "\n" + GetRandomTip();
        }

        public static string GetRawTip()
        {
            if (!LoadTip) return "";
            return GetRandomTip();
        }

        public static string GetRandomTip()
        {
            List<string> tipKeys = ApiManager.GetAllTips();

            if (tipKeys.Count == 0) return "OHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNOOHNO!";

            bool clearDatas = true;
            foreach (int val in tipUsesDatas.Values)
            {
                if (val <= tipMaxUses)
                {
                    clearDatas = false;
                    break;
                }
            }
            
            if (clearDatas) tipUsesDatas.Clear();

            string tip = null;

            while (tip == null)
            {
                if (tipKeys.Count <= 0) break;

                int tipNum = UnityEngine.Random.Range(0, tipKeys.Count);
                string _tip = tipKeys[tipNum];

                if (tipUsesDatas.ContainsKey(_tip))
                {
                    tipUsesDatas[_tip]++;

                    if (tipUsesDatas[_tip] > tipMaxUses)
                    {
                        tipKeys.Remove(_tip);
                        continue;
                    }

                    tip = _tip;
                }
                else
                {
                    tipUsesDatas.Add(_tip, 1);
                    tip = _tip;
                }
            }

            return tip.Localize();
        }

        [HarmonyPatch("ButtonPressed")]
        [HarmonyPostfix]
        private static void OnPressed(ref bool ___readyToStart)
        {
            if (___readyToStart) monitor.Deactivate();
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(ElevatorScreen __instance, ref Canvas ___canvas)
        {
            elvScreen = __instance;

            LocalizationManager localization = Singleton<LocalizationManager>.Instance;

            originalText = GetRawTip();

            monitor = CreateTipText(originalText);
            monitor.gameObject.SetActive(false);

            if (LoadTip)
            {
                elvScreen.StartCoroutine(ElevatorWaiter());

                Image tubesImage = Array.Find(
                    __instance.GetComponentsInChildren<Image>(), x => x.name == "Lives");

                StandardMenuButton button = ObjectsCreator.CreateSpriteButton(AssetsStorage.sprites["transparent"],
                    new Vector3(200f, 0f, 0f), __instance.Canvas.transform);
                button.name = "LivesCursorChecker";
                button.transform.SetSiblingIndex(tubesImage.transform.GetSiblingIndex() + 1);
                button.image.rectTransform.sizeDelta = new Vector2(100f, 232f);
                button.eventOnHigh = true;
                button.OnHighlight.AddListener(
                    () => tubesOverrider = SetOverride(
                        string.Format("Adv_Elv_TubesTip".Localize(), CoreGameManager.Instance.Lives + 1,
                        ReflectionHelper.GetValue<int>(CoreGameManager.Instance, "extraLives"),
                        CoreGameManager.Instance.Attempts
                        )));
                button.OffHighlight.AddListener(delegate ()
                {
                    tubesOverrider?.Release();
                    tubesOverrider = null;
                });
            }
        }

        private static IEnumerator ElevatorWaiter()
        {
            List<IEnumerator> enumerators = ReflectionHelper.GetValue<List<IEnumerator>>(elvScreen, "queuedEnumerators");
            while (enumerators.Count > 0 || ReflectionHelper.GetValue<bool>(elvScreen, "busy")) yield return null;
            monitor.Activate();
        }

        private static TipsMonitor CreateTipText(string text)
        {
            TipsMonitor tipsScreen = new GameObject("Tips Screen", typeof(RectTransform))
                    .AddComponent<TipsMonitor>();
            tipsScreen.transform.SetParent(elvScreen.Canvas.transform, false);
            tipsScreen.transform.localPosition = new Vector3(0f, 151.5f, 0f);
            tipsScreen.Initialize(text);
            tipsScreen.transform.SetSiblingIndex(elvScreen.GetComponentInChildren<BigScreen>().transform.GetSiblingIndex());
            return tipsScreen;
        }

        public static TipsMonitor.MonitorOverrider SetOverride(string key, int priority = 0)
        {
            if (monitor != null && LoadTip)
            {
                string text = LocalizationManager.Instance.GetLocalizedText(key);
                return monitor.Override(text, priority);
            }
            return null;
        }

    }
}
