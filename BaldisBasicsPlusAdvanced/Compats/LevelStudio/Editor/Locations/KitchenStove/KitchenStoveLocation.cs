using System;
using System.IO;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.KitchenStove
{
    internal class KitchenStoveLocation : SimpleLocation
    {

        public string prefabForBuilder;

        public float cookingTime;

        public float coolingTime;

        public KitchenStoveStructureLocation owner;

        public void Compile(StructureInfo info)
        {
            info.data.Add(new StructureDataInfo()
            {
                prefab = prefabForBuilder,
                position = position.ToData(),
                direction = (PlusDirection)direction
            });

            info.data.Add(new StructureDataInfo()
            {
                data = BitConverter.ToInt32(BitConverter.GetBytes(cookingTime), 0)
            });

            info.data.Add(new StructureDataInfo()
            {
                data = BitConverter.ToInt32(BitConverter.GetBytes(coolingTime), 0)
            });
        }

        public void ReadData(byte version, EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);
            cookingTime = reader.ReadSingle();
            coolingTime = reader.ReadSingle();
        }

        public void WriteData(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);
            writer.Write(cookingTime);
            writer.Write(coolingTime);
        }

        public void SettingsClicked()
        {
            // handler = EditorController.Instance.CreateUI<>(
            //"KitchenStoveConfig", AssetsHelper.modPath + "Compats/LevelStudio/UI/KitchenStoveConfig.json");
            //handler.OnInitialized(this);
        }

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder);
        }
    }
}
