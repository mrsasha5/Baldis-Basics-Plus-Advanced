using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
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
    internal class GenericPlateLocation : SimpleLocation, IEditorSettingsable
    {

        public string prefabForBuilder;

        public ushort uses;

        public float cooldown = -1f;

        public float unpressTime;

        public bool showsUses;

        public bool showsCooldown;

        public GenericPlateStructureLocation owner;

        public void LoadDefaults(string type)
        {
            BasePlate prefab =
                PlusStudioLevelLoader.
                    LevelLoaderPlugin.Instance.structureAliases[type].prefabAliases[prefabForBuilder].GetComponent<BasePlate>();

            uses = (ushort)prefab.Data.maxUses;
            unpressTime = prefab.Data.timeToUnpress;
            showsUses = prefab.Data.showsUses;
            showsCooldown = prefab.Data.showsCooldown;
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
                data = PlusStudioLevelLoader.WeirdTechExtensions.ConvertToIntNoRecast(cooldown)
            });

            info.data.Add(new StructureDataInfo()
            {
                data = PlusStudioLevelLoader.WeirdTechExtensions.ConvertToIntNoRecast(unpressTime)
            });

            info.data.Add(new StructureDataInfo()
            {
                data = showsUses.ToInt()
            });

            info.data.Add(new StructureDataInfo()
            {
                data = showsCooldown.ToInt()
            });
        }

        public void WriteData(BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);

            writer.Write(position.ToByte());
            writer.Write((byte)direction);

            writer.Write(uses);
            writer.Write(cooldown);
            writer.Write(unpressTime);
            writer.Write(showsUses);
            writer.Write(showsCooldown);
        }

        public void ReadData(byte version, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);

            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            uses = reader.ReadUInt16();
            cooldown = reader.ReadSingle();
            unpressTime = reader.ReadSingle();
            showsUses = reader.ReadBoolean();
            showsCooldown = reader.ReadBoolean();
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            base.InitializeVisual(visualObject);
            visualObject.GetComponent<SettingsComponent>().activateSettingsOn = this;
        }

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder);
        }

        public void SettingsClicked()
        {
            GenericPlateExchangeHandler handler = EditorController.Instance.CreateUI<GenericPlateExchangeHandler>(
                "GenericPlateConfig", AssetHelper.modPath + "Compats/LevelStudio/UI/GenericPlateConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }
    }

    internal class GenericPlateStructureLocation : StructureLocation
    {

        public const byte formatVersion = 0;

        public List<GenericPlateLocation> locations = new List<GenericPlateLocation>();

        public GenericPlateLocation CreateNewChild(EditorLevelData data,
            string prefabName, IntVector2 pos, Direction dir, bool disableChecks)
        {
            GenericPlateLocation loc = new GenericPlateLocation()
            {
                owner = this,
                prefabForBuilder = prefabName,
                position = pos,
                direction = dir,
                deleteAction = OnDeleteLocation
            };

            if (!disableChecks && !loc.ValidatePosition(data, ignoreSelf: true))
                return null;

            locations.Add(loc);

            return loc;
        }

        private bool OnDeleteLocation(EditorLevelData level, SimpleLocation loc)
        {
            locations.Remove((GenericPlateLocation)loc);
            EditorController.Instance.RemoveVisual(loc);

            return true;
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            foreach (GenericPlateLocation loc in locations)
            {
                loc.CompileData(structInfo);
            }

            return structInfo;
        }

        public override void CleanupVisual(GameObject visualObject)
        {

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
                    EditorController.Instance.RemoveVisual(locations[i]);
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
                GenericPlateLocation loc = CreateNewChild(data, null, default, default, disableChecks: true);
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
            compressor.AddStrings(locations.Select(x => x.prefabForBuilder));
        }
    }
}