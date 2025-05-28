using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Adjusters;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

#warning TODO: try to destroy adjusters

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class BaseProjectile : MonoBehaviour, IPrefab, IEntityTrigger
    {
        [SerializeField]
        protected Entity entity;

        [SerializeField]
        protected AudioManager audMan;

        [SerializeField]
        protected CapsuleCollider capsuleCollider;

        [SerializeField]
        protected SpriteRenderer renderer;

        [SerializeField]
        protected float speed;

        protected object launcher;

        protected EnvironmentController ec;

        protected bool flying;

        //only for building
        private string _name;

        private Sprite _sprite;

        private float _triggerRadius;
        //end

        public bool Flying => flying;

        public bool PlayerIsLauncher => launcher is PlayerManager;

        public bool NpcIsLauncher => launcher is NPC;

        public void Initialize(EnvironmentController ec, Vector3 pos, object launcher)
        {
            this.ec = ec;
            entity.Initialize(ec, pos);
            entity.OnEntityMoveInitialCollision += OnEntityMoveCollision;
            this.launcher = launcher;
        }

        private void Update()
        {
            VirtualUpdate();
        }

        protected virtual void VirtualUpdate()
        {
            if (flying)
                entity.UpdateInternalMovement(base.transform.forward * speed * ec.EnvironmentTimeScale);
        }

        protected void SetEntityName(string name)
        {
            this._name = name;
        }


        protected void SetEntityTrigger(float radius)
        {
            this._triggerRadius = radius;
        }

        protected void SetEntitySprite(Sprite sprite)
        {
            this._sprite = sprite;
        }

        protected virtual void BuildEntity()
        {
            EntityAdjuster entityBuilder = new EntityAdjuster(gameObject)
                .SetName(_name)
                .SetLayer("StandardEntities")
                .SetLayerCollisionMask(LayersHelper.gumCollisionMask) //like gum
                .AddRigidbody()
                .AddDefaultRenderBaseFunction(_sprite);

            if (_triggerRadius > 0f) entityBuilder.AddTrigger(_triggerRadius);

            entity = entityBuilder.Build();

            entity.SetGrounded(false);

            renderer = GetComponentInChildren<SpriteRenderer>();
        }

        public virtual void InitializePrefab(int variant)
        {
            SetEntityValues();
            BuildEntity();

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            capsuleCollider = GetComponent<CapsuleCollider>();

            capsuleCollider.height = 4f;

            speed = 80f;
        }

        protected virtual void SetEntityValues()
        {

        }

        public virtual void SetFlying(bool flying)
        {
            this.flying = flying;
            if (flying)
            {
                audMan.QueueAudio(AssetsStorage.sounds["whoosh"]);
                audMan.SetLoop(true);
            }
            else if (audMan.loop)
            {
                audMan.FlushQueue(true);
                //audMan.SetLoop(false);
            }
        }

        protected virtual void OnEntityMoveCollision(RaycastHit hit)
        {
            if (flying && hit.transform.gameObject.layer != 2)
            {
                OnEntityCollide(hit);
            }
        }

        protected virtual void OnEntityCollide(RaycastHit hit)
        {

        }

        public virtual void EntityTriggerEnter(Collider other)
        {
            
        }

        public virtual void EntityTriggerExit(Collider other)
        {
            
        }

        public virtual void EntityTriggerStay(Collider other)
        {
            
        }

        
    }
}
