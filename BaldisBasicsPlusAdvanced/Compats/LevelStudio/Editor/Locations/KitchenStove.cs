using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Extensions;
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
    internal class KitchenStoveLocation : SimpleLocation, IEditorSettingsable
    {
        public string prefabForBuilder;

        public ushort uses;

        public float cooldown = -1f;

        public float cookingTime;

        public float coolingTime;

        public bool showsUses;

        public bool showsCooldown;

        public KitchenStoveStructureLocation owner;

        public void Compile(StructureInfo info)
        {
            info.data.Add(new StructureDataInfo()
            {
                prefab = prefabForBuilder,
                position = position.ToData(),
                direction = (PlusDirection)direction
            });

            info.data.Add(new StructureDataInfo()
            {
                data = uses
            });

            info.data.Add(new StructureDataInfo()
            {
                data = cooldown.ConvertToIntNoRecast()
            });

            info.data.Add(new StructureDataInfo()
            {
                data = cookingTime.ConvertToIntNoRecast()
            });

            info.data.Add(new StructureDataInfo()
            {
                data = coolingTime.ConvertToIntNoRecast()
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

        public void LoadDefaults()
        {
            Game.Objects.Plates.KitchenStove.KitchenStove prefab =
                LevelLoaderPlugin.Instance.structureAliases[owner.type].prefabAliases[prefabForBuilder]
                .GetComponent<Game.Objects.Plates.KitchenStove.KitchenStove>();

            uses = (ushort)prefab.Data.maxUses;

            cookingTime = prefab.CookingTime;
            coolingTime = prefab.CoolingTime;

            showsUses = prefab.Data.showsUses;
            showsCooldown = prefab.Data.showsCooldown;
        }

        public void ReadData(byte version, EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);
            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            uses = reader.ReadUInt16();
            cooldown = reader.ReadSingle();
            cookingTime = reader.ReadSingle();
            coolingTime = reader.ReadSingle();
            showsUses = reader.ReadBoolean();
            showsCooldown = reader.ReadBoolean();
        }

        public void WriteData(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);
            writer.Write(position.ToByte());
            writer.Write((byte)direction);

            writer.Write(uses);
            writer.Write(cooldown);
            writer.Write(cookingTime);
            writer.Write(coolingTime);
            writer.Write(showsUses);
            writer.Write(showsCooldown);
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            base.InitializeVisual(visualObject);
            visualObject.GetComponent<SettingsComponent>().activateSettingsOn = this;
        }

        public void SettingsClicked()
        {
            KitchenStoveExchangeHandler handler = EditorController.Instance.CreateUI<KitchenStoveExchangeHandler>(
                "KitchenStoveConfig", AssetHelper.modPath + "Compats/LevelStudio/UI/KitchenStoveConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }

        public override GameObject GetVisualPrefab()
        {
            return LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder);
        }
    }

    internal class KitchenStoveStructureLocation : StructureLocation
    {
        public const byte formatVersion = 0;

        public List<KitchenStoveLocation> stoves = new List<KitchenStoveLocation>();

        public List<SimpleButtonLocation> buttons = new List<SimpleButtonLocation>();

        public KitchenStoveLocation CreateNewStove(EditorLevelData data,
            string prefab, IntVector2 pos, Direction dir, bool disableChecks = false)
        {
            KitchenStoveLocation loc = new KitchenStoveLocation()
            {
                owner = this,
                prefabForBuilder = prefab,
                position = pos,
                direction = dir,
                deleteAction = OnDeleteStove
            };

            if (!disableChecks && !loc.ValidatePosition(data, ignoreSelf: true))
                return null;

            stoves.Add(loc);

            return loc;
        }

        public SimpleButtonLocation CreateNewButton(EditorLevelData data, IntVector2 pos, Direction dir, bool disableChecks = false)
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

        private bool OnDeleteStove(EditorLevelData data, SimpleLocation loc)
        {
            int index = stoves.IndexOf((KitchenStoveLocation)loc);

            EditorController.Instance.RemoveVisual(loc);
            EditorController.Instance.RemoveVisual(buttons[index]);

            stoves.RemoveAt(index);
            buttons.RemoveAt(index);

            return true;
        }

        private bool OnDeleteButton(EditorLevelData data, SimpleLocation loc)
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
                CreateNewStove(data, null, default, default, disableChecks: true)
                    .ReadData(ver, data, reader, compressor);

                IntVector2 pos = reader.ReadByteVector2().ToInt();
                Direction dir = (Direction)reader.ReadByte();

                CreateNewButton(data, pos, dir, disableChecks: true);
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