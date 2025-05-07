using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.Overlays;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class StealingPlate : BaseCooldownPlate
    {

        protected override void setValues(ref PlateData plateData)
        {
            base.setValues(ref plateData);
            plateData.targetPlayer = true;
        }

        protected override void setTextures()
        {
            setTexturesByBaseName("adv_stealing_plate");
            setEditorSprite("adv_editor_stealing_plate");
        }

        protected override void virtualOnPress()
        {
            base.virtualOnPress();

            ItemManager itm = Singleton<CoreGameManager>.Instance.GetPlayer(0).itm;

            int itemsCount = itm.countItems();

            float itemTimePrice = 40f;

            if (itemsCount > 0)
            {
                itm.ClearItems();
                setCooldown(itemsCount * itemTimePrice);
                StartCoroutine(effect());
            }
            
        }

        private IEnumerator effect()
        {
            Canvas canvasOverlay = Instantiate(ObjectsStorage.Overlays["ElephantOverlay"]);
            canvasOverlay.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
            AudioManager audMan = ObjectsCreator.createAudMan(Vector3.zero);
            audMan.positional = false;

            audMan.PlaySingle(AssetsStorage.sounds["adv_elephant_hit"]);

            while (audMan.AnyAudioIsPlaying)
            {
                yield return null;
            }

            canvasOverlay.GetComponent<OverlayEffectsManager>().queueEffect<FadeOutEffect>(1f);
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
