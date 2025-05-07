using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.Behavior.States
{
    public class NPC_Confused : NpcState
    {
        private NpcState previousState;

        private float time;

        public NPC_Confused(NPC npc, NpcState previousState, float duration) : base(npc)
        {
            this.previousState = previousState;
            time = duration;
        }

        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime * npc.TimeScale;
            if (time <= 0f)
            {
                npc.behaviorStateMachine.ChangeState(previousState);
            }
        }

    }
}
