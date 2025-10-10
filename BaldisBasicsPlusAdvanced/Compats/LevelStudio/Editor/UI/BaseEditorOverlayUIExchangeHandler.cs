using PlusLevelStudio.UI;
using TMPro;
using UnityEngine.Events;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI
{
    internal class BaseEditorOverlayUIExchangeHandler : EditorOverlayUIExchangeHandler
    {

        /*public override void OnElementsCreated()
        {
            base.OnElementsCreated();

            foreach (EditorTextBox button in GetComponentsInChildren<EditorTextBox>())
            {
                if (button.OnPress == null) button.OnPress = new UnityEvent();
                button.OnPress.AddListener(() => OnPressAction(button));
            }
        }*/

        protected void CheckIfFloatIsVisualized(TMP_Text tmp)
        {
            if (!tmp.text.Contains(","))
                tmp.text += ",0";
        }

        /*protected virtual void OnPressAction(EditorTextBox button)
        {
            button.text.text = "";
        }*/

    }
}
