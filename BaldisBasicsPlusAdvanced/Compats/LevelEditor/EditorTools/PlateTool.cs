/*using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor.Visuals;
using PlusLevelFormat;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.EditorTools
{
    internal class PlateTool : ButtonTool
    {
        private Sprite sprite;

        private string _type;

        public override Sprite editorSprite => sprite;

        public PlateTool(string type) : base(type)
        {
            _type = type;
        }

        public void setSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public override void OnPlace(Direction dir)
        {
            EditorButtonPlacement wallLoca = new EditorButtonPlacement
            {
                position = selectedPosition.Value,
                direction = dir.ToData(),
                type = _type
            };

            if (!Singleton<PlusLevelEditor>.Instance.level.editorButtons.Contains(wallLoca))
            {
                Singleton<PlusLevelEditor>.Instance.level.editorButtons.Add(wallLoca);
            }

            PlateEditorVisual buttonEditorVisual = new GameObject
            {
                name = "ButtonRoot"
            }.AddComponent<PlateEditorVisual>();
            buttonEditorVisual.Setup(wallLoca);
            buttonEditorVisual.transform.position = Singleton<PlusLevelEditor>.Instance.ByteVectorToWorld(selectedPosition.Value);
            Singleton<PlusLevelEditor>.Instance.wallVisuals.Add(buttonEditorVisual);

            Singleton<PlusLevelEditor>.Instance.audMan.PlaySingle(BaldiLevelEditorPlugin.Instance.assetMan.Get<SoundObject>("Slap"));
        }
    }
}*/