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

        private TextMeshProUGUI cooldownTitle;

        private TextMeshProUGUI cooldown;

        private TextMeshProUGUI unpressTime;

        private TextMeshProUGUI showsUses;

        private TextMeshProUGUI showsCooldown;

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

            Transform transform4 = base.transform.Find("ShowsUses");
            if (transform4 != null)
            {
                showsUses = transform4.GetComponent<TextMeshProUGUI>();
            }

            Transform transform5 = base.transform.Find("ShowsCooldown");
            if (transform5 != null)
            {
                showsCooldown = transform5.GetComponent<TextMeshProUGUI>();
            }

            cooldownTitle = base.transform.Find("Cooldown").GetComponent<TextMeshProUGUI>();
        }

        public void Refresh()
        {
            if (uses != null)
            {
                if (loc.uses == 0) 
                    uses.text = "INF";
                else
                    uses.text = loc.uses.ToString();
            }

            if (cooldownTitle != null)
            {
                if (loc.cooldownOverridingAllowed)
                    cooldownTitle.color = Color.white;
                else
                    cooldownTitle.color = Color.grey;
            }

            if (cooldown != null)
            {
                if (!loc.cooldownOverridingAllowed) cooldown.text = "NO";
                else
                {
                    cooldown.text = loc.cooldown.ToString();
                    CheckIfFloatIsVisualized(cooldown);
                }
            }

            if (unpressTime != null)
            {
                unpressTime.text = loc.unpressTime.ToString();
                CheckIfFloatIsVisualized(unpressTime);
            }

            if (showsUses != null)
            {
                if (loc.showsUses)
                {
                    showsUses.color = Color.green;
                }
                else
                {
                    showsUses.color = Color.red;
                }
            }

            if (showsCooldown != null)
            {
                if (loc.showsCooldown)
                {
                    showsCooldown.color = Color.green;
                }
                else
                {
                    showsCooldown.color = Color.red;
                }
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
                if (float.TryParse((string)data, out var result))
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
            else if (message == "toggleUsesVisual")
            {
                loc.showsUses = !loc.showsUses;

                Refresh();
            }
            else if (message == "toggleCooldownVisual")
            {
                loc.showsCooldown = !loc.showsCooldown;

                Refresh();
            }
            else if (message == "toggleCooldownOverriding")
            {
                loc.cooldownOverridingAllowed = !loc.cooldownOverridingAllowed;

                Refresh();
            }

            base.SendInteractionMessage(message, data);

        }

    }
}
