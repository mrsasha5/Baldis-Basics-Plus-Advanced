using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class NPCController : BaseController
    {
        protected NPC npc;

        protected NPCControllerSystem nc;

        public virtual void initialize(NPC npc, PlayerControllerSystem pc)
        {
            this.npc = npc;
            this.nc = npc.getControllerSystem();
            entity = npc.GetComponent<Entity>();
            this.ec = npc.ec;
            this.pc = pc;
        }

    }
}
