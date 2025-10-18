using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.KitchenStove
{
    internal class KitchenStoveStructureLocation : StructureLocation
    {

        public const byte formatVersion = 0;

        public List<KitchenStoveLocation> stoves = new List<KitchenStoveLocation>();

        public List<SimpleButtonLocation> buttons = new List<SimpleButtonLocation>();

        public KitchenStoveLocation CreateNewStove(string prefab, IntVector2 pos, Direction dir, bool disableChecks = false)
        {
            KitchenStoveLocation loc = new KitchenStoveLocation()
            {
                owner = this,
                prefabForBuilder = prefab,
                position = pos,
                direction = dir,
                deleteAction = OnDeleteStove
            };

            if (!disableChecks && !loc.ValidatePosition(EditorController.Instance.levelData, ignoreSelf: true))
                return null;

            stoves.Add(loc);

            return loc;
        }

        public SimpleButtonLocation CreateNewButton(IntVector2 pos, Direction dir, bool disableChecks = false)
        {
            SimpleButtonLocation loc = new SimpleButtonLocation()
            {
                prefab = "button",
                position = pos,
                direction = dir,
                deleteAction = OnDeleteButton
            };

            if (!disableChecks && !loc.ValidatePosition(EditorController.Instance.levelData, ignoreSelf: true))
                return null;

            buttons.Add(loc);

            return loc;
        }

        private bool OnDeleteStove(EditorLevelData level, SimpleLocation loc)
        {
            int index = stoves.IndexOf((KitchenStoveLocation)loc);

            EditorController.Instance.RemoveVisual(loc);
            EditorController.Instance.RemoveVisual(buttons[index]);

            stoves.RemoveAt(index);
            buttons.RemoveAt(index);

            return true;
        }

        private bool OnDeleteButton(EditorLevelData level, SimpleLocation loc)
        {
            int index = buttons.IndexOf((SimpleButtonLocation)loc);

            EditorController.Instance.RemoveVisual(loc);
            EditorController.Instance.RemoveVisual(stoves[index]);

            stoves.RemoveAt(index);
            buttons.RemoveAt(index);
            return true;
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            for (int i = 0; i < stoves.Count; i++)
            {
                stoves[i].Compile(structInfo);

                structInfo.data.Add(new StructureDataInfo()
                {
                    position = PlusStudioLevelLoader.Extensions.ToData(buttons[i].position),
                    direction = (PlusDirection)buttons[i].direction
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
            for (int i = 0; i < stoves.Count; i++)
            {
                EditorController.Instance.AddVisual(stoves[i]);
                EditorController.Instance.AddVisual(buttons[i]);
            }
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            for (int i = 0; i < stoves.Count; i++)
            {
                EditorController.Instance.UpdateVisual(stoves[i]);
                EditorController.Instance.UpdateVisual(buttons[i]);
            }
        }

        public override void CleanupVisual(GameObject visualObject)
        {

        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            for (int i = 0; i < stoves.Count; i++)
            {
                stoves[i].position -= cellOffset;
                buttons[i].position -= cellOffset;
            }
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            for (int i = 0; i < stoves.Count; i++)
            {
                if (!stoves[i].ValidatePosition(data, ignoreSelf: true) || !buttons[i].ValidatePosition(data, ignoreSelf: true))
                {
                    OnDeleteStove(data, stoves[i]);
                    i--;
                }
            }

            return stoves.Count > 0;
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            byte ver = reader.ReadByte();

            if (ver > formatVersion)
                throw new System.Exception(LevelStudioIntegration.standardMsg_StructureVersionException);

            int count = reader.ReadInt32();

            while (count > 0)
            {
                CreateNewStove(null, default, default, disableChecks: true)
                    .ReadData(ver, data, reader, compressor);

                IntVector2 pos = reader.ReadByteVector2().ToInt();
                Direction dir = (Direction)reader.ReadByte();

                CreateNewButton(pos, dir, disableChecks: true);

                count--;
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write(formatVersion);
            writer.Write(stoves.Count);

            for (int i = 0; i < stoves.Count; i++)
            {
                stoves[i].WriteData(data, writer, compressor);

                writer.Write(buttons[i].position.ToByte());
                writer.Write((byte)buttons[i].direction);
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(stoves.Select(x => x.prefabForBuilder));
        }

    }
}
