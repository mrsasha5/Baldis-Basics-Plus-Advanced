/*using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.Virus
{
    public class Virus_Achoo : Virus_StateBase
    {
        private float bloatTime;

        private float time;

        private float maxAddendSize;

        private Virus_Infectious infectiousState;

        private float doNothingTime;

        public float MaxAddendSize => maxAddendSize;

        //public float BloatTime => bloatTime;

        public Virus_Achoo(NPC npc, Virus_Infectious infectiousState, float bloatTime, float maxAddendSize, float doNothingTime) : base(npc)
        {
            this.infectiousState = infectiousState;
            this.bloatTime = bloatTime;
            this.maxAddendSize = maxAddendSize;
            this.doNothingTime = doNothingTime;
        }

        public override void Enter()
        {
            base.Enter();
            this.time = bloatTime;
            virus.Achoo();
            npc.behaviorStateMachine.ChangeNavigationState(new NavigationState_DoNothing(npc, 0));
            Debug.Log("Achoo");
        }

        public override void Update()
        {
            base.Update();
            if (time > 0f)
            {
                time -= Time.deltaTime * npc.TimeScale;

                if (time > 0f)
                {
                    npc.spriteRenderer[0].transform.localScale = Vector3.one + Vector3.one * maxAddendSize * time / bloatTime;
                }
                else
                {
                    npc.spriteRenderer[0].transform.localScale = Vector3.one;
                }
            }
            else if (doNothingTime > 0f)
            {
                doNothingTime -= Time.deltaTime * npc.TimeScale;

                if (doNothingTime <= 0f)
                {
                    if (infectiousState.Achoos > 0)
                        npc.behaviorStateMachine.ChangeState(infectiousState);
                    else
                    {
                        npc.behaviorStateMachine.ChangeState(new Virus_Wandering(npc));
                    }
                }
            }
        }
    }
}
*/