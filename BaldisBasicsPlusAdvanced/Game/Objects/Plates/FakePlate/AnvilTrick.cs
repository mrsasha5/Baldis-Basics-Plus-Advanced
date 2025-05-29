using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.FakePlate
{
    public class AnvilTrick : BaseTrick
    {
        private float time;

        public override void OnPostInitialization()
        {
            base.OnPostInitialization();
            Plate.OverrideEntityTriggerSize(60f);
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (time > 0f)
            {
                time -= Time.deltaTime * Plate.Timescale;

                if (time <= 0f) Plate.EndTrick();
            }
        }

        public override void Reset()
        {
            base.Reset();
            Plate.SetSurprizeVisual(false, playAudio: false, playPressingAudio: true);
            Plate.SetRandomCooldown();
            Plate.ResetEntityTriggerSize();
        }

        public override void OnEntityStayTargetZone(Entity entity)
        {
            base.OnEntityStayTargetZone(entity);
            if (!activated && entity.CompareTag("Player"))
            {
                activated = true;
                time = 2f;
                Plate.SetSurprizeVisual(true, playAudio: false, playPressingAudio: false);
                Plate.AudMan.PlaySingle(AssetsStorage.sounds["adv_boing"]);

                AnvilProjectile projectile = Instantiate(ObjectsStorage.Objects["anvil_projectile"].GetComponent<AnvilProjectile>());
                projectile.Initialize(Plate.Ec, transform.position, this);
                projectile.SetFlying(true);
                projectile.SetHeight(2f);
                projectile.transform.forward = (entity.transform.position - transform.position).normalized;

            }
        }

    }
}
