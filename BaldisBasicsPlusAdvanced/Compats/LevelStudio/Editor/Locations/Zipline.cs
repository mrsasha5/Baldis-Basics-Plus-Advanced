using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Builders;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations
{
    internal class ZiplinePointLocation : PointLocation
    {
        public string prefabForBuilder;

        public ushort uses;

        public ushort percentageDistanceToBreak = 50;

        // Not serializable fields
        private bool destroyed;

        private GameObject hangerVisual;

        private ZiplinePointLocation connectedPoint;

        public ZiplineStructureLocation owner;

        public LineRenderer renderer;

        public void Compile(StructureInfo info)
        {
            info.data.Add(new StructureDataInfo()
            {
                prefab = prefabForBuilder,
                position = position.ToData(), // I am writing this way because
                                              // extension methods from Loader
                                              // and Studio are conflicting and when
                                              // I am using PlusStudioLevelLoader namespace
                                              // I cannot use .ToInt() for ByteVector2
                                              // anymore.
                direction = PlusDirection.North,
                data = EncodeData()
            });
        }

        private int EncodeData()
        {
            int num = 0;
            num |= uses;
            num = num << 16;
            num |= percentageDistanceToBreak;
            return num;
        }

        public void ReadData(BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);
            position = reader.ReadByteVector2().ToInt();

            uses = reader.ReadUInt16();
            percentageDistanceToBreak = reader.ReadUInt16();
        }

        public void WriteData(BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);
            writer.Write(position.ToByte());

            writer.Write(uses);
            writer.Write(percentageDistanceToBreak);
        }

        public void LoadDefaults()
        {
            ZiplineHanger hangerPrefab =
                LevelLoaderPlugin.Instance.structureAliases[owner.type].prefabAliases[prefabForBuilder].GetComponent<ZiplineHanger>();

            uses = (ushort)hangerPrefab.MinMaxUses.z;
        }

        public void ConnectTo(ZiplinePointLocation loc)
        {
            connectedPoint = loc;
            loc.connectedPoint = this;

            ZiplineHanger hangerPrefab =
                LevelLoaderPlugin.Instance.structureAliases[owner.type].prefabAliases[prefabForBuilder].GetComponent<ZiplineHanger>();

            Vector3 pos = position.GetVector3FromCellPosition();
            Vector3 endPos = loc.position.GetVector3FromCellPosition();

            hangerVisual = Object.Instantiate(LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder));
            hangerVisual.transform.position = pos + hangerPrefab.Offset * (endPos - pos).normalized + Vector3.up * 5f;
            hangerVisual.GetComponent<EditorSettingsableComponent>().OnSettingsClicked = OnSettingsClicked;
            hangerVisual.GetComponent<EditorDeletableObject>().toDelete = this;
        }

        private void OnSettingsClicked()
        {
            Singleton<EditorController>.Instance.HoldUndo();

            ZiplineExchangeHandler handler = EditorController.Instance.CreateUI<ZiplineExchangeHandler>(
                "ZiplineConfig", AssetHelper.modPath + "Compats/LevelStudio/UI/ZiplineHangerConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            base.CleanupVisual(visualObject);
            destroyed = true;
            owner.locations.Remove(this);

            if (hangerVisual != null) Object.Destroy(hangerVisual.gameObject);
            if (renderer != null) Object.Destroy(renderer.gameObject);
            if (connectedPoint != null && !connectedPoint.destroyed) EditorController.Instance.RemoveVisual(connectedPoint);
        }
    }

    internal class ZiplineStructureLocation : StructureLocation
    {
        public const byte formatVersion = 0;

        public List<ZiplinePointLocation> locations = new List<ZiplinePointLocation>();

        public ZiplinePointLocation CreateNewChild(
            EditorLevelData levelData, string hangerPrefab, IntVector2 pos, bool disableChecks = false)
        {
            if (!disableChecks)
            {
                for (int i = 0; i < locations.Count; i++)
                {
                    if (locations[i].position == pos)
                    {
                        return null;
                    }
                }
            }

            ZiplinePointLocation loc = new ZiplinePointLocation()
            {
                owner = this,
                prefabForBuilder = hangerPrefab, //Prefab for Structure Builder
                prefab = "adv_zipline_pillar", //Prefab for editor only
                position = pos,
                deleteAction = OnDeleteLocation
            };

            if (!disableChecks && !loc.ValidatePosition(levelData, ignoreSelf: false)) return null;

            locations.Add(loc);

            Structure_Zipline builderPre =
                (Structure_Zipline)PlusStudioLevelLoader.LevelLoaderPlugin.Instance.structureAliases[type].structure;

            if (locations.Count % 2 == 0)
            {
                loc.renderer = locations[locations.Count - 2].renderer;
                builderPre.UpdateRenderer(loc.renderer,
                    PlusLevelStudio.EditorExtensions.ToWorld(locations[locations.Count - 2].position),
                    PlusLevelStudio.EditorExtensions.ToWorld(locations[locations.Count - 1].position));
            }
            else
            {
                loc.renderer = builderPre.GetLineRenderer();
            }

            return loc;
        }

        private bool OnDeleteLocation(EditorLevelData level, PointLocation loc)
        {
            EditorController.Instance.RemoveVisual(loc);
            return true;
        }

        public override void CleanupVisual(GameObject visualObject)
        {

        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            for (int i = 0; i < locations.Count; i++)
            {
                locations[i].Compile(structInfo);
            }

            return structInfo;
        }

        public override GameObject GetVisualPrefab()
        {
            return null;
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                EditorController.Instance.AddVisual(locations[i]);
            }

            UpdateVisual(visualObject);
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            Structure_Zipline builderPre =
                    (Structure_Zipline)PlusStudioLevelLoader.LevelLoaderPlugin.Instance.structureAliases[type].structure;
            for (int i = 0; i < locations.Count; i++)
            {
                EditorController.Instance.UpdateVisual(locations[i]);
                if (i % 2 != 0)
                {
                    builderPre.UpdateRenderer(locations[i].renderer,
                        PlusLevelStudio.EditorExtensions.ToWorld(locations[i - 1].position),
                            PlusLevelStudio.EditorExtensions.ToWorld(locations[i].position));
                }
            }
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                locations[i].position -= cellOffset;
            }
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            if (locations.Count == 0) return false;

            for (int i = 0; i < locations.Count; i++)
            {
                if (!locations[i].ValidatePosition(data, ignoreSelf: false))
                {
                    EditorController.Instance.RemoveVisual(locations[i]);
                    if ((i + 1) % 2 == 0) i -= 2;
                    else i--;
                }
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
                CreateNewChild(data, "", default, disableChecks: true)
                    .ReadData(reader, compressor);
            }
            for (int i = 0; i < locations.Count; i += 2)
            {
                locations[i].ConnectTo(locations[i + 1]);
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write(formatVersion);
            writer.Write(locations.Count);
            for (int i = 0; i < locations.Count; i++)
            {
                locations[i].WriteData(writer, compressor);
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(locations.Select((x) => x.prefabForBuilder));
        }
    }
}
