using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyFacultyPlate;
using PlusLevelStudio.Editor;
using PlusLevelStudio.UI;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI
{
    public class NoisyPlateExchangeHandler : EditorOverlayUIExchangeHandler
    {
        private TextMeshProUGUI cooldown;

        private TextMeshProUGUI uses;

        private TextMeshProUGUI generosity;

        private TextMeshProUGUI pointsPerAlarm;

        private bool somethingChanged;

        private NoisyPlateRoomLocation loc;

        public void OnInitialized(NoisyPlateRoomLocation loc)
        {
            this.loc = loc;
        }

        public override void OnElementsCreated()
        {
            base.OnElementsCreated();

            cooldown = transform.Find("CooldownBox")?.GetComponent<TextMeshProUGUI>();
            uses = transform.Find("UsesBox")?.GetComponent<TextMeshProUGUI>();
            generosity = transform.Find("GenerosityBox")?.GetComponent<TextMeshProUGUI>();
            pointsPerAlarm = transform.Find("PointsPerAlarmBox")?.GetComponent<TextMeshProUGUI>();
        }

        public void Refresh()
        {
            if (cooldown != null)
            {
                if (loc.cooldown <= 0)
                    cooldown.text = "NO";
                else
                    cooldown.text = loc.cooldown.ToString();
            }

            if (uses != null)
            {
                if (loc.uses <= 0)
                    uses.text = "NO";
                else
                    uses.text = loc.uses.ToString();
            }
            
            if (generosity != null)
            {
                generosity.text = loc.generosity.ToString();
            }

            if (pointsPerAlarm != null)
            {
                pointsPerAlarm.text = loc.pointsPerAlarm.ToString();
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
            if (message == "setCooldown")
            {
                if (int.TryParse((string)data, out int result))
                {
                    loc.cooldown = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setUses")
            {
                if (int.TryParse((string)data, out int result))
                {
                    loc.uses = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setGenerosity")
            {
                if (int.TryParse((string)data, out int result))
                {
                    loc.generosity = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setPointsPerAlarm")
            {
                if (int.TryParse((string)data, out int result))
                {
                    loc.pointsPerAlarm = result;
                    somethingChanged = true;
                }

                Refresh();
            }

            base.SendInteractionMessage(message, data);
        }
    }
}
