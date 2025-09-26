using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips
{
    public class SpecialTripsRegistryManager
    {

        private static System.Random rng;

        private static List<FieldTripData> data = new List<FieldTripData>();

        public static void InitializeRng()
        {
            rng = new System.Random(CoreGameManager.Instance.Seed());
        }

        public static void ResetRng() => rng = null;

        public static void Add(params FieldTripData[] data)
        {
            SpecialTripsRegistryManager.data.AddRange(data);
        }

        public static FieldTripData GetRandomTripData()
        {
            return data[rng.Next(0, data.Count)];
        }

    }
}
