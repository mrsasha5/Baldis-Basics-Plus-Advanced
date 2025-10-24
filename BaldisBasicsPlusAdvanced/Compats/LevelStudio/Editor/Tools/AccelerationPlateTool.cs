using System;
using System.Collections.Generic;
using System.Text;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.AccelerationPlate;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.KitchenStove;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    internal class AccelerationPlateTool : EditorTool
    {
        private string type;

        private string prefab;

        private AccelerationPlateLocation notConnectedPlate;

        private bool buttonless;

        private IntVector2? selectPos;

        public override string id => $"structure_{type}_{prefab}" + (buttonless ? "_buttonless" : "");

        public AccelerationPlateTool(string type, string prefab, Sprite sprite, bool buttonless)
        {
            this.type = type;
            this.prefab = prefab;
            this.sprite = sprite;
            this.buttonless = buttonless;
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
                bool cancelTool = notConnectedPlate == null;
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
            if (notConnectedPlate != null)
            {
                AccelerationPlateStructureLocation structLoc =
                (AccelerationPlateStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

                EditorController.Instance.RemoveVisual(notConnectedPlate);
                structLoc.plates.Remove(notConnectedPlate);

                notConnectedPlate = null;
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
            AccelerationPlateStructureLocation structLoc =
                (AccelerationPlateStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

            if (notConnectedPlate == null)
            {
                EditorController.Instance.HoldUndo();

                notConnectedPlate =
                    structLoc.CreateNewPlate(EditorController.Instance.levelData, prefab, position, dir);

                if (notConnectedPlate == null) return;

                notConnectedPlate.LoadDefaults(type);

                EditorController.Instance.AddVisual(notConnectedPlate);

                selectPos = null;

                if (buttonless)
                {
                    EditorController.Instance.AddHeldUndo();
                    EditorController.Instance.SwitchToTool(null);
                    notConnectedPlate = null;
                }

                return;
            }
            else
            {
                SimpleButtonLocation buttonLoc = 
                    structLoc.CreateNewButton(EditorController.Instance.levelData, notConnectedPlate, position, dir);

                if (buttonLoc == null)
                    return;

                EditorController.Instance.AddVisual(buttonLoc);

                notConnectedPlate = null;

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
