using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.ComponentModel;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class ShieldController : BaseController
    {

        private OverlayEffectsManager overlayEffectsMan;

        private bool preparedToDestroy;

        private float fadeEffectsTime = 1f;

        private HudGauge gauge;

        private float baseTime;

        public override ControllerType Type => ControllerType.Effect;

        public override void OnInitialize()
        {
            invincibleEffect = true;
            if (owner == ControllerOwner.Player)
                InitializeOverlay();
        }

        public override void SetTime(float time)
        {
            base.SetTime(time);
            if (owner == ControllerOwner.Player)
            {
                gauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager
                    .ActivateNewGauge(AssetsStorage.sprites["adv_gauge_protection"], time);
            }
            baseTime = time;
        }

        private void InitializeOverlay()
        {
            overlayEffectsMan = GameObject.Instantiate(ObjectsStorage.Overlays["ShieldOverlay"].GetComponent<OverlayEffectsManager>());
            overlayEffectsMan.QueueEffect<FadeInEffect>(fadeEffectsTime);
            overlayEffectsMan.SetAlpha(0f);
            overlayEffectsMan.SetCanvasCam();
        }

        public override void VirtualTriggerEnter(Collider other)
        {
            base.VirtualTriggerEnter(other);
            if (!pm.invincible
                //&& !pm.itm.Has(Items.Apple)
                && other.TryGetComponent(out Baldi baldi)
                && !(baldi.behaviorStateMachine.currentState is Baldi_Chase_Broken)
                && !(baldi.behaviorStateMachine.currentState is Baldi_Praise)
                && !ReflectionHelper.GetValue<bool>(baldi, "breakRuler")
                && other.TryGetComponent(out NPCControllerSystem controllerSystem))
            {
                if (controllerSystem.CreateController(out BrokenRulerController controller))
                {
                    SetToDestroy();
                    ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_protected"]);
                    controller.Initialize(15f, true);
                }
            }
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (time <= fadeEffectsTime && !preparedToDestroy)
            {
                PrepareToDestroy();
            }
            gauge?.SetValue(baseTime, time);
        }

        private void PrepareToDestroy()
        {
            if (time > fadeEffectsTime) time = fadeEffectsTime;

            if (owner == ControllerOwner.Player)
            {
                overlayEffectsMan.QueueEffect<FadeOutEffect>(time);
                //ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_protection_ended"]);
            }
            preparedToDestroy = true;
        }

        public override void OnPostDestroying()
        {
            base.OnPostDestroying();
            GameObject.Destroy(overlayEffectsMan.gameObject);
            gauge?.Deactivate();
        }
    }
}
