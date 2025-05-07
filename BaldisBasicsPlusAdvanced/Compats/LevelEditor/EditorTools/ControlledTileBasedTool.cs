using BaldiLevelEditor;
using PlusLevelFormat;
using PlusLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools
{
    internal class ControlledTileBasedTool : TileBasedTool
    {

        private Sprite sprite;

        public override Sprite editorSprite => sprite; 

        public ControlledTileBasedTool(string tile) : base(tile)
        {
        }

        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }
}
