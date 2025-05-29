using BaldiLevelEditor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools
{
    internal class ControlledActivityTool : ActivityTool
    {
        private Sprite sprite;

        public override Sprite editorSprite => sprite;

        public ControlledActivityTool(string activity) : base(activity)
        {
        }

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }
}
