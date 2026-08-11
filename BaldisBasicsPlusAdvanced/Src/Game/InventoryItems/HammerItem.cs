using UnityEngine;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Cache;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems
{
    public class HammerItem : Item, IPrefab
    {
        [SerializeField]
        private float squishTime;

        [SerializeField]
        private float distance;

        private RaycastHit hit;

        public void InitializePrefab(int variant)
        {
            squishTime = 30f;
            distance = 17f;
        }

        public override bool Use(PlayerManager pm)
        {
            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out hit, distance, ~pm.gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                //ray ignores trigger colliders
                if (hit.transform.parent != null && hit.transform.parent.TryGetComponent(out SymbolMachine symbolMachine))
                {
                    bool result = symbolMachine.ReInit();
                    OnUsed(pm);
                    return result;
                }

                if (hit.transform.CompareTag("NPC") && hit.transform.TryGetComponent(out Entity entity))
                {
                    NPC npc = hit.transform.GetComponent<NPC>();

                    if (!ReflEvent_IsUsable(npc, pm)) return false;

                    object result = ReflectionHelper.NoCache_UseMethod(npc, "Adv_IsSquashable"); //only for NPC
                    if (result == null || ((bool)result)) entity.Squish(squishTime);

                    OnUsed(pm);
                    ReflEvent_OnHammerUse(npc, pm);
                    return entity.Squished;
                }

                if (hit.transform.TryGetComponent(out Fan fan) && fan.Broken)
                {
                    fan.Repair();
                    OnUsed(pm);
                    return !fan.Broken;
                }
            } 
            
            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out hit, distance, ~pm.gameObject.layer))
            {
                //ray don't ignores any colliders
                if (hit.transform.TryGetComponent(out Window window) && !window.open)
                {
                    if (!ReflEvent_IsUsable(window, pm)) return false;

                    if (ReflEvent_IsBreakable(window)) window.Break(true);

                    OnUsed(pm);
                    ReflEvent_OnHammerUse(window, pm);
                    return window.open;
                }

                if (hit.transform.TryGetComponent(out SwingDoor door))
                {
                    if (door.locked)
                    {
                        if (!ReflEvent_IsUsable(door, pm)) return false;

                        if (ReflEvent_IsBreakable(door)) door.Unlock();

                        OnUsed(pm);
                        ReflEvent_OnHammerUse(door, pm);
                        return !door.locked;
                    }
                }

                if (hit.transform.TryGetComponent(out StandardDoor standardDoor))
                {
                    if (standardDoor.locked)
                    {
                        if (!ReflEvent_IsUsable(standardDoor, pm)) return false;

                        if (ReflEvent_IsBreakable(standardDoor)) standardDoor.Unlock();

                        OnUsed(pm);
                        ReflEvent_OnHammerUse(standardDoor, pm);
                        return !standardDoor.locked;
                    }
                }
            }
            Destroy(base.gameObject);
            return false;
        }

        private void ReflEvent_OnHammerUse(object @object, PlayerManager pm)
        {
            ReflectionHelper.NoCache_UseMethod(@object, "Adv_OnHammerHit", pm);
        }

        private bool ReflEvent_IsBreakable(object @object)
        {
            object isBreakable = ReflectionHelper.NoCache_UseMethod(@object, "Adv_IsBreakable");

            return isBreakable == null || ((bool)isBreakable);
        }

        private bool ReflEvent_IsUsable(object @object, PlayerManager pm)
        {
            object isUsable = ReflectionHelper.NoCache_UseMethod(@object, "Adv_OnHammerPreHit", pm);

            if (isUsable != null && !((bool)isUsable)) return false;
            return true;
        }

        private void OnUsed(PlayerManager pm)
        {
            pm.ec.GetAudMan().PlaySingle(AssetStorage.sounds["hammer"]);
            pm.RuleBreak("Bullying", 1f);
            Destroy(gameObject);
        }

    }
}
