using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.Movement
{
    public class ForcedEntityBehaviour : TemporaryComponent
    {
        private float slamMagnitude;

        private float slamDistance;

        private float pushSpeed = 50f;

        private float pushAcceleration = -40f;

        private Vector3 direction;

        private Entity entity;

        private AudioManager audMan;

        private Force force;

        private bool makesNoises;

        public float DefaultSlamDistance => 4f;

        public void PostInit(Entity entity, Force force, Vector3 raycastDirection, float minSlamMagnitude, float slamDistance, bool makesNoises, float time = float.PositiveInfinity, bool calculateTime = true)
        {
            this.entity = entity;
            this.force = force;
            this.slamDistance = slamDistance;
            slamMagnitude = minSlamMagnitude;
            direction = raycastDirection;
            this.makesNoises = makesNoises;
            if (time != float.PositiveInfinity) this.time = time;

            audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position, time);
            audMan.PlaySingle(AssetsStorage.sounds["whoosh"]);
            audMan.transform.SetParent(entity.transform, true);
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (entity.Velocity.magnitude >= slamMagnitude)
            {
                if (Physics.Raycast(entity.transform.position, direction, out RaycastHit hit, slamDistance, ObjectsStorage.RaycastMaskIgnorableObjects))
                {
                    if (hit.transform.tag == "Window" && hit.transform.TryGetComponent(out Window window) && !window.IsOpen)
                    {
                        window.Break(makesNoises);
                    }
                }

                if (Physics.Raycast(entity.transform.position, direction, out RaycastHit _hit, slamDistance, ObjectsStorage.RaycastMaskIgnorableObjects, QueryTriggerInteraction.Ignore))
                {
                    Bang(entity);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Entity entity))
            {
                entity.AddForce(new Force(direction, pushSpeed, pushAcceleration));
                Bang(entity);
            }
        }

        private void Bang(Entity entity)
        {
            if (entity.TryGetComponent(out AudioManager audMan))
            {
                audMan.PlaySingle(AssetsStorage.sounds["bang"]);
            }
            else
            {
                ObjectsCreator.CreatePropagatedAudMan(transform.position, 3f).PlaySingle(AssetsStorage.sounds["bang"]);
            }
            if (makesNoises) ec.MakeNoise(transform.position, 64); //like First Prize
            DestroyAll();
            entity.RemoveForce(force);
        }

        private void DestroyAll()
        {
            Destroy(this);
            Destroy(audMan.gameObject);
        }

    }
}
