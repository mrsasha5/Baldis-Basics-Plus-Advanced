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

        public BaseSpawningData SetEndless(bool endlessMode)
        {
            this.endlessMode = endlessMode;
            return this;
        }

        public BaseSpawningData SetEditor(bool editorMode)
        {
            this.editor = editorMode;
            return this;
        }

        public BaseSpawningData SetBannedFloors(params int[] floors)
        {
            bannedFloors = floors;
            return this;
        }

        public virtual BaseSpawningData SetFalseEverywhere()
        {
            endlessMode = false;
            editor = false;
            neverAppear = false;
            SetBannedFloors();
            return this;
        }

        public T ConvertTo<T>() where T : BaseSpawningData
        {
            return (T)this;
        }

    }
}
