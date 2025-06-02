using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Spelling
{
    public class Spelloon : MonoBehaviour, IClickable<int>, IPrefab
    {
        public SymbolMachine symbolMachine;

        [SerializeField]
        private Balloon floater;

        [SerializeField]
        private Transform sprite;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private Entity entity;

        [SerializeField]
        private string value;

        private Vector3 spriteHeldPosition = Vector3.up * -4f + Vector3.forward * 4f;

        private Vector3 spriteInitPosition = Vector3.zero;

        private int initialSpriteLayer;

        private float targetOffset;

        private float currentOffset;

        private bool available = true;

        private bool popping;

        private int index;

        public bool trackingPlayer;

        public Balloon Floater => floater;

        public bool Available => available;

        public int Index => index;

        public AudioManager AudMan => audMan;

        public string Value => value;

        public bool Popping => popping;

        public void InitializePrefab(int variant)
        {
            floater = GetComponent<Balloon>();
            entity = GetComponent<Entity>();
            audMan = GetComponent<AudioManager>();
            sprite = GetComponentInChildren<SpriteRenderer>().transform;

            entity.SetGrounded(false);
        }

        public void InitializePrefabPost(string symbol, Sprite sprite)
        {
            value = symbol;
            GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        }

        public void Initialize(int index)
        {
            this.index = index;
        }

        private void Start()
        {
            spriteInitPosition = sprite.localPosition;
            initialSpriteLayer = sprite.gameObject.layer;
        }

        private void Update()
        {
            if (!trackingPlayer && currentOffset != targetOffset)
            {
                currentOffset = Mathf.MoveTowards(currentOffset, targetOffset, Time.deltaTime * 20f);
                sprite.localPosition = spriteInitPosition + Vector3.up * currentOffset;
            }

            if (Popping && !AudMan.AnyAudioIsPlaying)
            {
                Destroy(gameObject);
            }
        }

        public void Clicked(int player)
        {
            symbolMachine.NumberClicked(index);
        }

        public void ClickableSighted(int player)
        {
            if (!trackingPlayer)
            {
                targetOffset = -2f;
            }
        }

        public void ClickableUnsighted(int player)
        {
            if (!trackingPlayer)
            {
                targetOffset = 0f;
            }
        }

        public bool ClickableHidden()
        {
            return false;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return false;
        }

        public void TrackPlayer(PlayerManager player, int playerNumber)
        {
            trackingPlayer = true;
            sprite.localPosition = spriteInitPosition;
            floater.Entity.SetTrigger(value: false);
            StartCoroutine(Tracker(player, playerNumber));
        }

        public void ReInit()
        {
            trackingPlayer = false;
            Floater.Entity.SetTrigger(value: true);
            currentOffset = -2f;
            sprite.localPosition = spriteInitPosition + Vector3.up * currentOffset;
            available = true;
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
                    symbolMachine.NumberDropped();
                }

                popping = true;
                sprite.gameObject.SetActive(value: false);
                floater.Stop();
                entity.Enable(val: false);
                audMan.PlaySingle(AssetsStorage.sounds["pop"]);
            }
            else if (!base.gameObject.activeInHierarchy)
            {
                Object.Destroy(base.gameObject);
            }
        }

        private IEnumerator Tracker(PlayerManager player, int playerNumber)
        {
            floater.enabled = false;
            sprite.localPosition = spriteHeldPosition;
            sprite.gameObject.layer = LayersHelper.takenBalloonLayer;
            Entity entity = player.GetComponent<Entity>();
            while (trackingPlayer)
            {
                base.transform.position = player.transform.position;
                base.transform.rotation = player.transform.rotation;
                if (entity.Frozen || entity.InteractionDisabled)
                {
                    ReInit();
                    symbolMachine.NumberDropped();
                    targetOffset = 0f;
                }

                yield return null;
            }

            targetOffset = 0f;
            floater.enabled = true;
            base.transform.eulerAngles = Vector3.zero;
            sprite.gameObject.layer = initialSpriteLayer;
        }

        public void Use()
        {
            ReInit();
        }
    }
}