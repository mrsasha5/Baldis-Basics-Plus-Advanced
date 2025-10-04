using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline;
using PlusLevelStudio.Editor;
using PlusLevelStudio.UI;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI
{
    public class ZiplineExchangeHandler : EditorOverlayUIExchangeHandler
    {
        private TextMeshProUGUI uses;

        private TextMeshProUGUI distanceToBreak;

        private bool somethingChanged;

        private ZiplinePointLocation loc;

        public void OnInitialized(ZiplinePointLocation loc)
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

            Transform transform2 = base.transform.Find("DistanceBox");
            if (transform2 != null)
            {
                distanceToBreak = transform2.GetComponent<TextMeshProUGUI>();
            }
        }

        public void Refresh()
        {
            if (uses != null)
            {
                if (loc.uses == 0) uses.text = "INF";
                else uses.text = loc.uses.ToString();
            }

            if (distanceToBreak != null)
            {
                distanceToBreak.text = loc.percentageDistanceToBreak.ToString();
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
            else if (message == "setDistanceToBreak")
            {
                if (ushort.TryParse((string)data, out ushort result))
                {
                    loc.percentageDistanceToBreak = (ushort)Mathf.Clamp((int)result, 0, 100);
                    somethingChanged = true;
                }

                Refresh();
            }

            base.SendInteractionMessage(message, data);
        }
    }
}