/*using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.Virus
{
    public class Virus_AchooPrep : Virus_StateBase
    {
        private float inflatingTime;

        private float time;

        private Virus_Achoo achooState;

        private bool speedChanged;

        public Virus_AchooPrep(NPC npc, Virus_Infectious infectiousState , float inflatingTime, float bloatingTime, float maxAddendSize, float doNothingTime) : base(npc)
        {
            achooState = new Virus_Achoo(npc, infectiousState, bloatingTime, maxAddendSize, doNothingTime);
            this.inflatingTime = inflatingTime;
        }

        public override void Enter()
        {
            base.Enter();
            this.time = inflatingTime;
            Debug.Log("AchooPrep");
        }

        public override void Update()
        {
            base.Update();
            if (!speedChanged)
            {
                npc.Navigator.maxSpeed -= Time.deltaTime * npc.TimeScale * 5f;

                if (npc.Navigator.maxSpeed <= 0f)
                {
                    npc.Navigator.maxSpeed = 0f;
                    npc.behaviorStateMachine.ChangeNavigationState(new NavigationState_DoNothing(npc, 0));
                    speedChanged = true;
                }
            }

            if (speedChanged && time > 0f)
            {
                time -= Time.deltaTime * npc.TimeScale;

                if (time > 0f)
                {
                    npc.spriteRenderer[0].transform.localScale = Vector3.one + Vector3.one * achooState.MaxAddendSize * (inflatingTime - time) / inflatingTime;
                }
                else
                {
                    npc.spriteRenderer[0].transform.localScale = Vector3.one + Vector3.one * achooState.MaxAddendSize;
                    npc.behaviorStateMachine.ChangeState(achooState);
                }

            }
        }
        
    }
}
*/