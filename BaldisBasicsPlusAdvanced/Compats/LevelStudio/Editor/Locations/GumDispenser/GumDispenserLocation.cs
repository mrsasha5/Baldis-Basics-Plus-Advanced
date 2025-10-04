using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser
{
    public class GumDispenserLocation : SimpleLocation, IEditorSettingsable
    {

        public ushort uses = 5;

        public ushort cooldown = 30;

        public int EncodeData()
        {
            int num = 0;

            num |= uses;
            num = num << 16;
            num |= cooldown;

            return num;
        }

        public void LoadEncodedData(int num)
        {
            uses = (ushort)(num >> 16);
            cooldown = (ushort)num;
        }

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioPlugin.Instance.genericStructureDisplays["adv_" + prefab];
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            base.InitializeVisual(visualObject);
            visualObject.GetComponent<SettingsComponent>().activateSettingsOn = this;
        }

        public void SettingsClicked()
        {
            Singleton<EditorController>.Instance.HoldUndo();

            GumDispenserExchangeHandler handler = EditorController.Instance.CreateUI<GumDispenserExchangeHandler>(
                "GumDispenserConfig", AssetsHelper.modPath + "Compats/LevelStudio/UI/GumDispenserConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
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
