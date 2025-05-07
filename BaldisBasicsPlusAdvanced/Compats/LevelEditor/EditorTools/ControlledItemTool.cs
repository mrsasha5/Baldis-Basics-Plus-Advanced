using BaldiLevelEditor;
using PlusLevelFormat;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools
{
    internal class ControlledItemTool : ItemTool
    {
        public override Sprite editorSprite => sprite;

        private Sprite sprite;

        public ControlledItemTool(string obj) : base(obj)
        {
        }

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }
}
