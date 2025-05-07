using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SavedData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class MysteriousPortal : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private BoxCollider collider;

        private float ignoreCooldown = 2f;

        private ParticleSystem particleSystem;

        private MysteriousPortal connectedPortal;

        private EnvironmentController ec;

        private List<IEnumerator> animationsQueue = new List<IEnumerator>();

        private IEnumerator currentAnimation;

        private List<Entity> ignorableEntities = new List<Entity>();

        private List<float> ignorableTimes = new List<float>();

        private bool openPortal; //for animation

        private bool activated;

        private bool used;

        private bool isPlayerPortal;

        private bool isStatic;

        public bool Connected => connectedPortal != null;

        public bool IsPlayerPortal => isPlayerPortal;

        public AudioManager AudMan => audMan;

        public void InitializePrefab()
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(5f, 5f, 5f);
            collider.isTrigger = true;
            collider.enabled = false;
            
            spriteRenderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_portal"]);
            spriteRenderer.transform.parent = gameObject.transform;
            spriteRenderer.transform.localPosition = Vector3.zero;

            audMan = gameObject.AddComponent<PropagatedAudioManager>();
            Destroy(audMan.audioDevice.gameObject); //because don't need this in prefabs
        }

        public void InitializeParticleSystem()
        {
            particleSystem = gameObject.AddComponent<ParticleSystem>();

            MainModule main = particleSystem.main;
            EmissionModule emission = particleSystem.emission;
            ShapeModule shape = particleSystem.shape;
            TextureSheetAnimationModule anim = particleSystem.textureSheetAnimation;

            main.loop = true;
            main.startLifetime = 0.6f;
            main.startSpeed = -10f;
            main.startSize = 0.7f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.playOnAwake = false; //useless if isn't used by prefab.
            main.startColor = Color.white;

            emission.rateOverTime = 50f;

            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 15f;

            ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.material = AssetsStorage.materials["qmark_sheet"];

            anim.enabled = true; //not enabled
            anim.numTilesX = 4;
            anim.numTilesY = 2;
        }

        public void SetStatic(bool isStatic)
        {
            this.isStatic = isStatic;
        }

        public void PostInitialize(EnvironmentController ec, bool isPlayerPortal)
        {
            this.ec = ec;
            this.isPlayerPortal = isPlayerPortal;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            if (animationsQueue.Count > 0 && currentAnimation == null)
            {
                currentAnimation = animationsQueue[0];
                animationsQueue.RemoveAt(0);
            }
            if (currentAnimation != null && !currentAnimation.MoveNext())
            {
                currentAnimation = null;
            }

            for (int i = 0; i < ignorableTimes.Count; i++)
            {
                if (ignorableTimes[i] > 0f) ignorableTimes[i] -= Time.deltaTime;
            }
        }

        public void ConnectTo(MysteriousPortal portal)
        {
            connectedPortal = portal;
        }

        public void Activate()
        {
            SetAnim(true, 5f);
        }

        public void Deactivate()
        {
            SetAnim(false, 2.5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (activated && !used && Connected)
            {
                if (other.TryGetComponent(out MathMachineNumber balloon))
                {
                    balloon.trackPlayer = true; //to fix when balloons didn't respawn
                    balloon.Pop();
                    return;
                }

                if (other.TryGetComponent(out Entity entity))
                {
                    bool isNpc = entity.TryGetComponent(out NPC npc);
                    if (isNpc && !Refl_OnPreTeleportation(npc)) return;
                    Teleport(entity);
                    if (isNpc) Refl_Teleportation(npc);
                    return;
                }
            }
        }

        private bool Refl_OnPreTeleportation(object @object)
        {
            object result = ReflectionHelper.UseMethod(@object, "Adv_OnPreTeleportation");
            return result == null || (bool)result;
        }

        private void Refl_Teleportation(object @object)
        {
            ReflectionHelper.UseMethod(@object, "Adv_OnTeleportation");
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Entity entity))
            {
                if (ignorableEntities.Contains(entity))
                {
                    int index = ignorableEntities.IndexOf(entity);
                    if (ignorableTimes[index] <= 0f)
                    {
                        ignorableEntities.RemoveAt(index);
                        ignorableTimes.RemoveAt(index);
                    }
                }
            }
        }

        private void Teleport(Entity entity)
        {
            if (ignorableEntities.Contains(entity))
            {
                int index = ignorableEntities.IndexOf(entity);
                if (ignorableTimes[index] > 0f) return;
                else
                {
                    ignorableEntities.RemoveAt(index);
                    ignorableTimes.RemoveAt(index);
                }
            }
            if (connectedPortal.ignorableEntities.Contains(entity))
            {
                int index = connectedPortal.ignorableEntities.IndexOf(entity);
                if (connectedPortal.ignorableTimes[index] > 0f) return;
                else
                {
                    connectedPortal.ignorableEntities.RemoveAt(index);
                    connectedPortal.ignorableTimes.RemoveAt(index);
                }
            }

            entity.Teleport(connectedPortal.transform.position);

            audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
            connectedPortal.AudMan.PlaySingle(AssetsStorage.sounds["teleport"]);

            ignorableEntities.Add(entity);
            ignorableTimes.Add(ignoreCooldown);

            connectedPortal.ignorableEntities.Add(entity);
            connectedPortal.ignorableTimes.Add(ignoreCooldown);

            if (!isStatic)
            {
                used = true;
                Deactivate();
            }
            if (!connectedPortal.isStatic)
            {
                connectedPortal.used = true;
                connectedPortal.Deactivate();
            }
        }

        public void SetAnim(bool appearing, float baseTime = 5f)
        {
            animationsQueue.Add(SetAnimation(appearing, baseTime));
            openPortal = appearing;
        }

        private IEnumerator SetAnimation(bool appearing, float baseTime = 5f)
        {
            float time = baseTime;
            Color color = Color.white;
            float percent = 1f;

            if (appearing) audMan.PlaySingle(AssetsStorage.sounds["adv_magical_appearing"]);

            if (DataManager.ExtraSettings.particlesEnabled)
            {
                if (appearing)
                {
                    InitializeParticleSystem();
                    particleSystem.Play();
                }
                else if (particleSystem != null)
                {
                    MainModule main = particleSystem.main;
                    ShapeModule shape = particleSystem.shape;

                    main.startSpeed = 10f;
                    main.startLifetime = 2f;

                    shape.radius = 0f;

                    particleSystem.Play();
                }
            }

            while (true)
            {
                if (appearing)
                {
                    if (time > 0)
                    {
                        time -= Time.deltaTime * ec.NpcTimeScale;
                        percent = (baseTime - time) / baseTime;

                        color.a = percent;

                        if (percent < 0) transform.localScale = Vector3.zero; //do not spam in log about negative scale isn't supported!!!
                        else transform.localScale = Vector3.one * percent;

                        spriteRenderer.color = color;
                        yield return null;
                    }
                    else
                    {
                        transform.localScale = Vector3.one;
                        spriteRenderer.color = Color.white;
                        particleSystem?.Stop();
                        SetConnectedProperties();
                        yield break;
                    }
                }
                else
                {
                    SetConnectedProperties();
                    if (time > 0)
                    {
                        time -= Time.deltaTime * ec.NpcTimeScale;
                        percent = time / baseTime;

                        color.a = percent;

                        if (percent < 0) transform.localScale = Vector3.zero; //do not spam in log about negative scale isn't supported!!!
                        else transform.localScale = Vector3.one * percent;

                        spriteRenderer.color = color;
                        yield return null;
                    }
                    else
                    {
                        transform.localScale = Vector3.zero;
                        spriteRenderer.color = new Color(1, 1, 1, 0);
                        Destroy(gameObject);
                        yield break;
                    }
                }

            }

        }

        private void SetConnectedProperties()
        {
            if (openPortal && Connected)
            {
                spriteRenderer.sprite = AssetsStorage.sprites["adv_portal_opened"];
                activated = true;
                audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
                collider.enabled = true;
                if (!connectedPortal.activated) connectedPortal.SetConnectedProperties();
            } else if (!openPortal)
            {
                activated = false;
                spriteRenderer.sprite = AssetsStorage.sprites["adv_portal"];
                collider.enabled = false;
            }
        }

    }
}
