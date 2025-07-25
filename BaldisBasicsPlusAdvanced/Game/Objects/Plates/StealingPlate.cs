﻿using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class StealingPlate : BaseCooldownPlate
    {

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            //plateData.hasLight = true;
            //plateData.lightColor = Color.gray;
            plateData.allowsToCopyTextures = false;
            plateData.targetsPlayer = true;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_stealing_plate");
            SetEditorSprite("adv_editor_stealing_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();

            ItemManager itm = Singleton<CoreGameManager>.Instance.GetPlayer(0).itm;

            int itemsCount = itm.CountItems();

            float itemTimePrice = 40f;

            if (itemsCount > 0)
            {
                itm.ClearItems();
                SetCooldown(itemsCount * itemTimePrice);
                StartCoroutine(Effect());
            }
            
        }

        private IEnumerator Effect()
        {
            Canvas canvasOverlay = Instantiate(ObjectsStorage.Overlays["ElephantOverlay"]);
            canvasOverlay.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
            AudioManager audMan = ObjectsCreator.CreateAudMan(Vector3.zero);
            audMan.positional = false;

            audMan.PlaySingle(AssetsStorage.sounds["adv_elephant_hit"]);

            while (audMan.AnyAudioIsPlaying)
            {
                yield return null;
            }

            canvasOverlay.GetComponent<OverlayEffectsManager>().QueueEffect<FadeOutEffect>(1f);
            audMan.PlaySingle(AssetsStorage.sounds["adv_appearing"]);

            while (audMan.AnyAudioIsPlaying)
            {
                yield return null;
            }

            Destroy(canvasOverlay.gameObject);
            Destroy(audMan.gameObject);

            yield break;
        }
    }
}
