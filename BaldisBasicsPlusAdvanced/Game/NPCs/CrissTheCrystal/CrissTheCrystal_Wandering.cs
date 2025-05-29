using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.CrissTheCrystal
{
    public class CrissTheCrystal_Wandering : CrissTheCrystal_StateBase
    {
        private float time;

        private float phraseTime;

        public CrissTheCrystal_Wandering(CrissTheCrystal criss) : base(criss)
        {

        }

        public void SetTime(float time)
        {
            this.time = time;
        }

        public override void Enter()
        {
            base.Enter();
            time = Random.Range(criss.MinMaxCooldown.x, criss.MinMaxCooldown.y);
            phraseTime = Random.Range(20f, 40f);
            criss.Animator.SetDefaultAnimation("Walking", 1f);
            criss.SetWalkSpeed();
            npc.behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }

        public override void Update()
        {
            base.Update();
            if (time > 0f)
            {
                time -= Time.deltaTime * npc.TimeScale;
            }
            if (phraseTime > 0f)
            {
                phraseTime -= Time.deltaTime * npc.TimeScale;
                if (phraseTime <= 0f)
                {
                    phraseTime = Random.Range(20f, 40f);
                    criss.PlayRandomVoice("Wander");
                }
            }
        }

        public override void InPlayerSight(PlayerManager player)
        {
            base.InPlayerSight(player);
            if (time <= 0f && !player.Tagged)
            {
                npc.behaviorStateMachine.ChangeState(new CrissTheCrystal_ShootingLaser(
                    player.GetComponent<Entity>(), criss));
            }
        }
    }
}
