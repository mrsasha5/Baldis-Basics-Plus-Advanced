using System;
using System.Collections.Generic;
using System.IO;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser
{
    public class GumDispenserStructureLocation : StructureLocation
    {

        public List<SimpleButtonLocation> buttonLocations = new List<SimpleButtonLocation>();

        public List<SimpleLocation> dispenserLocations = new List<SimpleLocation>();

        public override void CleanupVisual(GameObject visualObject)
        {
            
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);
            return structInfo;
        }

        public override GameObject GetVisualPrefab()
        {
            return null;
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            if (dispenserLocations.Count == 0) return false;

            return true;
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            reader.ReadByte(); //Version
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                compressor.ReadStoredString(reader);
                reader.ReadByte();
                Direction dir = (Direction)reader.ReadByte();

                compressor.ReadStoredString(reader);
                reader.ReadByte();
                Direction dir2 = (Direction)reader.ReadByte();
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write((byte)1); //Version
            writer.Write(dispenserLocations.Count * 2);
            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                compressor.WriteStoredString(writer, dispenserLocations[i].prefab);
                writer.Write(dispenserLocations[i].position.ToByte());
                writer.Write((byte)dispenserLocations[i].direction);

                compressor.WriteStoredString(writer, buttonLocations[i].prefab);
                writer.Write(buttonLocations[i].position.ToByte());
                writer.Write((byte)buttonLocations[i].direction);
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                compressor.AddString(dispenserLocations[i].prefab);
                compressor.AddString(buttonLocations[i].prefab);
            }
        }

    }
}
