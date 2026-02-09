using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations
{
    internal class PulleyLocation : SimpleLocation, IEditorSettingsable
    {
        public string prefabForBuilder;

        public ushort uses;

        public int points;

        public int finalPoints;

        public float maxDistance;

        public PulleyStructureLocation owner;

        public void LoadDefaults(string type)
        {
            Game.Objects.Pulley prefab =
                PlusStudioLevelLoader.
                    LevelLoaderPlugin.Instance.structureAliases[type].prefabAliases[prefabForBuilder].GetComponent<Game.Objects.Pulley>();

            uses = (ushort)prefab.uses;
            points = (ushort)prefab.points;
            finalPoints = (ushort)prefab.finalPoints;
            maxDistance = prefab.maxDistance;
        }

        public void CompileData(StructureInfo info)
        {
            info.data.Add(new StructureDataInfo()
            {
                prefab = prefabForBuilder,
                position = PlusStudioLevelLoader.Extensions.ToData(position),
                direction = (PlusDirection)direction
            });

            info.data.Add(new StructureDataInfo()
            {
                data = uses
            });

            info.data.Add(new StructureDataInfo()
            {
                data = points
            });

            info.data.Add(new StructureDataInfo()
            {
                data = finalPoints
            });

            info.data.Add(new StructureDataInfo()
            {
                data = PlusStudioLevelLoader.WeirdTechExtensions.ConvertToIntNoRecast(maxDistance)
            });
        }

        public void ReadData(byte version, EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);
            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            uses = reader.ReadUInt16();
            points = reader.ReadInt32();
            finalPoints = reader.ReadInt32();
            maxDistance = reader.ReadSingle();
        }

        public void WriteData(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);
            writer.Write(position.ToByte());
            writer.Write((byte)direction);

            writer.Write(uses);
            writer.Write(points);
            writer.Write(finalPoints);
            writer.Write(maxDistance);
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            base.InitializeVisual(visualObject);
            visualObject.GetComponent<SettingsComponent>().activateSettingsOn = this;
        }

        public void SettingsClicked()
        {
            PulleyExchangeHandler handler = EditorController.Instance.CreateUI<PulleyExchangeHandler>(
                "PulleyConfig", AssetHelper.modPath + "Compats/LevelStudio/UI/PulleyConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }

        public override bool ValidatePosition(EditorLevelData data, bool ignoreSelf)
        {
            if (!data.WallFree(position, direction, ignoreSelf))
            {
                return false;
            }

            return base.ValidatePosition(data, ignoreSelf);
        }

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder);
        }
    }

    internal class PulleyStructureLocation : StructureLocation
    {
        public const byte formatVersion = 0;

        public List<PulleyLocation> locations = new List<PulleyLocation>();

        public PulleyLocation CreateNewChild(EditorLevelData data, string prefab, IntVector2 pos, Direction dir, bool disableChecks = false)
        {
            PulleyLocation loc = new PulleyLocation()
            {
                prefabForBuilder = prefab,
                position = pos,
                direction = dir,
                deleteAction = OnDeleteLocation,
                owner = this
            };

            if (!disableChecks && !loc.ValidatePosition(data, ignoreSelf: false))
                return null;

            locations.Add(loc);

            return loc;
        }

        private bool OnDeleteLocation(EditorLevelData level, SimpleLocation loc)
        {
            EditorController.Instance.RemoveVisual(loc);
            locations.Remove((PulleyLocation)loc);

            return true;
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            foreach (PulleyLocation loc in locations)
            {
                loc.CompileData(structInfo);
            }

            return structInfo;
        }

        public override GameObject GetVisualPrefab()
        {
            return null;
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            foreach (PulleyLocation location in locations)
            {
                EditorController.Instance.AddVisual(location);
            }
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            foreach (PulleyLocation location in locations)
            {
                EditorController.Instance.UpdateVisual(location);
            }
        }

        public override void CleanupVisual(GameObject visualObject)
        {

        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            foreach (PulleyLocation location in locations)
            {
                location.position -= cellOffset;
            }
        }

        public override bool OccupiesWall(IntVector2 pos, Direction dir)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].position == pos && locations[i].direction == dir) return true;
            }

            return base.OccupiesWall(pos, dir);
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (!locations[i].ValidatePosition(data, ignoreSelf: true) || !ValidatePositionInChildren(locations[i]))
                {
                    EditorController.Instance.RemoveVisual(locations[i]);
                    locations.RemoveAt(i);
                    i--;
                }
            }

            return locations.Count > 0;
        }

        private bool ValidatePositionInChildren(SimpleLocation child)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i] != child && locations[i].position == child.position && locations[i].direction == child.direction)
                    return false;
            }

            return true;
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            byte ver = reader.ReadByte();

            if (ver > formatVersion)
                throw new Exception(LevelStudioIntegration.standardMsg_StructureVersionException);

            int count = reader.ReadInt32();

            while (count > 0)
            {
                CreateNewChild(data, null, default, default, disableChecks: true)
                    .ReadData(ver, data, reader, compressor);
                count--;
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write(formatVersion);
            writer.Write(locations.Count);
            foreach (PulleyLocation location in locations)
            {
                location.WriteData(data, writer, compressor);
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(locations.Select(x => x.prefabForBuilder));
        }
    }
}