using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Enums;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class NPCTeleporterItem : Item
    {

        private float maxDistance = 5f;

        public override bool Use(PlayerManager pm)
        {
            Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out RaycastHit hit, float.PositiveInfinity, ~pm.gameObject.layer, QueryTriggerInteraction.Ignore);

            float distance = hit.distance > maxDistance ? maxDistance : hit.distance;

            int rotationMultiplier = Singleton<InputManager>.Instance.GetDigitalInput("LookBack", onDown: false) ? -1 : 1;

            Vector3 pos = pm.transform.position;
            Vector3 forward = pm.transform.forward * rotationMultiplier;

            pos += forward * distance;
            ObjectsCreator.createProjectile<TeleporterProjectile>("Projectile", AssetsStorage.sprites["adv_mysterious_teleporter"], pos, pm.ec, forward, 1.5f);
            Destroy(gameObject);
            return true;
        }
    }
}
