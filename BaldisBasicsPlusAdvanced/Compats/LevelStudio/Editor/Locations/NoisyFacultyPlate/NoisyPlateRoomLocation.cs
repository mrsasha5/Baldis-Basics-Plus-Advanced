using System.Collections.Generic;
using System.IO;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyFacultyPlate
{
    internal class NoisyPlateRoomLocation : IEditorSettingsable, IEditorDeletable
    {

        //Serializable
        public string builderPrefab;

        public int cooldown;

        public int uses;

        public int generosity;

        public int pointsPerAlarm;

        //Not serializable
        public EditorRoom room;

        public List<GameObject> allocatedPlates = new List<GameObject>();

        public NoisyPlateStructureLocation owner;

        public void LoadDefaultParameters()
        {
            NoisyPlate prefab = LevelLoaderPlugin.Instance.structureAliases[owner.type].prefabAliases[builderPrefab]
                .GetComponent<NoisyPlate>();

            cooldown = (int)prefab.Cooldown;
            uses = prefab.Data.maxUses;
            generosity = prefab.Generosity;
            pointsPerAlarm = prefab.PointsReward;
        }

        public void ReadParameters(byte version, BinaryReader reader)
        {
            byte paramsCount = reader.ReadByte();

            if (paramsCount > 0)
                cooldown = reader.ReadInt32();

            if (paramsCount > 1)
                uses = reader.ReadInt32();

            if (paramsCount > 2)
                generosity = reader.ReadInt32();

            if (paramsCount > 3)
                pointsPerAlarm = reader.ReadInt32();
        }

        public void WriteParameters(BinaryWriter writer)
        {
            writer.Write((byte)4);
            writer.Write(cooldown);
            writer.Write(uses);
            writer.Write(generosity);
            writer.Write(pointsPerAlarm);
        }

        public void SettingsClicked()
        {
            NoisyPlateExchangeHandler handler = EditorController.Instance.CreateUI<NoisyPlateExchangeHandler>(
                "NoisyRoomConfig", AssetsHelper.modPath + "Compats/LevelStudio/UI/NoisyRoomConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }

        public bool OnDelete(EditorLevelData data)
        {
            owner.OnRoomDelete(this, true);
            return true;
        }
        
    }
}
