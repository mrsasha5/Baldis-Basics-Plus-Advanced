using BaldiLevelEditor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools
{
    internal class ControlledNpcTool : NpcTool
    {
        public override Sprite editorSprite => sprite;

        private Sprite sprite;

        public ControlledNpcTool(string obj) : base(obj)
        {
        }

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }

    }
}
