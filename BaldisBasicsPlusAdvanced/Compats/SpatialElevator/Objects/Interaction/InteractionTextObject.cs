using System.Collections;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects.Interaction
{
    internal class InteractionTextObject : BaseInteractionObject<InteractionTextObject>
    {

        public bool ignoreSightedEvents = true;

        public FontStyles styleOnSight;

        public FontStyles styleOnUnsight;

        protected TextMeshPro tmp;

        public TextMeshPro Tmp => tmp;

        public InteractionTextObject Assign()
        {
            tmp = GetComponent<TextMeshPro>();
            return this;
        }

        public InteractionTextObject Assign(TextMeshPro tmpText)
        {
            tmp = tmpText;
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
                Color color = tmp.color;
                color.a = state ? 0f : 1f;
                tmp.color = color;
            }
        }

        private IEnumerator Animation(bool state)
        {
            if (state)
            {
                Color color = tmp.color;
                color.a = 1f;
                while (tmp.color.a > 0f)
                {
                    color.a -= Time.unscaledDeltaTime;
                    if (color.a < 0f) color.a = 0f;
                    tmp.color = color;
                    yield return null;
                }
            }
            else
            {
                Color color = tmp.color;
                color.a = 0f;
                while (tmp.color.a < 1f)
                {
                    color.a += Time.unscaledDeltaTime;
                    if (color.a > 1f) color.a = 1f;
                    tmp.color = color;
                    yield return null;
                }
            }
        }

        public InteractionTextObject SetClassicFontStyleOnSight()
        {
            ignoreSightedEvents = false;
            styleOnSight = FontStyles.Underline;
            styleOnUnsight = FontStyles.Normal;

            tmp.fontStyle = styleOnUnsight;

            return this;
        }

        public InteractionTextObject SetBoxColliderAppropriateSize()
        {
            SetBoxCollider(new Vector3(tmp.rectTransform.sizeDelta.x, tmp.rectTransform.sizeDelta.y, 1f));
            return this;
        }

        public InteractionTextObject SetDefaultParameters()
        {
            SetClassicFontStyleOnSight();
            SetBoxColliderAppropriateSize();
            return this;
        }

        public override void ClickableSighted(int player)
        {
            base.ClickableSighted(player);
            if (!ignoreSightedEvents)
                tmp.fontStyle = styleOnSight;
        }

        public override void ClickableUnsighted(int player)
        {
            base.ClickableUnsighted(player);
            if (!ignoreSightedEvents)
                tmp.fontStyle = styleOnUnsight;
        }
    }
}
