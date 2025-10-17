using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser;
using PlusLevelStudio.Editor;
using PlusLevelStudio.UI;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI
{

    internal class GumDispenserExchangeHandler : BaseEditorOverlayUIExchangeHandler
    {
        private TextMeshProUGUI uses;

        private TextMeshProUGUI cooldown;

        private bool somethingChanged;

        private GumDispenserLocation loc;

        public void OnInitialized(GumDispenserLocation loc)
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
        }

        public void Refresh()
        {
            if (uses != null)
            {
                uses.text = loc.uses.ToString();
            }

            if (cooldown != null)
            {
                cooldown.text = loc.cooldown.ToString();
                CheckIfFloatIsVisualized(cooldown);
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
                if (ushort.TryParse((string)data, out ushort result))
                {
                    loc.uses = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setCooldown")
            {
                if (float.TryParse((string)data, out float result))
                {
                    loc.cooldown = result;
                    somethingChanged = true;
                }

                Refresh();
            }

            base.SendInteractionMessage(message, data);
        }
    }
}
