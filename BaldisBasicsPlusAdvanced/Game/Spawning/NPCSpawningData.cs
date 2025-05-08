using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.Spawning
{
    public class NPCSpawningData : BaseSpawningData
    {

        private NPC npc;

        public NPC Npc => npc;

        public NPCSpawningData(string key, NPC npc) : base(key)
        {
            this.npc = npc;
        }

    }
}
