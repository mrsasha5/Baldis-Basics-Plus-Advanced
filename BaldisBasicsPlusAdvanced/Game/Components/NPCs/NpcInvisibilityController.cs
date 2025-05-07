using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.NPCs
{
    public class NpcInvisibilityController : NPCController
    {
        private bool ended;

        public override bool ToDestroy => ended; 

        public override void initialize(NPC npc, PlayerControllerSystem pc)
        {
            base.initialize(npc, pc);
            
        }

        public void postInit(float chalkEffectTime, float beginsIn, float endsIn)
        {
            if (chalkEffectTime != 0f) ObjectsCreator.addChalkCloudEffect(npc, chalkEffectTime, ec);
            npc.StartCoroutine(effect(beginsIn, endsIn));
        }

        private IEnumerator effect(float timeToDisappear, float effectTime)
        {
            bool hidden = false;
            while (effectTime > 0f)
            {
                if (timeToDisappear > 0)
                {
                    timeToDisappear -= Time.deltaTime;
                } else if (!hidden)
                {
                    disappear(npc, true);
                    hidden = true;
                } else
                {
                    effectTime -= Time.deltaTime;
                }
                yield return null;
            }
            if (npc.getControllerSystem().getControllersCount<NpcInvisibilityController>() <= 1) disappear(npc, false);
            ended = true;
            yield break;
        }


        private void disappear(NPC npc, bool hide)
        {
            if (npc.spriteBase != null) // checking in case someone doesn't use it
            {
                npc.spriteBase.SetActive(!hide);
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
}
