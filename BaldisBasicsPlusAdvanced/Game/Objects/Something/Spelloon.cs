using BaldisBasicsPlusAdvanced.Cache;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Something
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

        public bool trackPlayer;

        public Balloon Floater => floater;

        public bool Available => available;

        public string Value => value;

        public bool Popping => popping;

        public void initializePrefab()
        {
            floater = GetComponent<Balloon>();
            entity = GetComponent<Entity>();
            audMan = GetComponent<AudioManager>();
            sprite = GetComponentInChildren<SpriteRenderer>().transform;
        }

        public void initializePrefabPost(string symbol)
        {
            value = symbol;
            GetComponentInChildren<SpriteRenderer>().sprite = AssetsStorage.sprites["adv_balloon_" + symbol];
        }

        public void initialize(int index)
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
            if (!trackPlayer && currentOffset != targetOffset)
            {
                currentOffset = Mathf.MoveTowards(currentOffset, targetOffset, Time.deltaTime * 20f);
                sprite.localPosition = spriteInitPosition + Vector3.up * currentOffset;
            }
        }

        public void Clicked(int player)
        {
            symbolMachine.numberClicked(index);
        }

        public void ClickableSighted(int player)
        {
            if (!trackPlayer)
            {
                targetOffset = -2f;
            }
        }

        public void ClickableUnsighted(int player)
        {
            if (!trackPlayer)
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
            trackPlayer = true;
            sprite.localPosition = spriteInitPosition;
            floater.Entity.SetTrigger(value: false);
            StartCoroutine(tracker(player, playerNumber));
        }

        public void reInit()
        {
            trackPlayer = false;
            Floater.Entity.SetTrigger(value: true);
            currentOffset = -2f;
            sprite.localPosition = spriteInitPosition + Vector3.up * currentOffset;
        }

        public void disable()
        {
            entity.Enable(val: false);
        }

        public void pop()
        {
            if (!popping)
            {
                if (trackPlayer)
                {
                    symbolMachine.numberDropped();
                }

                popping = true;
                sprite.gameObject.SetActive(value: false);
                floater.Stop();
                entity.Enable(val: false);
                audMan.PlaySingle(AssetsStorage.sounds["pop"]);
                if (base.gameObject.activeInHierarchy)
                {
                    StartCoroutine(popWait());
                }
            }
            else if (!base.gameObject.activeInHierarchy)
            {
                Object.Destroy(base.gameObject);
            }
        }

        private IEnumerator tracker(PlayerManager player, int playerNumber)
        {
            floater.enabled = false;
            sprite.localPosition = spriteHeldPosition;
            sprite.gameObject.layer = 29;
            while (trackPlayer)
            {
                base.transform.position = player.transform.position;
                base.transform.rotation = player.transform.rotation;
                if (player.hidden)
                {
                    reInit();
                    symbolMachine.numberDropped();
                    targetOffset = 0f;
                }

                yield return null;
            }

            floater.enabled = true;
            base.transform.eulerAngles = Vector3.zero;
            sprite.gameObject.layer = initialSpriteLayer;
        }

        private IEnumerator popWait()
        {
            yield return null;
            while (audMan.QueuedAudioIsPlaying)
            {
                yield return null;
            }

            Object.Destroy(base.gameObject);
        }

        public void use()
        {
            available = false;
        }
    }
}