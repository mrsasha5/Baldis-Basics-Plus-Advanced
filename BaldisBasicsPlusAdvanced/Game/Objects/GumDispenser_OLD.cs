/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class GumDispenser_OLD : TileBasedObject, IButtonReceiver, IPrefab
    {
        [SerializeField]
        private CustomSpriteAnimator animator;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private float cooldownTime;

        [SerializeField]
        private int maxUses;

        [SerializeField]
        private int maxCrashedGums;

        private float time;

        private int uses;

        private List<GumProjectile> potentialGumsToDestroy = new List<GumProjectile>();

        public void InitializePrefab(int variant)
        {
            SpriteRenderer renderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_gum_dispenser_1"]);
            renderer.transform.parent.SetParent(transform);
            renderer.transform.parent.localPosition = Vector3.up * 5f;

            renderer.gameObject.AddComponent<PickupBob>();

            BoxCollider collider = gameObject.AddComponent<BoxCollider>(); //for the editor
            collider.isTrigger = true;
            collider.size = Vector3.one * 5f;
            collider.center = Vector3.up * 5f;

            animator = gameObject.AddComponent<CustomSpriteAnimator>();
            animator.spriteRenderer = renderer;

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            cooldownTime = 30f;
            maxUses = 3;
            maxCrashedGums = 3;
        }

        private void Start()
        {
            uses = maxUses;
            animator.PopulateAnimations(new Dictionary<string, Sprite[]>()
            {
                {
                    "flying",
                    new Sprite[] {
                        AssetsStorage.sprites["adv_gum_dispenser_1"],
                        AssetsStorage.sprites["adv_gum_dispenser_2"],
                        AssetsStorage.sprites["adv_gum_dispenser_3"],
                        AssetsStorage.sprites["adv_gum_dispenser_4"]
                    }
                }
            }, fps: 30);

            animator.SetDefaultAnimation("flying", 1f);
        }

        private void Update()
        {
            if (uses < maxUses && time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (time <= 0f)
                {
                    uses++;
                    time = cooldownTime;
                }
            }
        }

        [Obsolete("Aw, not best way.")]
        public void ChooseBestRotation()
        {
            Vector3[] forwards = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
            float lastDistance = float.NegativeInfinity;
            int chosen = 0;
            for (int i = 0; i < forwards.Length; i++)
            {
                Physics.Raycast(transform.position, forwards[i], out RaycastHit hit, float.PositiveInfinity, LayersHelper.ignorableCollidableObjects, QueryTriggerInteraction.Ignore);
                if (hit.distance > lastDistance)
                {
                    chosen = i;
                    lastDistance = hit.distance;
                }
            }
            transform.forward = forwards[chosen];

        }

        public void ButtonPressed(bool val)
        {
            if (uses > 0)
            {
                uses--;
                if (time <= 0f) time = cooldownTime;
                GumProjectile gum = GameObject.Instantiate(ObjectsStorage.Objects["gum"].GetComponent<GumProjectile>());
                gum.Initialize(ec, transform.position, this);
                gum.Reset();
                gum.transform.forward = transform.forward;

                potentialGumsToDestroy.Add(gum);

                audMan.PlaySingle(AssetsStorage.sounds["spit"]);

                if (potentialGumsToDestroy.Count > maxCrashedGums)
                {
                    for (int i = 0; i < potentialGumsToDestroy.Count; i++)
                    {
                        if (!potentialGumsToDestroy[i].Flying && !potentialGumsToDestroy[i].AttachedToSomebody)
                        {
                            Destroy(potentialGumsToDestroy[i].gameObject);
                            potentialGumsToDestroy.RemoveAt(i);
                            i--;
                            if (potentialGumsToDestroy.Count <= potentialGumsToDestroy.Count) break;
                        }
                    }
                }
            }
            
        }
    }
}*/
