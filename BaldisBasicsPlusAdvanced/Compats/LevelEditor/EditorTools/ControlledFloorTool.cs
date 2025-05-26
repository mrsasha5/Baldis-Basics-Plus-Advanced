using BaldiLevelEditor;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools
{
    internal class ControlledFloorTool : FloorTool
    {
        private Sprite sprite;

        public override Sprite editorSprite => sprite;

        public ControlledFloorTool(string room) : base(room)
        {
        }

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }
}
