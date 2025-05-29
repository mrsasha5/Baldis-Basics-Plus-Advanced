using UnityEngine;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using Rewired.UI.ControlMapper;
using BaldisBasicsPlusAdvanced.SaveSystem.Managers;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class PlayerWindController : BaseController
    {
        private WindObject windObj;

        private bool blowingAllowed = true;

        private bool lookBack;

        public override int MaxCount => 1;

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

            if (time > 0)
            {
                if (blowingAllowed && !playerHidden)
                    time -= Time.deltaTime * ec.EnvironmentTimeScale;
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

        public override void OnPreDestroying()
        {
            base.OnPreDestroying();
            windObj.SetActivityState(false);
            GameObject.Destroy(windObj.gameObject);
            ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_pah"]);
        }
    }
}
