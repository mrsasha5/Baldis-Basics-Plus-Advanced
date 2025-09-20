using PlusLevelStudio;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser
{
    public class GumDispenserLocation : SimpleLocation
    {

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioPlugin.Instance.genericStructureDisplays["adv_" + prefab];
        }

        public override bool ValidatePosition(EditorLevelData data, bool ignoreSelf)
        {
            if (!base.ValidatePosition(data, ignoreSelf))
            {
                return false;
            }

            if (!data.WallFree(position, direction, ignoreSelf))
            {
                return false;
            }

            return true;
        }

    }
}
