using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.Movement
{
    public class ForcedEntityBehaviour : MonoBehaviour
    {
        private float slamMagnitude;

        private float slamDistance;

        private float pushSpeed = 50f;

        private float pushAcceleration = -40f;

        private Vector3 direction;

        private Entity entity;

        private AudioManager audMan;

        private EnvironmentController ec;

        private Force force;

        private bool makesNoises;

        public float DefaultSlamDistance => 4f;

        public void Initialize(EnvironmentController ec, Entity entity, Force force, Vector3 raycastDirection, float minSlamMagnitude, float slamDistance, bool makesNoises)
        {
            this.ec = ec;
            this.entity = entity;
            this.force = force;
            this.slamDistance = slamDistance;
            slamMagnitude = minSlamMagnitude;
            direction = raycastDirection;
            this.makesNoises = makesNoises;

            audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position);
            audMan.QueueAudio(AssetsStorage.sounds["whoosh"]);
            audMan.SetLoop(true);
            audMan.transform.SetParent(entity.transform, true);
        }

        private void Update()
        {
            if (force.Dead || entity.Frozen)
            {
                Destroy(this);
                return;
            }

            if (entity.Velocity.magnitude >= slamMagnitude)
            {
                if (Physics.Raycast(entity.transform.position, direction, out RaycastHit hit, slamDistance, LayersHelper.ignorableCollidableObjects))
                {
                    if (hit.transform.tag == "Window" && hit.transform.TryGetComponent(out Window window) && !window.IsOpen)
                    {
                        window.Break(makesNoises);
                    }
                }

                if (Physics.Raycast(entity.transform.position, direction, out RaycastHit _hit, slamDistance, LayersHelper.ignorableCollidableObjects, QueryTriggerInteraction.Ignore))
                {
                    Bang();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Entity entity))
            {
                entity.AddForce(new Force(direction, pushSpeed, pushAcceleration));
                PlayBang();
            }
        }

        private void Bang(bool playBang = true)
        {
            if (playBang) PlayBang();
            Destroy(this);
            entity.RemoveForce(force);
        }

        private void PlayBang()
        {
            ObjectsCreator.CreatePropagatedAudMan(transform.position, destroyWhenAudioEnds: true).PlaySingle(AssetsStorage.sounds["bang"]);
            if (makesNoises) ec.MakeNoise(transform.position, 64); //like First Prize
        }

        private void OnDestroy()
        {
            Destroy(audMan.gameObject);
        }

    }
}
