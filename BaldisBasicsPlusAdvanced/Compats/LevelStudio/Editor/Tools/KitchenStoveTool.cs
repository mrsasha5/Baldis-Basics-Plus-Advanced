using System;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.KitchenStove;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    internal class KitchenStoveTool : EditorTool
    {

        private string type;

        private string prefab;

        private KitchenStoveLocation notConnectedStove;

        private IntVector2? selectPos;

        public override string id => $"structure_{type}_{prefab}";

        public KitchenStoveTool(string type, string prefab, Sprite sprite)
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
            if (selectPos == null)
            {
                bool cancelTool = notConnectedStove == null;
                ResetTool();
                return cancelTool;
            }
            else
            {
                selectPos = null;
                return false;
            }
        }

        public override void Exit()
        {
            ResetTool();
        }

        private void ResetTool()
        {
            if (notConnectedStove != null)
            {
                KitchenStoveStructureLocation structLoc =
                (KitchenStoveStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

                EditorController.Instance.RemoveVisual(notConnectedStove);
                structLoc.stoves.Remove(notConnectedStove);

                notConnectedStove = null;
            }
            selectPos = null;
            EditorController.Instance.CancelHeldUndo();
        }

        public override bool MousePressed()
        {
            if (EditorController.Instance.levelData
                .RoomIdFromPos(EditorController.Instance.mouseGridPosition, forEditor: true) == 0)
                return false;

            selectPos = EditorController.Instance.mouseGridPosition;

            EditorController.Instance.selector.SelectRotation(selectPos.Value, (Direction dir) => Place(selectPos.Value, dir));

            return false;
        }

        private void Place(IntVector2 position, Direction dir)
        {
            KitchenStoveStructureLocation structLoc =
                (KitchenStoveStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

            if (notConnectedStove == null)
            {
                EditorController.Instance.HoldUndo();

                notConnectedStove =
                    structLoc.CreateNewStove(prefab, position, dir);

                if (notConnectedStove == null) return;

                notConnectedStove.LoadDefaults();

                EditorController.Instance.AddVisual(notConnectedStove);

                selectPos = null;

                return;
            }
            else
            {
                SimpleButtonLocation buttonLoc = structLoc.CreateNewButton(position, dir);

                if (buttonLoc == null)
                    return;

                EditorController.Instance.AddVisual(buttonLoc);

                notConnectedStove = null;

                selectPos = null;
            }

            EditorController.Instance.AddHeldUndo();
            EditorController.Instance.SwitchToTool(null);
        }

        public override bool MouseReleased()
        {
            return false;
        }

    }
}
