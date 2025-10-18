using System;
using System.IO;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GenericPlate
{
    internal class GenericPlateLocation : SimpleLocation, IEditorSettingsable
    {

        public string prefabForBuilder;

        public ushort uses;

        public bool cooldownOverridden;

        public float cooldown;

        public float unpressTime;

        public bool showsUses;

        public bool showsCooldown;

        public GenericPlateStructureLocation owner;

        public void LoadDefaults(string type)
        {
            BasePlate prefab = 
                PlusStudioLevelLoader.
                    LevelLoaderPlugin.Instance.structureAliases[type].prefabAliases[prefabForBuilder].GetComponent<BasePlate>();

            uses = (ushort)prefab.Data.maxUses;
            unpressTime = prefab.Data.timeToUnpress;
            showsUses = prefab.Data.showsUses;
            showsCooldown = prefab.Data.showsCooldown;
        }

        public void CompileData(StructureInfo info)
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
                data = cooldownOverridden.ToInt()
            });

            info.data.Add(new StructureDataInfo()
            {
                data = BitConverter.ToInt32(BitConverter.GetBytes(cooldown), 0)
            });

            info.data.Add(new StructureDataInfo()
            {
                data = BitConverter.ToInt32(BitConverter.GetBytes(unpressTime), 0)
            });

            info.data.Add(new StructureDataInfo()
            {
                data = showsUses.ToInt()
            });

            info.data.Add(new StructureDataInfo()
            {
                data = showsCooldown.ToInt()
            });
        }

        public void WriteData(BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);

            writer.Write(position.ToByte());
            writer.Write((byte)direction);

            writer.Write(uses);
            writer.Write(cooldownOverridden);
            writer.Write(cooldown);
            writer.Write(unpressTime);
            writer.Write(showsUses);
            writer.Write(showsCooldown);
        }

        public void ReadData(byte version, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);

            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            uses = reader.ReadUInt16();
            cooldownOverridden = reader.ReadBoolean();
            cooldown = reader.ReadSingle();
            unpressTime = reader.ReadSingle();
            showsUses = reader.ReadBoolean();
            showsCooldown = reader.ReadBoolean();
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
            GenericPlateExchangeHandler handler = EditorController.Instance.CreateUI<GenericPlateExchangeHandler>(
                "GenericPlateConfig", AssetsHelper.modPath + "Compats/LevelStudio/UI/GenericPlateConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }
    }
}
