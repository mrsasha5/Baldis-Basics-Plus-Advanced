using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class StealingPlate : BasePlate
    {

        [SerializeField]
        private float timePerItem;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            timePerItem = 40f;
        }

        protected override void SetValues(PlateData data)
        {
            base.SetValues(data);
            data.MarkAsCooldownPlate();
            //plateData.hasLight = true;
            //plateData.lightColor = Color.gray;
            data.allowsToCopyTextures = false;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_stealing_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();

            ItemManager itm = entities[0]?.GetComponent<ItemManager>();

            int itemsCount = itm.CountItems();

            if (itemsCount > 0)
            {
                itm.ClearItems();
                SetCooldown(itemsCount * timePerItem);
                StartCoroutine(Effect());
            }
            
        }

        protected override bool IsPressable(Entity target)
        {
            return base.IsPressable(target) && target.CompareTag("Player");
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
