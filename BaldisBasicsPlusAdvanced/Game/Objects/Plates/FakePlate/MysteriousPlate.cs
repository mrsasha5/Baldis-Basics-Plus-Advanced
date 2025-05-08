using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.FakePlate
{
    public class MysteriousPlate : BaseCooldownPlate
    {
        [SerializeField]
        private MysteriousPlateEntityChecker checker;

        [SerializeField]
        private Material openedMaterial;

        [SerializeField]
        private float standardSphereSize;

        private static List<Type> tricks;

        private BaseTrick currentTrick;

        private bool entitySuckerMode;

        private bool overrideVisualPressedState;

        protected override bool VisualPressedStateOverridden => base.VisualPressedStateOverridden || overrideVisualPressedState;

        public bool EntitySuckerMode => entitySuckerMode;

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.showsCooldown = false; //no cooldown, players, haha.
            plateData.allowsToCopyTextures = false;
        }

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);

            standardSphereSize = 30f;

            openedMaterial = new Material(deactivatedMaterial);
            openedMaterial.mainTexture = AssetsStorage.textures["adv_fake_plate_surprize"];

            GameObject checkerObj = new GameObject("EntityChecker");
            checker = checkerObj.AddComponent<MysteriousPlateEntityChecker>();
            checker.plate = this;

            checkerObj.transform.SetParent(transform, false);
            checkerObj.transform.localPosition = new Vector3(0f, 5f, 0f);
            checkerObj.layer = LayersHelper.ignoreRaycastB;

            SphereCollider checkerCol = checkerObj.AddComponent<SphereCollider>();
            checkerCol.radius = standardSphereSize;
            checkerCol.isTrigger = true;

            checker.sphereCollider = checkerCol;

            tricks = new List<Type>();

            RegisterTrick<SurprizeTrick>();
            RegisterTrick<BoxingGloveTrick>();
            RegisterTrick<ObstacleTrick>();
            RegisterTrick<AnvilTrick>();
        }

        public void SetRandomCooldown(float min = 15f, float max = 80f)
        {
            SetCooldown(UnityEngine.Random.Range(min, max));
        }

        public void OverrideEntityTriggerSize(float size)
        {
            checker.sphereCollider.radius = size;
        }

        public void ResetEntityTriggerSize()
        {
            checker.sphereCollider.radius = standardSphereSize;
        }

        private void RegisterTrick<T>() where T : BaseTrick, new()
        {
            tricks.Add(typeof(T));
        }

        protected override void VirtualStart()
        {
            base.VirtualStart();
            System.Random rng = new System.Random((int)
                (transform.position.x + transform.position.y + transform.position.z)); //just depends on the position

            BasePlate[] basePlates = Array.FindAll(AssetsHelper.LoadAssets<BasePlate>(), x => x.Data.allowsToCopyTextures);
            int index = rng.Next(0, basePlates.Length);

            deactivatedMaterial = new Material(deactivatedMaterial);
            activatedMaterial = new Material(activatedMaterial);
            deactivatedMaterial.mainTexture = basePlates[index].UnpressedTex;
            activatedMaterial.mainTexture = basePlates[index].PressedTex;
            UpdateVisualPressedState(false);
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (currentTrick != null)
            {
                currentTrick.VirtualUpdate();
            } else RandomTrick();
        }

        public void SetSurprizeVisual(bool state, bool playAudio, bool playPressingAudio)
        {
            overrideVisualPressedState = state;

            if (state)
            {
                meshRenderers[0].material = openedMaterial;
                if (playAudio)
                {
                    audMan.FlushQueue(true);
                    audMan.QueueAudio(AssetsStorage.sounds["adv_suction_start"]);
                    audMan.QueueAudio(AssetsStorage.sounds["adv_suction_loop"]);
                    audMan.SetLoop(true);
                }
            } else if (playAudio)
            {
                audMan.FlushQueue(true);
                audMan.QueueAudio(AssetsStorage.sounds["adv_suction_end"]);
            }

            UpdateVisualPressedState(false, playPressingAudio);
        }

        public void SetEntitySuckerMode()
        {
            entitySuckerMode = true;
        }

        public void DisableEntitySuckerMode()
        {
            if (entitySuckerMode)
            {
                entitySuckerMode = false;
                UnpressImmediately(invokeEventOnUnpress: true, playSound: false);
            }
        }

        public void EndTrick()
        {
            if (currentTrick != null)
            {
                currentTrick.Reset();
                Destroy(currentTrick);
            }
        }

        private bool RandomTrick()
        {
            List<Type> tricks = new List<Type>(MysteriousPlate.tricks);
            if (IsUsable && tricks.Count > 0 && currentTrick == null)
            {

                while (tricks.Count > 0)
                {
                    currentTrick = (BaseTrick)gameObject.AddComponent(tricks[UnityEngine.Random.Range(0, tricks.Count)]);
                    currentTrick.Initialize(this);
                    if (currentTrick.IsSelectable())
                    {
                        currentTrick.OnPostInitialization();
                        return true;
                    }
                    else
                    {
                        Destroy(currentTrick);
                        tricks.Remove(currentTrick.GetType());
                    }
                }
                
                return false;
            }
            return false;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_fake_plate");
            SetEditorSprite("adv_editor_fake_plate");
        }

        private void OnEntityCatched(Entity entity)
        {
            if (currentTrick != null && currentTrick.OnEntityCatched(entity))
            {
                DisableEntitySuckerMode();
            }
        }

        private bool OnEntityEnterTriggerZone(Entity entity)
        {
            currentTrick?.OnEntityEnterTargetZone(entity);
            return true;
        }

        private void OnEntityStayTriggerZone(Entity entity)
        {
            currentTrick?.OnEntityStayTargetZone(entity);
        }

        private void OnEntityExitTriggerZone(Entity entity)
        {
            currentTrick?.OnEntityExitTargetZone(entity);
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            currentTrick?.OnPress();
        }

        protected override void VirtualOnUnpress()
        {
            base.VirtualOnUnpress();
            currentTrick?.OnUnpress();
        }

        public class MysteriousPlateEntityChecker : MonoBehaviour
        {
            public MysteriousPlate plate;

            public SphereCollider sphereCollider;

            private List<Entity> entities = new List<Entity>();

            private List<bool> entitiesChecked = new List<bool>();

            private List<MovementModifier> moveMods = new List<MovementModifier>();

            private List<bool> obstacleHits = new List<bool>();

            private float maxForce = 30f;

            private float _distance;

            private float _force;

            private bool _obstacleHit;

            private Vector3 _direction;

            private Ray ray;

            private RaycastHit[] hits;

            private List<Transform> hitTransforms = new List<Transform>();

            private void Update()
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    if (entities[i] == null)
                    {
                        entities.RemoveAt(i);
                        entitiesChecked.RemoveAt(i);
                        moveMods.RemoveAt(i);
                        obstacleHits.RemoveAt(i);
                        i--;
                        continue;
                    }

                    _distance = Vector3.Distance(transform.position, entities[i].transform.position);
                    ray = new Ray(transform.position, entities[i].transform.position - transform.position);
                    hits = Physics.RaycastAll(ray, _distance, LayersHelper.gumCollisionMask, QueryTriggerInteraction.Ignore);
                    hitTransforms.Clear();
                    _obstacleHit = false;
                    if (hits.Length != 0)
                    {
                        _obstacleHit = true;
                    }
                    obstacleHits[i] = _obstacleHit;

                    if (!_obstacleHit)
                    {
                        if (entitiesChecked[i] != true) entitiesChecked[i] = plate.OnEntityEnterTriggerZone(entities[i]);

                        plate.OnEntityStayTriggerZone(entities[i]);

                        if (plate.EntitySuckerMode)
                        {
                            _direction = base.transform.position + -entities[i].transform.position;
                            _direction.y = 0f;

                            _distance = Vector3.Distance(base.transform.position, entities[i].transform.position);
                            _force = (sphereCollider.radius - Mathf.Min(_distance, sphereCollider.radius)) / sphereCollider.radius * maxForce * plate.Timescale;
                            if (_force * Time.deltaTime > _distance)
                            {
                                moveMods[i].movementAddend = _direction.normalized * _distance / Time.deltaTime;
                            }
                            else
                            {
                                moveMods[i].movementAddend = _direction.normalized * _force;
                            }

                            if (_distance <= 1f && entities[i].Grounded)
                            {
                                plate.OnEntityCatched(entities[i]);
                                entities[i].Teleport(transform.position);
                            }
                        } else
                        {
                            moveMods[i].movementAddend = Vector3.zero;
                        }
                    }
                    else
                    {
                        moveMods[i].movementAddend = Vector3.zero;
                    }
                }
            }

            private void OnTriggerEnter(Collider other)
            {
                if (other.isTrigger && other.TryGetComponent(out Entity entity) && !entities.Contains(entity))
                {
                    entities.Add(entity);
                    entitiesChecked.Add(false);
                    moveMods.Add(new MovementModifier(Vector3.zero, 1f));
                    moveMods[moveMods.Count - 1].ignoreAirborne = true;
                    obstacleHits.Add(false);

                    entity.ExternalActivity.moveMods.Add(moveMods[moveMods.Count - 1]);
                }
            }

            private void OnTriggerExit(Collider other)
            {
                if (other.isTrigger && other.TryGetComponent(out Entity entity) && entities.Contains(entity))
                {
                    int index = entities.IndexOf(entity);

                    entity.ExternalActivity.moveMods.Remove(moveMods[index]);

                    entities.RemoveAt(index);
                    entitiesChecked.RemoveAt(index);
                    moveMods.RemoveAt(index);
                    plate.OnEntityExitTriggerZone(entity);
                    obstacleHits.RemoveAt(index);
                }
            }
        }
    }
}
