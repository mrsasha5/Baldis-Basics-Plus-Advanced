using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.AccelerationPlate;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals
{
    internal class AccelerationPlateArrow : MonoBehaviour, IEditorInteractable
    {

        public AccelerationPlateLocation location;

        public SpriteRenderer renderer;

        public byte index;

        public bool directionActive;

        public bool InteractableByTool(EditorTool tool)
        {
            return false;
        }

        public bool OnClicked()
        {
            if (index == 0) return false;

            EditorController.Instance.AddUndo();

            directionActive = !directionActive;
            UpdateVisual();

            return false;
        }

        public void UpdateVisual()
        {
            if (directionActive) renderer.color = Color.white;
            else renderer.color = new Color(1f, 1f, 1f, 0.25f);
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
