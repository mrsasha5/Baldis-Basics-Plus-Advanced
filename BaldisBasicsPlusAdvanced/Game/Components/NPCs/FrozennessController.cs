using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Systems.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.NPCs
{
    internal class FrozennessController : NPCController
    {

        private static MovementModifier npcMoveMod = new MovementModifier(Vector3.zero, 0.25f);

        private GameObject frozenSprite;

        public override void initialize(NPC npc, PlayerControllerSystem pc)
        {
            base.initialize(npc, pc);
            if (!entity.ExternalActivity.moveMods.Contains(npcMoveMod))
            {
                entity.ExternalActivity.moveMods.Add(npcMoveMod);
            }

            AudioManager audMan = ObjectsCreator.createPropagatedAudMan(entity.transform.position, 4f);
            audMan.transform.parent = entity.transform;
            audMan.PlaySingle(AssetsStorage.sounds["adv_frozen"]);

            frozenSprite = ObjectsCreator.createSpriteRendererBillboard(AssetsStorage.sprites["adv_frozen_enemy"]);
            frozenSprite.transform.name = "adv_frozen_enemy";
            frozenSprite.transform.parent = entity.transform;
            frozenSprite.transform.localPosition = Vector3.up * -3.5f;
        }

        public override void virtualUpdate()
        {
            if (time > 0)
            {
                time -= Time.deltaTime * ec.NpcTimeScale;
            }
        }

        public override void onDestroying()
        {
            base.onDestroying();
            if (entity.ExternalActivity.moveMods.Contains(npcMoveMod) && nc.getControllersCount<FrozennessController>() == 1) entity.ExternalActivity.moveMods.Remove(npcMoveMod);
            GameObject.Destroy(frozenSprite);
        }

    }
}
