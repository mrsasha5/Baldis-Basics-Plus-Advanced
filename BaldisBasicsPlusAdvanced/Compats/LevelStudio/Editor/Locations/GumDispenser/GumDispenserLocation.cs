using System.IO;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser
{
    internal class GumDispenserLocation : SimpleLocation, IEditorSettingsable
    {

        public string prefabForBuilder;

        public ushort uses = 5;

        public ushort cooldown = 30;

        public void CompileData(EditorLevelData data, BaldiLevel level, StructureInfo info)
        {
            info.data.Add(new StructureDataInfo()
            {
                prefab = prefabForBuilder,
                position = PlusStudioLevelLoader.Extensions.ToData(position),
                direction = (PlusDirection)direction,
                data = EncodeData()
            });
        }

        public void ReadData(byte version, EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);
            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            uses = reader.ReadUInt16();
            cooldown = reader.ReadUInt16();

            prefab = "adv_" + prefabForBuilder;
        }

        public void WriteData(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);
            writer.Write(position.ToByte());
            writer.Write((byte)direction);

            writer.Write(uses);
            writer.Write(cooldown);
        }

        private int EncodeData()
        {
            int num = 0;

            num |= uses;
            num <<= 16;
            num |= cooldown;

            return num;
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
