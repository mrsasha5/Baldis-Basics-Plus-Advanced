using BaldiLevelEditor;
using PlusLevelFormat;
using System.Numerics;
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

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }
}
