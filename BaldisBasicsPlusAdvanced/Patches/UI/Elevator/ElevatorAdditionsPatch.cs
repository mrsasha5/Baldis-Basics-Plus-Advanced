using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using MTM101BaldAPI.Components.Animation;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Patches.UI.Elevator
{
    //I would override prefab, but Mystman copied that in MainMenu and overriding prefab is not affecting instance from MainMenu
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class ElevatorAdditionsPatch
    {

        private class UnityEventsTracker : MonoBehaviour
        {

            private void OnDestroy()
            {
                loseAnimationQueued = false;
                tubesUpdateRequired = false;
                lastSentLifes = null;
            }

        }

        public static ElevatorScreen elvScreen;

        private static AudioSource lifesAudio;

        private static TipsMonitor monitor;

        private static string originalText;

        private static Dictionary<string, int> tipUsesData = new Dictionary<string, int>();

        private static int tipMaxUses = 3;

        private static StandardMenuButton tubesButton;

        private static TipsMonitor.MonitorOverrider tubesOverrider;

        private static Image tubesGlowImage;

        private static Image[] masks = new Image[3];

        public static Sprite[] explosionSprites = new Sprite[12];

        private static CustomImageAnimator explosionAnimator;

        private static Vector3[] explosionPositions = new Vector3[]
        {
            new Vector3(165f, -35f, 0f),
            new Vector3(190f, -40f, 0f),
            new Vector3(225f, -45f, 0f)
        };

        private static float[] startChargeHeights = new float[]
        {
            97f,
            97f,
            97f
        };

        private static float[] endChargeHeights = new float[]
        {
            37f,
            27f,
            15f
        };

        private static bool loseAnimationQueued = false;

        private static bool tubesUpdateRequired = false;

        private static int? lastSentLifes;

        public static bool LoadTip => !(ObjectsStorage.TipKeys.Count == 0) &&
                (OptionsDataManager.ExtraSettings.GetValue<bool>("tips"));

        public static bool AnimationsEnabled => OptionsDataManager.ExtraSettings.GetValue<bool>("elevator_animations");

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
        [HarmonyPrefix]
        private static bool OnUpdateLives()
        {
            int currentLives = CoreGameManager.Instance.Lives + 1;

            if (AnimationsEnabled)
            {
                if (!loseAnimationQueued && tubesUpdateRequired)
                    OnNewLifesAdded();

                if (lastSentLifes != null && currentLives != lastSentLifes)
                {
                    if (loseAnimationQueued)
                        tubesUpdateRequired = true;
                    else
                        OnNewLifesAdded();
                }
                else lastSentLifes = currentLives;
            }

            tubesOverrider?.UpdateText(string.Format("Adv_Elv_TubesTip".Localize(), currentLives,
                                    ReflectionHelper.GetValue<int>(CoreGameManager.Instance, "extraLives"),
                                    CoreGameManager.Instance.Attempts));

            if (currentLives > 3) currentLives = 3;

            if (tubesGlowImage != null) 
                tubesGlowImage.sprite = AssetsStorage.sprites[$"adv_elv_tubes_glow_{currentLives}"];

            return !loseAnimationQueued;
        }

        [HarmonyPatch("ButtonPressed")]
        [HarmonyPostfix]
        private static void OnPressed(ref bool ___readyToStart)
        {
            if (___readyToStart) monitor.Deactivate();
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(ElevatorScreen __instance, ref Canvas ___canvas, ref AudioManager ___audMan)
        {
            elvScreen = __instance;
            elvScreen.gameObject.AddComponent<UnityEventsTracker>();
            ___audMan.positional = false;

            if (AnimationsEnabled)
            {
                lifesAudio = new GameObject("LifesAudio").AddComponent<AudioSource>();
                lifesAudio.transform.SetParent(__instance.transform, false);
                lifesAudio.ignoreListenerPause = true;
            }

            originalText = GetRawTip();

            monitor = CreateTipText(originalText);
            monitor.gameObject.SetActive(false);

            InitializeTubesGlowImage();

            if (LoadTip)
            {
                elvScreen.StartCoroutine(ElevatorWaiter());
                InitializeTubesButton();
            }

            if (AnimationsEnabled)
            {
                InitializeMasks();

                if (loseAnimationQueued) PrepareLoseAnimation();
            }
        }

        private static void InitializeTubesGlowImage()
        {
            Image tubesImage = Array.Find(elvScreen.GetComponentsInChildren<Image>(), x => x.name == "Lives");

            int glowNum = CoreGameManager.Instance.Lives + 1;

            if (glowNum > 3) glowNum = 3;

            tubesGlowImage = UIHelpers.CreateImage(AssetsStorage.sprites[$"adv_elv_tubes_glow_{glowNum}"],
                parent: elvScreen.Canvas.transform, Vector3.zero, correctPosition: false);

            tubesGlowImage.transform.SetSiblingIndex(tubesImage.transform.GetSiblingIndex() + 1);
            tubesGlowImage.ToCenter();
            tubesGlowImage.transform.localPosition = Vector3.right * 160f;
            tubesGlowImage.gameObject.SetActive(false);
        }

        private static void InitializeTubesButton()
        {
            Image tubesImage = Array.Find(elvScreen.GetComponentsInChildren<Image>(), x => x.name == "Lives");

            int glowNum = CoreGameManager.Instance.Lives + 1;

            if (glowNum > 3) glowNum = 3;

            tubesButton = ObjectsCreator.CreateSpriteButton(AssetsStorage.sprites["transparent"],
                Vector3.right * 200f, elvScreen.Canvas.transform);
            tubesButton.name = "LivesCursorChecker";
            tubesButton.transform.SetSiblingIndex(tubesGlowImage.transform.GetSiblingIndex() + 1);
            tubesButton.image.rectTransform.sizeDelta = new Vector2(100f, 232f);
            tubesButton.eventOnHigh = true;
            tubesButton.OnHighlight.AddListener(delegate()
            {
                tubesOverrider = SetOverride(
                    string.Format("Adv_Elv_TubesTip".Localize(), CoreGameManager.Instance.Lives + 1,
                    ReflectionHelper.GetValue<int>(CoreGameManager.Instance, "extraLives"),
                    CoreGameManager.Instance.Attempts));
                tubesGlowImage?.gameObject.SetActive(true);
            }
            );
            tubesButton.OffHighlight.AddListener(delegate ()
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

        private static void InitializeMasks()
        {
            Image tubesImage = Array.Find(elvScreen.GetComponentsInChildren<Image>(), x => x.name == "Lives");

            for (int i = 0; i < masks.Length; i++)
            {
                masks[i] = UIHelpers.CreateImage(AssetsStorage.sprites[$"adv_elv_tube_mask_{i + 1}"],
                    elvScreen.Canvas.transform, Vector3.zero, correctPosition: false);
                masks[i].name = $"TubeMask_{i + 1}";
                masks[i].transform.SetSiblingIndex(tubesImage.transform.GetSiblingIndex() + 1);
                masks[i].ToCenter();
                masks[i].transform.localPosition = Vector3.right * 160f;
                masks[i].gameObject.AddComponent<Mask>();
                masks[i].color = new Color(1f, 0f, 0f, 0.5f);
                masks[i].gameObject.SetActive(false);
            }

        }

        public static void StartLoseAnimation()
        {
            if (AnimationsEnabled)
                loseAnimationQueued = true;
        }

        private static void OnNewLifesAdded()
        {
            if (tubesButton != null) tubesButton.gameObject.SetActive(false);
            lifesAudio.volume = 1f;
            lifesAudio.PlayOneShot(AssetsStorage.sounds["power_breaker_lights_on"].soundClip, 2f);
            elvScreen.StartCoroutine(NewLifesAnimator());
        }

        private static IEnumerator NewLifesAnimator()
        {
            tubesGlowImage.gameObject.SetActive(true);

            Color color = Color.white;
            tubesGlowImage.color = color;

            while (color.a > 0f)
            {
                color.a -= Time.unscaledDeltaTime;
                tubesGlowImage.color = color;
                yield return null;
            }

            tubesGlowImage.gameObject.SetActive(false);
            if (tubesButton != null) tubesButton.gameObject.SetActive(true);
            tubesGlowImage.color = Color.white;
        }

        private static void PrepareLoseAnimation()
        {
            Image tubesImage = Array.Find(elvScreen.GetComponentsInChildren<Image>(), x => x.name == "Lives");

            int lifes = CoreGameManager.Instance.Lives + 1;
            Sprite[] lifeImages = ReflectionHelper.GetValue<Sprite[]>(elvScreen, "lifeImages");

            if (lifes > 2) return; //Cannot animate that

            tubesImage.sprite = lifeImages[lifes + 1];

            masks[lifes].gameObject.SetActive(true);

            lifesAudio.clip = AssetsStorage.sounds["adv_turning_loop"].soundClip;
            lifesAudio.loop = true;
            lifesAudio.Play();

            Image explosion =
                UIHelpers.CreateImage(explosionSprites[0], elvScreen.Canvas.transform, Vector3.zero, false);
            explosion.name = "Explosion";
            explosion.transform.SetSiblingIndex(tubesGlowImage.transform.GetSiblingIndex() + 1);
            explosion.ToCenter();
            explosion.transform.localPosition = explosionPositions[lifes];
            explosion.color = new Color(1f, 1f, 1f, 0f);
            explosionAnimator = explosion.gameObject.AddComponent<CustomImageAnimator>();
            explosionAnimator.useScaledTime = false;
            explosionAnimator.timeScale = TimeScaleType.Null;
            explosionAnimator.image = explosion;
            explosionAnimator.AddAnimation("standard", new SpriteAnimation(10, explosionSprites));
            explosionAnimator.gameObject.SetActive(false);

            elvScreen.StartCoroutine(ExplosionAnimator(3f, 2f, 1.5f, lifes));
        }

        private static IEnumerator ExplosionAnimator(float delay, float timeBeforeUpdateTubes, float time, int index)
        {
            LevelBuilder builder = GameObject.FindObjectOfType<LevelBuilder>();

            Image tubesImage = Array.Find(elvScreen.GetComponentsInChildren<Image>(), x => x.name == "Lives");
            Sprite[] lifeImages = ReflectionHelper.GetValue<Sprite[]>(elvScreen, "lifeImages");

            Image chargeImage = UIHelpers.CreateImage(AssetsStorage.sprites["adv_white"],
                masks[index].transform, Vector3.zero, correctPosition: false);
            chargeImage.name = "TubeCharge";
            chargeImage.transform.localPosition = Vector3.up * startChargeHeights[index];
            chargeImage.rectTransform.sizeDelta = new Vector2(500f, 200f);
            chargeImage.color = new Color(0f, 0f, 0f, 0.7f);

            Vector3 pos = chargeImage.transform.localPosition;

            float speed = Math.Abs(startChargeHeights[index] - endChargeHeights[index]) / time;
            float _delay = delay;

            while (delay > 0f)
            {
                delay -= Time.unscaledDeltaTime;
                if (delay < 0f) delay = 0f;
                lifesAudio.volume = (1f - (delay / _delay)) * 0.75f;
                yield return null;
            }

            while (!builder.levelCreated)
            {
                yield return null;
            }

            float time2 = 0.5f;
            while (time2 > 0f)
            {
                time2 -= Time.unscaledDeltaTime;
                yield return null;
            }

            lifesAudio.loop = false;
            lifesAudio.clip = null;
            lifesAudio.PlayOneShot(AssetsStorage.sounds["adv_turning_end"].soundClip);

            while (pos.y > endChargeHeights[index])
            {
                pos.y -= Time.unscaledDeltaTime * speed;

                if (pos.y <= endChargeHeights[index]) pos.y = endChargeHeights[index];

                chargeImage.transform.localPosition = pos;

                yield return null;
            }

            explosionAnimator.gameObject.SetActive(true);
            explosionAnimator.image.color = Color.white;
            explosionAnimator.Play("standard", 99f);
            explosionAnimator.ChangeSpeed(1f);

            bool livesUpdated = false;

            lifesAudio.PlayOneShot(AssetsStorage.sounds["explosion"].soundClip);

            while (explosionAnimator.AnimationId != null)
            {
                if (!livesUpdated && explosionAnimator.AnimationFrame >= 5)
                {
                    tubesImage.sprite = lifeImages[index];
                    masks[index].gameObject.SetActive(false);
                }
                yield return null;
            }

            explosionAnimator.gameObject.SetActive(false);
            tubesImage.sprite = lifeImages[index];
            masks[index].gameObject.SetActive(false);

            while (timeBeforeUpdateTubes > 0f)
            {
                timeBeforeUpdateTubes -= Time.unscaledDeltaTime;
                yield return null;
            }

            loseAnimationQueued = false;

            if (tubesUpdateRequired)
                ReflectionHelper.UseMethod(elvScreen, "UpdateLives");
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
