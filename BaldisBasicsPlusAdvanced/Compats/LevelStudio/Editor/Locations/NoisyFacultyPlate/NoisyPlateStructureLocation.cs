using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyFacultyPlate
{
    internal class NoisyPlateStructureLocation : StructureLocation
    {

        public const byte formatVersion = 0;

        public List<NoisyPlateRoomLocation> infectedRooms = new List<NoisyPlateRoomLocation>();

        public NoisyPlateRoomLocation CreateAndAddRoom(string prefab, EditorRoom targetRoom)
        {
            if (targetRoom == EditorController.Instance.levelData.hall) return null;

            for (int i = 0; i < infectedRooms.Count; i++)
            {
                if (infectedRooms[i].room == targetRoom) return null;
            }

            NoisyPlateRoomLocation noisyPlateRoomLocation = new NoisyPlateRoomLocation();
            noisyPlateRoomLocation.prefabForBuilder = prefab;
            noisyPlateRoomLocation.owner = this;
            noisyPlateRoomLocation.room = targetRoom;

            infectedRooms.Add(noisyPlateRoomLocation);
            return noisyPlateRoomLocation;
        }

        public void OnRoomDelete(NoisyPlateRoomLocation room, bool performCheck)
        {
            infectedRooms.Remove(room);
            ClearAllAllocatedPlatesForRoom(room);
            if (performCheck)
            {
                DeleteIfInvalid();
            }
        }

        public override GameObject GetVisualPrefab()
        {
            return null;
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            for (int i = 0; i < infectedRooms.Count; i++)
            {
                UpdateVisualForRoom(infectedRooms[i]);
            }
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            for (int i = 0; i < infectedRooms.Count; i++)
            {
                UpdateVisualForRoom(infectedRooms[i]);
            }
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            for (int i = 0; i < infectedRooms.Count; i++)
            {
                ClearAllAllocatedPlatesForRoom(infectedRooms[i]);
            }
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            for (int i = 0; i < infectedRooms.Count; i++)
            {
                infectedRooms[i].CompileData(data, level, structInfo);
            }

            return structInfo;
        }

        private void ClearAllAllocatedPlatesForRoom(NoisyPlateRoomLocation room)
        {
            for (int num = room.allocatedPlates.Count - 1; num >= 0; num--)
            {
                Object.Destroy(room.allocatedPlates[num]);
                room.allocatedPlates.Remove(room.allocatedPlates[num]);
            }
        }

        private void UpdateVisualForRoom(NoisyPlateRoomLocation room)
        {
            List<DoorLocation> list = EditorController.Instance.levelData
                .doors.Where(x => x.DoorConnectedToRoom(EditorController.Instance.levelData, room.room, forEditor: true)).ToList();

            List<IntVector2> coveredPositions = new List<IntVector2>();

            List<GameObject> list2 = new List<GameObject>(room.allocatedPlates);
            while (list.Count > 0)
            {
                DoorLocation doorLocation = list[0];
                list.RemoveAt(0);
                if (LevelStudioPlugin.Instance.doorIngameStatus[doorLocation.type] != DoorIngameStatus.AlwaysObject)
                {
                    IntVector2 vector = doorLocation.position;
                    Direction direction = doorLocation.direction.GetOpposite();
                    if (EditorController.Instance.levelData.RoomFromPos(vector, forEditor: true) != room.room)
                    {
                        vector = doorLocation.position + doorLocation.direction.ToIntVector2();
                        direction = doorLocation.direction;
                    }

                    if (coveredPositions.Contains(vector)) continue;
                    else coveredPositions.Add(vector);

                    GameObject gameObject;
                    if (list2.Count > 0)
                    {
                        gameObject = list2[0];
                        list2.RemoveAt(0);
                    }
                    else
                    {
                        gameObject = Object.Instantiate(LevelStudioIntegration.GetVisualPrefab(type, room.prefabForBuilder));
                        gameObject.GetComponent<EditorDeletableObject>().toDelete = room;
                        gameObject.GetComponent<SettingsComponent>().activateSettingsOn = room;
                        room.allocatedPlates.Add(gameObject);
                    }

                    gameObject.transform.position = vector.ToWorld();
                    gameObject.transform.rotation = direction.ToRotation();
                }
            }

            while (list2.Count > 0)
            {
                GameObject gameObject2 = list2[0];
                list2.RemoveAt(0);
                Object.Destroy(gameObject2);
                room.allocatedPlates.Remove(gameObject2);
            }
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            for (int num = infectedRooms.Count - 1; num >= 0; num--)
            {
                if (!data.rooms.Contains(infectedRooms[num].room))
                {
                    OnRoomDelete(infectedRooms[num], performCheck: false);
                }
            }

            return infectedRooms.Count > 0;
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            byte ver = reader.ReadByte();

            if (ver > formatVersion)
                throw new System.Exception(LevelStudioIntegration.standardMsg_StructureVersionException);

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                NoisyPlateRoomLocation loc = CreateAndAddRoom(null, null);
                loc.ReadData(ver, data, reader, compressor);
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write(formatVersion);
            writer.Write(infectedRooms.Count);
            for (int i = 0; i < infectedRooms.Count; i++)
            {
                infectedRooms[i].WriteData(data, writer, compressor);
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(infectedRooms.Select(x => x.prefabForBuilder));
        }

        public override bool ShouldUpdateVisual(PotentialStructureUpdateReason reason)
        {
            return reason == PotentialStructureUpdateReason.CellChange;
        }
    }

}