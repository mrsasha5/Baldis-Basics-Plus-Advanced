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

        public List<GumDispenserLocation> dispenserLocations = new List<GumDispenserLocation>();

        public GumDispenserLocation CreateNewDispenser(EditorLevelData level, string pre, 
            IntVector2 pos, Direction dir, bool disableChecks)
        {
            GumDispenserLocation loc = new GumDispenserLocation()
            {
                prefab = pre,
                position = pos,
                direction = dir,
                deleteAction = OnDeleteDispenser
            };

            if (!disableChecks && !loc.ValidatePosition(level, ignoreSelf: false)) return null;

            dispenserLocations.Add(loc);
            
            return loc;
        }

        public SimpleButtonLocation CreateNewButton(EditorLevelData level, string pre, IntVector2 pos, Direction dir, bool disableChecks)
        {
            SimpleButtonLocation loc = new SimpleButtonLocation()
            {
                prefab = pre,
                position = pos,
                direction = dir,
                deleteAction = OnDeleteButton
            };

            if (!disableChecks && !loc.ValidatePosition(level, ignoreSelf: false)) return null;

            buttonLocations.Add(loc);

            return loc;
        }

        //I can't do this, otherwise the builder will consider its children are invalid
        public override bool OccupiesWall(IntVector2 pos, Direction dir)
        {
            /*for (int i = 0; i < dispenserLocations.Count; i++)
            {
                if (dispenserLocations[i].position == pos && dispenserLocations[i].direction == dir) return true;
            }

            for (int i = 0; i < buttonLocations.Count; i++)
            {
                if (buttonLocations[i].position == pos && buttonLocations[i].direction == dir) return true;
            }*/

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
                structInfo.data.Add(new StructureDataInfo()
                {
                    prefab = dispenserLocations[i].prefab,
                    position = new MystIntVector2(dispenserLocations[i].position.x, dispenserLocations[i].position.z),
                    direction = (PlusDirection)dispenserLocations[i].direction
                });

                structInfo.data.Add(new StructureDataInfo()
                {
                    prefab = buttonLocations[i].prefab,
                    position = new MystIntVector2(buttonLocations[i].position.x, buttonLocations[i].position.z),
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
                if (!dispenserLocations[i].ValidatePosition(data, ignoreSelf: false) || 
                    !buttonLocations[i].ValidatePosition(data, ignoreSelf: false))
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

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            reader.ReadByte(); //Version
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string prefab = compressor.ReadStoredString(reader);
                IntVector2 pos = reader.ReadByteVector2().ToInt();
                Direction dir = (Direction)reader.ReadByte();

                string prefab2 = compressor.ReadStoredString(reader);
                IntVector2 pos2 = reader.ReadByteVector2().ToInt();
                Direction dir2 = (Direction)reader.ReadByte();

                CreateNewDispenser(data, prefab, pos, dir, disableChecks: true);
                CreateNewButton(data, prefab2, pos2, dir2, disableChecks: true);
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write((byte)1); //Version
            writer.Write(dispenserLocations.Count);
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
