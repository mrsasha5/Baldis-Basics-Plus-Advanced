using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.KitchenStove;
using PlusLevelStudio.Editor;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI
{

    internal class KitchenStoveExchangeHandler : BaseEditorOverlayUIExchangeHandler
    {
        private TextMeshProUGUI uses;

        private TextMeshProUGUI cooldown;

        private TextMeshProUGUI cookingTime;

        private TextMeshProUGUI coolingTime;

        private TextMeshProUGUI showsUses;

        private TextMeshProUGUI showsCooldown;

        private TextMeshProUGUI cooldownTitle;

        private bool somethingChanged;

        private KitchenStoveLocation loc;

        public void OnInitialized(KitchenStoveLocation loc)
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

            Transform transform3 = base.transform.Find("CookingTimeBox");
            if (transform3 != null)
            {
                cookingTime = transform3.GetComponent<TextMeshProUGUI>();
            }

            Transform transform4 = base.transform.Find("CoolingTimeBox");
            if (transform4 != null)
            {
                coolingTime = transform4.GetComponent<TextMeshProUGUI>();
            }

            Transform transform5 = base.transform.Find("ShowsUses");
            if (transform5 != null)
            {
                showsUses = transform5.GetComponent<TextMeshProUGUI>();
            }

            Transform transform6 = base.transform.Find("ShowsCooldown");
            if (transform6 != null)
            {
                showsCooldown = transform6.GetComponent<TextMeshProUGUI>();
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
                if (loc.cooldownOverridden)
                    cooldownTitle.color = Color.white;
                else
                    cooldownTitle.color = Color.grey;
            }

            if (cooldown != null)
            {
                if (!loc.cooldownOverridden) cooldown.text = "NO";
                else
                {
                    cooldown.text = loc.cooldown.ToString();
                    CheckIfFloatIsVisualized(cooldown);
                }
            }

            if (cookingTime != null)
            {
                cookingTime.text = loc.cookingTime.ToString();
                CheckIfFloatIsVisualized(cookingTime);
            }

            if (coolingTime != null)
            {
                coolingTime.text = loc.coolingTime.ToString();
                CheckIfFloatIsVisualized(coolingTime);
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
            else if (message == "setCookingTime")
            {
                if (float.TryParse((string)data, out float result))
                {
                    loc.cookingTime = result;
                    somethingChanged = true;
                }

                Refresh();
            }
            else if (message == "setCoolingTime")
            {
                if (float.TryParse((string)data, out float result))
                {
                    loc.coolingTime = result;
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
                loc.cooldownOverridden = !loc.cooldownOverridden;

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
