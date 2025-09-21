using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{

    public class GumDispenserTool : EditorTool
    {

        private string type;

        private string dispenserPre;

        private string buttonPre;

        private IntVector2? selectPos;

        private GumDispenserLocation notConnectedDispenser;

        public override string id => $"structure_{type}_{dispenserPre}_{buttonPre}";

        public GumDispenserTool(string type, string dispenserPre, string buttonPre, Sprite sprite)
        {
            this.type = type;
            this.dispenserPre = dispenserPre;
            this.buttonPre = buttonPre;
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
            ResetTool();
            return true;
        }

        public override void Exit()
        {
            ResetTool();
        }

        private void ResetTool()
        {
            if (notConnectedDispenser != null)
            {
                GumDispenserStructureLocation structLoc =
                (GumDispenserStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

                EditorController.Instance.RemoveVisual(notConnectedDispenser);
                structLoc.dispenserLocations.Remove(notConnectedDispenser);

                notConnectedDispenser = null;
            }
            selectPos = null;
        }

        public override bool MousePressed()
        {
            selectPos = EditorController.Instance.mouseGridPosition;

            EditorController.Instance.selector.SelectRotation(selectPos.Value, (Direction dir) => Place(selectPos.Value, dir));

            return false;
        }

        private void Place(IntVector2 position, Direction dir)
        {
            GumDispenserStructureLocation structLoc =
                (GumDispenserStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

            if (notConnectedDispenser == null)
            {
                notConnectedDispenser =
                    structLoc.CreateNewDispenser(
                        EditorController.Instance.levelData, dispenserPre, position, dir, false);

                if (notConnectedDispenser == null) return;

                EditorController.Instance.AddVisual(notConnectedDispenser);

                selectPos = null;

                return;
            }
            else
            {
                SimpleButtonLocation buttonLoc = structLoc.CreateNewButton(
                    EditorController.Instance.levelData, buttonPre, position, dir, false);

                if (buttonLoc == null) return;

                EditorController.Instance.AddVisual(buttonLoc);

                notConnectedDispenser = null;
            }

            EditorController.Instance.SwitchToTool(null);
        }

        public override bool MouseReleased()
        {
            return false;
        }
    }
}
