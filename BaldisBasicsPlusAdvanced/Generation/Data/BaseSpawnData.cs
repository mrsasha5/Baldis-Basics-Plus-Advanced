using System;
using System.Linq;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class BaseSpawnData<T> : ISpawnData<T>
    {
        internal static int[] _intArr = new int[0];
        internal static LevelType[] _levelTypeArr = new LevelType[0];
        internal static WeightData[] _weightDataArr = new WeightData[0];

        protected int[] bannedFloors = _intArr;

        protected LevelType[] levelTypes = _levelTypeArr;

        protected WeightData[] weights = _weightDataArr;

        protected T instance;

        public int[] BannedFloors => bannedFloors;

        public LevelType[] LevelTypes => levelTypes;

        public WeightData[] Weights => weights;

        public T Instance => instance;

        public IStandardSpawnData AddWeight(int floor, int weight)
        {
            weights = weights.AddToArray(new WeightData(floor, weight));
            return this;
        }

        public IStandardSpawnData SetBannedFloors(params int[] floors)
        {
            bannedFloors = floors;
            return this;
        }

        public IStandardSpawnData SetLevelTypes(params LevelType[] levelTypes)
        {
            this.levelTypes = levelTypes;
            return this;
        }

        public int GetWeight(int floor, LevelType levelType)
        {
            if (Weights.Length == 0 || BannedFloors.Contains(floor) || !LevelTypes.Contains(levelType)) return 0;

            for (int i = 0; i < Weights.Length; i++)
            {
                if (Weights[i].floor == floor)
                {
                    return Weights[i].weight;
                }
            }

            return FindNearestWeightForFloor(Weights, floor);
        }

        public virtual void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            throw new NotImplementedException();
        }

        protected int FindNearestWeightForFloor(WeightData[] values, int floor)
        {
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
