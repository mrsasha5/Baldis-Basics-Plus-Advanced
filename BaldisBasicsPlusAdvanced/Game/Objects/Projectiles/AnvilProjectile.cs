using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class AnvilProjectile : BaseHeightableProjectile
    {
        protected override void SetEntityValues()
        {
            base.SetEntityValues();
            SetEntityName("Anvil Projectile");
            SetEntitySprite(AssetStorage.sprites["adv_anvil_projectile"]);
            SetEntityTrigger(3f);
        }

        public override void EntityTriggerEnter(Entity otherEntity, Collider other, bool validCollision)
        {
            base.EntityTriggerEnter(otherEntity, other, validCollision);
            if (validCollision && flying)
            {
                if (otherEntity != null)
                {
                    otherEntity.Squish(10f);
                    otherEntity.SetSpeedEffect(0.25f, 10f);
                    ObjectCreator.CreatePropagatedAudioManager(entity.transform.position, destroyWhenAudioEnds: true)
                        .PlaySingle(AssetStorage.sounds["adv_metal_blow"]);
                }
                else if (other.transform.TryGetComponent(out Window window))
                {
                    window.Break(makeNoise: false);
                }
            }
        }

        protected override void OnEntityCollide(RaycastHit hit)
        {
            base.OnEntityCollide(hit);
            Destroy();
        }

        protected override void Destroy()
        {
            base.Destroy();
            ObjectCreator.CreatePropagatedAudioManager(transform.position, destroyWhenAudioEnds: true)
                .PlaySingle(AssetStorage.sounds["adv_metal_blow"]);
        }

    }
}
