using System;
using System.Collections;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator
{

    public class TipsMonitor : MonoBehaviour
    {

        private AudioManager audMan;

        private Image[] images;

        private TMP_Text tmp;

        private string originalText;

        private bool activated;

        private List<IEnumerator> animations;

        //private int index;

        public void Initialize(string originalText)
        {
            animations = new List<IEnumerator>();
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

            //AssetsStorage.spriteSheets["adv_tip_screen_forward_static_sheet"]
        }

        private void Update()
        {
            if (animations.Count > 0)
            {
                if (!animations[0].MoveNext())
                {
                    animations.RemoveAt(0);
                }
            }
        }

        private void PrepareShowTip()
        {
            images[1].gameObject.SetActive(true);
        }

        private void ShowTip()
        {
            tmp.enabled = true;
            images[1].sprite = AssetsStorage.sprites["adv_tip_screen_forward"];
        }

        private void PrepareHideTip()
        {
            tmp.enabled = false;
        }

        private void HideTip()
        {
            images[1].gameObject.SetActive(false);
            images[1].enabled = false;
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
            QueueAnimation(images[0], AssetsStorage.spriteSheets["adv_tips_screen"], time: 1f);
            QueueStaticAnimation(onAnimationStart: PrepareShowTip, onAnimationEnd: ShowTip);
        }

        public void Deactivate()
        {
            QueueStaticAnimation(onAnimationStart: PrepareHideTip, onAnimationEnd: HideTip);
            QueueAnimation(images[0], AssetsStorage.spriteSheets["adv_tips_screen_reversed"], time: 1f);
        }

        public void Override(string text)
        {
            if (activated) QueueStaticAnimation(onAnimationStart: PrepareHideTip, onAnimationEnd: ShowTip);
            tmp.text = text;
        }

        public void ResetOverride()
        {
            QueueStaticAnimation(onAnimationStart: PrepareHideTip, onAnimationEnd: ShowTip);
            tmp.text = originalText;
        }

        private void QueueStaticAnimation(Action onAnimationStart = null, Action onAnimationEnd = null)
        {
            onAnimationStart += OnStaticStart;
            onAnimationEnd += OnStaticEnd;
            QueueRepeatableAnimation(
                images[1], AssetsStorage.spriteSheets["adv_tip_screen_forward_static_sheet"],
                    time: 0.25f, speed: 10f, onAnimationStart, onAnimationEnd);
        }

        private void QueueRepeatableAnimation(Image image, Sprite[] sprites, float time, float speed,
            Action onAnimationStart = null, Action onAnimationEnd = null)
        {
            animations.Add(RepeatableAnimator(image, sprites, time, speed, onAnimationStart, onAnimationEnd));
        }

        private void QueueAnimation(Image image, Sprite[] sprites, float time,
            Action onAnimationStart = null, Action onAnimationEnd = null)
        {
            animations.Add(Animator(image, sprites, time, onAnimationStart, onAnimationEnd));
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

                if (currentIndex > sprites.Length) currentIndex = 0f;

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

                image.sprite = sprites[(int)(time / maxTime * maxIndex)];

                yield return null;
            }

            image.sprite = sprites[maxIndex];

            onAnimationEnd?.Invoke();
        }

        //I decided to cut this implementation because animators from the API are a cancerous tumor.
        /*
        private AudioManager audMan;

        private Image[] images;

        private CustomImageAnimator animator;

        private CustomImageAnimator maskAnimator;

        private TMP_Text tmp;

        private string originalText;

        private string playingAnim;

        private string playingMaskAnim;

        private bool activated;

        private List<IEnumerator> animations;

        private IEnumerator staticEnumerator;

        public void Initialize(string originalText)
        {
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
            imageForward.enabled = false;

            maskAnimator = imageForward.gameObject.AddComponent<CustomImageAnimator>();
            maskAnimator.image = imageForward;
            maskAnimator.useUnscaledTime = true;
            maskAnimator.PopulateAnimations(new Dictionary<string, Sprite[]>()
            {
                { "static", AssetsStorage.spriteSheets["adv_tip_screen_forward_static_sheet"]}
            }, fps: 1);

            tmp = UIHelpers.CreateText<TextMeshProUGUI>(
                BaldiFonts.ComicSans12, originalText, imageForward.transform, Vector3.up * -14f, false);
            tmp.transform.localScale = 1f / imageForward.transform.localScale.x * (Vector3.right + Vector3.up) + Vector3.forward;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.rectTransform.sizeDelta = new Vector2(325, 50);
            tmp.enabled = false;

            animator = image.gameObject.AddComponent<CustomImageAnimator>();
            animator.image = image;
            animator.useUnscaledTime = true;
            animator.PopulateAnimations(new Dictionary<string, Sprite[]>()
            {
                { "appearing", AssetsStorage.spriteSheets["adv_tips_screen"]},
                { "disappearing", AssetsStorage.spriteSheets["adv_tips_screen"].Reverse().ToArray()}
            }, fps: 1);
        }

        public void Activate()
        {
            if (activated) return;

            activated = true;
            tmp.enabled = false;
            maskAnimator.image.enabled = false;

            gameObject.SetActive(true);
            animator.Play("appearing", 8f); //MTM101 forgot to invoke ChangeSpeed???
            animator.ChangeSpeed(8f);
        }

        public void Deactivate()
        {
            if (!activated) return;

            activated = false;
            SetStaticAnimation();
        }

        private void OnAnimationStart(string name)
        {
            switch (name)
            {
                case "static":
                    
                    tmp.enabled = false;

                    audMan.QueueAudio(AssetsStorage.sounds["static"]);
                    audMan.SetLoop(true);
                    break;
                case "disappearing":
                    tmp.enabled = false;
                    maskAnimator.image.enabled = false;
                    break;
            }
        }

        private void OnAnimationEnd(string name)
        {
            switch (name)
            {
                case "appearing":
                    animator.image.sprite =
                        AssetsStorage.spriteSheets["adv_tips_screen"].Last(); //WHY DID YOU ALLOW TO ANIMATION TO IGNORE LAST FRAME??
                    SetStaticAnimation();
                    break;
                case "static":
                    audMan.FlushQueue(true);

                    if (activated)
                    {
                        maskAnimator.image.sprite = AssetsStorage.sprites["adv_tip_screen_forward"];
                        tmp.enabled = true;
                    }
                    else
                    {
                        animator.Play("disappearing", 8f);
                    }
                    
                    break;
            }
        }

        private void Update()
        {
            //THE FUCK I PUT IT IN 0.2.5.2 RELEASE
            //IT'S FOR DEBUGGING, NOT A FEATURE
            /if (Input.GetKeyDown(KeyCode.T))
            /{
            /    List<string> tips = ApiManager.GetAllTips();
            /    
            /    if (index > tips.Count - 1) index = 0;

            /    tmp.text = tips[index].Localize();
            /    SetStaticAnimation();
            /    index++;
            /}

            //MTM101 add delegates like onAnimationEnd and etc
            //DON'T TORTURE ME
            if (playingAnim == null && animator.currentAnimationName != "")
            {
                playingAnim = animator.currentAnimationName;
                OnAnimationStart(playingAnim);
            }
            else if (playingAnim != null && animator.currentAnimationName != playingAnim)
            {
                OnAnimationEnd(playingAnim);
                playingAnim = null;
            }
            if (playingMaskAnim == null && maskAnimator.currentAnimationName != null)
            {
                playingMaskAnim = maskAnimator.currentAnimationName;
                OnAnimationStart(playingMaskAnim);
            }
            else if (playingMaskAnim != null && maskAnimator.currentAnimationName != playingMaskAnim)
            {
                OnAnimationEnd(playingMaskAnim);
                playingMaskAnim = null;
            }
        }

        //It's supposed to be api feature as well as other things, dev shouldn't inventing bike in that case
        //The meaning of API is lost
        private IEnumerator AnimatorBreaker(CustomImageAnimator animator, float time)
        {
            while (time > 0f)
            {
                time -= Time.unscaledDeltaTime;
                yield return null;
            }

            animator.SetDefaultAnimation("", 1f);
        }

        public void SetStaticAnimation()
        {
            maskAnimator.image.enabled = true;
            maskAnimator.SetDefaultAnimation("static", 10f);
            maskAnimator.ChangeSpeed(10f);

            if (staticEnumerator != null) StopCoroutine(staticEnumerator);

            staticEnumerator = AnimatorBreaker(maskAnimator, 0.25f);

            StartCoroutine(staticEnumerator);
        }

        public void Override(string text)
        {
            if (activated) SetStaticAnimation();
            tmp.text = text;
        }

        public void ResetOverride()
        {
            SetStaticAnimation();
            tmp.text = originalText;
        }
        */
    }
}
