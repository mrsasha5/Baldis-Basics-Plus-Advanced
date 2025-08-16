using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator
{
#warning Fix visual issue
    public class TipsMonitor : MonoBehaviour
    {

        private AudioManager audMan;

        private CustomImageAnimator animator;

        private CustomImageAnimator maskAnimator;

        private TMP_Text tmp;

        private string originalText;

        private string playingAnim;

        private string playingMaskAnim;

        private bool activated;

        private List<IEnumerator> animations;

        private IEnumerator staticEnumerator;

        //private int index;

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
            /*if (Input.GetKeyDown(KeyCode.T))
            {
                List<string> tips = ApiManager.GetAllTips();
                
                if (index > tips.Count - 1) index = 0;

                tmp.text = tips[index].Localize();
                SetStaticAnimation();
                index++;
            }*/

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
    }
}
