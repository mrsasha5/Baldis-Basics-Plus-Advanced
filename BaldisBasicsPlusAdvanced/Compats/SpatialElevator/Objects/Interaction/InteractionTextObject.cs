using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects.Interaction
{
    public class InteractionTextObject : BaseInteractionObject<InteractionTextObject>
    {

        public bool ignoreSightedEvents = true;

        public FontStyles styleOnSight;

        public FontStyles styleOnUnsight;

        protected TextMeshPro tmp;

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

        public InteractionTextObject SetClassicFontStyleOnSight()
        {
            ignoreSightedEvents = false;
            styleOnSight = FontStyles.Underline;
            styleOnUnsight = FontStyles.Normal;

            tmp.fontStyle = styleOnUnsight;

            return this;
        }

        public InteractionTextObject SetDefaultParameters()
        {
            SetClassicFontStyleOnSight();
            SetBoxCollider(new Vector3(tmp.rectTransform.sizeDelta.x, tmp.rectTransform.sizeDelta.y, 1f));
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
