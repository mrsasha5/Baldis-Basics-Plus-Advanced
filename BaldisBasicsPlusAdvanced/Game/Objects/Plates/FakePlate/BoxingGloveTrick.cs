using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.FakePlate
{
    public class BoxingGloveTrick : BaseTrick
    {
        private float appearingSpeed = 50f;

        private float disappearingSpeed = 5f;

        private float initialSpeed = 200f;

        private float acceleration = -60f;

        private SpriteRenderer renderer;

        public override void OnPostInitialization()
        {
            base.OnPostInitialization();
            renderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_boxing_glove_trick"]);
            renderer.transform.parent.SetParent(Plate.transform, false);
            renderer.transform.localPosition = Vector3.up * -5f;
            renderer.gameObject.SetActive(false);
        }

        public override void Reset()
        {
            Plate.SetRandomCooldown();
            Plate.SetSurprizeVisual(false, playAudio: false, playPressingAudio: true);
            Destroy(renderer.transform.parent.gameObject);
        }

        public override void OnPress()
        {
            base.OnPress();
            if (Plate.Entities.Find(x => x.CompareTag("Player")) && !activated)
            {
                activated = true;
                StartCoroutine(HitAllEntities());
            }   
        }

        private IEnumerator HitAllEntities()
        {
            renderer.gameObject.SetActive(true);
            Plate.SetSurprizeVisual(true, playAudio: false, playPressingAudio: false);
            Plate.AudMan.PlaySingle(AssetsStorage.sounds["adv_boing"]);

            float maxHeight = 8f;

            float addendHeight = 0f;

            while (addendHeight < maxHeight)
            {
                addendHeight += Time.deltaTime * Plate.Timescale * appearingSpeed;
                renderer.transform.localPosition = Vector3.up * (-5f + addendHeight);
                yield return null;
            }

            addendHeight = maxHeight;
            renderer.transform.localPosition = Vector3.up * (-5f + addendHeight);

            if (Plate.Entities.Count > 0) Plate.AudMan.PlaySingle(AssetsStorage.sounds["bang"]);

            for (int i = 0; i < Plate.Entities.Count; i++)
            {
                Plate.Entities[i].AddForceWithBehaviour(
                    (Plate.Entities[i].transform.position - Plate.transform.position).normalized, initialSpeed, acceleration, Plate.Entities[i].CompareTag("Player"));
            }

            float timeCooldown = 3f;

            while (timeCooldown > 0f)
            {
                timeCooldown -= Time.deltaTime * Plate.Timescale;
                yield return null;
            }

            while (addendHeight > 0f)
            {
                addendHeight -= Time.deltaTime * Plate.Timescale * disappearingSpeed;
                renderer.transform.localPosition = Vector3.up * (-5f + addendHeight);
                yield return null;
            }

            Plate.EndTrick();
            yield break;
        }

    }
}
