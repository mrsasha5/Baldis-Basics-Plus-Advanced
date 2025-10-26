using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Pulley;
using PlusLevelStudio.Editor;
using PlusLevelStudio.Editor.Tools;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    internal class PulleyTool : PlaceAndRotateTool
    {

        private string type;

        private string prefab;

        public override string id => $"structure_{type}_{prefab}";

        public PulleyTool(string type, string prefab, Sprite sprite)
        {
            this.type = type;
            this.prefab = prefab;
            this.sprite = sprite;
        }

        protected override bool TryPlace(IntVector2 position, Direction dir)
        {
            EditorController.Instance.AddUndo();

            PulleyStructureLocation structLoc = 
                (PulleyStructureLocation)EditorController.Instance.AddOrGetStructureToData(type, onlyOne: true);

            PulleyLocation location = structLoc.CreateNewChild(EditorController.Instance.levelData, prefab, position, dir);

            if (location != null)
            {
                location.LoadDefaults(type);
                EditorController.Instance.AddVisual(location);
                return true;
            }

            return false;
        }

    }
}
