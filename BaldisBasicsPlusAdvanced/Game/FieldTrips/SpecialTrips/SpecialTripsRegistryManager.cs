﻿using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips
{
    public class SpecialTripsRegistryManager
    {

        private static System.Random rng;

        private static List<FieldTripData> datas = new List<FieldTripData>();

        public static void InitializeRng()
        {
            rng = new System.Random(CoreGameManager.Instance.Seed());
        }

        public static void ResetRng() => rng = null;

        public static void Add(params FieldTripData[] data)
        {
            datas.AddRange(data);
        }

        public static FieldTripData GetRandomTripData()
        {
            return datas[rng.Next(0, datas.Count)];
        }

    }
}
