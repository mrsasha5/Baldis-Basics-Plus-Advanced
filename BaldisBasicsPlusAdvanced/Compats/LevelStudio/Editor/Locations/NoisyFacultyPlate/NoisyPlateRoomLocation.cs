using System.Collections.Generic;
using System.IO;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyFacultyPlate
{
    internal class NoisyPlateRoomLocation : IEditorSettingsable, IEditorDeletable
    {

        //Serializable
        public string prefabForBuilder; 

        public float cooldown;

        public ushort uses;

        public ushort generosity;

        public int pointsPerAlarm;

        //Not serializable
        public EditorRoom room;

        public List<GameObject> allocatedPlates = new List<GameObject>();

        public NoisyPlateStructureLocation owner;

        public void LoadDefaults()
        {
            NoisyPlate prefab = LevelLoaderPlugin.Instance.structureAliases[owner.type].prefabAliases[prefabForBuilder]
                .GetComponent<NoisyPlate>();

            cooldown = (ushort)prefab.Cooldown;
            uses = (ushort)prefab.Data.maxUses;
            generosity = (ushort)prefab.Generosity;
            pointsPerAlarm = prefab.PointsReward;
        }

        public void CompileData(EditorLevelData data, BaldiLevel level, StructureInfo info)
        {
            info.data.Add(new StructureDataInfo()
            {
                prefab = prefabForBuilder,
                data = data.rooms.IndexOf(room)
            });

            info.data.Add(new StructureDataInfo()
            {
                data = cooldown.ConvertToIntNoRecast()
            });

            info.data.Add(new StructureDataInfo()
            {
                data = uses
            });

            info.data.Add(new StructureDataInfo()
            {
                data = generosity
            });

            info.data.Add(new StructureDataInfo()
            {
                data = pointsPerAlarm
            });
        }

        public void ReadData(byte version, EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);
            ushort roomId = reader.ReadUInt16();

            room = data.RoomFromId(roomId);

            cooldown = reader.ReadSingle();
            uses = reader.ReadUInt16();
            generosity = reader.ReadUInt16();
            pointsPerAlarm = reader.ReadInt32();
        }

        public void WriteData(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);
            writer.Write(data.IdFromRoom(room));

            writer.Write(cooldown);
            writer.Write(uses);
            writer.Write(generosity);
            writer.Write(pointsPerAlarm);
        }

        public void SettingsClicked()
        {
            NoisyPlateExchangeHandler handler = EditorController.Instance.CreateUI<NoisyPlateExchangeHandler>(
                "NoisyRoomConfig", AssetHelper.modPath + "Compats/LevelStudio/UI/NoisyRoomConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }

        public bool OnDelete(EditorLevelData data)
        {
            owner.OnRoomDelete(this, true);
            return true;
        }
        
    }
}
