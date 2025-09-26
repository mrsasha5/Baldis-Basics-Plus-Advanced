using System.Collections.Generic;
using System.IO;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyPlate
{
    public class NoisyPlateStructureLocation : StructureLocation
    {
        public class RoomDataContainer
        {

            public int index = 0;

            public string prefab = ""; //StructureData prefab

            public List<SimpleLocation> locations = new List<SimpleLocation>();

            public void RefreshLocations()
            {

            }

        }

        public List<RoomDataContainer> rooms = new List<RoomDataContainer>();

        public void AddRoom(EditorLevelData level, EditorRoom room, string prefab)
        {
            int i = level.rooms.IndexOf(room);

            RoomDataContainer roomData = new RoomDataContainer
            {
                index = i,
                prefab = prefab
            };

            rooms.Add(roomData);
        }

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
            throw new System.NotImplementedException();
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            throw new System.NotImplementedException();
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            throw new System.NotImplementedException();
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            throw new System.NotImplementedException();
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write((byte)1); //Version
            writer.Write(rooms.Count);
            foreach (RoomDataContainer room in rooms)
            {

            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            throw new System.NotImplementedException();
        }
    }
}
