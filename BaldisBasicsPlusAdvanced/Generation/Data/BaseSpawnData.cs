using System;
using System.Linq;
using HarmonyLib;
using MTM101BaldAPI;
using Newtonsoft.Json;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    internal struct StandardGenerationData
    {
        public static int[] _intArr = new int[0];
        public static LevelType[] _levelTypeArr = new LevelType[0];
        public static WeightData[] _weightDataArr = new WeightData[0];

        private int[] floors;

        private LevelType[] levelTypes;

        private WeightData[] weights;

        [JsonProperty("bannedFloors")]
        public int[] BannedFloors
        {
            get
            {
                if (floors == null) return _intArr;
                return floors;
            }
            set
            {
                floors = value;
            }
        }

        public LevelType[] LevelTypes
        {
            get
            {
                if (levelTypes == null) return _levelTypeArr;
                return levelTypes;
            }
            set
            {
                levelTypes = value;
            }
        }

        [JsonProperty("levelTypes")]
        private string[] Serialization_LevelTypes
        {
            set
            {
                levelTypes = new LevelType[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    levelTypes[i] = EnumExtensions.GetFromExtendedName<LevelType>(value[i]);
                }
            }
        }

        [JsonProperty("weights")]
        public WeightData[] Weights
        {
            get
            {
                if (weights == null) return _weightDataArr;
                return weights;
            }
            set
            {
                weights = value;
            }
        }

        public bool IsFloorIncluded(int floor, LevelType levelType)
        {
            return !BannedFloors.Contains(floor) && LevelTypes.Contains(levelType);
        }

        public int GetWeight(int floor, LevelType levelType)
        {
            if (Weights.Length == 0 || !IsFloorIncluded(floor, levelType)) return 0;

            for (int i = 0; i < Weights.Length; i++)
            {
                if (Weights[i].floor == floor)
                {
                    return Weights[i].weight;
                }
            }

            return WeightData.FindNearestWeightForFloor(Weights, floor);
        }
    }

    internal interface ISpawnData<T> : IStandardSpawnData
    {
        T Instance { get; }
    }

    internal interface IStandardSpawnData
    {
        int GetWeight(int floor, LevelType levelType);
        IStandardSpawnData SetBannedFloors(params int[] floors);
        IStandardSpawnData SetLevelTypes(params LevelType[] levelTypes);
        IStandardSpawnData AddWeight(int floor, int weight);
        void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject);
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class BaseSpawnData<T> : ISpawnData<T>
    {
        protected StandardGenerationData standardData;

        protected T instance;

        public T Instance => instance;

        public IStandardSpawnData AddWeight(int floor, int weight)
        {
            standardData.Weights = standardData.Weights.AddToArray(new WeightData(floor, weight));
            return this;
        }

        public IStandardSpawnData SetBannedFloors(params int[] floors)
        {
            standardData.BannedFloors = floors;
            return this;
        }

        public IStandardSpawnData SetLevelTypes(params LevelType[] levelTypes)
        {
            standardData.LevelTypes = levelTypes;
            return this;
        }

        public virtual int GetWeight(int floor, LevelType levelType)
        {
            return standardData.GetWeight(floor, levelType);
        }

        public virtual void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            throw new NotImplementedException();
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal struct WeightData
    {
        [JsonProperty]
        public int floor;

        [JsonProperty]
        public int weight;

        public WeightData(int floor, int weight)
        {
            this.floor = floor;
            this.weight = weight;
        }

        public static int FindNearestWeightForFloor(WeightData[] values, int floor)
        {
            if (values.Length == 0) return 0;

            int difference = Mathf.Abs(values[0].floor - floor);
            int nearestFloorIndex = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (Mathf.Abs(values[i].floor - floor) < difference)
                {
                    difference = Mathf.Abs(values[i].floor - floor);
                    nearestFloorIndex = i;
                }
            }
            return values[nearestFloorIndex].weight;
        }
    }
}
