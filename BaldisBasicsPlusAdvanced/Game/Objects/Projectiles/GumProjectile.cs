using BaldisBasicsPlusAdvanced.Cache;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class GumProjectile : ProjectileBase
    {
        [SerializeField]
        internal GameObject flyingSprite;

        [SerializeField]
        internal GameObject groundedSprite;

        [SerializeField]
        internal Canvas canvas;

        [SerializeField]
        internal MovementModifier moveMod;

        [SerializeField]
        internal MovementModifier playerMod;

        [SerializeField]
        private float setTime;

        private ActivityModifier actMod;

        private Collider hitCollider;

        private Vector3 hitColliderPosition;

        private Quaternion hitColliderRotation;

        private bool done;

        private bool hidden;

        private bool cut;

        private bool attachedToPlayer;

        public bool AttachedToSomebody => actMod != null;

        public bool AttachedToPlayer => attachedToPlayer;

        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public override void InitializePrefab()
        {
            entity = gameObject.GetComponent<Entity>();
            audMan = gameObject.AddComponent<PropagatedAudioManager>();
            setTime = 10f;
        }

        protected override void Update()
        {
            if (!hidden)
            {
                if (flying)
                {
                    entity.UpdateInternalMovement(base.transform.forward * speed * ec.EnvironmentTimeScale);
                }
                else if (actMod != null)
                {
                    entity.UpdateInternalMovement(Vector3.zero);
                    base.transform.position = actMod.transform.position;
                }

                if (done && (!hitCollider.enabled || hitCollider.transform.position != hitColliderPosition || hitCollider.transform.rotation != hitColliderRotation))
                {
                    Hide();
                }
            }
        }

        public override void EntityTriggerEnter(Collider other)
        {
            base.EntityTriggerEnter(other);
            if (!flying)
            {
                return;
            }

            if (other.isTrigger && (other.CompareTag("Player") || other.CompareTag("NPC"))) //&& (other.transform != beans.transform || leftBeans))
            {
                actMod = other.GetComponent<ActivityModifier>();
                flying = false;
                flyingSprite.SetActive(value: false);
                groundedSprite.SetActive(value: true);
                StartCoroutine(Timer(setTime));
                audMan.FlushQueue(endCurrent: true);
                if (other.CompareTag("Player"))
                {
                    actMod.moveMods.Add(playerMod);
                    canvas.gameObject.SetActive(value: true);
                    canvas.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(other.GetComponent<PlayerManager>().playerNumber).canvasCam;
                    attachedToPlayer = true;
                    //playerGum.Add(this);
                    //beans.HitPlayer();
                    //beans.GumHit(this, hitSelf: false);
                    Singleton<CoreGameManager>.Instance.audMan.PlaySingle(AssetsStorage.sounds["splat"]);
                }
                else
                {
                    actMod.moveMods.Add(moveMod);
                    //beans.HitNPC();
                    //beans.GumHit(this, other.transform == beans.transform);
                    audMan.PlaySingle(AssetsStorage.sounds["splat"]);
                }
            }
            else if (!other.isTrigger)
            {
                _ = other.gameObject.layer;
                _ = 2;
            }
        }

        protected override void OnEntityCollide(RaycastHit hit)
        {
            base.OnEntityCollide(hit);
            SetFlying(false);
            entity.SetFrozen(value: true);

            base.transform.rotation = Quaternion.LookRotation(hit.normal * -1f, Vector3.up);

            audMan.FlushQueue(endCurrent: true);
            audMan.PlaySingle(AssetsStorage.sounds["splat"]);

            done = true;
            hitCollider = hit.collider;
            hitColliderPosition = hitCollider.transform.position;
            hitColliderRotation = hitCollider.transform.rotation;
        }

        private IEnumerator Timer(float time)
        {
            while (time > 0f && !cut)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }

            cut = false;
            actMod.moveMods.Remove(moveMod);
            actMod.moveMods.Remove(playerMod);
            //playerGum.Remove(this);
            Hide();
        }

        public void Reset()
        {
            hidden = false;
            done = false;
            attachedToPlayer = false;
            StopAllCoroutines();
            flyingSprite.SetActive(value: true);
            groundedSprite.SetActive(value: false);
            SetFlying(true);//flying = true;
            canvas.gameObject.SetActive(value: false);
            //leftBeans = false;
            if (actMod != null)
            {
                actMod.moveMods.Remove(moveMod);
                actMod.moveMods.Remove(playerMod);
            }

            //playerGum.Remove(this);
            entity.SetFrozen(value: false);
            entity.SetActive(value: true);
            entity.SetHeight(5f);
            //audMan.QueueAudio(AssetsStorage.sounds["whoosh"]);
            //audMan.SetLoop(val: true);
        }

        public void Hide()
        {
            hidden = true;
            done = false;
            attachedToPlayer = false;
            entity.SetActive(value: false);
            entity.UpdateInternalMovement(Vector3.zero);
            flyingSprite.SetActive(value: false);
            groundedSprite.SetActive(value: false);
            SetFlying(false);//flying = false;
            canvas.gameObject.SetActive(value: false);
        }

        public void Cut()
        {
            cut = true;
        }

        private void OnDisable()
        {
            attachedToPlayer = false;
            //playerGum.Remove(this);
        }


    }
}
