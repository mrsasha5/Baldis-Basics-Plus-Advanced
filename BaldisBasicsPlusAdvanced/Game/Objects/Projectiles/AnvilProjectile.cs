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

        public override void EntityTriggerEnter(Collider other, bool validCollision)
        {
            base.EntityTriggerEnter(other, validCollision);
            if (validCollision && flying)
            {
                if (other.TryGetComponent(out Entity entity))
                {
                    entity.Squish(10f);
                    //cus nobody shouldn't disable environment
                    entity.SetSpeedEffect(0.25f, 10f);
                    ObjectCreator.CreatePropagatedAudMan(entity.transform.position, destroyWhenAudioEnds: true)
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
            ObjectCreator.CreatePropagatedAudMan(transform.position, destroyWhenAudioEnds: true)
                .PlaySingle(AssetStorage.sounds["adv_metal_blow"]);
        }

    }
}
