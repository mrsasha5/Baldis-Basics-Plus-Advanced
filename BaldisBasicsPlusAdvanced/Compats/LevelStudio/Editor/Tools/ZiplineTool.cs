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

        private ZiplineStructureLocation structLoc;


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
            return true;
        }

        public override void Exit()
        {
            
        }

        public override void Update()
        {
            EditorController.Instance.selector.SelectTile(EditorController.Instance.mouseGridPosition);
            //for (int i = 0; i < structLoc.locations.Count; i += 2)
            //{
            //location.locations[]
            //}
        }

        public override bool MousePressed()
        {
            IntVector2 mousePos = EditorController.Instance.mouseGridPosition;
            PlusStudioLevelFormat.Cell cell = EditorController.Instance.levelData.GetCellSafe(mousePos);

            if (cell == null) return false;

            if (structLoc == null || structLoc.locations.Count % 2 == 0)
            {
                EditorController.Instance.AddUndo();
            }

            structLoc = (ZiplineStructureLocation)EditorController.Instance.AddOrGetStructureToData("adv_zipline", onlyOne: true);

            ZiplinePointLocation pillarLoc = structLoc.CreateNewChild(hangerPre, cell.position.ToInt());

            EditorController.Instance.AddVisual(pillarLoc);

            return structLoc.locations.Count % 2 == 0;
        }

        public override bool MouseReleased()
        {
            return false;
        }

    }
}
