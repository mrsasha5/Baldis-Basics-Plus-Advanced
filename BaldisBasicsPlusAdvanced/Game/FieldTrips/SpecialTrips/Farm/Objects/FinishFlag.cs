using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm.Objects
{
    public class FinishFlag : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private SpriteRenderer renderer;

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
            renderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_farm_flag"]);
            renderer.transform.parent.SetParent(transform, false);
            renderer.transform.parent.localPosition = Vector3.up * 84f;

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = Vector3.one * 10f;
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (fieldTripManager != null && other.CompareTag("Player"))
            {
                fieldTripManager.OnWin();
            }
        }
    }
}
