using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.Managers;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.Objects.Farm;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private float motorVolume;

        [SerializeField]
        private float speed;

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
            //motorAudMan.audioDevice.spatialBlend = 1f;
            //motorAudMan.audioDevice.rolloffMode = AudioRolloffMode.Custom;
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
            //buzzerAudMan.audioDevice.spatialBlend = 1f;
            //buzzerAudMan.audioDevice.rolloffMode = AudioRolloffMode.Custom;

            GameObject trapObj = new GameObject("Trap");
            trapObj.transform.SetParent(transform, false);
            trapObj.transform.localPosition = Vector3.up * 20f;

            BoxCollider trapCollider = trapObj.AddComponent<BoxCollider>();
            //trapCollider.isTrigger = true;
            trapCollider.size = Vector3.one;
            trapCollider.gameObject.SetRigidbody();
            //rb.maxDepenetrationVelocity = float.MaxValue;

            trapObj.AddComponent<CubeTrap>().reaper = this;

            trapObj.transform.localScale = new Vector3(250f, 25f, 20f);
        }

        public void Initialize(FarmFieldTripManager gameMan, EnvironmentController ec)
        {
            tripManager = gameMan;
            this.ec = ec;
        }

        public void Begin()
        {
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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Wall"))
            {
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
