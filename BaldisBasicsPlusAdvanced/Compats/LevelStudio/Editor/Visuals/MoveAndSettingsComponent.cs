using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals
{
    internal class MoveAndSettingsComponent : MonoBehaviour, IEditorInteractable
    {

        public IEditorMovable location;

        public Vector3 offset = Vector3.up * 7f;

        public bool InteractableByTool(EditorTool tool)
        {
            return false;
        }

        public bool OnClicked()
        {
            EditorController.Instance.selector.SelectObject(location, MoveAxis.All, RotateAxis.Full);
            EditorController.Instance.selector.ShowSettingsSelect(transform.TransformPoint(offset), 
                ((IEditorSettingsable)location).SettingsClicked);

            return false;
        }

        public bool OnHeld()
        {
            throw new System.NotImplementedException();
        }

        public void OnReleased()
        {
            throw new System.NotImplementedException();
        }
    }
}
