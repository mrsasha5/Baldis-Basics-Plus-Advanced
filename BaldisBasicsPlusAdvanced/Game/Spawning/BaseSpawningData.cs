using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Spawning
{
    public class BaseSpawningData
    {
        private string name;

        private bool endlessMode = false;

        private bool editor = true;

        //private bool neverAppear = false;

        private int[] bannedFloors = new int[0];

        private Dictionary<int, int> weights = new Dictionary<int, int>();

        private bool forced;

        public int[] BannedFloors => bannedFloors;

        public string Name => name;

        public bool EndlessMode => endlessMode;

        public bool Editor => editor;

        public bool Forced => forced;

        //public Dictionary<int, int> Weights => weights;

        public BaseSpawningData(string key)
        {
            name = key;
        }

        public int GetWeight(int floor)
        {
            if (weights.Count == 0) return 0;

            if (weights.ContainsKey(floor))
            {
                return weights[floor];
            }
            else
            {
                int nearestFloor = MathHelper.FindNearestValue(weights.Keys.ToArray(), floor);

                return weights[nearestFloor];
            }
        }

        public BaseSpawningData SetWeight(int floor, int weight)
        {
            if (!weights.ContainsKey(floor))
            {
                weights.Add(floor, weight);
            }
            else
            {
                weights[floor] = weight;
            }
            return this;
        }

        public BaseSpawningData SetForced(bool isForced)
        {
            forced = isForced;
            return this;
        }

        public BaseSpawningData SetEndless(bool endlessMode)
        {
            this.endlessMode = endlessMode;
            return this;
        }

        public BaseSpawningData SetEditor(bool editorMode)
        {
            editor = editorMode;
            return this;
        }

        public BaseSpawningData SetBannedFloors(params int[] floors)
        {
            bannedFloors = floors;
            return this;
        }

        public virtual BaseSpawningData DoNotSpawn()
        {
            endlessMode = false;
            editor = false;
            weights.Clear();
            forced = false;
            SetBannedFloors();
            return this;
        }

        public T ConvertTo<T>() where T : BaseSpawningData
        {
            return (T)this;
        }

    }
}
