using System;
using System.IO;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GenericPlate
{
    public class GenericPlateLocation : SimpleLocation
    {

        public string prefabName;

        public int cooldown;

        public int uses;

        public float unpressTime;

        public void WriteData(BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabName);

            writer.Write(position.ToByte());
            writer.Write((byte)direction);

            writer.Write((byte)3);
            writer.Write(cooldown);
            writer.Write(uses);
            writer.Write(unpressTime);
        }

        public void ReadData(byte version, BinaryReader reader, StringCompressor compressor)
        {
            prefabName = compressor.ReadStoredString(reader);

            prefab = LevelStudioIntegration.genericPlateVisuals[prefabName];

            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            byte parameters = reader.ReadByte();

            if (parameters > 0)
                cooldown = reader.ReadInt32();
            if (parameters > 1)
                uses = reader.ReadInt32();
            if (parameters > 2)
                unpressTime = reader.ReadSingle();
        }

    }
}
