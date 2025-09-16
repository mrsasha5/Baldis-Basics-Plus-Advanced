using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.UI;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class BlindController : BaseController
    {
        private Canvas canvas;

        private Image image;

        private HudGauge gauge;

        private float baseTime;

        public override void OnInitialize()
        {
            base.OnInitialize();
            entity.SetBlinded(true);
            if (owner == ControllerOwner.Player)
            {
                canvas = ObjectsCreator.CreateCanvas(setGlobalCam: true, planeDistance: 2f);
                canvas.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                scaler.screenMatchMode = ScreenMatchMode.MatchWidthOrHeight;

                image = UIHelpers.CreateImage(AssetsStorage.sprites["adv_white"], canvas.transform, Vector3.zero,
                    correctPosition: false);
                image.ToCenter();
                image.rectTransform.sizeDelta = new Vector2(480f, 360f);
                image.color = Color.white;
            }
        }

        public void SetGauge(HudGauge gauge)
        {
            this.gauge = gauge;
        }

        public override void SetTime(float time)
        {
            base.SetTime(time);
            baseTime = time;
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            gauge?.SetValue(baseTime, time);
        }

        public override void OnPreDestroying()
        {
            base.OnPreDestroying();
            entity.SetBlinded(false);
            if (owner == ControllerOwner.Player)
            {
                entity.StartCoroutine(FadeOutEffect());
            }
            gauge?.Deactivate();
        }

        private IEnumerator FadeOutEffect()
        {
            float val = 1f;
            Color color = new Color(1f, 1f, 1f, val);

            while (val > 0f)
            {
                val -= Time.deltaTime * 0.5f;
                color.a = val;
                image.color = color;
                yield return null;
            }

            GameObject.Destroy(canvas.gameObject);
        }

    }
}
