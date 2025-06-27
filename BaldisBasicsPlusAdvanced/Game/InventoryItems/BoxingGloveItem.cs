using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems
{
    public class BoxingGloveItem : BaseMultipleUsableItem, IPrefab
    {
        [SerializeField]
        private float pushSpeed;

        [SerializeField]
        private float pushAcceleration;

        [SerializeField]
        private float distance;

        [SerializeField]
        private float pushedStateTime;

        public void InitializePrefab(int variant)
        {
            pushSpeed = 125f;
            pushAcceleration = -62.5f;
            distance = 17f;
            pushedStateTime = 5f;
        }

        public override bool Use(PlayerManager pm)
        {
            this.pm = pm;
            if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out RaycastHit hit, distance, ~pm.gameObject.layer, QueryTriggerInteraction.Ignore))
            {
                //ray ignores trigger colliders

                if (CanBePushed(hit) 
                    && hit.transform.TryGetComponent(out Entity entity) && !entity.Frozen)
                {
                    NPC npc = hit.transform.GetComponent<NPC>();
                    if (npc == null || (npc != null && Refl_IsUsable(npc, pm)))
                    {
                        PlaySound(pm);
                        pm.RuleBreak("Bullying", 1f);

                        if (npc == null || (npc != null && Refl_IsPushable(npc))) Push(entity, pm);

                        Destroy(gameObject);
                        if (npc != null) Relf_OnBoxingGloveHit(npc, pm);
                        return ReturnOnUse();
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
                        return ReturnOnUse();
                    }
                }
            }

            Destroy(base.gameObject);
            return false;
        }

        private bool CanBePushed(RaycastHit hit)
        {
            return hit.transform.CompareTag("NPC") || hit.transform.gameObject.TryGetComponent(out AnvilProjectile _);
        }

        private void Push(Entity entity, PlayerManager pm)
        {
            Vector3 forward = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward;

            entity.AddForceWithBehaviour(forward, pushSpeed, pushAcceleration, makesNoises: true);

            entity.StartCoroutine(LockMotion(entity, pushedStateTime));
            /*npc.getControllerSystem().createController(out PushedNpcController controller);
            controller.setTime(pushedStateTime);*/
        }

        private IEnumerator LockMotion(Entity entity, float lockTime)
        {
            MovementModifier moveMod = new MovementModifier(Vector3.zero, 0f);

            entity.ExternalActivity.moveMods.Add(moveMod);
            while (lockTime > 0f)
            {
                lockTime -= Time.deltaTime;
                yield return null;
            }
            entity.ExternalActivity.moveMods.Remove(moveMod);

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
