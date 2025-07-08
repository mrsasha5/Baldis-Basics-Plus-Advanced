using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    internal class FrozennessController : BaseController
    {

        private static MovementModifier moveMod = new MovementModifier(Vector3.zero, 0.25f);

        private GameObject frozenSprite;

        public override ControllerType Type => ControllerType.Effect;

        public override void OnInitialize()
        {
            if (!entity.ExternalActivity.moveMods.Contains(moveMod))
            {
                entity.ExternalActivity.moveMods.Add(moveMod);
            }

            AudioManager audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position, destroyWhenAudioEnds: true);
            audMan.transform.parent = entity.transform;
            audMan.PlaySingle(AssetsStorage.sounds["adv_frozen"]);

            frozenSprite = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_frozen_enemy"]).transform.parent.gameObject;
            frozenSprite.transform.name = "adv_frozen_enemy";
            frozenSprite.transform.parent = entity.transform;
            frozenSprite.transform.localPosition = Vector3.up * -3.5f;
        }

        public override void VirtualUpdate()
        {
            if (time > 0)
            {
                time -= Time.deltaTime * ec.NpcTimeScale;
            }
        }

        public override void OnPreDestroying()
        {
            base.OnPreDestroying();
            if (entity.ExternalActivity.moveMods.Contains(moveMod) && controllerSystem.GetAbsoluteControllersCount<FrozennessController>() <= 1) entity.ExternalActivity.moveMods.Remove(moveMod);
            UnityEngine.Object.Destroy(frozenSprite);
        }

    }
}
