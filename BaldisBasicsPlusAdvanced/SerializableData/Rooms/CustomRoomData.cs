using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BaldisBasicsPlusAdvanced.Helpers;
using Newtonsoft.Json;

namespace BaldisBasicsPlusAdvanced.SerializableData.Rooms
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

        public bool? autoAssignRoomFunctionContainer;

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

        public void InheritProperties()
        {
            if (inheritPaths != null)
            {
                for (int i = 0; i < inheritPaths.Length; i++)
                {
                    CustomRoomData data =
                    JsonConvert.DeserializeObject<CustomRoomData>(
                        File.ReadAllText(AssetsHelper.modPath + "Premades/Rooms/Patterns/" + inheritPaths[i]));
                    data.InheritProperties();
                    InheritFrom(data);
                }
            }
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
