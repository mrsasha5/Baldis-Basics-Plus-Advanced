using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GumDispenser
{
    internal class GumDispenserStructureLocation : StructureLocation
    {

        public const byte formatVersion = 0;

        public List<SimpleButtonLocation> buttonLocations = new List<SimpleButtonLocation>();

        public List<GumDispenserLocation> dispenserLocations = new List<GumDispenserLocation>();

        public GumDispenserLocation CreateNewDispenser(EditorLevelData level, string pre, 
            IntVector2 pos, Direction dir, bool disableChecks)
        {
            GumDispenserLocation loc = new GumDispenserLocation()
            {
                owner = this,
                prefabForBuilder = pre,
                position = pos,
                direction = dir,
                deleteAction = OnDeleteDispenser
            };

            if (!disableChecks && (!loc.ValidatePosition(level, ignoreSelf: true) || !ValidatePositionInChildren(loc)))
                return null;

            dispenserLocations.Add(loc);

            return loc;
        }

        public SimpleButtonLocation CreateNewButton(EditorLevelData level, IntVector2 pos, Direction dir, bool loadingMode)
        {
            SimpleButtonLocation loc = new SimpleButtonLocation()
            {
                prefab = "button",
                position = pos,
                direction = dir,
                deleteAction = OnDeleteButton
            };

            if (!loadingMode && (!loc.ValidatePosition(level, ignoreSelf: true) || !ValidatePositionInChildren(loc)))
                return null;

            buttonLocations.Add(loc);

            return loc;
        }

        public override bool OccupiesWall(IntVector2 pos, Direction dir)
        {
            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                if (dispenserLocations[i].position == pos && dispenserLocations[i].direction == dir) return true;
            }

            for (int i = 0; i < buttonLocations.Count; i++)
            {
                if (buttonLocations[i].position == pos && buttonLocations[i].direction == dir) return true;
            }

            return base.OccupiesWall(pos, dir);
        }

        private bool OnDeleteDispenser(EditorLevelData level, SimpleLocation loc)
        {
            int index = dispenserLocations.IndexOf((GumDispenserLocation)loc);

            EditorController.Instance.RemoveVisual(loc);
            EditorController.Instance.RemoveVisual(buttonLocations[index]);

            dispenserLocations.RemoveAt(index);
            buttonLocations.RemoveAt(index);

            return true;
        }

        private bool OnDeleteButton(EditorLevelData level, SimpleLocation loc)
        {
            int index = buttonLocations.IndexOf((SimpleButtonLocation)loc);

            EditorController.Instance.RemoveVisual(loc);
            EditorController.Instance.RemoveVisual(dispenserLocations[index]);

            dispenserLocations.RemoveAt(index);
            buttonLocations.RemoveAt(index);

            return true;
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                dispenserLocations[i].CompileData(data, level, structInfo);

                structInfo.data.Add(new StructureDataInfo()
                {
                    prefab = buttonLocations[i].prefab,
                    position = PlusStudioLevelLoader.Extensions.ToData(buttonLocations[i].position),
                    direction = (PlusDirection)buttonLocations[i].direction
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
            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                EditorController.Instance.AddVisual(dispenserLocations[i]);
                EditorController.Instance.AddVisual(buttonLocations[i]);
            }
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                dispenserLocations[i].position -= cellOffset;
                buttonLocations[i].position -= cellOffset;
            }
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                EditorController.Instance.UpdateVisual(dispenserLocations[i]);
                EditorController.Instance.UpdateVisual(buttonLocations[i]);
            }
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            if (dispenserLocations.Count == 0) return false;

            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                if (!dispenserLocations[i].ValidatePosition(data, ignoreSelf: true) || 
                    !buttonLocations[i].ValidatePosition(data, ignoreSelf: true) || 
                    !ValidatePositionInChildren(dispenserLocations[i]) || !ValidatePositionInChildren(buttonLocations[i]))
                {
                    EditorController.Instance.RemoveVisual(dispenserLocations[i]);
                    dispenserLocations.RemoveAt(i);

                    EditorController.Instance.RemoveVisual(buttonLocations[i]);
                    buttonLocations.RemoveAt(i);
                    i--;
                }
            }

            return true;
        }

        private bool ValidatePositionInChildren(SimpleLocation child)
        {
            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                if (dispenserLocations[i] != child && dispenserLocations[i].position == child.position &&
                    dispenserLocations[i].direction == child.direction) return false;
            }

            for (int i = 0; i < buttonLocations.Count; i++)
            {
                if (buttonLocations[i] != child && buttonLocations[i].position == child.position &&
                    buttonLocations[i].direction == child.direction) return false;
            }

            return true;
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            byte ver = reader.ReadByte();

            if (ver > formatVersion)
                throw new System.Exception(LevelStudioIntegration.standardMsg_StructureVersionException);

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                CreateNewDispenser(data, null, default, default, disableChecks: true)
                    .ReadData(ver, data, reader, compressor);

                IntVector2 pos = reader.ReadByteVector2().ToInt();
                Direction dir = (Direction)reader.ReadByte();
                
                CreateNewButton(data, pos, dir, loadingMode: true);
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write(formatVersion);
            writer.Write(dispenserLocations.Count);
            for (int i = 0; i < dispenserLocations.Count; i++)
            {
                dispenserLocations[i].WriteData(data, writer, compressor);

                writer.Write(buttonLocations[i].position.ToByte());
                writer.Write((byte)buttonLocations[i].direction);
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(dispenserLocations.Select(x => x.prefabForBuilder));
        }

    }
}
