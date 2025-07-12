using System;
using System.Reflection;
using Newtonsoft.Json;

namespace BaldisBasicsPlusAdvanced.SerializableData.Rooms
{
    [JsonObject]
    public class CustomRoomData
    {
        public int? weight;

        public int? minItemValue;

        public int? maxItemValue;

        public string categoryName;

        public string doorMatsName;

        public string lightPre;

        public string functionContainerName;

        public bool? isOffLimits;

        public bool? autoAssignRoomFunctionContainer;

        public bool? keepTextures;

        public int[] bannedFloors;

        public string[] levelTypes;

        public bool? endlessMode;

        public bool? isAHallway;

        public bool? isPotentialPostPlotSpecialHall;

        public bool? squaredShape;

        public string inheritPath;

        [NonSerialized]
        public WeightedRoomAsset weightedRoomAsset;

        public void InheritFrom(CustomRoomData roomData)
        {
            FieldInfo[] fields = typeof(CustomRoomData).GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].GetCustomAttribute<NonSerializedAttribute>() == null && fields[i].GetValue(this) == null)
                {
                    fields[i].SetValue(this, fields[i].GetValue(roomData));
                }
            }

        }

    }
}
