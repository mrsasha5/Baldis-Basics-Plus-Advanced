using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals;
using BaldisBasicsPlusAdvanced.Extensions;
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
    internal class AccelerationPlateLocation : SimpleLocation, IEditorSettingsable, IEditorMovable
    {
        public string prefabForBuilder;

        public ushort uses;

        public float cooldown = -1f;

        public float unpressTime;

        public float initialSpeed;

        public float acceleration;

        public bool showsUses;

        public bool showsCooldown;

        public AccelerationPlateStructureLocation owner;

        public SimpleButtonLocation button;

        public AccelerationPlateArrow[] arrows;

        private byte[] serializedArrows = new byte[0];

        public void LoadDefaults(string type)
        {
            Game.Objects.Plates.AccelerationPlate prefab = PlusStudioLevelLoader.LevelLoaderPlugin.Instance.structureAliases[type]
                .prefabAliases[prefabForBuilder].GetComponent<Game.Objects.Plates.AccelerationPlate>();

            uses = (ushort)prefab.Data.maxUses;
            unpressTime = prefab.Data.timeToUnpress;
            initialSpeed = prefab.initialSpeed;
            acceleration = prefab.acceleration;
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
                data = serializedArrows.Length
            });

            foreach (byte arrow in serializedArrows)
            {
                info.data.Add(new StructureDataInfo()
                {
                    //Why it's called WeirdTech
                    data = PlusStudioLevelLoader.WeirdTechExtensions.ConvertToIntNoRecast(arrow * 90f)
                });
            }

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
                data = PlusStudioLevelLoader.WeirdTechExtensions.ConvertToIntNoRecast(initialSpeed)
            });

            info.data.Add(new StructureDataInfo()
            {
                data = PlusStudioLevelLoader.WeirdTechExtensions.ConvertToIntNoRecast(acceleration)
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
            writer.Write((byte)CountActiveArrows());

            List<byte> _arrows = new List<byte>();

            for (byte i = 0; i < arrows.Length; i++)
            {
                if (arrows[i].directionActive)
                {
                    writer.Write(i);
                    _arrows.Add(i);
                }
            }

            serializedArrows = _arrows.ToArray();

            writer.Write(uses);
            writer.Write(cooldown);
            writer.Write(unpressTime);
            writer.Write(initialSpeed);
            writer.Write(acceleration);
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
            serializedArrows = new byte[reader.ReadByte()];

            for (byte i = 0; i < serializedArrows.Length; i++)
            {
                serializedArrows[i] = reader.ReadByte();
            }

            uses = reader.ReadUInt16();
            cooldown = reader.ReadSingle();
            unpressTime = reader.ReadSingle();
            initialSpeed = reader.ReadSingle();
            acceleration = reader.ReadSingle();
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
            visualObject.GetComponent<MoveAndSettingsComponent>().location = this;

            arrows = EditorController.Instance.GetVisual(this).GetComponentsInChildren<AccelerationPlateArrow>(includeInactive: true);
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].location = this;
                for (byte j = 0; j < serializedArrows.Length; j++)
                {
                    if (i == serializedArrows[j])
                    {
                        arrows[i].directionActive = true;
                        break;
                    }
                }
            }

            UpdateArrows();
        }

        public void SettingsClicked()
        {
            AccelerationPlateExchangeHandler handler = EditorController.Instance.CreateUI<AccelerationPlateExchangeHandler>(
                "AccelerationPlateConfig", AssetHelper.modPath + "Compats/LevelStudio/UI/AccelerationPlateConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }

        public void Selected()
        {
            if (button != null)
            {
                for (int i = 0; i < arrows.Length; i++)
                {
                    arrows[i].gameObject.SetActive(true);
                }
            }
        }

        public void Unselected()
        {
            if (button != null)
            {
                for (int i = 0; i < arrows.Length; i++)
                {
                    arrows[i].gameObject.SetActive(false);
                }
            }
        }

        public void MoveUpdate(Vector3? position, Quaternion? rotation)
        {

        }

        public Transform GetTransform()
        {
            return EditorController.Instance.GetVisual(this).transform;
        }

        public int CountActiveArrows()
        {
            int count = 0;
            for (int i = 0; i < arrows.Length; i++)
            {
                if (arrows[i].directionActive) count++;
            }
            return count;
        }

        public void UpdateArrows()
        {
            for (int i = 1; i < arrows.Length; i++) //First arrow is always active and doesn't need to be updated
            {
                arrows[i].UpdateVisual();
            }
        }

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder);
        }
    }

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
            EditorController.Instance.RemoveVisual(loc);
            if (((AccelerationPlateLocation)loc).button != null)
                EditorController.Instance.RemoveVisual(((AccelerationPlateLocation)loc).button);

            plates.Remove((AccelerationPlateLocation)loc);

            return true;
        }

        private bool OnDeleteButton(EditorLevelData level, SimpleLocation loc)
        {
            for (int i = 0; i < plates.Count; i++)
            {
                if (plates[i].button == loc)
                {
                    EditorController.Instance.RemoveVisual(plates[i]);
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
                if (plates[i].button != null) plates[i].button.position -= cellOffset;
            }
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            for (int i = 0; i < plates.Count; i++)
            {
                if (!plates[i].ValidatePosition(data, ignoreSelf: true) ||
                    plates[i].button != null && !plates[i].button.ValidatePosition(data, ignoreSelf: true))
                {
                    OnDeletePlate(data, plates[i]);
                    i--;
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