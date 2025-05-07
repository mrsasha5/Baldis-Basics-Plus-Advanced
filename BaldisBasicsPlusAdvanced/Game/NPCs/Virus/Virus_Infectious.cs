/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects;
using MTM101BaldAPI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.Virus
{
    public class Virus_Infectious : Virus_StateBase
    {
        protected PlayerManager pm;

        protected float cooldown;

        private int? achoos;

        public int Achoos => (int)achoos;

        public Virus_Infectious(NPC npc, int? achoos = null) : base(npc)
        {
            this.achoos = achoos;
        }

        public override void Enter()
        {
            base.Enter();
            //npc.spriteRenderer[0].material.SetColor(Color.red);
            if (achoos == null) achoos = GenerateAchoosCount();
            virus.SetRunningSpeed();
            cooldown = NewCooldown();
            npc.behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            Debug.Log("Infectious");
        }

        public override void Exit()
        {
            base.Exit();
            //npc.spriteRenderer[0].material.SetColor(Color.white);
        }

        public override void Update()
        {
            base.Update();
            if (cooldown > 0f && achoos > 0)
            {
                cooldown -= Time.deltaTime * npc.TimeScale;

                if (cooldown <= 0f)
                {
                    npc.behaviorStateMachine.ChangeState(new Virus_AchooPrep(npc, this, 
                        inflatingTime: 1f, bloatingTime: 0.25f, maxAddendSize: 1f, doNothingTime: 2f));
                    achoos--;
                }
            }
            else if (achoos <= 0)
            {
                npc.behaviorStateMachine.ChangeState(new Virus_Wandering(npc));
            }
        }

        private int GenerateAchoosCount()
        {
            return Random.Range(3, 8);
        }

        private float NewCooldown()
        {
            return Random.Range(10f, 20f);
        }

    }
}
*/