/*using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.Virus
{
    public class Virus_Wandering : Virus_StateBase
    {
        private float time;

        public Virus_Wandering(NPC npc) : base(npc)
        {
        }

        public override void Enter()
        {
            base.Enter();
            time = Random.Range(30f, 75f);
            virus.SetWalkSpeed();
            npc.behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            Debug.Log("Wandering");
        }

        public override void Update()
        {
            base.Update();
            if (time > 0f)
            {
                time -= Time.deltaTime * npc.TimeScale;

                if (time <= 0f)
                {
                    npc.behaviorStateMachine.ChangeState(new Virus_Infectious(npc));
                }
            }
        }
    }
}
*/