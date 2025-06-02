using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.Managers;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.Objects.Farm;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.NPCs
{
    public class Reaper : MonoBehaviour, IPrefab
    {

        [SerializeField]
        private SpriteRenderer renderer;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private AudioManager buzzerAudMan;

        [SerializeField]
        private AudioManager motorAudMan;

        [SerializeField]
        private SoundObject audReap;

        [SerializeField]
        private SoundObject audShred;

        [SerializeField]
        private ParticleSystem particleSystem;

        [SerializeField]
        private float motorVolume;

        [SerializeField]
        private float warmingSpeed;

        [SerializeField]
        private float coolingSpeed;

        [SerializeField]
        private float speed;

        private Color spriteColor;

        private float keepWarmTime;

        private float spriteWarmValue;

        private EnvironmentController ec;

        private FarmFieldTripManager tripManager;

        private bool active;

        private bool reapedPlayer;

        public void InitializePrefab(int variant)
        {
            transform.localScale = new Vector3(2f, 1f, 1f);

            audShred = AssetsStorage.sounds["buzz_lose"];
            audReap = AssetsStorage.sounds["adv_reaper_gotta_reap"];
            speed = 5f;
            motorVolume = 1f;
            warmingSpeed = 0.1f;
            coolingSpeed = 0.01f;

            renderer = ObjectsCreator.CreateSpriteRenderer(AssetsStorage.sprites["adv_reaper"], isBillboard: false);
            renderer.transform.SetParent(transform, false);
            renderer.transform.localPosition = Vector3.up * 95f;

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0f, 20f, 10f);
            boxCollider.size = new Vector3(170f, 10f, 1f); //170f 10f 1f size from 0.2
            boxCollider.isTrigger = true;
            gameObject.SetRigidbody();

            AudioSource audSource = gameObject.AddComponent<AudioSource>();
            audMan = gameObject.AddComponent<AudioManager>();
            audMan.audioDevice = audSource;

            motorAudMan = ObjectsCreator.CreateAudMan(Vector3.zero);
            motorAudMan.name = "Motor";
            motorAudMan.transform.SetParent(transform, false);
            motorAudMan.audioDevice.volume = 0f;
            motorAudMan.volumeModifier = 0f;
            ReflectionHelper.SetValue<SoundObject[]>(motorAudMan, "soundOnStart", new SoundObject[]
            {
                AssetsStorage.sounds["adv_motor_loop"]
            });
            ReflectionHelper.SetValue<bool>(motorAudMan, "loopOnStart", true);

            buzzerAudMan = ObjectsCreator.CreateAudMan(Vector3.zero);
            buzzerAudMan.name = "Buzzer";
            buzzerAudMan.transform.SetParent(transform, false);
            buzzerAudMan.audioDevice.volume = 0.5f;
            buzzerAudMan.volumeModifier = 0.5f;

            GameObject trapObj = new GameObject("Trap");
            trapObj.transform.SetParent(transform, false);
            trapObj.transform.localPosition = Vector3.up * 20f;

            BoxCollider trapCollider = trapObj.AddComponent<BoxCollider>();
            //trapCollider.isTrigger = true;
            trapCollider.size = Vector3.one;
            trapCollider.gameObject.SetRigidbody();
            //rb.maxDepenetrationVelocity = float.MaxValue;

            trapObj.AddComponent<CubeTrap>().reaper = this;

            particleSystem = new GameObject("ParticleSystem").AddComponent<ParticleSystem>();
            particleSystem.transform.SetParent(transform);
            particleSystem.transform.localPosition = Vector3.up * -5f;

            ParticleSystemRenderer particlesRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            particlesRenderer.material = AssetsHelper.LoadAsset<Material>("DustTest");
            particlesRenderer.material.shader = Shader.Find("Shader Graphs/Standard");
            particlesRenderer.material.SetColor(Color.gray);

            MainModule main = particleSystem.main;
            main.gravityModifier = -4f;
            main.startLifetime = 5f;
            main.startSize = 9f;
            main.startSpeed = 0f;
            main.maxParticles = 10000;

            ShapeModule shape = particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(600f, 25f, 20f);

            EmissionModule emission = particleSystem.emission;
            emission.rateOverTime = 0;
            emission.rateOverDistance = 100;
            emission.enabled = false;

            VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.y = 24f;
            velocityOverLifetime.radialMultiplier = 1.5f;

            trapObj.transform.localScale = new Vector3(250f, 25f, 20f);
        }

        public void Initialize(FarmFieldTripManager gameMan, EnvironmentController ec)
        {
            tripManager = gameMan;
            this.ec = ec;
        }

        public void Begin()
        {
            spriteColor = Color.white;
            spriteWarmValue = 1f;
            gameObject.SetActive(true);
            active = true;
            audMan.PlaySingle(audReap);
            StartCoroutine(Animator());
            StartCoroutine(VolumeController());
        }

        private IEnumerator VolumeController()
        {
            float baseTime = 10f;
            float time = baseTime;
            while (time > 0f)
            {
                time -= Time.deltaTime * ec.NpcTimeScale;
                if (time < 0f) time = 0f;
                motorAudMan.audioDevice.volume = (baseTime - time) / baseTime * motorVolume;
                motorAudMan.volumeModifier = motorAudMan.audioDevice.volume;
                yield return null;
            }
        }

        private IEnumerator Animator()
        {
            float value = 0f;
            Color color = new Color(1f, 1f, 1f, value);
            while (value < 1f)
            {
                value += Time.deltaTime;
                if (value > 1f) value = 1f;
                color.a = value;
                renderer.color = color;
                yield return null;
            }
        }

        private void Update()
        {
            if (active)
                transform.position += transform.forward * -speed * Time.deltaTime * ec.NpcTimeScale;

            if (keepWarmTime > 0f)
            {
                keepWarmTime -= Time.deltaTime * ec.NpcTimeScale;
                spriteWarmValue = Mathf.Clamp01(spriteWarmValue - warmingSpeed * Time.deltaTime * ec.NpcTimeScale);
                spriteColor.g = spriteWarmValue;
                spriteColor.b = spriteWarmValue;
                renderer.color = spriteColor;
            }
            else
            {
                if (particleSystem.emission.enabled)
                {
                    EmissionModule emission = particleSystem.emission;
                    emission.enabled = false;
                }
                spriteWarmValue = Mathf.Clamp01(spriteWarmValue + coolingSpeed * Time.deltaTime * ec.NpcTimeScale);
                spriteColor.g = spriteWarmValue;
                spriteColor.b = spriteWarmValue;
                renderer.color = spriteColor;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Wall"))
            {
                if (!particleSystem.emission.enabled)
                {
                    EmissionModule emission = particleSystem.emission;
                    emission.enabled = true;
                }

                keepWarmTime = 3f;
                Cell cell = ec.CellFromPosition(other.transform.parent.position);
                Direction direction = Direction.Null;
                foreach (Direction dir in Directions.All())
                {
                    if (cell.Tile.Collider(dir) == other.gameObject)
                    {
                        direction = dir;
                        break;
                    }
                }
                if (direction != Direction.Null)
                {
                    ec.CreateCell(cell.ConstBin & ~Directions.ToBinary(direction), cell.room.transform,
                        cell.position, cell.room); //bye bye wall
                    buzzerAudMan.PlaySingle(audShred);
                }
            }
            else if (other.CompareTag("Player") && !reapedPlayer)
            {
                reapedPlayer = true;
                tripManager.SpawnBaldi();
            }
        }

        private class CubeTrap : MonoBehaviour
        {
            public Reaper reaper;

            private List<Entity> frozenEntities = new List<Entity>();

            private void OnDestroy()
            {
                foreach (Entity entity in frozenEntities)
                {
                    entity.SetFrozen(false);
                }
            }

            private void OnTriggerEnter(Collider other)
            {
                if (other.TryGetComponent(out Entity entity) && (reaper.ec.GetBaldi() == null ||
                    other.gameObject != reaper.ec.GetBaldi().gameObject))
                {
                    entity.SetFrozen(true);
                    frozenEntities.Add(entity);
                }
            }

            private void OnTriggerExit(Collider other)
            {
                if (other.TryGetComponent(out Entity entity) && (reaper.ec.GetBaldi() == null ||
                    other.gameObject != reaper.ec.GetBaldi().gameObject))
                {
                    entity.SetFrozen(false);
                    frozenEntities.Remove(entity);
                }
            }

        }
    }
}
