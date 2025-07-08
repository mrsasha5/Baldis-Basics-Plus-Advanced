using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class MysteriousTeleporterProjectile : BaseProjectile
    {

        protected override void SetEntityValues()
        {
            base.SetEntityValues();
            SetEntityName("Mysterious Teleporter");
            SetEntitySprite(AssetsStorage.sprites["adv_mysterious_teleporter"]);
            SetEntityTrigger(1.5f);
        }

        public override void EntityTriggerEnter(Collider other)
        {
            if (flying && other.isTrigger && other.CompareTag("NPC") && other.TryGetComponent(out NPC npc)
                && PlayerIsLauncher && Refl_OnMysteriousTeleporterPreHit(npc, (PlayerManager)launcher))
            {
                SetCorruptionEffect(npc);
                SetFlying(false);
                Destroy(gameObject);
                if (PlayerIsLauncher) Refl_OnMysteriousTeleporterHit(npc, (PlayerManager)launcher);
            }
        }

        protected override void OnEntityCollide(RaycastHit hit)
        {
            SetFlying(false);
            Destroy(gameObject);
            AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(transform.position, destroyWhenAudioEnds: true);
            audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
        }

        public override void EntityTriggerStay(Collider other)
        {
        }

        public override void EntityTriggerExit(Collider other)
        {
        }

        private void OnCorruptionEffectPreEnd(NPC npc)
        {
            npc.GetComponent<Entity>().DangerousTeleportation();
        }

        private void SetCorruptionEffect(NPC npc)
        {
            CorruptionNpcEffect corruptionEffect = npc.gameObject.AddComponent<CorruptionNpcEffect>();
            corruptionEffect.Initialize(npc.spriteRenderer[0]);
            corruptionEffect.Hit();
            AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(npc.transform.position, destroyWhenAudioEnds: true);
            audMan.transform.SetParent(npc.transform, true);
            audMan.PlaySingle(AssetsStorage.sounds["buzz_lose"]);
            corruptionEffect.onEffectPreEnd = delegate {
                OnCorruptionEffectPreEnd(npc);
            };
        }

        private bool Refl_OnMysteriousTeleporterPreHit(object @object, PlayerManager pm)
        {
            object result = ReflectionHelper.UseMethod(@object, "Adv_OnMysteriousTeleporterPreHit", pm);
            return result == null || (bool)result;
        }

        private void Refl_OnMysteriousTeleporterHit(object @object, PlayerManager pm)
        {
            ReflectionHelper.UseMethod(@object, "Adv_OnMysteriousTeleporterHit", pm);
        }
    }
}
