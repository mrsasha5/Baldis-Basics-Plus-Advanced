using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyFacultyPlate;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    public class NoisyPlateTool : EditorTool
    {

        public string type;

        public string platePre;

        private EditorRoom foundRoom;

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
            EditorRoom editorRoom = foundRoom;
            foundRoom = EditorController.Instance.levelData.RoomFromPos(EditorController.Instance.mouseGridPosition, forEditor: true);
            StructureLocation structureData = EditorController.Instance.GetStructureData(type);
            bool flag = true;
            if (structureData != null)
            {
                flag = ((NoisyPlateStructureLocation)structureData).infectedRooms
                    .Find((NoisyPlateRoomLocation x) => x.room == foundRoom) == null;
            }

            if (editorRoom != foundRoom)
            {
                if (editorRoom != null)
                {
                    EditorController.Instance.HighlightCells(EditorController.Instance.levelData.GetCellsOwnedByRoom(editorRoom), "none");
                }

                EditorController.Instance.HighlightCells(EditorController.Instance.levelData.GetCellsOwnedByRoom(foundRoom), 
                    flag ? "yellow" : "red");
            }
        }

        public override bool Cancelled()
        {
            foundRoom = null;
            return true;
        }

        public override void Exit()
        {
            foundRoom = null;
        }

        public override bool MousePressed()
        {
            foundRoom = 
                EditorController.Instance.levelData.RoomFromPos(EditorController.Instance.mouseGridPosition, forEditor: true);

            EditorController.Instance.HoldUndo();

            NoisyPlateStructureLocation structLoc = 
                (NoisyPlateStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

            if (foundRoom == null)
            {
                EditorController.Instance.CancelHeldUndo();
                return false;
            }

            NoisyPlateRoomLocation loc = structLoc.CreateAndAddRoom(platePre, foundRoom);

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
            return false;
        }

    }
}
