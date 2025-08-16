using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{

    //EntityPull doesn't provide delegates, damn
    //Own implementation will be more easy to control
    //And if EntityPull was really good for actual using
    //Then Mystman would change Whirlpool implementation
    //But he uses that for black hole which is not worried about how close player is
    //It just has another trigger collider and another class for checking if entity is triggered

    public class AdvancedEntityPull : MonoBehaviour
    {

        public delegate void OnEntityCatched(EntitySuckingData entityData);

        public class EntitySuckingData
        {

            private AdvancedEntityPull pull;

            public Entity entity;

            public MovementModifier moveMod;

            public bool catched;

            public EntitySuckingData(AdvancedEntityPull pull)
            {
                this.pull = pull;
            }

            public void Release()
            {
                entity.ExternalActivity.moveMods.Remove(moveMod);
                pull.entities.Remove(this);
            }

        }

        [SerializeField]
        private CapsuleCollider capsuleCollider;

        [SerializeField]
        private LayerMask layerMask = LayersHelper.gumCollisionMask;

        public float maxForce = 20f;

        public float distanceToCatch = 1f;

        public bool ignoreAirborne;

        public OnEntityCatched onEntityCatched;

        private List<EntitySuckingData> entities = new List<EntitySuckingData>();

        private EnvironmentController ec;

        private float _force;

        private Ray ray;

        private RaycastHit[] hits;

        private List<Transform> hitTransforms = new List<Transform>();

        private Vector3 _direction;

        private float _distance;

        private bool _obstacleHit;

        public void CreateSphere(float radius)
        {
            CreateCapsule(radius, radius);
        }

        public void CreateCapsule(float height, float radius)
        {
            Assign(gameObject.AddComponent<CapsuleCollider>());
            capsuleCollider.isTrigger = true;

            capsuleCollider.height = height;
            capsuleCollider.radius = radius;
        }

        public void Assign(CapsuleCollider collider) => capsuleCollider = collider;

        public void Initialize(EnvironmentController ec)
        {
            this.ec = ec;
        }

        private void Update()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].entity == null)
                {
                    entities.RemoveAt(i);
                    i--;
                    continue;
                }

                _distance = Vector3.Distance(transform.position, entities[i].entity.transform.position);
                ray = new Ray(transform.position, entities[i].entity.transform.position - transform.position);
                hits = Physics.RaycastAll(ray, _distance, layerMask, QueryTriggerInteraction.Ignore);
                hitTransforms.Clear();
                _obstacleHit = false;
                if (hits.Length != 0)
                {
                    _obstacleHit = true;
                }

                if (!_obstacleHit)
                {
                    _direction = transform.position - entities[i].entity.transform.position;
                    _direction.y = 0f;
                    _distance = Vector3.Distance(transform.position, entities[i].entity.transform.position);
                    _force = (capsuleCollider.radius - Mathf.Min(_distance, capsuleCollider.radius)) / capsuleCollider.radius * 
                        maxForce * ec.EnvironmentTimeScale;

                    if (_distance < distanceToCatch && !entities[i].catched)
                    {
                        entities[i].catched = true;
                        onEntityCatched?.Invoke(entities[i]);
                    }

                    if (_force * Time.deltaTime > _distance)
                    {
                        entities[i].moveMod.movementAddend = _direction.normalized * _distance / Time.deltaTime;
                    }
                    else
                    {
                        entities[i].moveMod.movementAddend = _direction.normalized * _force;
                    }
                }
                else
                {
                    entities[i].moveMod.movementAddend = Vector3.zero;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
            {
                Entity component = other.GetComponent<Entity>();
                if (component != null)
                {   
                    MovementModifier movementModifier = new MovementModifier(Vector3.zero, 1f);
                    movementModifier.ignoreAirborne = ignoreAirborne;
                    entities.Add(new EntitySuckingData(this)
                    {
                        entity = component,
                        moveMod = movementModifier
                    });
                    component.ExternalActivity.moveMods.Add(movementModifier);

                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.isTrigger)
            {
                return;
            }

            Entity component = other.GetComponent<Entity>();
            if (!(component != null))
            {
                return;
            }

            for (int i = 0; i < entities.Count; i++)
            {
                if (component == entities[i].entity)
                {
                    entities[i].Release();
                    break;
                }
            }
        }

        public void ClearMoveMods()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Release();
                i--;
            }
        }

        private void OnDestroy()
        {
            ClearMoveMods();
        }

    }
}
