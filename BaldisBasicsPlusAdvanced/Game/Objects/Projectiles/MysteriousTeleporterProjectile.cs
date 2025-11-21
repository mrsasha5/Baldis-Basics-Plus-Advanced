using System;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class MysteriousTeleporterProjectile : BaseProjectile
    {

        protected override void SetEntityValues()
        {
            base.SetEntityValues();
            SetEntityName("Mysterious Teleporter");
            SetEntitySprite(AssetStorage.sprites["adv_mysterious_teleporter"]);
            SetEntityTrigger(1.5f);
        }

        public override void EntityTriggerEnter(Collider other, bool validCollision)
        {
            if (validCollision && flying && other.isTrigger && other.CompareTag("NPC") && other.TryGetComponent(out NPC npc)
                && PlayerIsLauncher && ReflEvent_OnMysteriousTeleporterPreHit(npc, (PlayerManager)launcher))
            {
                SetCorruptionEffect(npc);
                SetFlying(false);
                Destroy(gameObject);
                if (PlayerIsLauncher) ReflEvent_OnMysteriousTeleporterHit(npc, (PlayerManager)launcher);
            }
        }

        protected override void OnEntityCollide(RaycastHit hit)
        {
            SetFlying(false);
            Destroy(gameObject);
            AudioManager audMan = ObjectCreator.CreatePropagatedAudMan(transform.position, destroyWhenAudioEnds: true);
            audMan.PlaySingle(AssetStorage.sounds["teleport"]);
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
            AudioManager audMan = ObjectCreator.CreatePropagatedAudMan(npc.transform.position, destroyWhenAudioEnds: true);
            audMan.transform.SetParent(npc.transform, true);
            audMan.PlaySingle(AssetStorage.sounds["buzz_lose"]);
            corruptionEffect.onEffectPreEnd = delegate {
                OnCorruptionEffectPreEnd(npc);
            };
        }

        private bool ReflEvent_OnMysteriousTeleporterPreHit(object @object, PlayerManager pm)
        {
            object result = ReflectionHelper.NoCache_UseMethod(@object, "Adv_OnMysteriousTeleporterPreHit", pm);
            return result == null || (bool)result;
        }

        private void ReflEvent_OnMysteriousTeleporterHit(object @object, PlayerManager pm)
        {
            ReflectionHelper.NoCache_UseMethod(@object, "Adv_OnMysteriousTeleporterHit", pm);
        }
    }
}
