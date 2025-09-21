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

        public override string id => $"structure_{type}_{hangerPre}";

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
            OnResetTool();
            return true;
        }

        public override void Exit()
        {
            OnResetTool();
        }

        private void OnResetTool()
        {
            if (notConnectedPoint != null)
            {
                EditorController.Instance.RemoveVisual(notConnectedPoint);
                notConnectedPoint = null;
            }
            EditorController.Instance.CancelHeldUndo();
        }

        public override void Update()
        {
            EditorController.Instance.selector.SelectTile(EditorController.Instance.mouseGridPosition);
        }

        public override bool MousePressed()
        {
            ZiplineStructureLocation structLoc = 
                (ZiplineStructureLocation)EditorController.Instance.AddOrGetStructureToData("adv_zipline", onlyOne: true);

            if (notConnectedPoint == null)
            {
                EditorController.Instance.HoldUndo();
            }

            ZiplinePointLocation newPointLoc = 
                structLoc.CreateNewChild(EditorController.Instance.levelData, hangerPre, EditorController.Instance.mouseGridPosition);

            if (newPointLoc == null) return false;

            if (notConnectedPoint != null)
            {
                notConnectedPoint.ConnectTo(newPointLoc);
                notConnectedPoint = null;

                EditorController.Instance.AddHeldUndo();

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
