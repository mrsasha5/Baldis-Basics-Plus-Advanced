using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Food
{
    public class PlateFoodTrap : MonoBehaviour, IEntityPrefab, IClickable<int>
    {
        [SerializeField]
        private SoundObject audPlaceOnGround;

        [SerializeField]
        private SoundObject audOpen;

        [SerializeField]
        private SoundObject audEat;

        [SerializeField]
        private SpriteRenderer[] renderers;

        [SerializeField]
        private Entity entity;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private Effect effect;

        [SerializeField]
        private float minHeight;

        [SerializeField]
        private float height;

        [SerializeField]
        private float gravity;

        private EnvironmentController ec;

        private float time;

        private bool ready;

        private bool opened;

        private bool eaten;

        public void InitializePrefab(Entity entity, int variant)
        {
            audPlaceOnGround = AssetsStorage.sounds["food_plate_drop"];
            audOpen = AssetsStorage.sounds["food_plate_lift"];
            audEat = AssetsStorage.sounds["adv_yum"];

            entity.gameObject.SetRigidbody();

            this.entity = entity;
            entity.SetGrounded(false);
            audMan = gameObject.AddComponent<PropagatedAudioManager>();
            ReflectionHelper.SetValue<bool>(audMan, "disableSubtitles", true);

            minHeight = 1.55f;
            height = 5f;
            gravity = 15f;

            renderers = new SpriteRenderer[3];

            renderers[0] = GetComponentInChildren<SpriteRenderer>(); //plate renderer
            renderers[1] = ObjectsCreator.CreateSpriteRenderer(null);
            renderers[2] = ObjectsCreator.CreateSpriteRenderer(AssetsStorage.sprites["food_plate_cover"]);

            renderers[0].transform.parent.gameObject
                .AddComponent<SortingGroup>(); //oh oh!!!! Finally, I can change the rendering priority without any layers conflicts!!

            renderers[0].sortingOrder = -2;
            renderers[1].sortingOrder = -1;

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].transform.SetParent(ReflectionHelper.GetValue<Transform>(entity, "rendererBase"), false);

                float size = 0.025f;
                if (i == 1) size = 1.5f;

                renderers[i].transform.localScale = Vector3.one * size;
            }

            if (variant != 1) effect = Effect.Enjoyment;
        }

        private void Update()
        {
            if (!ready)
            {
                height -= Time.deltaTime * ec.EnvironmentTimeScale * gravity;

                if (height < minHeight)
                {
                    height = minHeight;
                    ready = true;
                    audMan.PlaySingle(audPlaceOnGround);
                    entity.SetGrounded(true);
                }

                entity.SetHeight(height);
            }

            if (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                if (time <= 0f) Destroy(gameObject);
            }
        }

        public void AssignVisualWith(ItemObject itemObject)
        {
            renderers[1].sprite = itemObject.itemSpriteLarge;
        }

        public void Initialize(EnvironmentController ec, Vector3 pos, Vector3? forceDirection = null, float force = 25f)
        {
            this.ec = ec;
            entity.Initialize(ec, pos);
            if (forceDirection != null) entity.AddForce(new Force((Vector3)forceDirection, force, -force));
        }

        public void Eat(Entity entity)
        {
            eaten = true;
            audMan.PlaySingle(audEat);
            renderers[1].enabled = false;
            time = 10f;

            if (effect == Effect.Enjoyment)
            {
                
                if (entity.CompareTag("Player"))
                {
                    entity.SetSpeedEffect(1.4f, 10f,
                        ObjectsStorage.ItemObjects["CookedChickenLeg"].itemSpriteSmall);
                    entity.GetComponent<PlayerMovement>().AddStamina(25f, false);
                }
                else 
                {
                    entity.SetSpeedEffect(0f, 8f);
                }
            } else
            {
                if (entity.CompareTag("Player"))
                {
                    PlayerMovement movement = entity.GetComponent<PlayerMovement>();
                    movement.AddStamina(-25f, false);
                    if (movement.stamina < 0f) movement.stamina = 0f;
                }
                else
                {
                    entity.SetSpeedEffect(0f, 3f);
                }
            }
            
        }

        public void Open()
        {
            if (!opened)
            {
                opened = true;
                audMan.PlaySingle(audOpen);
                StartCoroutine(OpenAnimation());
            }
            
        }

        private void OnTriggerStay(Collider other)
        {
            if (!ClickableHidden() && other.CompareTag("NPC"))
            {
                NPC npc = other.GetComponent<NPC>();
                Open();
                Eat(npc.GetComponent<Entity>());
            }
        }

        private IEnumerator OpenAnimation()
        {
            float maxHeight = 8f;
            Vector3 pos = renderers[2].transform.localPosition;
            Color color = Color.white;
            while (pos.y < maxHeight)
            {
                pos.y += Time.deltaTime * 30f;

                if (pos.y > maxHeight) pos.y = maxHeight;
                color.a = (maxHeight - pos.y) / maxHeight;

                renderers[2].color = color;
                renderers[2].transform.localPosition = pos;
                yield return null;
            }

            Destroy(renderers[2].gameObject);
        }

        public void Clicked(int player)
        {
            if (!ClickableHidden())
            {
                if (!opened) Open();
                else
                {
                    Eat(Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<Entity>());
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
            return eaten && opened;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return false;
        }

        public enum Effect
        {
            Vomit,
            Enjoyment
        }
    }
}
