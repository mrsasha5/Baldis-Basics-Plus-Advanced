using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using Newtonsoft.Json;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SerializableData
{
    [JsonObject]
    public class CustomRoomData
    {
        public Dictionary<int, int> weights;

        public int? minItemValue;

        public int? maxItemValue;

        public string categoryName;

        public string doorMatsName;

        public string lightPre;

        public string functionContainerName;

        public bool? offLimits;

        public bool? keepTextures;

        public int[] bannedFloors;

        public string[] levelTypes;

        public bool? endlessMode;

        public bool? isAHallway;

        public bool? isPotentialPostPlotSpecialHall;

        public bool? squaredShape;

        public string[] inheritPaths;

        [NonSerialized]
        public RoomAsset roomAsset;

        public static CustomRoomData RoomFromFile(string path)
        {
            string folderPath = path.Replace(Path.GetFileName(path), "");

            if (!File.Exists(folderPath + Path.GetFileNameWithoutExtension(path) + ".json"))
                return null;

            string jsonData = File.ReadAllText(folderPath + Path.GetFileNameWithoutExtension(path) + ".json");

            CustomRoomData roomData = JsonConvert.DeserializeObject<CustomRoomData>(jsonData);

            roomData.InheritProperties();

            BinaryReader reader = new BinaryReader(File.OpenRead(path));
            BaldiRoomAsset formatAsset = BaldiRoomAsset.Read(reader);
            reader.Close();

            RoomAsset roomAsset = LevelImporter.CreateVanillaRoomAsset(formatAsset);
            roomData.roomAsset = roomAsset;
            roomData.LoadRoomAssetProperties();

            return roomData;
        }

        public void InheritProperties()
        {
            if (inheritPaths != null)
            {
                for (int i = 0; i < inheritPaths.Length; i++)
                {
                    CustomRoomData data =
                    JsonConvert.DeserializeObject<CustomRoomData>(
                        File.ReadAllText(AssetHelper.modPath + "Data/Rooms/Patterns/" + inheritPaths[i]));
                    data.InheritProperties();
                    InheritFrom(data);
                }
            }
        }

        public void LoadRoomAssetProperties()
        {
            if (!string.IsNullOrEmpty(functionContainerName))
            {
                roomAsset.roomFunctionContainer = AssetHelper.LoadAsset<RoomFunctionContainer>(functionContainerName);
            }

            if (minItemValue != null) roomAsset.minItemValue = (int)minItemValue;
            if (maxItemValue != null) roomAsset.maxItemValue = (int)maxItemValue;
            if (keepTextures != null) roomAsset.keepTextures = (bool)keepTextures;
            if (offLimits != null) roomAsset.offLimits = (bool)offLimits;

            if (!string.IsNullOrEmpty(doorMatsName))
                roomAsset.doorMats = Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                x => x.name == doorMatsName);

            if (!string.IsNullOrEmpty(lightPre))
            {
                roomAsset.lightPre = AssetHelper.LoadAsset<Transform>(lightPre);
            }

            roomAsset.category = EnumExtensions.GetFromExtendedName<RoomCategory>(categoryName);
        }

        private void InheritFrom(CustomRoomData roomData)
        {
            FieldInfo[] fields = typeof(CustomRoomData).
                GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].GetValue(roomData) != null && 
                    fields[i].GetCustomAttribute<NonSerializedAttribute>() == null && 
                    fields[i].GetValue(this) == null)
                {
                    fields[i].SetValue(this, fields[i].GetValue(roomData));
                }
            }

        }

    }
}
