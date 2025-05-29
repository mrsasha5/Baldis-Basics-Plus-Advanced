using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using Rewired;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.CrissTheCrystal
{
    public class CrissTheCrystal_ShootingLaser : CrissTheCrystal_StateBase
    {
        private Entity target;

        private bool playerInSight;

        private Vector3 laserDirection;

        public CrissTheCrystal_ShootingLaser(Entity entity, CrissTheCrystal criss) : base(criss)
        {
            this.target = entity;
        }

        public override void Enter()
        {
            base.Enter();
            npc.StartCoroutine(Waits());
        }

        private IEnumerator ShootLaser()
        {
            npc.Navigator.maxSpeed = 0;
            npc.Navigator.SetSpeed(0f);
            //state.priority = 0;

            UpdateDirection();

            RaycastHit hit;

            float baseTime = 5f;
            float time = baseTime;

            Vector3[] positions = new Vector3[2];

            Color color = new Color(1f, 1f, 1f);

            criss.Laser.SetActive(true);

            while (time > 0f)
            {
                time -= Time.deltaTime * npc.TimeScale;

                color.g = time / baseTime;
                color.b = color.g;
                npc.spriteRenderer[0].color = color;

                if (target == null) break;

                Physics.Raycast(npc.transform.position, laserDirection, out hit, float.PositiveInfinity, LayersHelper.gumCollisionMask, 
                    QueryTriggerInteraction.Ignore);

                positions[0] = npc.transform.position - Vector3.up * 1f;
                positions[1] = npc.transform.position + laserDirection * hit.distance - Vector3.up * 1f;

                criss.Laser.SetLaserPositions(positions);
                yield return null;
            }

            criss.Laser.SetActive(false);

            npc.behaviorStateMachine.ChangeState(new CrissTheCrystal_Crazy(criss));
        }

        private void UpdateDirection()
        {
            if (playerInSight)
            {
                laserDirection = GetDirection();
            }
        }

        private Vector3 GetDirection()
        {
            return Directions.DirFromVector3(target.transform.position - npc.transform.position, 45f).ToVector3();
        }

        private IEnumerator Waits()
        {
            npc.Navigator.FindPath(target.transform.position);

            criss.AudMan.FlushQueue(true);
            criss.PlayRandomVoice("Attack");
            ChangeNavigationState(new NavigationState_TargetPosition(npc, 63, npc.Navigator.NextPoint));

            UpdateDirection();

            while (npc.Navigator.CurrentDestination != Vector3.zero
                || criss.AudMan.AnyAudioIsPlaying)
            {
                UpdateDirection();
                yield return null;
            }

            criss.Animator.SetDefaultAnimation("Shooting", 10f);
            criss.Animator.Play("PreparingToShoot", 0.05f);

            while (criss.Animator.currentAnimationName == "PreparingToShoot")
            {
                UpdateDirection();
                yield return null;
            }

            UpdateDirection();

            npc.StartCoroutine(ShootLaser());
        }

        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            playerInSight = true;
        }

        public override void PlayerLost(PlayerManager player)
        {
            base.PlayerLost(player);
            playerInSight = false;
        }
    }
}
