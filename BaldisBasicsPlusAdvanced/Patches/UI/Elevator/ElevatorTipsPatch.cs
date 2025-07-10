using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.UI.Elevator
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class ElevatorTipsPatch
    {
        private static ElevatorScreen elvScreen;

        private static TipsMonitor monitor;

        private static string originalText;

        private static Dictionary<string, int> tipUsesDatas = new Dictionary<string, int>();

        private static int tipMaxUses = 3;

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
        [HarmonyPrefix]
        private static void OnStart(ElevatorScreen __instance, ref Canvas ___canvas)
        {
            elvScreen = __instance;

            LocalizationManager localization = Singleton<LocalizationManager>.Instance;

            originalText = GetRawTip();

            monitor = CreateTipText(originalText);
            monitor.gameObject.SetActive(false);

            if (LoadTip)
                __instance.OnLoadReady += monitor.Activate;
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

        public static void SetOverride(bool state, string key, bool overrideWorksEvenTipsDisabled = false)
        {
            SetOverride(monitor, state, key, overrideWorksEvenTipsDisabled);
        }

#warning TODO: overrideWorksEvenTipsDisabled checking
        public static void SetOverride(TipsMonitor monitor, bool state, string key, bool overrideWorksEvenTipsDisabled = false)
        {
            if (state)
            {
                if (monitor != null && (LoadTip || overrideWorksEvenTipsDisabled))
                {
                    string text = LocalizationManager.Instance.GetLocalizedText(key);
                    monitor.Override(text);
                    if (!LoadTip) monitor.Activate();
                }
            }
            else
            {
                if (monitor != null)
                {
                    if (!LoadTip) monitor.Deactivate();
                    else monitor.ResetOverride();
                }
            }
        }

    }
}
