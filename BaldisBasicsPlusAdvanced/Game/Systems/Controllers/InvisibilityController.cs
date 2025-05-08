using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class InvisibilityController : BaseController
    {
        private bool ended;

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
            while (effectTime > 0f)
            {
                if (timeToDisappear > 0)
                {
                    timeToDisappear -= Time.deltaTime;
                }
                else if (!hidden)
                {
                    if (owner == ControllerOwner.Player)
                        SetInvisibility(pm, true);
                    else if (owner == ControllerOwner.NPC)
                        SetInvisibility(npc, true);
                    hidden = true;
                }
                else
                {
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
            
            ended = true;
            yield break;
        }

        private void SetInvisibility(PlayerManager pm, bool hide)
        {
            pm.SetInvisible(hide);
            AudioManager audMan = ec.GetAudMan();

            if (hide)
            {
                audMan.PlaySingle(AssetsStorage.sounds["adv_disappearing"]);
            } else if (controllerSystem.GetControllersCount<InvisibilityController>() <= 1)
            {
                audMan.PlaySingle(AssetsStorage.sounds["adv_appearing"]);
            }
        }

        private void SetInvisibility(NPC npc, bool hide)
        {
            npc.GetComponent<Entity>().SetVisible(!hide);
            PropagatedAudioManager audMan = npc.GetComponent<PropagatedAudioManager>();

            if (audMan != null)
            {
                if (hide)
                {
                    audMan.PlaySingle(AssetsStorage.sounds["adv_disappearing"]);
                }
                else
                {
                    audMan.PlaySingle(AssetsStorage.sounds["adv_appearing"]);
                }

            }
        }

    }
}
