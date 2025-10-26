using System.Globalization;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyFacultyPlate;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Pulley;
using PlusLevelStudio.Editor;
using TMPro;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI
{
    internal class PulleyExchangeHandler : BaseEditorOverlayUIExchangeHandler
    {

        private TextMeshProUGUI uses;

        private TextMeshProUGUI points;

        private TextMeshProUGUI finalPoints;

        private TextMeshProUGUI maxDistance;

        private bool somethingChanged;

        private PulleyLocation loc;

        public void OnInitialized(PulleyLocation loc)
        {
            this.loc = loc;
        }

        public override void OnElementsCreated()
        {
            base.OnElementsCreated();
            
            uses = transform.Find("UsesBox")?.GetComponent<TextMeshProUGUI>();
            points = transform.Find("PointsBox")?.GetComponent<TextMeshProUGUI>();
            finalPoints = transform.Find("FinalPointsBox")?.GetComponent<TextMeshProUGUI>();
            maxDistance = transform.Find("MaxDistanceBox")?.GetComponent<TextMeshProUGUI>();
        }

        public void Refresh()
        {
            if (uses != null)
            {
                uses.text = loc.uses.ToString();
            }

            if (points != null)
            {
                points.text = loc.points.ToString();
            }

            if (finalPoints != null)
            {
                finalPoints.text = loc.finalPoints.ToString();
            }

            if (maxDistance != null)
            {
                maxDistance.text = loc.maxDistance.ToString();
                CheckIfFloatIsVisualized(maxDistance);
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
            else if (message == "setPoints")
            {
                if (ushort.TryParse((string)data, out ushort result))
                {
                    loc.points = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setFinalPoints")
            {
                if (ushort.TryParse((string)data, out ushort result))
                {
                    loc.finalPoints = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            if (message == "setMaxDistance")
            {
                if (float.TryParse((string)data, NumberStyles.Float, AdvancedCore.StandardCultureInfo, out float result))
                {
                    loc.maxDistance = result;
                    somethingChanged = true;
                }

                Refresh();
            }

            base.SendInteractionMessage(message, data);
        }

    }
}
