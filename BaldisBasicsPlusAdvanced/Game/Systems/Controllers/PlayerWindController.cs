using UnityEngine;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using Rewired.UI.ControlMapper;
using BaldisBasicsPlusAdvanced.SaveSystem.Managers;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using BaldisBasicsPlusAdvanced.Extensions;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class PlayerWindController : BaseController
    {
        private WindObject windObj;

        private bool blowingAllowed = true;

        private bool lookBack;

        private float baseTime;

        private HudGauge gauge;

        public override int MaxCount => 1;

        public override void SetTime(float time)
        {
            base.SetTime(time);
            if (owner == ControllerOwner.Player)
            {
                gauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager
                    .ActivateNewGauge(ObjectsStorage.ItemObjects["WindBlower"].itemSpriteSmall, time);
            }
            baseTime = time;
        }

        public void CreateWind(int size, float speed)
        {
            windObj = ObjectsCreator.CreateWindObject(size, speed, true);
            windObj.transform.SetParent(pm.transform, false);

            ReflectionHelper.SetValue(windObj.GetComponentInChildren<AudioManager>(), "disableSubtitles", true);

            windObj.AudMan.OverrideSourcePosition(pm.transform.position);

            windObj.SetActivityState(true);
        }

        public override void VirtualUpdate()
        {
            windObj.AudMan.OverrideSourcePosition(pm.transform.position);

            if (Input.GetKeyDown(KeyBindingsManager.Keys["wind_blower_switch"].Button))
            {
                blowingAllowed = !blowingAllowed;
                StartFadeEffect(!blowingAllowed);
            }

            bool playerHidden = entity.Frozen || entity.InteractionDisabled;

            if ((!blowingAllowed || playerHidden) && !windObj.Hidden)
            {
                windObj.SetActivityState(false);
            }
            else if (blowingAllowed && !playerHidden && windObj.Hidden)
            {
                windObj.SetActivityState(true);
            }

            if (time > 0 && blowingAllowed && !playerHidden)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                gauge?.SetValue(baseTime, time);
            }

            if (lookBack != Singleton<InputManager>.Instance.GetDigitalInput("LookBack", onDown: false))
            {
                lookBack = Singleton<InputManager>.Instance.GetDigitalInput("LookBack", onDown: false);
                Quaternion quaternion = windObj.transform.rotation;

                Vector3 rotatedBack = windObj.transform.parent.rotation.eulerAngles;
                rotatedBack.x += 180f;

                quaternion.eulerAngles = lookBack ? rotatedBack : windObj.transform.parent.rotation.eulerAngles;
                windObj.transform.rotation = quaternion;
            }
        }

        private void StartFadeEffect(bool fadeIn)
        {
            if (gauge == null) return;
            Image image = gauge.GetComponentsInChildren<Image>().Last();
            image.StopAllCoroutines();
            image.StartCoroutine(FadeEffect(!fadeIn));
        }

        private IEnumerator FadeEffect(bool isOut)
        {
            float val = isOut ? 0.5f : 1f;

            Image image = gauge.GetComponentsInChildren<Image>().Last();
            Color color = new Color(val, val, val, val);

            if (isOut)
            {
                while (val < 1f)
                {
                    val = Mathf.Clamp(val + Time.unscaledDeltaTime, 0.5f, 1f);
                    color.r = color.g = color.b = color.a = val;
                    image.color = color;
                    yield return null;
                }

            } else
            {
                while (val > 0.5f)
                {
                    val = Mathf.Clamp(val - Time.unscaledDeltaTime, 0.5f, 1f);
                    color.r = color.g = color.b = color.a = val;
                    image.color = color;
                    yield return null;
                }
            }

        }

        public override void OnPreDestroying()
        {
            base.OnPreDestroying();
            windObj.SetActivityState(false);
            GameObject.Destroy(windObj.gameObject);
            ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_pah"]);
            gauge?.Deactivate();
        }
    }
}
