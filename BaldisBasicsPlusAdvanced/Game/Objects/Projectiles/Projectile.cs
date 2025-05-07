using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using MidiPlayerTK;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private EnvironmentController ec;

        private AudioManager audMan;

        private float speed = 80f;

        private bool flying;

        private Vector3 forward;

        private bool collided;

        public void Initialize(EnvironmentController environmentController, Vector3 pos, Vector3 forward)
        {
            ec = environmentController;
            transform.position = pos;
            this.forward = forward;

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            setFlying(true);
        }

        protected virtual void setFlying(bool flying)
        {
            this.flying = flying;
            if (flying)
            {
                audMan.QueueAudio(AssetsStorage.sounds["whoosh"]);
                audMan.SetLoop(true);
            }
            else if (audMan.loop)
            {
                audMan.FlushQueue(true);
                audMan.SetLoop(flying);
            }
        }

        protected virtual void Update()
        {
            if (flying)
            {
                transform.position += forward * speed * Time.deltaTime * ec.EnvironmentTimeScale;
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (collided) return;

            bool cancelDestroy = true;

            if (other.isTrigger && other.CompareTag("NPC"))
            {
                NPC npc = other.GetComponent<NPC>();
                cancelDestroy = false;
                onNpcCollide(npc, ref cancelDestroy);
                
            } //18 - ClickableCollidable
            else if (other.CompareTag("Player"))
            {
                PlayerManager pm = other.GetComponent<PlayerManager>();
                cancelDestroy = false;
                onPlayerCollide(pm, ref cancelDestroy);
            }
            else if (isObstacle(other))
            {
                cancelDestroy = false;
                onObstacleCollide(other, ref cancelDestroy);
            }

            if (!cancelDestroy)
            {
                collided = true;
                Destroy(gameObject);
            }
        }

        protected virtual bool isObstacle(Collider other)
        {
            return !(other.gameObject.layer == 18 || other.isTrigger);
        }

        protected virtual void onNpcCollide(NPC npc, ref bool cancelDestroy)
        {

        }

        protected virtual void onPlayerCollide(PlayerManager pm, ref bool cancelDestroy)
        {

        }

        protected virtual void onObstacleCollide(Collider other, ref bool cancelDestroy)
        {

        }

    }
}
