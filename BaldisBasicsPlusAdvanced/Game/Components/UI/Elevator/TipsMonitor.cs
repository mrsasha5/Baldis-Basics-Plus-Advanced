using System;
using System.Collections;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BepInEx;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Rewired.Platforms.Custom.CustomPlatformUnifiedKeyboardSource.KeyPropertyMap;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator
{

    public class TipsMonitor : MonoBehaviour
    {

        public class MonitorOverrider
        {

            public TipsMonitor monitor;

            public int priority;

            public string text;

            public Action onUpdate;

            public void UpdateText(string text)
            {
                this.text = text;
                if (monitor.overrider == this)
                {
                    monitor.tmp.text = this.text;
                }
            }

            public void Release()
            {
                monitor.ResetOverrider(this);
            }

        }

        private AudioManager audMan;

        private Image[] images;

        private TMP_Text tmp;

        private string originalText;

        private List<IEnumerator> animations;

        private List<MonitorOverrider> overriders;

        private IEnumerator staticAnimatation;

        private bool activated;

        private bool levelGenError;

        private MonitorOverrider overrider;

        public void Initialize(string originalText)
        {
            animations = new List<IEnumerator>();
            overriders = new List<MonitorOverrider>();
            images = new Image[2];

            this.originalText = originalText;

            audMan = ObjectsCreator.CreateAudMan(gameObject);
            audMan.ignoreListenerPause = true;
            audMan.useUnscaledPitch = true;
            ReflectionHelper.SetValue<bool>(audMan, "disableSubtitles", true);

            Image image =
                UIHelpers.CreateImage(AssetsStorage.spriteSheets["adv_tips_screen"][0], transform, Vector3.zero, false);
            image.rectTransform.localScale = new Vector3(1.105f, 1.105f, 1f);

            Image imageForward =
                UIHelpers.CreateImage(AssetsStorage.sprites["adv_tip_screen_forward"], transform, Vector3.zero, false);
            imageForward.rectTransform.localScale = image.rectTransform.localScale;
            imageForward.gameObject.AddComponent<Mask>();
            imageForward.gameObject.SetActive(false);

            images[0] = image;
            images[1] = imageForward;

            tmp = UIHelpers.CreateText<TextMeshProUGUI>(
                BaldiFonts.ComicSans12, originalText, imageForward.transform, Vector3.up * -14f, false);
            tmp.transform.localScale = 1f / imageForward.transform.localScale.x * (Vector3.right + Vector3.up) + Vector3.forward;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.rectTransform.sizeDelta = new Vector2(325, 50);
            tmp.enabled = false;
        }

        private void Update()
        {
            if (CoreGameManager.Instance.levelGenError && !levelGenError)
            {
                ElevatorAdditionsPatch.SetOverride("Adv_Elv_LevelGenErrorTip", priority: 128);
                levelGenError = true;
            }

            if (animations.Count > 0 && staticAnimatation == null)
            {
                if (!animations[0].MoveNext())
                {
                    animations.RemoveAt(0);
                }
            }

            if (staticAnimatation != null && !staticAnimatation.MoveNext())
            {
                staticAnimatation = null;
            }

            overrider?.onUpdate?.Invoke();
        }

        private void PrepareShowTip()
        {
            images[1].gameObject.SetActive(true);
            SetStaticAnimation(onAnimationEnd: ShowTip);
        }

        private void ShowTip()
        {
            tmp.enabled = true;
            images[1].sprite = AssetsStorage.sprites["adv_tip_screen_forward"];
        }

        private void PrepareHideTip()
        {
            tmp.enabled = false;
            SetStaticAnimation(onAnimationEnd: HideTip);
        }

        private void HideTip()
        {
            images[1].gameObject.SetActive(false);
        }

        private void OnStaticStart()
        {
            audMan.QueueAudio(AssetsStorage.sounds["static"]);
            audMan.SetLoop(true);
        }

        private void OnStaticEnd()
        {
            audMan.FlushQueue(true);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            QueueAnimation(images[0], AssetsStorage.spriteSheets["adv_tips_screen"], time: 0.75f, 
                onAnimationEnd: delegate() { PrepareShowTip(); activated = true; });
        }

        public void Deactivate()
        {
            QueueAnimation(images[0], AssetsStorage.spriteSheets["adv_tips_screen_reversed"], time: 0.75f,
                onAnimationStart: delegate () { PrepareHideTip(); activated = false; });
        }

        public MonitorOverrider Override(string text, int priority = 0)
        {
            MonitorOverrider overrider = new MonitorOverrider();
            overrider.text = text;
            overrider.monitor = this;
            overrider.priority = priority;
            overriders.Add(overrider);

            MonitorOverrider lastOverrider = this.overrider;
            UpdateCurrentOverrider();
            if (lastOverrider != this.overrider) OverrideText(this.overrider.text);

            return overrider;
        }

        public void ResetOverrider(MonitorOverrider overrider)
        {
            overriders.Remove(overrider);
            MonitorOverrider lastOverrider = this.overrider;
            UpdateCurrentOverrider();
            if (lastOverrider == this.overrider) return;

            if (this.overrider == null)
            {
                OverrideText(originalText);
            }
            else
            {
                OverrideText(this.overrider.text);
            }         
        }

        private void OverrideText(string text)
        {
            tmp.text = text;

            if (activated)
            {
                tmp.enabled = false;
                PrepareShowTip();
            }
        }

        private void UpdateCurrentOverrider()
        {
            if (overriders.Count == 0)
            {
                overrider = null;
                return;
            }

            int maxIndex = 0;
            int maxPriority = overriders[0].priority;
            for (int i = 0; i < overriders.Count; i++)
            {
                if (overriders[i].priority >= maxPriority)
                {
                    maxIndex = i;
                    maxPriority = overriders[i].priority;
                }
            }

            overrider = overriders[maxIndex];
        }

        private void SetStaticAnimation(Action onAnimationStart = null, Action onAnimationEnd = null)
        {
            onAnimationStart += OnStaticStart;
            onAnimationEnd += OnStaticEnd;
            staticAnimatation = RepeatableAnimator(
                images[1], AssetsStorage.spriteSheets["adv_tip_screen_forward_static_sheet"],
                    time: 0.25f, speed: 10f, onAnimationStart, onAnimationEnd);
        }

        private IEnumerator QueueRepeatableAnimation(Image image, Sprite[] sprites, float time, float speed,
            Action onAnimationStart = null, Action onAnimationEnd = null)
        {
            IEnumerator enumerator = RepeatableAnimator(image, sprites, time, speed, onAnimationStart, onAnimationEnd);
            animations.Add(enumerator);
            return enumerator;
        }

        private IEnumerator QueueAnimation(Image image, Sprite[] sprites, float time,
            Action onAnimationStart = null, Action onAnimationEnd = null)
        {
            IEnumerator enumerator = Animator(image, sprites, time, onAnimationStart, onAnimationEnd);
            animations.Add(enumerator);
            return enumerator;
        }

        private IEnumerator RepeatableAnimator(Image image, Sprite[] sprites, float time, float speed,
            Action onAnimationStart = null, Action onAnimationEnd = null)
        {
            float maxTime = time;

            time = 0f;
            float currentIndex = 0f;

            onAnimationStart?.Invoke();

            while (time < maxTime)
            {
                time += Time.unscaledDeltaTime;
                currentIndex += Time.unscaledDeltaTime * speed;

                if (currentIndex >= sprites.Length) currentIndex = 0f;

                image.sprite = sprites[(int)currentIndex];

                yield return null;
            }

            //image.sprite = sprites[maxIndex]; //Doesn't make sense
            onAnimationEnd?.Invoke();
        }

        private IEnumerator Animator(Image image, Sprite[] sprites, float time,
            Action onAnimationStart = null, Action onAnimationEnd = null)
        {
            float maxTime = time;
            int maxIndex = sprites.Length - 1;

            time = 0f;

            onAnimationStart?.Invoke();

            while (time < maxTime)
            {
                time += Time.unscaledDeltaTime;

                if (time > maxTime) time = maxTime;

                image.sprite = sprites[(int)(time / maxTime * maxIndex)];

                yield return null;
            }

            image.sprite = sprites[maxIndex];

            onAnimationEnd?.Invoke();
        }

#if DEBUG

        public void Debug_Override(string newText)
        {
            originalText = newText;
            tmp.text = originalText;
        }

#endif

    }
}
