using System;
using System.Collections;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class BaseBalloonBehaviour : MonoBehaviour, IPrefab, IClickable<int>
    {
        [SerializeField]
        protected SoundObject audReveal;

        [SerializeField]
        protected SpriteRenderer spriteRenderer;

        [SerializeField]
        protected Balloon floater;

        [SerializeField]
        protected Entity entity;

        [SerializeField]
        protected GameObject explosionPre;

        [SerializeField]
        protected float balloonAnimationSpeedMult;

        [SerializeField]
        protected bool offsetAnimationEnabled;

        protected Vector3 spriteHeldPosition = Vector3.up * -4f + Vector3.forward * 4f;

        protected Vector3 spriteInitPosition = Vector3.zero;

        protected int initialSpriteLayer;

        protected float targetOffset;

        protected float currentOffset;

        protected bool trackingPlayer;

        protected bool popping;

        public SpriteRenderer Renderer => spriteRenderer;

        public Balloon Floater => floater;

        public virtual void InitializePrefab(int variant)
        {
            balloonAnimationSpeedMult = 4f;
            offsetAnimationEnabled = true;

            audReveal = AssetStorage.sounds["balloon_reveal"];
            floater = GetComponent<Balloon>();
            entity = GetComponent<Entity>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            entity.SetGrounded(false);

            explosionPre = AssetHelper.LoadAsset<GameObject>("QuickPop");
        }

        protected virtual void VirtualStart()
        {

        }

        protected virtual void VirtualUpdate()
        {

        }

        protected void MakeStaticBehaviour()
        {
            entity.SetFrozen(true);
        }

        public virtual void Clicked(int player)
        {

        }

        public virtual void OnDropped()
        {

        }

        private void Start()
        {
            spriteInitPosition = spriteRenderer.transform.localPosition;
            initialSpriteLayer = spriteRenderer.gameObject.layer;
            VirtualStart();
        }

        private void Update()
        {
            if (offsetAnimationEnabled && !trackingPlayer && currentOffset != targetOffset)
            {
                currentOffset = Mathf.MoveTowards(currentOffset, targetOffset, Time.deltaTime * 20f);
                spriteRenderer.transform.localPosition = spriteInitPosition + Vector3.up * currentOffset;
            }
            VirtualUpdate();
        }

        public void TrackPlayer(PlayerManager player, int playerNumber)
        {
            trackingPlayer = true;
            spriteRenderer.transform.localPosition = spriteInitPosition;
            floater.Entity.SetTrigger(value: false);
            StartCoroutine(Tracker(player, playerNumber));
        }

        public void ReInit()
        {
            trackingPlayer = false;
            floater.Entity.SetTrigger(value: true);
            currentOffset = -2f;
            spriteRenderer.transform.localPosition = spriteInitPosition + Vector3.up * currentOffset;
        }

        public void Disable()
        {
            entity.Enable(val: false);
        }

        public void Pop()
        {
            if (!popping)
            {
                if (trackingPlayer)
                {
                    OnDropped();
                }

                popping = true;
                Instantiate(explosionPre).transform.position = transform.position;
                Destroy(gameObject);
            }
        }

        private IEnumerator Tracker(PlayerManager player, int playerNumber)
        {
            floater.enabled = false;
            spriteRenderer.transform.localPosition = spriteHeldPosition;
            spriteRenderer.gameObject.layer = LayerHelper.takenBalloonLayer;
            Entity entity = player.GetComponent<Entity>();
            while (trackingPlayer)
            {
                transform.position = player.transform.position;
                transform.rotation = player.transform.rotation;
                if (entity.Frozen || entity.InteractionDisabled)
                {
                    ReInit();
                    OnDropped();
                    targetOffset = 0f;
                }

                yield return null;
            }

            targetOffset = 0f;
            floater.enabled = true;
            transform.eulerAngles = Vector3.zero;
            spriteRenderer.gameObject.layer = initialSpriteLayer;
        }

        protected void PlayRevealAnimation(Sprite newSprite)
        {
            StartCoroutine(Revealer(newSprite));
        }

        protected void PlayUnrevealAnimation(Sprite newSprite)
        {
            StartCoroutine(Unrevealer(newSprite));
        }

        private IEnumerator Revealer(Sprite revealedSprite)
        {
            Vector3 _scale = spriteRenderer.transform.localScale;
            float scale3 = 1f;
            while (scale3 > 0f)
            {
                yield return null;
                scale3 = (_scale.y = scale3 - Time.deltaTime * balloonAnimationSpeedMult);
                spriteRenderer.transform.localScale = _scale;
            }

            scale3 = (_scale.y = 0f);
            spriteRenderer.transform.localScale = _scale;
            spriteRenderer.sprite = revealedSprite;
            while (scale3 < 1f)
            {
                yield return null;
                scale3 = (_scale.y = scale3 + Time.deltaTime * balloonAnimationSpeedMult);
                spriteRenderer.transform.localScale = _scale;
            }

            scale3 = 1f;
            _scale.y = scale3;
            spriteRenderer.transform.localScale = _scale;
        }

        private IEnumerator Unrevealer(Sprite newSprite)
        {
            Vector3 _scale = spriteRenderer.transform.localScale;
            float scale3 = 1f;
            while (scale3 > 0f)
            {
                yield return null;
                scale3 = (_scale.y = scale3 - Time.deltaTime * balloonAnimationSpeedMult);
                spriteRenderer.transform.localScale = _scale;
            }

            scale3 = (_scale.y = 0f);
            spriteRenderer.transform.localScale = _scale;
            spriteRenderer.sprite = newSprite;
            while (scale3 < 1f)
            {
                yield return null;
                scale3 = (_scale.y = scale3 + Time.deltaTime * balloonAnimationSpeedMult);
                spriteRenderer.transform.localScale = _scale;
            }

            scale3 = 1f;
            _scale.y = scale3;
            spriteRenderer.transform.localScale = _scale;
        }

        public virtual void ClickableSighted(int player)
        {
            if (!trackingPlayer)
            {
                targetOffset = -2f;
            }
        }

        public virtual void ClickableUnsighted(int player)
        {
            if (!trackingPlayer)
            {
                targetOffset = 0;
            }
        }

        public virtual bool ClickableHidden()
        {
            return false;
        }

        public virtual bool ClickableRequiresNormalHeight()
        {
            return false;
        }
    }
}
