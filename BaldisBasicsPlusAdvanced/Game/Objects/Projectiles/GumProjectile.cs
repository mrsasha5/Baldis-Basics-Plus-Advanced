using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class GumProjectile : BaseProjectile
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

        [SerializeField]
        private Sprite gaugeIcon;

        private ActivityModifier actMod;

        private Collider hitCollider;

        private Vector3 hitColliderPosition;

        private Quaternion hitColliderRotation;

        private HudGauge gauge;

        private float aliveTime;

        private bool done;

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

        public override void InitializePrefab(int variant)
        {
            gaugeIcon = AssetsHelper.LoadAsset<Sprite>("beans_gum_icon");

            entity = gameObject.GetComponent<Entity>();
            audMan = gameObject.AddComponent<PropagatedAudioManager>();
            setTime = 10f;
        }

        protected override void VirtualUpdate()
        {
            if (aliveTime > 0f)
            {
                aliveTime -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (aliveTime <= 0f) Destroy();
            }

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
                Destroy();
            }
        }

        public override void EntityTriggerEnter(Collider other, bool validCollision)
        {
            base.EntityTriggerEnter(other, validCollision);
            if (!flying || !validCollision)
            {
                return;
            }

            if (other.isTrigger && (other.CompareTag("Player") || other.CompareTag("NPC")))
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
                    canvas.worldCamera = CoreGameManager.Instance.GetCamera(other.GetComponent<PlayerManager>().playerNumber).canvasCam;
                    attachedToPlayer = true;
                    gauge = CoreGameManager.Instance.GetHud(0).gaugeManager.ActivateNewGauge(gaugeIcon, setTime);;
                    CoreGameManager.Instance.audMan.PlaySingle(AssetsStorage.sounds["splat"]);
                }
                else
                {
                    actMod.moveMods.Add(moveMod);
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

            aliveTime = 10f;

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
                gauge?.SetValue(setTime, time);
                yield return null;
            }

            gauge?.Deactivate();
            cut = false;
            actMod.moveMods.Remove(moveMod);
            actMod.moveMods.Remove(playerMod);

            Destroy();
        }

        public void Reset()
        {
            done = false;
            attachedToPlayer = false;
            StopAllCoroutines();
            flyingSprite.SetActive(value: true);
            groundedSprite.SetActive(value: false);
            SetFlying(true);
            canvas.gameObject.SetActive(value: false);

            if (actMod != null)
            {
                actMod.moveMods.Remove(moveMod);
                actMod.moveMods.Remove(playerMod);
            }

            entity.SetFrozen(value: false);
            entity.SetActive(value: true);
            entity.SetHeight(5f);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void Cut()
        {
            cut = true;
        }

    }
}
