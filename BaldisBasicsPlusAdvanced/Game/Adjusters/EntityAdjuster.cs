using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.ObjectCreation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Adjusters
{
    public class EntityAdjuster
    {
        private static readonly FieldInfo _rendererBase = AccessTools.Field(typeof(Entity), "rendererBase");

        private static readonly FieldInfo _collider = AccessTools.Field(typeof(Entity), "collider");

        private static readonly FieldInfo _trigger = AccessTools.Field(typeof(Entity), "trigger");

        private static readonly FieldInfo _externalActivity = AccessTools.Field(typeof(Entity), "externalActivity");

        private static readonly FieldInfo _collisionLayerMask = AccessTools.Field(typeof(Entity), "collisionLayerMask");

        private static readonly FieldInfo _defaultLayer = AccessTools.Field(typeof(Entity), "defaultLayer");

        private string entityName = "Unnamed";

        private float baseRadius = 1f;

        private float triggerRadius;

        private LayerMask layer;

        private bool addRb;

        private GameObject obj;

        private LayerMask collisionLayerMask;

        private Func<Entity, Transform> addRenderBaseFunc;

        public EntityAdjuster(GameObject gameObject)
        {
            obj = gameObject;
        }

        public Entity Build()
        {
            obj.ConvertToPrefab(setActive: false);
            obj.name = entityName;
            obj.layer = layer;
            Entity entity = obj.AddComponent<Entity>();
            CapsuleCollider capsuleCollider = obj.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = baseRadius;
            CapsuleCollider capsuleCollider2 = obj.AddComponent<CapsuleCollider>();
            capsuleCollider2.isTrigger = true;
            capsuleCollider2.radius = ((triggerRadius > 0f) ? triggerRadius : baseRadius);
            capsuleCollider2.enabled = triggerRadius > 0f;
            _trigger.SetValue(entity, capsuleCollider2);
            _collider.SetValue(entity, capsuleCollider);
            _defaultLayer.SetValue(entity, obj.layer);
            _collisionLayerMask.SetValue(entity, collisionLayerMask);
            _externalActivity.SetValue(entity, obj.AddComponent<ActivityModifier>());

            if (addRb)
            {
                Rigidbody rb = entity.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.angularDrag = 0;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.freezeRotation = true;
                rb.isKinematic = true;
                rb.mass = 0;
            }

            if (addRenderBaseFunc != null)
            {
                _rendererBase.SetValue(entity, addRenderBaseFunc(entity));
            }

            return entity;
        }

        public EntityAdjuster SetBaseRadius(float radius)
        {
            baseRadius = radius;
            return this;
        }


        public EntityAdjuster AddRenderbaseFunction(Func<Entity, Transform> function)
        {
            addRenderBaseFunc = function;
            return this;
        }

        public EntityAdjuster AddDefaultRenderBaseFunction(Sprite sprite)
        {
            addRenderBaseFunc = delegate (Entity ent)
            {
                Transform transform = new GameObject().transform;
                transform.name = "RenderBase";
                transform.transform.parent = ent.transform;
                transform.gameObject.layer = ent.gameObject.layer;
                GameObject gameObject = new GameObject();
                gameObject.transform.parent = transform.transform;
                gameObject.layer = LayerHelper.billboard;
                SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.material = new Material(ObjectCreators.SpriteMaterial);
                spriteRenderer.name = "Sprite";
                spriteRenderer.sprite = sprite;
                return transform;
            };
            return this;
        }

        public EntityAdjuster AddRigidbody()
        {
            addRb = true;
            return this;
        }

        public EntityAdjuster AddTrigger(float radius)
        {
            triggerRadius = radius;
            return this;
        }

        public EntityAdjuster SetName(string name)
        {
            entityName = name;
            return this;
        }

        public EntityAdjuster SetLayer(string layer)
        {
            this.layer = LayerHelper.LayerFromName(layer);
            return this;
        }


        public EntityAdjuster SetLayerCollisionMask(LayerMask mask)
        {
            collisionLayerMask = mask;
            return this;
        }
    }
}
