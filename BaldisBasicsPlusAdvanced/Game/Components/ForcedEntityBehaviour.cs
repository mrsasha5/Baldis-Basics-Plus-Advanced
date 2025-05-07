using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
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

        public void postInit(Entity entity, Force force, Vector3 raycastDirection, float minSlamMagnitude, float slamDistance, float time = float.PositiveInfinity)
        {
            this.entity = entity;
            this.force = force;
            this.slamDistance = slamDistance;
            this.slamMagnitude = minSlamMagnitude;
            this.direction = raycastDirection;
            if (time != float.PositiveInfinity) this.time = time;

            audMan = ObjectsCreator.createPropagatedAudMan(entity.transform.position, time);
            audMan.PlaySingle(AssetsStorage.sounds["whoosh"]);
            audMan.transform.SetParent(entity.transform, true);
        }

        protected override void virtualUpdate()
        {
            base.virtualUpdate();
            if (entity.Velocity.magnitude >= slamMagnitude)
            {
                if (Physics.Raycast(entity.transform.position, direction, out RaycastHit hit, slamDistance, ObjectsStorage.RaycastMaskIgnorableObjects))
                {
                    if (hit.transform.tag == "Window" && hit.transform.TryGetComponent(out Window window) && !window.IsOpen)
                    {
                        window.Break(true);
                    }
                }

                if (Physics.Raycast(entity.transform.position, direction, out RaycastHit _hit, slamDistance, ObjectsStorage.RaycastMaskIgnorableObjects, QueryTriggerInteraction.Ignore))
                {
                    bang(entity);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Entity entity))
            {
                entity.AddForce(new Force(direction, pushSpeed, pushAcceleration));
                bang(entity);
            }
        }

        private void bang(Entity entity)
        {
            if (entity.TryGetComponent(out AudioManager audMan))
            {
                audMan.PlaySingle(AssetsStorage.sounds["bang"]);
            }
            else
            {
                ObjectsCreator.createPropagatedAudMan(transform.position, 3f).PlaySingle(AssetsStorage.sounds["bang"]);
            }
            ec.MakeNoise(transform.position, 64); //like First Prize
            destroyAll();
            entity.RemoveForce(force);
        }

        private void destroyAll()
        {
            Destroy(this);
            Destroy(audMan.gameObject);
        }

    }
}
