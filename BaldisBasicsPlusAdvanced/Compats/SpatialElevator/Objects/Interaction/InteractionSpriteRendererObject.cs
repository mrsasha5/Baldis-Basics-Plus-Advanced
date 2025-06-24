using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects.Interaction
{
    public class InteractionSpriteRendererObject : BaseInteractionObject<InteractionSpriteRendererObject>
    {

        public bool ignoreSightedEvents = true;

        public Sprite spriteOnSight;

        public Sprite spriteOnUnsight;

        protected SpriteRenderer renderer;

        public SpriteRenderer Renderer => renderer;

        public InteractionSpriteRendererObject Assign()
        {
            renderer = GetComponent<SpriteRenderer>();
            return this;
        }

        public InteractionSpriteRendererObject Assign(SpriteRenderer renderer)
        {
            this.renderer = renderer;
            return this;
        }

        public override void Hide(bool state, bool animation)
        {
            base.Hide(state, animation);

            collider.enabled = !state;

            StopAllCoroutines();

            if (animation)
            {
                StartCoroutine(Animation(state));
            }
            else
            {
                Color color = renderer.color;
                color.a = state ? 0f : 1f;
                renderer.color = color;
            }
        }

        private IEnumerator Animation(bool state)
        {
            if (state)
            {
                Color color = renderer.color;
                color.a = 1f;
                while (renderer.color.a > 0f)
                {
                    color.a -= Time.unscaledDeltaTime;
                    if (color.a < 0f) color.a = 0f;
                    renderer.color = color;
                    yield return null;
                }
            } else
            {
                Color color = renderer.color;
                color.a = 0f;
                while (renderer.color.a < 1f)
                {
                    color.a += Time.unscaledDeltaTime;
                    if (color.a > 1f) color.a = 1f;
                    renderer.color = color;
                    yield return null;
                }
            }
        }

        public InteractionSpriteRendererObject SetBoxColliderAppropriateSize()
        {
            SetBoxCollider(new Vector3(renderer.size.x, renderer.size.y, 1f));
            return this;
        }

        public InteractionSpriteRendererObject SetDefaultParameters()
        {
            ignoreSightedEvents = false;
            SetBoxColliderAppropriateSize();
            return this;
        }

        public InteractionSpriteRendererObject SetSprites(Sprite onSight, Sprite onUnsight)
        {
            spriteOnSight = onSight;
            spriteOnUnsight = onUnsight;

            renderer.sprite = spriteOnUnsight;

            return this;
        }

        public override void ClickableSighted(int player)
        {
            base.ClickableSighted(player);
            if (!ignoreSightedEvents)
            {
                renderer.sprite = spriteOnSight;
            }
        }

        public override void ClickableUnsighted(int player)
        {
            base.ClickableUnsighted(player);
            if (!ignoreSightedEvents)
            {
                renderer.sprite = spriteOnUnsight;
            }
        }

    }
}
