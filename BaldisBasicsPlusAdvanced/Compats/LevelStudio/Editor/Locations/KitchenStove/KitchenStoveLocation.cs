using System;
using System.IO;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Extensions;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.KitchenStove
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
}
