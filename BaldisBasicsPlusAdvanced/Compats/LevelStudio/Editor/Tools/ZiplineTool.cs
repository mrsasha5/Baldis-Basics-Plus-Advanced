using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    public class ZiplineTool : EditorTool
    {

        private string type;

        private string hangerPre;

        public override string id => "structure_" + type + "_" + hangerPre;

        private ZiplinePointLocation notConnectedPoint;


        public ZiplineTool(string type, string hangerPre, Sprite sprite)
        {
            this.type = type;
            this.hangerPre = hangerPre;
            this.sprite = sprite;
        }

        public override void Begin()
        {
            
        }

        public override bool Cancelled()
        {
            if (notConnectedPoint != null)
            {
                EditorController.Instance.RemoveVisual(notConnectedPoint);
                notConnectedPoint = null;
            }
            return true;
        }

        public override void Exit()
        {
            notConnectedPoint = null;
        }

        public override void Update()
        {
            EditorController.Instance.selector.SelectTile(EditorController.Instance.mouseGridPosition);
        }

        public override bool MousePressed()
        {
            IntVector2 mousePos = EditorController.Instance.mouseGridPosition;
            PlusStudioLevelFormat.Cell cell = EditorController.Instance.levelData.GetCellSafe(mousePos);

            if (cell == null) return false;

            if (notConnectedPoint == null)
            {
                EditorController.Instance.AddUndo();
            }

            ZiplineStructureLocation structLoc = 
                (ZiplineStructureLocation)EditorController.Instance.AddOrGetStructureToData("adv_zipline", onlyOne: true);

            ZiplinePointLocation newPointLoc = 
                structLoc.CreateNewChild(EditorController.Instance.levelData, hangerPre, cell.position.ToInt());

            if (newPointLoc == null) return false;

            if (notConnectedPoint != null)
            {
                notConnectedPoint.ConnectTo(newPointLoc);
                notConnectedPoint = null;
            }
            else
            {
                notConnectedPoint = newPointLoc;
            }

            EditorController.Instance.AddVisual(newPointLoc);

            return structLoc.locations.Count % 2 == 0;
        }

        public override bool MouseReleased()
        {
            return false;
        }

    }
}
