using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game
{
    public class BaseSpawningData
    {
        private string name;

        private bool endlessMode = false;

        private bool editor = true;

        private bool neverAppear = false;

        private int[] bannedFloors = new int[0];

        public int[] BannedFloors => bannedFloors;

        public string Name => name;

        public bool EndlessMode => endlessMode;

        public bool Editor => editor;

        public bool NeverAppear => neverAppear;

        public BaseSpawningData(string key)
        {
            name = key;
        }

        public BaseSpawningData setEndless(bool endlessMode)
        {
            this.endlessMode = endlessMode;
            return this;
        }

        public BaseSpawningData setEditor(bool editorMode)
        {
            this.editor = editorMode;
            return this;
        }

        public BaseSpawningData setBannedFloors(params int[] floors)
        {
            bannedFloors = floors;
            return this;
        }

        public virtual BaseSpawningData setFalseEverywhere()
        {
            endlessMode = false;
            editor = false;
            neverAppear = false;
            setBannedFloors();
            return this;
        }

        public T convertTo<T>() where T : BaseSpawningData
        {
            return (T)this;
        }

    }
}
