using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class CorruptionProjectile : Projectile
    {

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
        }

        protected override void onNpcCollide(NPC npc, ref bool cancelDestroy)
        {
            if (npc.name != "Chalkles(Clone)")
            {
                CorruptionNpcEffect corruptionEffect = npc.gameObject.AddComponent<CorruptionNpcEffect>();
                corruptionEffect.initialize(npc.spriteRenderer[0]);
                corruptionEffect.hit();
                AudioManager audMan = ObjectsCreator.createPropagatedAudMan(npc.transform.position, 3f);
                audMan.transform.SetParent(npc.transform, true);
                audMan.PlaySingle(AssetsStorage.sounds["buzz_lose"]);
                corruptionEffect.onEffectPreEnd = delegate { onEffectPreEnd(npc); };
            }
            else
            {
                cancelDestroy = true;
            }
        }

        protected virtual void onEffectPreEnd(NPC npc)
        {

        }

    }
}
