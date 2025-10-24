using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System.IO;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using PlusLevelStudio;
using BaldisBasicsPlusAdvanced.Extensions;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.AccelerationPlate
{
    internal class AccelerationPlateLocation : SimpleLocation
    {

        public string prefabForBuilder;

        public ushort uses;

        public float cooldown;

        public float unpressTime;

        public bool showsUses;

        public bool showsCooldown;

        public AccelerationPlateStructureLocation owner;

        public SimpleButtonLocation button;

        public AccelerationPlateArrow[] arrows;

        public byte directionsCount = 1;

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
                direction = (PlusDirection)direction,
                data = directionsCount
            });

            info.data.Add(new StructureDataInfo()
            {
                data = uses
            });

            //Why it's called WeirdTech
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

            if (button != null)
            {
                info.data.Add(new StructureDataInfo()
                {
                    position = PlusStudioLevelLoader.Extensions.ToData(button.position),
                    direction = (PlusDirection)button.direction
                });
            }
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

            writer.Write(button != null);
            if (button != null)
            {
                writer.Write(button.position.ToByte());
                writer.Write((byte)button.direction);
            }
        }

        public void ReadData(byte version, EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);

            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            uses = reader.ReadUInt16();
            cooldown = reader.ReadSingle();
            unpressTime = reader.ReadSingle();
            showsUses = reader.ReadBoolean();
            showsCooldown = reader.ReadBoolean();

            if (reader.ReadBoolean())
            {
                owner.CreateNewButton(data, this, PlusStudioLevelLoader.Extensions.ToInt(reader.ReadByteVector2()), 
                    (Direction)reader.ReadByte(), disableChecks: true);
            }
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            base.InitializeVisual(visualObject);
            arrows = visualObject.GetComponentsInChildren<AccelerationPlateArrow>();
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].location = this;
            }
            UpdateArrows();
        }

        public void UpdateArrows()
        {
            for (int i = 1; i < 4; i++)
            {
                arrows[i].renderer.color = Color.gray;
            }

            for (int i = 1; i < directionsCount; i++)
            {
                arrows[i].renderer.color = Color.white;
            }
        }

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder);
        }
    }
}
