using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

        private static Dictionary<string, int> tipUsesData = new Dictionary<string, int>();

        private static int tipMaxUses = 3;

        private static TipsMonitor.MonitorOverrider tubesOverrider;

        private static Image tubesGlowImage;

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

            bool clearData = true;
            foreach (int val in tipUsesData.Values)
            {
                if (val <= tipMaxUses)
                {
                    clearData = false;
                    break;
                }
            }
            
            if (clearData) tipUsesData.Clear();

            string tip = null;

            while (tip == null)
            {
                if (tipKeys.Count <= 0) break;

                int tipNum = UnityEngine.Random.Range(0, tipKeys.Count);
                string _tip = tipKeys[tipNum];

                if (tipUsesData.ContainsKey(_tip))
                {
                    if (tipUsesData[_tip] > tipMaxUses)
                    {
                        tipKeys.Remove(_tip);
                        continue;
                    }

                    tipUsesData[_tip]++;

                    tip = _tip;
                }
                else
                {
                    tipUsesData.Add(_tip, 1);
                    tip = _tip;
                }
            }

            return tip.Localize();
        }

        [HarmonyPatch("UpdateLives")]
        [HarmonyPostfix]
        private static void OnUpdateLives()
        {
            int currentLives = CoreGameManager.Instance.Lives + 1;

            tubesOverrider?.UpdateText(string.Format("Adv_Elv_TubesTip".Localize(), currentLives,
                    ReflectionHelper.GetValue<int>(CoreGameManager.Instance, "extraLives"),
                    CoreGameManager.Instance.Attempts));

            if (currentLives > 3) currentLives = 3;

            if (tubesGlowImage != null) 
                tubesGlowImage.sprite = AssetsStorage.sprites[$"adv_elevator_tubes_glow_{currentLives}"];
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
                InitializeTubesButton(__instance);
            }
        }

        private static void InitializeTubesButton(ElevatorScreen elvScreen)
        {
            Image tubesImage = Array.Find(
                    elvScreen.GetComponentsInChildren<Image>(), x => x.name == "Lives");

            int glowNum = CoreGameManager.Instance.Lives + 1;

            if (glowNum > 3) glowNum = 3;

            tubesGlowImage = UIHelpers.CreateImage(AssetsStorage.sprites[$"adv_elevator_tubes_glow_{glowNum}"],
                parent: elvScreen.Canvas.transform, Vector3.zero, correctPosition: false);

            tubesGlowImage.transform.SetSiblingIndex(tubesImage.transform.GetSiblingIndex() + 1);
            tubesGlowImage.ToCenter();
            tubesGlowImage.transform.localPosition = Vector3.right * 161f;
            tubesGlowImage.gameObject.SetActive(false);

            StandardMenuButton button = ObjectsCreator.CreateSpriteButton(AssetsStorage.sprites["transparent"],
                Vector3.right * 200f, elvScreen.Canvas.transform);
            button.name = "LivesCursorChecker";
            button.transform.SetSiblingIndex(tubesGlowImage.transform.GetSiblingIndex() + 1);
            button.image.rectTransform.sizeDelta = new Vector2(100f, 232f);
            button.eventOnHigh = true;
            button.OnHighlight.AddListener(delegate()
            {
                tubesOverrider = SetOverride(
                    string.Format("Adv_Elv_TubesTip".Localize(), CoreGameManager.Instance.Lives + 1,
                    ReflectionHelper.GetValue<int>(CoreGameManager.Instance, "extraLives"),
                    CoreGameManager.Instance.Attempts));
                tubesGlowImage?.gameObject.SetActive(true);
            }
            );
            button.OffHighlight.AddListener(delegate ()
            {
                tubesGlowImage?.gameObject.SetActive(false);
                tubesOverrider?.Release();
                tubesOverrider = null;
            });
            
        }

        private static IEnumerator ElevatorWaiter()
        {
            List<IEnumerator> enumerators = ReflectionHelper.GetValue<List<IEnumerator>>(elvScreen, "queuedEnumerators");
            while (enumerators.Count > 0 || ReflectionHelper.GetValue<bool>(elvScreen, "busy")) yield return null;
            monitor.Activate();
#if DEBUG
            Debug_InitializeSwitchButton();
#endif
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

#if DEBUG

        private static int debug_modIndex;

        private static int debug_tipIndex;

        private static void Debug_InitializeSwitchButton()
        {
            if (ObjectsStorage.TipKeys.Count == 0) return;
            debug_modIndex = 0;
            debug_tipIndex = 0;

            KeyValuePair<PluginInfo, List<string>> pair = ObjectsStorage.TipKeys.ElementAt(debug_modIndex);

            TextMeshProUGUI pluginText = UIHelpers.CreateText<TextMeshProUGUI>(
                BaldiFonts.ComicSans12,
                $"Chosen plugin:\n{pair.Key.Metadata.GUID}",
                elvScreen.Canvas.transform, Vector3.up * 30f, false);
            pluginText.alignment = TextAlignmentOptions.Center;

            TextMeshProUGUI switchText = UIHelpers.CreateText<TextMeshProUGUI>(
                BaldiFonts.ComicSans18, 
                "Switch to next", 
                elvScreen.Canvas.transform, Vector3.up * -20f, false);
            switchText.alignment = TextAlignmentOptions.Center;

            StandardMenuButton pluginButton =
                ObjectsCreator.AddButtonProperties(pluginText, new Vector2(150f, 50f), underlineOnHighlight: true);
            pluginButton.OnPress.AddListener(() => Debug_SwitchPlugin(pluginText));

            StandardMenuButton switchButton =
                ObjectsCreator.AddButtonProperties(switchText, new Vector2(150f, 50f), underlineOnHighlight: true);
            switchButton.OnPress.AddListener(() => Debug_SwitchTip(switchText));

            pluginButton.transform.SetSiblingIndex(monitor.transform.GetSiblingIndex());
            switchButton.transform.SetSiblingIndex(monitor.transform.GetSiblingIndex());

            monitor.Debug_Override(pair.Value.Count > 0 ? pair.Value[debug_tipIndex].Localize() : "NO REGISTERED TIPS!");
        }

        private static void Debug_SwitchPlugin(TextMeshProUGUI tmp)
        {
            debug_modIndex++;
            debug_tipIndex = 0;
            if (debug_modIndex >= ObjectsStorage.TipKeys.Count)
            {
                debug_modIndex = 0;
            }

            KeyValuePair<PluginInfo, List<string>> pair = ObjectsStorage.TipKeys.ElementAt(debug_modIndex);

            tmp.text = $"Chosen plugin:\n{pair.Key.Metadata.GUID}";

            monitor.Debug_Override(pair.Value.Count > 0 ? pair.Value[debug_tipIndex].Localize() : "NO REGISTERED TIPS!");
        }

        private static void Debug_SwitchTip(TextMeshProUGUI tmp)
        {
            debug_tipIndex++;

            KeyValuePair<PluginInfo, List<string>> pair = ObjectsStorage.TipKeys.ElementAt(debug_modIndex);

            if (debug_tipIndex >= pair.Value.Count)
            {
                debug_tipIndex = 0;
            }

            monitor.Debug_Override(pair.Value.Count > 0 ? pair.Value[debug_tipIndex].Localize() : "NO REGISTERED TIPS!");
        }

#endif

    }
}
