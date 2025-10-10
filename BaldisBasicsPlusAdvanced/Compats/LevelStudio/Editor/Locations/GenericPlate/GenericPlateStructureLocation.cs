using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GenericPlate
{
    internal class GenericPlateStructureLocation : StructureLocation
    {

        public const byte formatVersion = 0;

        public List<GenericPlateLocation> locations = new List<GenericPlateLocation>();

        public GenericPlateLocation CreateNewChild(string prefabName, IntVector2 pos, Direction dir, bool disableChecks)
        {
            GenericPlateLocation loc = new GenericPlateLocation()
            {
                prefabName = prefabName,
                position = pos,
                direction = dir,
                deleteAction = OnDeleteLocation
            };

            if (!disableChecks && !loc.ValidatePosition(EditorController.Instance.levelData, ignoreSelf: true))
                return null;

            if (prefabName != null)
            {
                loc.prefab = LevelStudioIntegration.genericPlateVisuals[prefabName];
            }

            locations.Add(loc);

            return loc;
        }

        private bool OnDeleteLocation(EditorLevelData level, SimpleLocation loc)
        {
            locations.Remove((GenericPlateLocation)loc);
            EditorController.Instance.RemoveVisual(loc);

            return true;
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            foreach (GenericPlateLocation loc in locations)
            {
                structInfo.data.Add(new StructureDataInfo()
                {
                    prefab = loc.prefabName,
                    position = PlusStudioLevelLoader.Extensions.ToData(loc.position),
                    direction = (PlusDirection)loc.direction
                });

                structInfo.data.Add(new StructureDataInfo()
                {
                    data = loc.uses
                });

                structInfo.data.Add(new StructureDataInfo()
                {
                    data = loc.cooldown
                });

                structInfo.data.Add(new StructureDataInfo()
                {
                    data = BitConverter.ToInt32(BitConverter.GetBytes(loc.unpressTime), 0)
                });
            }

            return structInfo;
        }

        public override GameObject GetVisualPrefab()
        {
            return null;
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            foreach (GenericPlateLocation loc in locations)
            {
                EditorController.Instance.AddVisual(loc);
            }
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            foreach (GenericPlateLocation loc in locations)
            {
                loc.UpdateVisual(visualObject);
            }
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            foreach (GenericPlateLocation loc in locations)
            {
                loc.position -= cellOffset;
            }
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (!locations[i].ValidatePosition(data, ignoreSelf: true))
                {
                    locations.RemoveAt(i);
                    i--;
                }
            }

            return locations.Count > 0;
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            byte ver = reader.ReadByte();

            if (ver > formatVersion)
                throw new Exception(LevelStudioIntegration.standardMsg_StructureVersionException);

            int count = reader.ReadInt32();
            while (count > 0)
            {
                GenericPlateLocation loc = CreateNewChild(null, default, default, disableChecks: true);
                loc.ReadData(ver, reader, compressor);
                count--;
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write(formatVersion);

            writer.Write(locations.Count);
            foreach (GenericPlateLocation location in locations)
            {
                location.WriteData(writer, compressor);
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(locations.Select(x => x.prefabName));
        }

    }
}
