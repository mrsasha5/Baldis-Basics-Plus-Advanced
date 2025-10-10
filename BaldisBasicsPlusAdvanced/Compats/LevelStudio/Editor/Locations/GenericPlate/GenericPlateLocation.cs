using System.IO;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.GenericPlate
{
    internal class GenericPlateLocation : SimpleLocation, IEditorSettingsable
    {

        public string prefabName;

        public int cooldown;

        public int uses;

        public float unpressTime;

        public override void InitializeVisual(GameObject visualObject)
        {
            base.InitializeVisual(visualObject);
            visualObject.GetComponent<SettingsComponent>().activateSettingsOn = this;
        }

        public void LoadDefaultParameters(string type)
        {
            BasePlate prefab = 
                PlusStudioLevelLoader.
                    LevelLoaderPlugin.Instance.structureAliases[type].prefabAliases[prefabName].GetComponent<BasePlate>();

            uses = prefab.Data.maxUses;
            unpressTime = prefab.Data.timeToUnpress;
        }

        public void WriteData(BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabName);

            writer.Write(position.ToByte());
            writer.Write((byte)direction);

            writer.Write((byte)3);
            writer.Write(cooldown);
            writer.Write(uses);
            writer.Write(unpressTime);
        }

        public void ReadData(byte version, BinaryReader reader, StringCompressor compressor)
        {
            prefabName = compressor.ReadStoredString(reader);

            prefab = LevelStudioIntegration.genericPlateVisuals[prefabName];

            position = reader.ReadByteVector2().ToInt();
            direction = (Direction)reader.ReadByte();

            byte parameters = reader.ReadByte();

            if (parameters > 0)
                cooldown = reader.ReadInt32();
            if (parameters > 1)
                uses = reader.ReadInt32();
            if (parameters > 2)
                unpressTime = reader.ReadSingle();
        }

        public void SettingsClicked()
        {
            GenericPlateExchangeHandler handler = EditorController.Instance.CreateUI<GenericPlateExchangeHandler>(
                "GenericPlateConfig", AssetsHelper.modPath + "Compats/LevelStudio/UI/GenericPlateConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }
    }
}
