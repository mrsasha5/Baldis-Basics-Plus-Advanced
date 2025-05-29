using System;

namespace BaldisBasicsPlusAdvanced.SerializableData
{
    [Serializable]
    public class CustomRoomData
    {
        public int weight;

        public int minItemValue;

        public int maxItemValue;

        public string categoryName;

        public string doorMatsName;

        public string lightPre;

        public bool isOffLimits;

        public bool autoAssignRoomFunctionContainer;

        public bool keepTextures;

        public int[] bannedFloors;

        public string[] levelTypes;

        public bool endlessMode;

        public bool isAHallway;

        public bool isPotentialPrePlotSpecialHall;

        public bool isPotentialPostPlotSpecialHall;

        public bool squaredShape;

        [NonSerialized]
        public WeightedRoomAsset weightedRoomAsset;

    }
}
