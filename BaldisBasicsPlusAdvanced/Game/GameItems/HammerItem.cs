using UnityEngine;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Enums;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Game.Objects;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class HammerItem : Item
    {
        private RaycastHit hit;

        private float squishTime = 30f;

        private float distance = 17f; //default for portal poster - 10

        public override bool Use(PlayerManager pm)
        {
            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out hit, distance, ~pm.gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                //ray ignores trigger colliders
                if (hit.transform.tag == "NPC" && hit.transform.TryGetComponent(out Entity entity))
                {
                    entity.Squish(squishTime);
                    onUsed(pm);
                    return true;
                }

                /*if (hit.transform.TryGetComponent(out Fan fan) && fan.Broken)
                {
                    fan.repair(livingTime: 20f);
                    onUsed(pm);
                    return true;
                }*/
            } 
            
            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out hit, distance, ~pm.gameObject.layer))
            {
                //ray don't ignores any colliders
                if (hit.transform.tag == "Window" && hit.transform.TryGetComponent(out Window window) && !window.open)
                {
                    window.Break(true);
                    onUsed(pm);
                    return true;
                }

                if (hit.transform.TryGetComponent(out SwingDoor door))
                {
                    if (door.locked)
                    {
                        door.Unlock();
                        onUsed(pm);
                        return true;
                    }
                }

                if (hit.transform.TryGetComponent(out StandardDoor standardDoor))
                {
                    if (standardDoor.locked)
                    {
                        standardDoor.Unlock();
                        onUsed(pm);
                    }
                }
            }
            Destroy(base.gameObject);
            return false;
        }

        private void onUsed(PlayerManager pm)
        {
            AudioManager audMan = pm.ec.getAudMan();
            audMan.PlaySingle(AssetsStorage.sounds["hammer"]);
            pm.RuleBreak("Bullying", 1f);
            Destroy(gameObject);
        }
    }
}
