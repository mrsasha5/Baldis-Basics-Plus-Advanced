using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GenericPlate;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    public class GenericPlateTool : EditorTool
    {

        private string type;

        private string prefab;

        private IntVector2? selectPos;

        public override string id => $"structure_{type}_{prefab}";

        public GenericPlateTool(string type, string prefab, Sprite sprite)
        {
            this.type = type;
            this.prefab = prefab;
            this.sprite = sprite;
        }

        public override void Begin()
        {
            
        }

        public override void Update()
        {
            if (!selectPos.HasValue)
            {
                EditorController.Instance.selector.SelectTile(EditorController.Instance.mouseGridPosition);
            }
        }

        public override bool Cancelled()
        {
            if (selectPos.HasValue)
            {
                selectPos = null;
                return false;
            }

            return true;
        }

        public override void Exit()
        {
            selectPos = null;
        }

        private void Place(Direction dir)
        {
            EditorController.Instance.HoldUndo();

            GenericPlateStructureLocation structLoc =
                (GenericPlateStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

            GenericPlateLocation loc = 
                structLoc.CreateNewChild(prefab, selectPos.Value, dir, disableChecks: false);

            if (loc != null)
            {
                EditorController.Instance.AddVisual(loc);

                EditorController.Instance.AddHeldUndo();
                EditorController.Instance.SwitchToTool(null);
            }
            else
            {
                EditorController.Instance.CancelHeldUndo();
            }
        }

        public override bool MousePressed()
        {
            if (EditorController.Instance.levelData
                .RoomIdFromPos(EditorController.Instance.mouseGridPosition, forEditor: true) == 0)
                return false;

            selectPos = EditorController.Instance.mouseGridPosition;

            EditorController.Instance.selector.SelectRotation(selectPos.Value, Place);

            return false;
        }

        public override bool MouseReleased()
        {
            return false;
        }
    }
}
