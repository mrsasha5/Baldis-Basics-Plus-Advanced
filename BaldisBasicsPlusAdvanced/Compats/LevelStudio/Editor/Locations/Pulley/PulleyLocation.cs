using System.IO;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Pulley
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
                "PulleyConfig", AssetsHelper.modPath + "Compats/LevelStudio/UI/PulleyConfig.json");
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
}
