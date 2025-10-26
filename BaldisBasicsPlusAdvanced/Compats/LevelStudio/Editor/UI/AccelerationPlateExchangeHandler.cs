using System.Globalization;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.AccelerationPlate;
using PlusLevelStudio.Editor;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI
{
    internal class AccelerationPlateExchangeHandler : BaseEditorOverlayUIExchangeHandler
    {

        private TextMeshProUGUI uses;

        private TextMeshProUGUI cooldownTitle;

        private TextMeshProUGUI cooldown;

        private TextMeshProUGUI unpressTime;

        private TextMeshProUGUI initialSpeed;

        private TextMeshProUGUI acceleration;

        private TextMeshProUGUI showsUses;

        private TextMeshProUGUI showsCooldown;

        private bool somethingChanged;

        private AccelerationPlateLocation loc;

        public void OnInitialized(AccelerationPlateLocation loc)
        {
            this.loc = loc;
        }

        public override void OnElementsCreated()
        {
            base.OnElementsCreated();
            uses = transform?.Find("UsesBox")?.GetComponent<TextMeshProUGUI>();
            cooldown = transform?.Find("CooldownBox")?.GetComponent<TextMeshProUGUI>();
            unpressTime = transform?.Find("UnpressTimeBox")?.GetComponent<TextMeshProUGUI>();
            initialSpeed = transform?.Find("InitialSpeedBox")?.GetComponent<TextMeshProUGUI>();
            acceleration = transform?.Find("AccelerationBox")?.GetComponent<TextMeshProUGUI>();
            showsUses = transform?.Find("ShowsUses")?.GetComponent<TextMeshProUGUI>();
            showsCooldown = transform.Find("ShowsCooldown")?.GetComponent<TextMeshProUGUI>();
            cooldownTitle = transform?.Find("Cooldown")?.GetComponent<TextMeshProUGUI>();
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
                if (loc.cooldown >= 0f)
                    cooldownTitle.color = Color.white;
                else
                    cooldownTitle.color = Color.grey;
            }

            if (cooldown != null)
            {
                if (loc.cooldown < 0f) cooldown.text = "NO";
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

            if (initialSpeed != null)
            {
                initialSpeed.text = loc.initialSpeed.ToString();
                CheckIfFloatIsVisualized(initialSpeed);
            }

            if (acceleration != null)
            {
                acceleration.text = loc.acceleration.ToString();
                CheckIfFloatIsVisualized(acceleration);
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
                if (float.TryParse((string)data, NumberStyles.Float, AdvancedCore.StandardCultureInfo, out float result))
                {
                    loc.cooldown = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setUnpressTime")
            {
                if (float.TryParse((string)data, NumberStyles.Float, AdvancedCore.StandardCultureInfo, out float result))
                {
                    loc.unpressTime = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setInitSpeed")
            {
                if (float.TryParse((string)data, NumberStyles.Float, AdvancedCore.StandardCultureInfo, out float result))
                {
                    loc.initialSpeed = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setAcceleration")
            {
                if (float.TryParse((string)data, NumberStyles.Float, AdvancedCore.StandardCultureInfo, out float result))
                {
                    loc.acceleration = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "toggleUsesVisual")
            {
                loc.showsUses = !loc.showsUses;

                Refresh();
            }
            else if (message == "toggleCooldownOverriding")
            {
                if (loc.cooldown >= 0f) loc.cooldown = -1f;
                else loc.cooldown = 0f;

                Refresh();
            }
            else if (message == "toggleCooldownVisual")
            {
                loc.showsCooldown = !loc.showsCooldown;

                Refresh();
            }
            

            base.SendInteractionMessage(message, data);

        }

    }
}
