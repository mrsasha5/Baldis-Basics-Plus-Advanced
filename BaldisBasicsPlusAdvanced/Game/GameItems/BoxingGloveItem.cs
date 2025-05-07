using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.NPCs;
using BaldisBasicsPlusAdvanced.Game.NPCs.Behavior.States;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tripolygon.UModeler;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class BoxingGloveItem : BaseMultipleUsableItem
    {
        private float pushSpeed = 125f;

        private float pushAcceleration = -62.5f;

        private float distance = 17f;

        private string[] ignorableNPCs = new string[] { "Bully(Clone)", "ChalkFace(Clone)" };

        private float pushedStateTime = 5f;

        public override bool Use(PlayerManager pm)
        {
            this.pm = pm;
            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out RaycastHit hit, distance, ~pm.gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                //ray ignores trigger colliders
                if (hit.transform.tag == "NPC" && !ignorableNPCs.Contains(hit.transform.name) && hit.transform.TryGetComponent(out Entity entity))
                {
                    push(hit.transform.GetComponent<NPC>(), entity, pm);
                    playSound(pm);
                    pm.RuleBreak("Bullying", 1f);
                    Destroy(gameObject);
                    return !onUse();
                }
            }

            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out hit, distance, ~pm.gameObject.layer))
            {
                //ray don't ignores any colliders
                if (hit.transform.tag == "Window" && hit.transform.TryGetComponent(out Window window) && !window.open)
                {
                    window.Break(true);
                    playSound(pm);
                    pm.RuleBreak("Bullying", 1f);
                    Destroy(gameObject);
                    return !onUse();
                }
            }

            Destroy(base.gameObject);
            return false;
        }

        private void push(NPC npc, Entity entity, PlayerManager pm)
        {
            entity.AddForce(new Force(Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, pushSpeed, pushAcceleration));
            npc.getControllerSystem().createController(out PushedNpcController controller);
            controller.setTime(pushedStateTime);
            //npc.gameObject.AddComponent<PushedNPCBehaviour>().initialize(pm.ec, pushedStateTime, npc);
            //npc.behaviorStateMachine.ChangeState(new NPC_Pushed(npc, npc.behaviorStateMachine.currentState, pushedStateTime));
        }

        private void playSound(PlayerManager pm)
        {
            AudioManager audMan = pm.ec.getAudMan();
            audMan.PlaySingle(AssetsStorage.sounds["bang"]);
        }
    }
}
