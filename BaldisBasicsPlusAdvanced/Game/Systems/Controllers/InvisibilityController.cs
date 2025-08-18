using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI.Registers;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class InvisibilityController : BaseController
    {
        private bool ended;

        private HudGauge gauge;

        public override bool ToDestroy => ended;

        public override ControllerType Type => ControllerType.Effect;

        public void SetValuesToStart(float chalkEffectTime, float beginsIn, float endsIn)
        {
            if (chalkEffectTime != 0f) ObjectsCreator.AddChalkCloudEffect(entity.transform, chalkEffectTime, ec);
            entity.StartCoroutine(Effect(beginsIn, endsIn));
        }

        private IEnumerator Effect(float timeToDisappear, float effectTime)
        {
            bool hidden = false;
            float baseTime = effectTime;
            while (effectTime > 0f)
            {
                if (timeToDisappear > 0)
                {
                    timeToDisappear -= Time.deltaTime;
                }
                else if (!hidden)
                {
                    if (owner == ControllerOwner.Player)
                    {
                        SetInvisibility(pm, true);
                        gauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager
                            .ActivateNewGauge(ItemMetaStorage.Instance.FindByEnum(Items.InvisibilityElixir).value.itemSpriteSmall, 
                            effectTime);
                    }
                    else if (owner == ControllerOwner.NPC)
                        SetInvisibility(npc, true);
                    hidden = true;
                }
                else
                {
                    gauge?.SetValue(baseTime, effectTime);
                    effectTime -= Time.deltaTime;
                }
                yield return null;
            }

            if (owner == ControllerOwner.Player)
            {
                SetInvisibility(pm, false);
            } else if (owner == ControllerOwner.NPC)
            {
                if (controllerSystem.GetControllersCount<InvisibilityController>() <= 1) SetInvisibility(npc, false);
            }

            gauge?.Deactivate();

            ended = true;
            yield break;
        }

        private void SetInvisibility(PlayerManager pm, bool hide)
        {
            pm.SetHidden(hide);
            AudioManager audMan = ec.GetAudMan();

            /*if (hide)
            {
                audMan.PlaySingle(AssetsStorage.sounds["adv_disappearing"]);
            } else if (controllerSystem.GetControllersCount<InvisibilityController>() <= 1)
            {
                audMan.PlaySingle(AssetsStorage.sounds["adv_appearing"]);
            }*/
        }

        private void SetInvisibility(NPC npc, bool hide)
        {
            npc.GetComponent<Entity>().SetVisible(!hide);
            PropagatedAudioManager audMan = npc.GetComponent<PropagatedAudioManager>();

            /*if (audMan != null)
            {
                if (hide)
                {
                    audMan.PlaySingle(AssetsStorage.sounds["adv_disappearing"]);
                }
                else
                {
                    audMan.PlaySingle(AssetsStorage.sounds["adv_appearing"]);
                }
            }*/
        }

    }
}
