using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Projectiles
{
    public class TeleporterProjectile : CorruptionProjectile
    {

        protected override void onEffectPreEnd(NPC npc)
        {
            base.onEffectPreEnd(npc);
            npc.dangerousTeleportation();
        }

        protected override void onPlayerCollide(PlayerManager pm, ref bool cancelDestroy)
        {
            cancelDestroy = true;
        }

        protected override void onObstacleCollide(Collider other, ref bool _)
        {
            AudioManager audMan = ObjectsCreator.createPropagatedAudMan(transform.position, 3f);
            ReflectionHelper.setValue<float>(audMan, "propagatedDistance", 101f); //if it > than maxDistance => subtitle scale is 0
            audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
        }

    }
}
