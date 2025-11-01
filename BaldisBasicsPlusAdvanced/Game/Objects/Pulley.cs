using System.Collections;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class Pulley : MonoBehaviour, IClickable<int>, IPrefab
    {
        [SerializeField]
        private SoundObject audSnap;

        [SerializeField]
        private SoundObject audBreak;

        [SerializeField]
        private SoundObject audClick;

        [SerializeField]
        private SoundObject audLaunch;

        [SerializeField]
        private SoundObject[] audProgress;

        [SerializeField]
        private Texture2D[] backgrounds;

        [SerializeField]
        private SphereCollider collider;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        internal int uses;

        [SerializeField]
        internal int points;

        [SerializeField]
        internal int finalPoints;

        [SerializeField]
        internal float maxDistance;

        [SerializeField]
        private Color handleColor;

        [SerializeField]
        private Vector2 minMaxMultiplierSpeed;

        [SerializeField]
        private float offsetForward;

        [SerializeField]
        private float offsetHeight;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private AudioManager motorAudMan;

        private EnvironmentController ec;

        //1 - Background
        //2 - Pulley
        private MeshRenderer[] renderers;

        private IEnumerator fadeOutAnimator;

        private int currentBackgroundIndex;

        private Color color;

        private float distance;

        private Vector3 velocity;

        private Vector3 position;

        private Vector3 offsetPosition;

        private Vector3[] positions;

        private MovementModifier moveMod;

        private Entity entity;

        //It exists only for fixing cursor on one frame after returning back
        private bool showCursor = true;

        private bool taken;

        private bool broken;

        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public Texture2D FirstBg => backgrounds[0];

        public void InitializePrefab(int variant)
        {
            audSnap = AssetsStorage.sounds["bal_break"];
            audBreak = AssetsStorage.sounds["adv_super_break"];
            audClick = AssetsStorage.sounds["adv_pulley_click"];
            audLaunch = AssetsStorage.sounds["grapple_launch"];
            audProgress = new SoundObject[] {
                AssetsStorage.sounds["ytp_pickup_0"],
                AssetsStorage.sounds["ytp_pickup_1"],
                AssetsStorage.sounds["ytp_pickup_2"],
            };

            handleColor = Color.yellow;

            audMan = ObjectsCreator.CreateAudMan(gameObject);
            motorAudMan = ObjectsCreator.CreateAudMan(Vector3.zero);
            motorAudMan.name = "Motor";
            motorAudMan.transform.SetParent(transform, false);

            backgrounds = new Texture2D[5];

            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i] = AssetsStorage.textures["adv_pulley_base" + (i + 1)];
            }

            spriteRenderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_pulley"]);
            spriteRenderer.transform.parent.SetParent(transform);
            spriteRenderer.color = Color.yellow;
            collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = 2f;
            collider.isTrigger = true;

            uses = 4;
            points = 10;
            finalPoints = 40;
            minMaxMultiplierSpeed = new Vector2(0.35f, 1f);

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1977f;
            lineRenderer.endWidth = 0.1977f;
            lineRenderer.allowOcclusionWhenDynamic = false;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.material = new Material(AssetsStorage.materials["black_behind"]);
            lineRenderer.enabled = false;

            maxDistance = 40f;
            offsetForward = 3f;
            offsetHeight = -1f;
        }

        private void Update()
        {
            if (!showCursor) showCursor = true;

            if (fadeOutAnimator != null)
            {
                if (!fadeOutAnimator.MoveNext()) fadeOutAnimator = null;
            }
        }

        private void LateUpdate()
        {
            if (taken)
            {
                if (entity == null)
                {
                    ReturnBack();
                    return;
                }

                velocity = transform.position;

                transform.position = entity.transform.position 
                    + entity.transform.forward * offsetForward + Vector3.up * offsetHeight;

                velocity = transform.position - velocity;

                distance = Vector3.Distance(position, transform.position);

                moveMod.movementMultiplier =
                    minMaxMultiplierSpeed.x + (minMaxMultiplierSpeed.y - minMaxMultiplierSpeed.x) * 
                    (1f - Mathf.Clamp01(distance / maxDistance));

                if (velocity.magnitude > 0.01f)
                {
                    if (!motorAudMan.AnyAudioIsPlaying)
                    {
                        motorAudMan.QueueAudio(audClick);
                        motorAudMan.SetLoop(true);
                    }

                    motorAudMan.pitchModifier = 2f * Mathf.Clamp01(distance / maxDistance);
                }
                else
                {
                    if (motorAudMan.AnyAudioIsPlaying)
                        motorAudMan.FlushQueue(true);
                    motorAudMan.pitchModifier = 1f;
                }

                UpdateLineRenderer();
                UpdateBackgroundRenderer();

                if (distance > maxDistance)
                {
                    ReturnBack();
                }
            }

            if (broken && !audMan.AnyAudioIsPlaying) Destroy(gameObject);
        }

        private IEnumerator FadeOutAnimator()
        {
            float percent = 1f;
            float speed = 0.25f;

            int newBackgroundIndex = backgrounds.Length - 1;

            while (percent > 0f)
            {
                percent = Mathf.Clamp01(percent - Time.deltaTime * speed);

                newBackgroundIndex = (int)((backgrounds.Length - 1) * percent);

                if (currentBackgroundIndex != newBackgroundIndex)
                {
                    //dingAudMan.PlaySingle(audProgress[(int)((audProgress.Length - 1) * percent)]);
                }

                currentBackgroundIndex = newBackgroundIndex;

                renderers[0].material.mainTexture = backgrounds[currentBackgroundIndex];

                yield return null;
            }
        }

        private void UpdateBackgroundRenderer()
        {
            if (fadeOutAnimator == null)
            {
                float percent = Mathf.Clamp01(distance / maxDistance);

                int newBackgroundIndex = (int)((backgrounds.Length - 1) * percent);

                if (currentBackgroundIndex != newBackgroundIndex)
                {
                    //dingAudMan.PlaySingle(audProgress[(int)((audProgress.Length - 1) * percent)]);
                }

                currentBackgroundIndex = newBackgroundIndex;

                renderers[0].material.mainTexture = backgrounds[currentBackgroundIndex];
            }   
        }

        private void UpdateLineRenderer()
        {
            color.r = Mathf.Clamp(distance / maxDistance, 0f, 1f);

            lineRenderer.material.SetColor(color);

            positions[0] = position;
            positions[1] = transform.position;
            lineRenderer.SetPositions(positions);
        }

        public void Initialize(EnvironmentController ec, Vector3 pos, Vector3 offsetPos, MeshRenderer[] renderers)
        {
            this.ec = ec;
            positions = new Vector3[2];
            transform.position = pos + offsetPos;
            position = pos;
            spriteRenderer.enabled = false;
            this.renderers = renderers;

            //dingAudMan.transform.SetParent(renderers[0].transform, false);
            renderers[1].material.SetColor(handleColor);

            UpdateBackgroundRenderer();
        }

        public void Clicked(int player)
        {
            if (!ClickableHidden())
            {
                Take(Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<Entity>());
                spriteRenderer.gameObject.layer = LayersHelper.takenBalloonLayer;
            }
        }

        private void ReturnBack()
        {
            if (uses <= 1)
            {
                Break();
                return;
            }

            showCursor = false;

            UpdateBackgroundRenderer();

            fadeOutAnimator = FadeOutAnimator();

            spriteRenderer.gameObject.layer = 0;
            collider.enabled = true;
            taken = false;
            lineRenderer.enabled = false;
            spriteRenderer.enabled = false;
            renderers[1].enabled = true; //Show pulley on wall
            transform.position = position + offsetPosition;
            audMan.PlaySingle(audSnap);
            motorAudMan.FlushQueue(true);

            if (entity != null)
            {
                entity.ExternalActivity.moveMods.Remove(moveMod);

                if (entity.CompareTag("Player"))
                {
                    ec.MakeNoise(transform.position, 64);
                    Singleton<CoreGameManager>.Instance.AddPoints(points, 0, true);
                }
            }

            uses--;
        }

        public void Take(Entity entity)
        {
            collider.enabled = false;
            taken = true;
            spriteRenderer.enabled = true;
            renderers[1].enabled = false; //hide pulley
            lineRenderer.enabled = true;

            this.entity = entity;
            moveMod = new MovementModifier(Vector3.zero, 1f);
            entity.ExternalActivity.moveMods.Add(moveMod);

            audMan.PlaySingle(audLaunch);
        }

        public void Break()
        {
            uses = 0;
            taken = false;

            spriteRenderer.gameObject.layer = 0;
            lineRenderer.enabled = false;
            spriteRenderer.enabled = false;
            Destroy(renderers[1].gameObject); //destroy pulley renderer

            UpdateBackgroundRenderer();

            transform.position = position + offsetPosition;

            audMan.PlaySingle(audBreak);
            motorAudMan.FlushQueue(true);

            broken = true;

            if (entity != null)
            {
                entity.ExternalActivity.moveMods.Remove(moveMod);

                if (entity.CompareTag("Player"))
                {
                    ec.MakeNoise(transform.position, 64);
                    Singleton<CoreGameManager>.Instance.AddPoints(finalPoints, 0, true);
                }
            }
        }

        public void ClickableSighted(int player)
        {
            
        }

        public void ClickableUnsighted(int player)
        {
            
        }

        public bool ClickableHidden()
        {
            return taken || broken || !showCursor;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return true;
        }
    }
}
