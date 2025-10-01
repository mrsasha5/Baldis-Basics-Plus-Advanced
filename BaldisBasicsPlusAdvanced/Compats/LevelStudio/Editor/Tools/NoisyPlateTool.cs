using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyFacultyPlate;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    public class NoisyPlateTool : EditorTool
    {

        public string type;

        public string platePre;

        public override string id => $"structure_{type}_{platePre}";

        public NoisyPlateTool(string type, string platePre, Sprite sprite)
        {
            this.type = type;
            this.platePre = platePre;
            this.sprite = sprite;
        }

        public override void Begin()
        {
            
        }

        public override void Update()
        {

        }

        public override bool Cancelled()
        {
            return true;
        }

        public override void Exit()
        {
            
        }

        public override bool MousePressed()
        {
            EditorRoom room = 
                EditorController.Instance.levelData.RoomFromPos(EditorController.Instance.mouseGridPosition, forEditor: true);

            EditorController.Instance.HoldUndo();

            NoisyPlateStructureLocation structLoc = 
                (NoisyPlateStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

            if (room == null)
            {
                EditorController.Instance.CancelHeldUndo();
                return false;
            }

            NoisyPlateRoomLocation loc = structLoc.CreateAndAddRoom(platePre, room);

            if (loc == null)
            {
                EditorController.Instance.CancelHeldUndo();
                return false;
            }

            loc.LoadDefaultParameters();

            EditorController.Instance.AddHeldUndo();

            EditorController.Instance.UpdateVisual(structLoc);

            return true;
        }

        public override bool MouseReleased()
        {
            return true;
        }

    }
}
