using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Food
{
    public class GroundDough : MonoBehaviour, IEntityPrefab, IEntityTrigger
    {
        [SerializeField]
        private SoundObject audOnSlip;

        [SerializeField]
        private SoundObject audSput;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private Entity entity;

        [SerializeField]
        private float gravity;

        [SerializeField]
        private float minHeight;

        [SerializeField]
        private float height;

        [SerializeField]
        private float lifetime;

        [SerializeField]
        private MovementModifier moveMod;

        private EnvironmentController ec;

        private Entity target;

        private NPC npcTarget;

        private PlayerManager pmTarget;

        private HudGauge gauge;

        private float baseTime;

        private bool ready;

        private bool active;

        public void InitializePrefab(Entity entity, int variant)
        {
            audOnSlip = AssetsStorage.sounds["banana_slip"];
            audSput = AssetsStorage.sounds["banana_sput"];

            SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
            renderer.transform.localScale = new Vector3(2.5f, 2f, 2f);

            entity.gameObject.SetRigidbody();
            this.entity = entity;

            entity.SetGrounded(false);

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            moveMod = new MovementModifier(Vector3.zero, 0.95f)
            {
                ignoreAirborne = true
            };

            lifetime = 25f;

            height = 5f;
            minHeight = 0.8f;
            gravity = 15f;
        }

        public void Initialize(EnvironmentController ec, Vector3 pos, Vector3? forceDirection = null, float force = 25f)
        {
            this.ec = ec;

            entity.Initialize(ec, pos);
            if (forceDirection != null) entity.AddForce(new Force((Vector3)forceDirection, force, -force));
        }

        private void Update()
        {
            if (ready && active)
            {
                if (target == null)
                {
                    Destroy(gameObject);
                    return;
                }

                //Mystman pls fix your buggy guilt's system
                if (npcTarget != null)
                {
                    if (entity.CurrentRoom == null || (entity.CurrentRoom.category != RoomCategory.Office
                    && npcTarget.Character != Character.Principal))
                    {
                        ReflectionHelper.UseMethod(npcTarget, "SetGuilt", 1f, "Bullying");
                    } 
                    else if (entity.CurrentRoom.category == RoomCategory.Office) npcTarget?.ClearGuilt();
                }

                if (pmTarget != null)
                {
                    if (entity.CurrentRoom == null || entity.CurrentRoom.category != RoomCategory.Office)
                        pmTarget.RuleBreak("Bullying", 1f);
                    else pmTarget.ClearGuilt();
                }

                if (lifetime > 0f)
                {
                    lifetime -= Time.deltaTime * ec.EnvironmentTimeScale;
                    gauge?.SetValue(baseTime, lifetime);
                    if (lifetime <= 0f)
                    {
                        Destroy(gameObject);
                    }
                }
            }

            if (!ready)
            {
                height -= Time.deltaTime * ec.EnvironmentTimeScale * gravity;

                if (height < minHeight)
                {
                    height = minHeight;
                    entity.SetGrounded(true);
                    ready = true;
                    audMan.PlaySingle(audSput);
                }

                entity.SetHeight(height);
            }
        }

        private void LateUpdate()
        {
            if (ready && active && target != null)
            {
                transform.position = target.transform.position;
                //entity.SetHeight(... + minHeight);
            }
        }

        public void EntityTriggerEnter(Collider other, bool validCollision)
        {
            if (validCollision && ready && this.target == null && !other.TryGetComponent(out GroundDough _)
                && other.TryGetComponent(out Entity target) && target.Grounded)
            {
                this.target = target;
                audMan.PlaySingle(audOnSlip);

                target.AddForce(new Force(target.transform.forward, 50f, -50f));
                target.ExternalActivity.moveMods.Add(moveMod);

                //entity.SetInteractionState(false);
                //entity.SetFrozen(true);

                npcTarget = other.GetComponent<NPC>();
                pmTarget = other.GetComponent<PlayerManager>();
                active = true;
                baseTime = lifetime;

                if (pmTarget != null)
                    gauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager
                            .ActivateNewGauge(ObjectsStorage.ItemObjects["Dough"].itemSpriteSmall, lifetime);
            }
        }

        private void OnDestroy()
        {
            target?.ExternalActivity.moveMods.Remove(moveMod);
            gauge?.Deactivate();
        }

        public void EntityTriggerStay(Collider other, bool validCollision)
        {

        }

        public void EntityTriggerExit(Collider other, bool validCollision)
        {
            
        }
        
    }
}
