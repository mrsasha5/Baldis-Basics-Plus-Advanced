using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class ZiplineHanger : MonoBehaviour, IPrefab, IClickable<int>
    {
        [SerializeField]
        private float height;

        [SerializeField]
        private float requiredDistance;

        [SerializeField]
        private float acceleration;

        [SerializeField]
        private float startSpeed;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private AudioManager motorAudMan;

        [SerializeField]
        private SpriteRenderer renderer;

        [SerializeField]
        private float offset;

        [SerializeField]
        private IntVector2 minMaxUses;

        [SerializeField]
        private bool hasInfinityUses;

        [SerializeField]
        private bool canAcceptNPCs;

        [SerializeField]
        private Sprite brokenSprite;

        private KeyValuePair<Vector3, Vector3> positions;

        private MovementModifier moveMod;

        private Entity entity;

        private EntityOverrider overrider;

        private EnvironmentController ec;

        private bool moving;

        private float ignoresNpcsTime;

        private bool broken;

        private float distance;

        private float distanceToBreak;

        private float speed;

        private int uses;

        private int nextPos;

        private Vector3 direction;

        public float Offset => offset;

        public void InitializePrefab(int variant)
        {
            height = 7f;
            requiredDistance = 5f;
            acceleration = 25f;
            //startSpeed = 5f;
            offset = 1f;

            hasInfinityUses = true;
            canAcceptNPCs = true;

            renderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_hanger"]);
            renderer.transform.parent.SetParent(transform, false);
            renderer.transform.localPosition += Vector3.up * 3f;

            brokenSprite = AssetsStorage.sprites["adv_broken_hanger"];

            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = 3f;
            collider.isTrigger = true;

            motorAudMan = ObjectsCreator.CreatePropagatedAudMan(Vector3.zero);
            motorAudMan.transform.SetParent(transform, false);

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            if (variant == 2)
            {
                renderer.sprite = AssetsStorage.sprites["adv_black_hanger"];
                brokenSprite = AssetsStorage.sprites["adv_broken_black_hanger"];
                hasInfinityUses = false;
                minMaxUses.x = 4;
                minMaxUses.z = 9;
                canAcceptNPCs = false;
            }
        }

        public void Initialize(EnvironmentController ec, System.Random rng, KeyValuePair<Vector3, Vector3> vector3s)
        {
            this.ec = ec;
            moveMod = new MovementModifier(Vector3.zero, 1f);
            positions = new KeyValuePair<Vector3, Vector3>(
                new Vector3(vector3s.Key.x, 0f, vector3s.Key.z),
                new Vector3(vector3s.Value.x, 0f, vector3s.Value.z));
            transform.position = new Vector3(positions.Key.x, 5f, positions.Key.z);
            if (!hasInfinityUses)
                uses = rng.Next(minMaxUses.x, minMaxUses.z + 1);
        }

        private void Update()
        {
            if (ignoresNpcsTime > 0f) ignoresNpcsTime -= Time.deltaTime * ec.EnvironmentTimeScale;

            if (moving)
            {
                speed += Time.deltaTime * acceleration * ec.EnvironmentTimeScale;

                motorAudMan.pitchModifier = (speed - startSpeed) / 100f + 1f;

                if (entity != null && Vector3.Distance(entity.transform.position, transform.position) >= requiredDistance)
                {
                    KickEntity(entity);
                }

                transform.position += Time.deltaTime * direction * speed * ec.EnvironmentTimeScale;
                distance -= Time.deltaTime * speed * ec.EnvironmentTimeScale;
                entity?.Teleport(transform.position);

                if (uses <= 1 && !hasInfinityUses && distanceToBreak > distance)
                {
                    Break();
                }

                if (distance <= 0f)
                {
                    OnZiplineEnds();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (canAcceptNPCs && !moving && ignoresNpcsTime <= 0f && other.CompareTag("NPC"))
            {
                NPC npc = other.GetComponent<NPC>();
                if (npc.GetMeta().tags.Contains("student") && SetEntity(npc.GetComponent<Entity>(), kickEntity: false))
                {
                    SetMoving();
                    ignoresNpcsTime = Random.Range(5f, 15f);
                }
            }
        }

        public void Break()
        {
            if (!broken)
            {
                KickEntity(entity);
                audMan.PlaySingle(AssetsStorage.sounds["bal_break"]);
                broken = true;
                renderer.sprite = brokenSprite;
            }
        }

        private void OnZiplineEnds()
        {
            if (!hasInfinityUses) uses--;
            moving = false;
            if (nextPos == 0)
            {
                transform.position = new Vector3(positions.Key.x, 5f, positions.Key.z);
            } else
            {
                transform.position = new Vector3(positions.Value.x, 5f, positions.Value.z);
            }

            entity?.Teleport(transform.position);

            KickEntity(entity);

            motorAudMan.FlushQueue(true);
            audMan.PlaySingle(AssetsStorage.sounds["adv_wood_3"]);
            audMan.PlaySingle(AssetsStorage.sounds["lock_door_stop"]);
        }

        public void Clicked(int player)
        {
            if (ClickableHidden()) return;
            PlayerManager pm = Singleton<CoreGameManager>.Instance.GetPlayer(player);

            Entity entity = pm.GetComponent<Entity>();

            if (SetEntity(entity, kickEntity: true))
            {
                SetMoving();
            } else
            {
                pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["error_maybe"]);
            }

            PlayerInteractionController.Instance.SetGameTip(player);
        }

        private bool SetMoving()
        {
            if (!moving)
            {
                nextPos++;

                if (nextPos >= 2) nextPos = 0;

                moving = true;
                speed = startSpeed;
                direction = (positions.Key - positions.Value).normalized;
                direction.y = 0f;
                if (nextPos == 1) direction *= -1;

                distance = Vector3.Distance(positions.Key, positions.Value);
                audMan.PlaySingle(AssetsStorage.sounds["grapple_clang"]);

                motorAudMan.pitchModifier = 1f;
                motorAudMan.QueueAudio(AssetsStorage.sounds["grapple_loop"]);
                motorAudMan.SetLoop(true);

                if (!hasInfinityUses) distanceToBreak = distance * Random.value;
                return true;
            }
            return false;
        }

        private void KickEntity(Entity entity)
        {
            if (entity != null)
            {
                overrider.Release();

                entity.ExternalActivity.moveMods.Remove(moveMod);
                this.entity = null;
            }
        }

        private bool SetEntity(Entity entity, bool kickEntity)
        {
            if (kickEntity && this.entity != null) KickEntity(this.entity);
            else if (!kickEntity && this.entity != null) return false;

            this.entity = entity;
            overrider = new EntityOverrider();

            if (entity != null && entity.Override(overrider))
            {
                entity.Teleport(transform.position);

                entity.ExternalActivity.moveMods.Add(moveMod);

                overrider.SetInteractionState(false);
                overrider.SetFrozen(true);
                overrider.SetHeight(height);
                overrider.SetGrounded(false);
                return true;
            }

            overrider = null;
            return false;
        }

        public bool ClickableHidden()
        {
            return moving || broken;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return true;
        }

        public void ClickableSighted(int player)
        {
            if (!ClickableHidden()) PlayerInteractionController.Instance.SetGameTip(player, "Adv_Tip_ZiplineHanger");
        }

        public void ClickableUnsighted(int player)
        {
            PlayerInteractionController.Instance.SetGameTip(player);
        }
    }
}
