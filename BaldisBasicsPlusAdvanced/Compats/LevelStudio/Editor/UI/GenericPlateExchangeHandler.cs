using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GenericPlate;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser;
using PlusLevelStudio.Editor;
using PlusLevelStudio.UI;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI
{
    internal class GenericPlateExchangeHandler : BaseEditorOverlayUIExchangeHandler
    {

        private TextMeshProUGUI uses;

        private TextMeshProUGUI cooldown;

        private TextMeshProUGUI unpressTime;

        private bool somethingChanged;

        private GenericPlateLocation loc;

        public void OnInitialized(GenericPlateLocation loc)
        {
            this.loc = loc;
        }

        public override void OnElementsCreated()
        {
            base.OnElementsCreated();
            Transform transform = base.transform.Find("UsesBox");
            if (transform != null)
            {
                uses = transform.GetComponent<TextMeshProUGUI>();
            }

            Transform transform2 = base.transform.Find("CooldownBox");
            if (transform2 != null)
            {
                cooldown = transform2.GetComponent<TextMeshProUGUI>();
            }

            Transform transform3 = base.transform.Find("UnpressTimeBox");
            if (transform3 != null)
            {
                unpressTime = transform3.GetComponent<TextMeshProUGUI>();
            }
        }

        public void Refresh()
        {
            if (uses != null)
            {
                if (loc.uses == 0) uses.text = "INF";
                else
                    uses.text = loc.uses.ToString();
            }

            if (cooldown != null)
            {
                if (loc.cooldown == 0) cooldown.text = "NO";
                else cooldown.text = loc.cooldown.ToString();
            }

            if (unpressTime != null)
            {
                unpressTime.text = loc.unpressTime.ToString();
                CheckIfFloatIsVisualized(unpressTime);
            }
        }

        public override bool OnExit()
        {
            if (somethingChanged)
            {
                EditorController.Instance.AddHeldUndo();
            }
            else
            {
                EditorController.Instance.CancelHeldUndo();
            }

            return base.OnExit();
        }

        public override void SendInteractionMessage(string message, object data)
        {
            if (message == "setUses")
            {
                if (int.TryParse((string)data, out int result))
                {
                    loc.uses = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setCooldown")
            {
                if (int.TryParse((string)data, out int result))
                {
                    loc.cooldown = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setUnpressTime")
            {
                if (float.TryParse((string)data, out float result))
                {
                    loc.unpressTime = result;
                    somethingChanged = true;
                }

                Refresh();
            }

            base.SendInteractionMessage(message, data);

        }

    }
}
