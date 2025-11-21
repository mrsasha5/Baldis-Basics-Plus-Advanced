using System.Collections;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm.Objects
{
    public class FinishFlag : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private SpriteRenderer renderer;

        [SerializeField]
        private BoxCollider collider;

        [SerializeField]
        private int ytpsReward;

        [SerializeField]
        private float disappearingSpeed;

        private bool touched;

        private FarmFieldTripManager fieldTripManager;

        public void Initialize(FarmFieldTripManager manager)
        {
            fieldTripManager = manager;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void InitializePrefab(int variant)
        {
            renderer = ObjectCreator.CreateSpriteRendererBase(AssetStorage.sprites["adv_farm_flag"]);
            renderer.transform.parent.SetParent(transform, false);
            renderer.transform.parent.localPosition = Vector3.up * 84f;

            collider = gameObject.AddComponent<BoxCollider>();
            collider.size = Vector3.one * 10f + Vector3.up * 90f;
            collider.center = Vector3.up * 45f;
            collider.isTrigger = true;

            if (variant == 2)
            {
                ytpsReward = 25;
                disappearingSpeed = 0.25f;
            }
            else ytpsReward = -1;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !touched)
            {
                touched = true;

                if (fieldTripManager != null)
                {
                    fieldTripManager.OnWin();
                }

                if (ytpsReward > 0)
                {
                    CoreGameManager.Instance.AddPoints(ytpsReward, 0, playAnimation: true);
                    CoreGameManager.Instance.audMan.PlaySingle(AssetStorage.sounds["ytp_pickup_2"]);
                    StartCoroutine(Disappear());
                }
            }
        }

        private IEnumerator Disappear()
        {
            MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();

            float percent = 1f;

            Color color = renderer.color;

            while (percent > 0f)
            {
                percent -= Time.deltaTime * disappearingSpeed;
                if (percent < 0f) percent = 0f;
                color.a = percent;
                renderer.color = color;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
