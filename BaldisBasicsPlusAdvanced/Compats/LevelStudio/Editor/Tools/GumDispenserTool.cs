using System;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser;
using PlusLevelStudio.Editor;
using PlusLevelStudio.Editor.Tools;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{

#warning Fix all issues with dispensers and tool
    public class GumDispenserTool : PlaceAndRotateTool
    {

        private string type;

        private string dispenserPre;

        private string buttonPre;

        private GumDispenserLocation notConnectedDispenser;

        public override string id => $"structure_{type}_{dispenserPre}_{buttonPre}";

        public GumDispenserTool(string type, string dispenserPre, string buttonPre, Sprite sprite)
        {
            this.type = type;
            this.dispenserPre = dispenserPre;
            this.buttonPre = buttonPre;
            this.sprite = sprite;
        }

        public override bool Cancelled()
        {
            if (base.Cancelled())
            {
                ResetTool();
                return true;
            }
            
            return false;
        }

        public override void Exit()
        {
            base.Exit();
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
        }

        public override bool ValidLocation(IntVector2 position)
        {
            return base.ValidLocation(position);
        }

        protected override bool TryPlace(IntVector2 position, Direction dir)
        {
            GumDispenserStructureLocation structLoc =
                (GumDispenserStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

            if (notConnectedDispenser == null)
            {
                notConnectedDispenser =
                    structLoc.CreateNewDispenser(
                        EditorController.Instance.levelData, dispenserPre, position, dir, false);

                if (notConnectedDispenser == null) return false;

                EditorController.Instance.AddVisual(notConnectedDispenser);

                return false;
            }
            else
            {
                SimpleButtonLocation buttonLoc = structLoc.CreateNewButton(
                    EditorController.Instance.levelData, buttonPre, position, dir, false);

                if (buttonLoc == null) return false;

                EditorController.Instance.AddVisual(buttonLoc);

                notConnectedDispenser = null;
            }

            return true;
        }


    }
}
