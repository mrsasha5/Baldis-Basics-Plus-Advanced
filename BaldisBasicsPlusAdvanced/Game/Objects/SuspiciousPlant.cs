using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class SuspiciousPlant : MonoBehaviour, IPrefab
    {
        private PlayerManager pm;

        public void InitializePrefab(int variant)
        {
            SpriteRenderer renderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["plant"]);
            renderer.transform.parent.SetParent(transform);

            float val = 1f;// 0.75f;
            renderer.color = new Color(val, val, val, 1f);

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(4f, 10f, 4f);
            gameObject.layer = LayersHelper.ignoreRaycastB;
            //renderer.transform.localPosition = Vector3.up * 5f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && pm == null)
            {
                pm = other.GetComponent<PlayerManager>();
                pm.SetInvisible(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && pm != null)
            {
                pm.SetInvisible(false);
                pm = null;
            }
        }
    }
}
