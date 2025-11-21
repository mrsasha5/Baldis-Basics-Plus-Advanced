using System;
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

        public float cooldown = 30;

        public GumDispenserStructureLocation owner;

        public void CompileData(EditorLevelData data, BaldiLevel level, StructureInfo info)
        {
            info.data.Add(new StructureDataInfo()
            {
                prefab = prefabForBuilder,
                position = PlusStudioLevelLoader.Extensions.ToData(position),
                direction = (PlusDirection)direction
            });

            info.data.Add(new StructureDataInfo()
            {
                data = uses
            });

            info.data.Add(new StructureDataInfo()
            {
                data = PlusStudioLevelLoader.WeirdTechExtensions.ConvertToIntNoRecast(cooldown)
            });
        }

        public void ReadData(byte version, EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);
            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            uses = reader.ReadUInt16();
            cooldown = reader.ReadSingle();
        }

        public void WriteData(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);
            writer.Write(position.ToByte());
            writer.Write((byte)direction);

            writer.Write(uses);
            writer.Write(cooldown);
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            base.InitializeVisual(visualObject);
            visualObject.GetComponent<SettingsComponent>().activateSettingsOn = this;
        }

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder);
        }

        public void SettingsClicked()
        {
            EditorController.Instance.HoldUndo();

            GumDispenserExchangeHandler handler = EditorController.Instance.CreateUI<GumDispenserExchangeHandler>(
                "GumDispenserConfig", AssetHelper.modPath + "Compats/LevelStudio/UI/GumDispenserConfig.json");
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
