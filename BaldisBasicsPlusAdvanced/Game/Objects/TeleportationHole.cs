using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.InventoryItems;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using static UnityEngine.ParticleSystem;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class TeleportationHole : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private SphereCollider sphereCollider;

        [SerializeField]
        private ParticleSystem[] particleSystems;

        [SerializeField]
        private GameObject childObj;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private AudioManager audManForBoom;

        [SerializeField]
        private float pitchMultiplier;

        //private float centerForceOffset = 0f; //2f - whirlpool

        private float maxForce = 40f; //18f - whirlpool

        private EnvironmentController ec;

        private List<Entity> entities = new List<Entity>();

        private List<MovementModifier> moveMods = new List<MovementModifier>();

        private List<bool> obstacleHits = new List<bool>();

        private float _distance;

        private float _force;

        private bool _obstacleHit;

        private bool ended;

        private float time;

        private Ray ray;

        private RaycastHit[] hits;

        private List<Transform> hitTransforms = new List<Transform>();

        private Vector3 _direction;

        public void InitializePrefab(int variant)
        {
            audMan = gameObject.AddComponent<PropagatedAudioManager>();
            pitchMultiplier = 0.15f;

            GameObject childObj = new GameObject("AudMan");
            childObj.transform.SetParent(transform, false);

            gameObject.layer = LayersHelper.ignoreRaycastB;

            audManForBoom = childObj.AddComponent<PropagatedAudioManager>();
            ReflectionHelper.SetValue<bool>(audManForBoom, "disableSubtitles", true);

            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 60f;
            sphereCollider.isTrigger = true;

            this.childObj = new GameObject("ExplosionEffect");
            this.childObj.transform.SetParent(transform, false);
            this.childObj.transform.localPosition = Vector3.up * -5f;

            particleSystems = new ParticleSystem[2];
            particleSystems[0] = InitializeParticleSystem(childObj);
            particleSystems[1] = InitializeParticleSystem(gameObject);
            OverrideParticleSystem(particleSystems[1]);
        }

        public ParticleSystem InitializeParticleSystem(GameObject baseObj)
        {
            ParticleSystem particleSystem = baseObj.AddComponent<ParticleSystem>();

            MainModule main = particleSystem.main;
            EmissionModule emission = particleSystem.emission;
            ShapeModule shape = particleSystem.shape;
            TextureSheetAnimationModule anim = particleSystem.textureSheetAnimation;

            main.loop = false;
            main.startLifetime = 1.5f;
            main.startSpeed = 30f;
            main.startSize = 0.7f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.playOnAwake = false; 
            main.startColor = Color.white;

            emission.rateOverTime = 0f;
            emission.SetBursts(new Burst[] {
                    new Burst(0f, 200f)
                }
            );

            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0f;

            ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.material = AssetsStorage.materials["qmark_sheet"];

            anim.enabled = true; //not enabled
            anim.numTilesX = 4;
            anim.numTilesY = 2;
            return particleSystem;
        }

        private void OverrideParticleSystem(ParticleSystem particleSystem)
        {
            MainModule main = particleSystem.main;
            EmissionModule emission = particleSystem.emission;
            ShapeModule shape = particleSystem.shape;

            float time = 1f;

            main.loop = true;
            
            main.startSpeed = -1f * sphereCollider.radius / time;
            main.startLifetime = sphereCollider.radius / main.startSpeed.constant * -1f;
            main.startSize = 0.7f;

            emission.rateOverTime = 300f;
            emission.SetBursts(new Burst[0]);

            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = sphereCollider.radius;
        }

        public void Initialize(EnvironmentController ec, float endsIn)
        {
            this.ec = ec;
            time = endsIn;
            audManForBoom.PlaySingle(AssetsStorage.sounds["adv_explosion"]);
            if (OptionsDataManager.ExtraSettings.GetValue<bool>("particles"))
            {
                particleSystems[0].Play();
                particleSystems[1].Play();
                audMan.QueueAudio(AssetsStorage.sounds["adv_activation_start"]);
                audMan.QueueAudio(AssetsStorage.sounds["adv_activation_loop"]);
                audMan.SetLoop(true);
            }
            DestroyWindows();
        }

        private void Update()
        {
            if (time > 0f) time -= Time.deltaTime * ec.EnvironmentTimeScale;

            if (audMan.audioDevice.loop)
                audMan.pitchModifier += Time.deltaTime * ec.EnvironmentTimeScale * pitchMultiplier;
            else audMan.pitchModifier = 1f;

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] == null)
                {
                    entities.RemoveAt(i);
                    moveMods.RemoveAt(i);
                    obstacleHits.RemoveAt(i);
                    i--;
                    continue;
                }

                _distance = Vector3.Distance(base.transform.position, entities[i].transform.position);
                ray = new Ray(base.transform.position, entities[i].transform.position - base.transform.position);
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
                    //_direction = base.transform.position + base.transform.forward * centerForceOffset - entities[i].transform.position;
                    _direction = base.transform.position + - entities[i].transform.position;
                    _direction.y = 0f;
                    //_distance = Vector3.Distance(base.transform.position + base.transform.forward * centerForceOffset + Vector3.up * 5f, entities[i].transform.position);
                    _distance = Vector3.Distance(base.transform.position + Vector3.up * 5f, entities[i].transform.position);
                    _force = (sphereCollider.radius - Mathf.Min(_distance, sphereCollider.radius)) / sphereCollider.radius * maxForce * ec.EnvironmentTimeScale;
                    if (_force * Time.deltaTime > _distance)
                    {
                        moveMods[i].movementAddend = _direction.normalized * _distance / Time.deltaTime;
                    }
                    else
                    {
                        moveMods[i].movementAddend = _direction.normalized * _force;
                    }

                    if (_distance <= 6f && entities[i].Grounded)
                    {
                        entities[i].RandomTeleport();
                    }
                    
                }
                else
                {
                    moveMods[i].movementAddend = Vector3.zero;
                }
            }

            if (time < 0f && !ended)
            {
                ended = true;
                for (int i = 0; i < entities.Count; i++)
                {
                    if (entities[i] == null)
                    {
                        entities.RemoveAt(i);
                        moveMods.RemoveAt(i);
                        obstacleHits.RemoveAt(i);
                        i--;
                        continue;
                    } else
                    {
                        entities[i].ExternalActivity.moveMods.Remove(moveMods[i]);

                        if (!obstacleHits[i])
                            entities[i].RandomTeleport();

                        entities.RemoveAt(i);
                        moveMods.RemoveAt(i);
                        obstacleHits.RemoveAt(i);
                        i--;
                        continue;
                    }
                }

                childObj.transform.localPosition = Vector3.zero; //normalize explosion pos
                particleSystems[0].Play(); //EXPLOSION!!1!

                //particleSystems[1].Stop();
                Destroy(particleSystems[1]);
                Destroy(GetComponent<ParticleSystemRenderer>());
                Destroy(gameObject, 5f);

                audMan.FlushQueue(true);
                audMan.QueueAudio(AssetsStorage.sounds["adv_activation_end"]);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ended) return;
            if (other.TryGetComponent(out TeleportationBombItem _)) return;
            if (other.isTrigger && other.TryGetComponent(out Entity entity) && !entities.Contains(entity))
            {
                entities.Add(entity);

                MovementModifier moveMod = new MovementModifier(Vector3.zero, 1f);
                moveMod.ignoreAirborne = true;

                moveMods.Add(moveMod);
                obstacleHits.Add(false);

                entity.ExternalActivity.moveMods.Add(moveMods[moveMods.Count - 1]);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (ended) return;
            if (other.isTrigger && other.TryGetComponent(out Entity entity) && entities.Contains(entity))
            {
                int index = entities.IndexOf(entity);

                entity.ExternalActivity.moveMods.Remove(moveMods[index]);

                entities.RemoveAt(index);
                moveMods.RemoveAt(index);
                obstacleHits.RemoveAt(index);
            }
        }

        private void DestroyWindows()
        {
            List<Window> windows = FindObjectsOfType<Window>().ToList();
            for (int i = 0; i < windows.Count; i++)
            {
                if (Vector3.Distance(transform.position, windows[i].transform.position) >= sphereCollider.radius / 2f)
                {
                    windows.RemoveAt(i);
                    i--;
                    continue;
                }
                windows[i].Break(true);
            }
        }
    }
}
