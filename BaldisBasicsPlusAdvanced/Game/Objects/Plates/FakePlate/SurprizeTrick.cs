using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.FakePlate
{
    public class SurprizeTrick : BaseTrick
    {
        private float baldiSinkSpeed = 0.5f;

        private float teleportSinkSpeed = 0.5f;

        private float time;

        public override void Reset()
        {
            base.Reset();
            Plate.ResetEntityTriggerSize();
            Plate.SetSurprizeVisual(false, playAudio: true, playPressingAudio: true);
            Plate.DisableEntitySuckerMode();
            Plate.SetRandomCooldown();
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (activated && time > 0f)
            {
                time -= Time.deltaTime * Plate.Timescale;
                if (time <= 0f) Plate.EndTrick();
            }
        }

        public override void OnEntityStayTargetZone(Entity entity)
        {
            base.OnEntityStayTargetZone(entity);

            if (activated || !entity.CompareTag("Player")) return;
            activated = true;
            Plate.SetSurprizeVisual(true, playAudio: true, playPressingAudio: false);
            Plate.OverrideEntityTriggerSize(70f);

            bool anybodyIsTeleported = TryToTeleport();

            if (!anybodyIsTeleported)
            {
                Plate.SetEntitySuckerMode();
                time = 15f;
            }
        }

        private bool TryToTeleport()
        {
            if (Random.Range(0, 101) >= 50)
            {
                for (int i = 0; i < Plate.Ec.Npcs.Count; i++)
                {
                    NPC npc = Plate.Ec.Npcs[i];
                    if (!(npc is Baldi)) continue;

                    EntityOverrider entityOverrider = new EntityOverrider();
                    if (npc.GetComponent<Entity>().Override(entityOverrider))
                    {
                        npc.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_bal_surprize"]);
                        npc.behaviorStateMachine.ChangeState(new Baldi_Praise(npc, (Baldi)npc,
                            npc.behaviorStateMachine.CurrentState, 2f)); //I didn't use native method cus I don't want audio
                        npc.GetComponent<Entity>().SpectacularTeleport(Plate.transform.position);
                        StartCoroutine(Teleport(npc.GetComponent<Entity>(), entityOverrider));

                        Plate.AudMan.FlushQueue(true);
                        Plate.AudMan.QueueAudio(AssetsStorage.sounds["adv_suction_start"]);
                        Plate.AudMan.QueueAudio(AssetsStorage.sounds["adv_suction_loop"]);
                        Plate.AudMan.SetLoop(true);

                        return true;
                        //break;
                    }
                }
            }

            return false;
        }

        public override bool OnEntityCatched(Entity entity)
        {
            base.OnEntityCatched(entity);
            EntityOverrider entityOverrider = new EntityOverrider();
            if (entity.Override(entityOverrider))
            {
                StartCoroutine(TeleportEntityOnCatch(entity, entityOverrider));
                time = -1f;
                return true;
            }
            return false;
        }

        private IEnumerator TeleportEntityOnCatch(Entity subject, EntityOverrider entityOverrider)
        {
            //entityOverrider.Set
            entityOverrider.SetFrozen(value: true);
            entityOverrider.SetInteractionState(value: false);
            entityOverrider.SetGrounded(true);

            float sinkPercent = 1f;
            entityOverrider.SetHeight(subject.InternalHeight * sinkPercent);
            while (sinkPercent > 0.1f)
            {
                if (subject == null)
                {
                    yield break;
                }

                sinkPercent -= Time.deltaTime * Plate.Timescale * teleportSinkSpeed;
                entityOverrider.SetHeight(subject.InternalHeight * sinkPercent);
                yield return null;
            }

            entityOverrider.Release();
            entityOverrider.SetFrozen(value: false);
            entityOverrider.SetInteractionState(value: true);

            subject?.RandomTeleport();
            Plate.EndTrick();
            
            yield break;
        }

        private IEnumerator Teleport(Entity subject, EntityOverrider entityOverrider)
        {
            entityOverrider.SetFrozen(value: true);
            entityOverrider.SetInteractionState(value: false);
            entityOverrider.SetGrounded(true);

            float sinkPercent = 0f;
            entityOverrider.SetHeight(subject.InternalHeight * sinkPercent);
            while (sinkPercent < 1f)
            {
                if (subject == null)
                {
                    yield break;
                }

                sinkPercent += Time.deltaTime * Plate.Ec.EnvironmentTimeScale * baldiSinkSpeed;
                entityOverrider.SetHeight(subject.InternalHeight * sinkPercent);
                yield return null;
            }

            entityOverrider.Release();
            entityOverrider.SetFrozen(value: false);
            entityOverrider.SetInteractionState(value: true);

            if (subject.TryGetComponent(out NPC npc))
            {
                npc.Navigator.SetSpeed(0f);
            }

            Plate.EndTrick();

            yield break;
        }

    }
}
