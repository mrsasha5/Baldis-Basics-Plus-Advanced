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

        public bool InteractableByTool(EditorTool tool)
        {
            return true;
        }

        public bool OnClicked()
        {
            location.directionsCount = (byte)(index + 1);
            location.UpdateArrows();

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
