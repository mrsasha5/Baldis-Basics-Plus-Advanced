using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects.Interaction
{
    public class InteractionSpriteRendererObject : BaseInteractionObject<InteractionSpriteRendererObject>
    {

        public bool ignoreSightedEvents = true;

        public Sprite spriteOnSight;

        public Sprite spriteOnUnsight;

        protected SpriteRenderer renderer;

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

        public InteractionSpriteRendererObject SetDefaultParameters()
        {
            ignoreSightedEvents = false;
            SetBoxCollider(new Vector3(renderer.size.x, renderer.size.y, 1f));
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
