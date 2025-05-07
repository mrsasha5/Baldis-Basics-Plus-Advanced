using BaldiLevelEditor;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools
{
    internal class ControlledRotatableTool : RotateAndPlacePrefab
    {
        private Sprite sprite;

        public override Sprite editorSprite => sprite;

        public ControlledRotatableTool(string obj) : base(obj)
        {
        }

        public void setSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }

    }
}
