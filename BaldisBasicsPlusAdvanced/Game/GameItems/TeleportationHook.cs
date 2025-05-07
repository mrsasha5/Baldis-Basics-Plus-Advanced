/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class TeleportationHook : BaseMultipleUsableItem, IPrefab
    {
        private EnvironmentController ec;

        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private Entity entity;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private Transform cracks;

        [SerializeField]
        private LayerMaskObject layerMask;//2113537;

        private Vector3[] positions = new Vector3[2];

        [SerializeField]
        private float initialForce = 50f;

        [SerializeField]
        private float speed = 300f;

        public float force;

        public float pressure;

        public float initialDistance;

        public float time;

        private bool locked;

        private bool snapped;

        public void initializePrefab()
        {
            gameObject.tag = "GrapplingHook";
            gameObject.layer = LayerMask.NameToLayer("StandardEntities");

            GameObject baseRenderer = ObjectsCreator.createSpriteRendererBillboard(CachedAssets.sprites["grappling_hook"]);
            baseRenderer.transform.parent = transform;

            lineRenderer = Instantiate(CachedAssets.gameObjects["line_renderer_hook"].GetComponent<LineRenderer>());
            lineRenderer.material = new Material(AssetsHelper.loadAsset<Shader>("Unlit/Color"));
            lineRenderer.material.color = new Color(1f, 0f, 0.78f, 1f);

            cracks = Instantiate(CachedAssets.gameObjects["cracks_hook"].transform);

            lineRenderer.transform.parent = transform;
            cracks.transform.parent = transform;

            layerMask = AssetsHelper.loadAsset<LayerMaskObject>("ProjectileNoEntity");

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            entity = gameObject.AddComponent<Entity>();

            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = 0.25f;
            SphereCollider capsuleCollider2 = gameObject.AddComponent<SphereCollider>();
            capsuleCollider2.isTrigger = true;
            capsuleCollider2.radius = 0.5f;

            ActivityModifier activityModifier = gameObject.AddComponent<ActivityModifier>();

            PrivateDataHelper.SetValue<Collider>(entity, "trigger", capsuleCollider2);
            PrivateDataHelper.SetValue<Collider>(entity, "collider", capsuleCollider);
            PrivateDataHelper.SetValue<LayerMask>(entity, "collisionLayerMask", layerMask.mask);
            PrivateDataHelper.SetValue<ActivityModifier>(entity, "externalActivity", activityModifier);
        }

        public override bool Use(PlayerManager pm)
        {
            if (!pm.hidden)
            {
                this.ec = pm.ec;
                this.pm = pm;
                base.transform.position = pm.transform.position;
                base.transform.rotation = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.rotation;
                entity.Initialize(ec, base.transform.position);
                entity.OnEntityMoveInitialCollision += OnEntityMoveCollision;
                audMan.PlaySingle(CachedAssets.sounds["grapple_launch"]);
                return !onUse();
            }
            Destroy(gameObject);
            return false;
        }

        private void Update()
        {
            if (pm.hidden)
            {
                snap();
            }

            if (!locked)
            {
                entity.UpdateInternalMovement(base.transform.forward * speed * ec.EnvironmentTimeScale);
                time += Time.deltaTime * ec.EnvironmentTimeScale;
                if (time > 60f)
                {
                    Destroy(base.gameObject);
                }

                return;
            }

            entity.UpdateInternalMovement(Vector3.zero);
        }

        private void LateUpdate()
        {
            positions[0] = base.transform.position;
            positions[1] = pm.transform.position - Vector3.up * 1f;
            lineRenderer.SetPositions(positions);
        }

        private void OnEntityMoveCollision(RaycastHit hit)
        {
            if (layerMask.Contains(hit.collider.gameObject.layer) && !locked)
            {
                locked = true;
                entity.SetFrozen(value: true);
                force = initialForce;
                audMan.PlaySingle(CachedAssets.sounds["grapple_clang"]);
                cracks.rotation = Quaternion.LookRotation(hit.normal * -1f, Vector3.up);
                cracks.gameObject.SetActive(value: true);

                this.pm.Teleport(this.transform.position - (this.transform.forward * 2f));

                audMan.PlaySingle(CachedAssets.sounds["teleport"]);

                lineRenderer.enabled = false;
                StartCoroutine(EndDelay());
            }
        }

        private void snap()
        {
            snapped = true;
            audMan.FlushQueue(endCurrent: true);
            audMan.QueueAudio(CachedAssets.sounds["bal_break"]);
            lineRenderer.enabled = false;
            StartCoroutine(EndDelay());
        }

        private void teleport()
        {

        }

        private IEnumerator EndDelay()
        {
            while (audMan.AnyAudioIsPlaying)
            {
                yield return null;
            }

            End();
        }
        private void End()
        {
            Destroy(base.gameObject);
        }
    }
}
*/