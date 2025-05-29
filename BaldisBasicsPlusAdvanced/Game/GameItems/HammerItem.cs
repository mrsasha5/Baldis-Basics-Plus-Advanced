using UnityEngine;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;

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
                if (hit.transform.parent != null && hit.transform.parent.TryGetComponent(out SymbolMachine symbolMachine))
                {
                    bool result = symbolMachine.ReInit();
                    OnUsed(pm);
                    return result;
                }

                if (hit.transform.CompareTag("NPC") && hit.transform.TryGetComponent(out Entity entity))
                {
                    NPC npc = hit.transform.GetComponent<NPC>();

                    if (!Refl_IsUsable(npc, pm)) return false;

                    object result = ReflectionHelper.UseMethod(npc, "Adv_IsSquashable"); //only for NPC
                    if (result == null || ((bool)result)) entity.Squish(squishTime);

                    OnUsed(pm);
                    Refl_OnHammerUse(npc, pm);
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
                    if (!Refl_IsUsable(window, pm)) return false;

                    if (Refl_IsBreakable(window)) window.Break(true);

                    OnUsed(pm);
                    Refl_OnHammerUse(window, pm);
                    return window.open;
                }

                if (hit.transform.TryGetComponent(out SwingDoor door))
                {
                    if (door.locked)
                    {
                        if (!Refl_IsUsable(door, pm)) return false;

                        if (Refl_IsBreakable(door)) door.Unlock();

                        OnUsed(pm);
                        Refl_OnHammerUse(door, pm);
                        return !door.locked;
                    }
                }

                if (hit.transform.TryGetComponent(out StandardDoor standardDoor))
                {
                    if (standardDoor.locked)
                    {
                        if (!Refl_IsUsable(standardDoor, pm)) return false;

                        if (Refl_IsBreakable(standardDoor)) standardDoor.Unlock();

                        OnUsed(pm);
                        Refl_OnHammerUse(standardDoor, pm);
                        return !standardDoor.locked;
                    }
                }
            }
            Destroy(base.gameObject);
            return false;
        }

        private void Refl_OnHammerUse(object @object, PlayerManager pm)
        {
            ReflectionHelper.UseMethod(@object, "Adv_OnHammerHit", pm);
        }

        private bool Refl_IsBreakable(object @object)
        {
            object isBreakable = ReflectionHelper.UseMethod(@object, "Adv_IsBreakable");

            return isBreakable == null || ((bool)isBreakable);
        }

        private bool Refl_IsUsable(object @object, PlayerManager pm)
        {
            object isUsable = ReflectionHelper.UseMethod(@object, "Adv_OnHammerPreHit", pm);

            if (isUsable != null && !((bool)isUsable)) return false;
            return true;
        }

        private void OnUsed(PlayerManager pm)
        {
            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["hammer"]);
            pm.RuleBreak("Bullying", 1f);
            Destroy(gameObject);
        }
    }
}
