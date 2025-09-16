using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Game.Spawning
{
    public class BaseSpawningData
    {
        private string name;

        private bool endlessMode = false;

        private bool editor = true;

        private int[] bannedFloors = new int[0];

        private LevelType[] levelTypes = new LevelType[0];

        private Dictionary<int, int> weights = new Dictionary<int, int>();

        private bool forced;

        public int[] BannedFloors => bannedFloors;

        public string Name => name;

        public bool EndlessMode => endlessMode;

        public bool Editor => editor;

        public bool Forced => forced;

        public LevelType[] LevelTypes => levelTypes;


        public BaseSpawningData(string key)
        {
            name = key;
        }

        public int GetWeight(int floor)
        {
            return weights.GetWeight(floor);
        }

        public BaseSpawningData SetLevelTypes(params LevelType[] levels)
        {
            levelTypes = levels;
            return this;
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
            SetLevelTypes();
            return this;
        }

        public T ConvertTo<T>() where T : BaseSpawningData
        {
            return (T)this;
        }

    }
}
