using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class ZiplineHanger : MonoBehaviour, IPrefab, IClickable<int>
    {
        [SerializeField]
        private SoundObject audRestore;

        [SerializeField]
        private SoundObject audBreak;

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
        private Sprite genericSprite;

        [SerializeField]
        private Sprite brokenSprite;

        private KeyValuePair<Vector3, Vector3> positions;

        private MovementModifier moveMod;

        private Entity entity;

        private EntityOverrider overrider;

        private EnvironmentController ec;

        private bool moving;

        private bool broken;

        private float distance;

        private float distanceToBreak;

        private float speed;

        private int uses;

        private int positionIndex;

        private Vector3 direction;

        public float Offset => offset;

        public void InitializePrefab(int variant)
        {
            const float pixelsPerUnit = 25f;

            audRestore = AssetsStorage.sounds["adv_appearing"];
            audBreak = AssetsStorage.sounds["bal_break"];

            height = 7f;
            requiredDistance = 5f;
            acceleration = 25f;
            //startSpeed = 5f;
            offset = 1f;

            hasInfinityUses = true;
            canAcceptNPCs = true;

            genericSprite = AssetsHelper.SpriteFromFile("Textures/Objects/Hangers/adv_hanger.png", pixelsPerUnit);
            brokenSprite = AssetsHelper.SpriteFromFile("Textures/Objects/Hangers/adv_broken_hanger.png", pixelsPerUnit);

            renderer = ObjectsCreator.CreateSpriteRendererBase(genericSprite);
            renderer.transform.parent.SetParent(transform, false);
            renderer.transform.localPosition += Vector3.up * 3f;

            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = 3f;
            collider.isTrigger = true;

            motorAudMan = ObjectsCreator.CreatePropagatedAudMan(Vector3.zero);
            motorAudMan.transform.SetParent(transform, false);

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            if (variant == 2)
            {
                genericSprite = AssetsHelper.SpriteFromFile("Textures/Objects/Hangers/adv_black_hanger.png", pixelsPerUnit);
                brokenSprite = AssetsHelper.SpriteFromFile("Textures/Objects/Hangers/adv_broken_black_hanger.png", pixelsPerUnit);
                renderer.sprite = genericSprite;
                hasInfinityUses = false;
                minMaxUses.x = 4;
                minMaxUses.z = 9;
                canAcceptNPCs = false;
            }
        }

        public void Initialize(EnvironmentController ec, KeyValuePair<Vector3, Vector3> vector3s)
        {
            this.ec = ec;
            moveMod = new MovementModifier(Vector3.zero, 1f);
            positions = new KeyValuePair<Vector3, Vector3>(
                new Vector3(vector3s.Key.x, 0f, vector3s.Key.z),
                new Vector3(vector3s.Value.x, 0f, vector3s.Value.z));
            transform.position = new Vector3(positions.Key.x, 5f, positions.Key.z);
            if (!hasInfinityUses)
                uses = new System.Random(CoreGameManager.Instance.Seed()).Next(minMaxUses.x, minMaxUses.z + 1);
        }

        private void Update()
        {
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
            if (!broken && canAcceptNPCs && !moving && other.CompareTag("NPC"))
            {
                NPC npc = other.GetComponent<NPC>();

                Cell currentCell = ec.CellFromPosition(npc.transform.position);
                Cell destPoint = ec.CellFromPosition(positionIndex == 1 ? positions.Key : positions.Value);

                ec.FindPath(npc.Navigator._startTile, npc.Navigator._targetTile, PathType.Nav, out List<Cell> path, out bool success);

                int destIndex = path.IndexOf(destPoint);
                int curCellIndex = path.IndexOf(currentCell);

                if (success && curCellIndex != -1 && 
                    (destIndex != -1 ? (curCellIndex < destIndex) : 
                        (Vector3.Distance(npc.Navigator._targetTile.FloorWorldPosition, destPoint.FloorWorldPosition) <= 50f)) && 
                        ReflEvent_OnPreTakingZipline(npc) &&
                        SetEntity(npc.Navigator.Entity, kickEntity: false))
                {
                    SetMoving();
                    npc.transform.forward = direction;
                    ReflEvent_OnTakingZipline(npc);
                }
            }
        }

        public void Restore(bool playSound = true)
        {
            if (broken)
            {
                if (playSound)
                    audMan.PlaySingle(audRestore);
                broken = false;
                renderer.sprite = genericSprite;
            }
        }

        public void Break(bool playSound = true)
        {
            if (!broken)
            {
                KickEntity(entity);
                if (playSound)
                    audMan.PlaySingle(audBreak);
                broken = true;
                renderer.sprite = brokenSprite;
            }
        }

        private void OnZiplineEnds()
        {
            if (!hasInfinityUses) uses--;
            moving = false;
            if (positionIndex == 0)
            {
                transform.position = new Vector3(positions.Key.x, 5f, positions.Key.z);
            } else
            {
                transform.position = new Vector3(positions.Value.x, 5f, positions.Value.z);
            }

            if (entity != null)
            {
                entity.Teleport(transform.position);

                if (entity.CompareTag("Player"))
                {
                    ec.MakeNoise(transform.position, 64);
                }

                KickEntity(entity);
            }

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
                positionIndex++;

                if (positionIndex >= 2) positionIndex = 0;

                moving = true;
                speed = startSpeed;
                direction = (positions.Key - positions.Value).normalized;
                direction.y = 0f;
                if (positionIndex == 1) direction *= -1f;

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

        private void ReflEvent_OnTakingZipline(Object @object)
        {
            ReflectionHelper.NoCache_UseMethod(@object, "Adv_OnTakingZipline", overrider);
        }

        private bool ReflEvent_OnPreTakingZipline(Object @object)
        {
            object result = ReflectionHelper.NoCache_UseMethod(@object, "Adv_OnPreTakingZipline");
            return result == null || (bool)result;
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
                overrider.SetGrounded(false);
                overrider.SetHeight(height);
                overrider.entity?.UpdateHeightAndScale(); //MYSTMAN DID WRONG ORDER IN CODE, BRUH!!!
                return true;
            }

            entity = null;
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
