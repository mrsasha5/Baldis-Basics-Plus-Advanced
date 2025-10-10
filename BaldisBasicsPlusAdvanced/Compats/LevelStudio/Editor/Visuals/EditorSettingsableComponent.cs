using System;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals
{
    internal class EditorSettingsableComponent : MonoBehaviour, IEditorSettingsable
    {

        private Action onSettingsClicked;

        public Action OnSettingsClicked
        {
            set
            {
                onSettingsClicked = value;
                GetComponent<SettingsComponent>().activateSettingsOn = this;
            }
        }

        public void SettingsClicked()
        {
            onSettingsClicked?.Invoke();
        }
    }
}
