using BaldisBasicsPlusAdvanced.Cache;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.CrissTheCrystal
{
    public class CrissTheCrystal_Crazy : CrissTheCrystal_StateBase
    {
        private float time;

        private float baseTime;

        private Color color;

        public CrissTheCrystal_Crazy(CrissTheCrystal criss) : base(criss)
        {
        }

        public override void Enter()
        {
            base.Enter();
            baseTime = 20f;
            time = baseTime;
            color = new Color(1f, 0f, 0f, 1f);
            criss.Animator.SetDefaultAnimation("Crazy_Running", 50f);
            criss.Animator.Play("TurnsCrazy", 1f);
            criss.StartCoroutine(WaitsAnimation());
        }

        private IEnumerator WaitsAnimation()
        {
            while (criss.Animator.currentAnimationName == "TurnsCrazy")
            {
                yield return null;
            }
            criss.SetRunSpeed();
            criss.PlayRandomVoice("AfterAttack");
            npc.behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }

        public override void Update()
        {
            base.Update();
            if (time > 0f)
            {
                time -= Time.deltaTime * criss.TimeScale;

                color.g = (baseTime - time) / baseTime;
                color.b = color.g;
                npc.spriteRenderer[0].color = color;

                if (time <= 0f)
                {
                    npc.behaviorStateMachine.ChangeState(new CrissTheCrystal_Wandering(criss));
                }
            }
        }

        public override void OnStateTriggerEnter(Collider other)
        {
            base.OnStateTriggerEnter(other);
            if (other.TryGetComponent(out Entity entity))
            {
                entity.AddForce(new Force((entity.transform.position - criss.transform.position).normalized, 50f, -50f));
                criss.AudMan.PlaySingle(AssetsStorage.sounds["bang"]);
            }
        }
    }
}
