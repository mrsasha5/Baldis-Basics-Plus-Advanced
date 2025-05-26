using BaldiLevelEditor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools
{
    internal class ControlledObjectTool : ObjectTool
    {
        private Sprite sprite;

        public override Sprite editorSprite => sprite; 

        public ControlledObjectTool(string obj) : base(obj)
        {
        }

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }
}
