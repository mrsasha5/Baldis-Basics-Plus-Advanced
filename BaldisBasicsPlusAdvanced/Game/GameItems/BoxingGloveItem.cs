using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.Movement;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using Rewired.UI.ControlMapper;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

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
                    NPC npc = hit.transform.GetComponent<NPC>();
                    if (Refl_IsUsable(npc, pm))
                    {
                        PlaySound(pm);
                        pm.RuleBreak("Bullying", 1f);

                        if (Refl_IsPushable(npc)) Push(npc, entity, pm);

                        Destroy(gameObject);
                        Relf_OnBoxingGloveHit(npc, pm);
                        return OnUse();
                    }
                    Destroy(base.gameObject);
                    return false;
                }
            }

            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out hit, distance, ~pm.gameObject.layer))
            {
                //ray don't ignores any colliders
                if (hit.transform.tag == "Window" && hit.transform.TryGetComponent(out Window window) && !window.open)
                {
                    if (Refl_IsUsable(window, pm))
                    {
                        PlaySound(pm);
                        pm.RuleBreak("Bullying", 1f);

                        if (Refl_IsPushable(window)) window.Break(true);

                        Destroy(gameObject);
                        Relf_OnBoxingGloveHit(window, pm);
                        if (!window.open) return false;
                        return OnUse();
                    }
                }
            }

            Destroy(base.gameObject);
            return false;
        }

        private void Push(NPC npc, Entity entity, PlayerManager pm)
        {
            Vector3 forward = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward;
            Force force = new Force(forward, pushSpeed, pushAcceleration);
            entity.AddForce(force);

            float time = Math.Abs(pushSpeed / pushAcceleration);
            ForcedEntityBehaviour behaviour = entity.gameObject.AddComponent<ForcedEntityBehaviour>();
            behaviour.Initialize(Singleton<BaseGameManager>.Instance.Ec, time);
            behaviour.PostInit(entity, force, forward, 0.25f, behaviour.DefaultSlamDistance, makesNoises: true, time);

            npc.StartCoroutine(LockMotion(npc, pushedStateTime));
            /*npc.getControllerSystem().createController(out PushedNpcController controller);
            controller.setTime(pushedStateTime);*/
        }

        private IEnumerator LockMotion(NPC npc, float lockTime)
        {
            MovementModifier moveMod = new MovementModifier(Vector3.zero, 0f);

            ActivityModifier actModifier = npc.GetComponent<Entity>().ExternalActivity;
            actModifier.moveMods.Add(moveMod);
            while (lockTime > 0f)
            {
                lockTime -= Time.deltaTime;
                yield return null;
            }
            actModifier.moveMods.Remove(moveMod);

            yield break;
        }

        private void Relf_OnBoxingGloveHit(object @object, PlayerManager pm)
        {
            ReflectionHelper.UseMethod(@object, "Adv_OnBoxingGloveHit", pm);
        }

        private bool Refl_IsPushable(object @object)
        {
            object isPushable = ReflectionHelper.UseMethod(@object, "Adv_IsPushable");
            return isPushable == null || ((bool)isPushable);
        }

        private bool Refl_IsUsable(object @object, PlayerManager pm)
        {
            object result = ReflectionHelper.UseMethod(@object, "Adv_OnBoxingGlovePreHit", pm);
            return result == null || ((bool)result);
        }

        private void PlaySound(PlayerManager pm)
        {
            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["bang"]);
        }
    }
}
