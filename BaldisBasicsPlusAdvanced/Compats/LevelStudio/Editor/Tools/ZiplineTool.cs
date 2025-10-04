using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline;
using BaldisBasicsPlusAdvanced.Game.Builders;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    public class ZiplineTool : EditorTool
    {

        private string type;

        private string hangerPre;

        private static Vector3[] _vectors = new Vector3[2];

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
            if (notConnectedPoint != null)
            {
                OnResetTool();
                return false;
            }
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
            if (notConnectedPoint != null && notConnectedPoint.renderer != null)
            {
                _vectors[0] = new Vector3(notConnectedPoint.position.x * 10 + 5, 9f, notConnectedPoint.position.z * 10 + 5);
                _vectors[1] = new Vector3(EditorController.Instance.mouseGridPosition.x * 10 + 5, 9f, 
                    EditorController.Instance.mouseGridPosition.z * 10 + 5);
                notConnectedPoint.renderer.SetPositions(_vectors);
            }
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
