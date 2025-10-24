using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.AccelerationPlate
{
    internal class AccelerationPlateStructureLocation : StructureLocation
    {

        public const byte formatVersion = 0;

        public List<AccelerationPlateLocation> plates = new List<AccelerationPlateLocation>();

        public AccelerationPlateLocation CreateNewPlate(EditorLevelData data, string prefab, IntVector2 pos, Direction dir, 
            bool disableChecks = false)
        {
            AccelerationPlateLocation loc = new AccelerationPlateLocation()
            {
                owner = this,
                prefabForBuilder = prefab,
                position = pos,
                direction = dir,
                deleteAction = OnDeletePlate
            };

            if (!disableChecks && !loc.ValidatePosition(data, ignoreSelf: true))
                return null;

            plates.Add(loc);

            return loc;
        }

        public SimpleButtonLocation CreateNewButton(EditorLevelData data, AccelerationPlateLocation plate, IntVector2 pos, Direction dir, 
            bool disableChecks = false)
        {
            SimpleButtonLocation loc = new SimpleButtonLocation()
            {
                prefab = "button",
                position = pos,
                direction = dir,
                deleteAction = OnDeleteButton
            };

            if (!disableChecks && !loc.ValidatePosition(data, ignoreSelf: true))
                return null;

            plate.button = loc;

            return loc;
        }

        private bool OnDeletePlate(EditorLevelData level, SimpleLocation loc)
        {
            int index = plates.IndexOf((AccelerationPlateLocation)loc);

            EditorController.Instance.RemoveVisual(loc);
            EditorController.Instance.RemoveVisual(plates[index].button);

            plates.RemoveAt(index);

            return true;
        }

        private bool OnDeleteButton(EditorLevelData level, SimpleLocation loc)
        {
            for (int i = 0; i < plates.Count; i++)
            {
                if (plates[i].button == loc)
                {
                    EditorController.Instance.RemoveVisual(plates[i].button);
                    plates.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            foreach (AccelerationPlateLocation location in plates)
            {
                location.CompileData(structInfo);
            }

            return structInfo;
        }

        public override GameObject GetVisualPrefab()
        {
            return null;
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            for (int i = 0; i < plates.Count; i++)
            {
                EditorController.Instance.AddVisual(plates[i]);
                if (plates[i].button != null) 
                    EditorController.Instance.AddVisual(plates[i].button);
            }
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            for (int i = 0; i < plates.Count; i++)
            {
                EditorController.Instance.UpdateVisual(plates[i]);
                if (plates[i].button != null)
                    EditorController.Instance.UpdateVisual(plates[i].button);
            }
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            for (int i = 0; i < plates.Count; i++)
            {
                plates[i].position -= cellOffset;
                plates[i].button.position -= cellOffset;
            }
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            for (int i = 0; i < plates.Count; i++)
            {
                if (!plates[i].ValidatePosition(data, ignoreSelf: true) || !plates[i].button.ValidatePosition(data, ignoreSelf: true))
                {
                    OnDeletePlate(data, plates[i]);
                }
            }

            return plates.Count > 0;
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            byte ver = reader.ReadByte();

            if (ver > formatVersion)
                throw new Exception(LevelStudioIntegration.standardMsg_StructureVersionException);

            int count = reader.ReadInt32();

            while (count > 0)
            {
                AccelerationPlateLocation loc = CreateNewPlate(data, null, default, default, disableChecks: true);
                loc.ReadData(ver, data, reader, compressor);
                count--;
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write(formatVersion);
            writer.Write(plates.Count);
            for (int i = 0; i < plates.Count; i++)
            {
                plates[i].WriteData(writer, compressor);
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(plates.Select(x => x.prefabForBuilder));
        }
    }
}
