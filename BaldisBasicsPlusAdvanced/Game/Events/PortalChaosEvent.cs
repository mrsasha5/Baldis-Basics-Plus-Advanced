using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class PortalChaosEvent : BaseRandomEvent
    {
        private static List<MysteriousPortal[]> mysteriousPortals = new List<MysteriousPortal[]>();

        private static List<Coroutine> coroutines = new List<Coroutine>();

        public override void prepareData()
        {
            base.prepareData();
            descriptionKey = "EventAdv_PortalChaos";
        }

        public override void Begin()
        {
            base.Begin();
            coroutines.Add(StartCoroutine(spawnPortals(90)));
        }

        public override void End()
        {
            base.End();
            StopCoroutine(coroutines[0]);
            coroutines.RemoveAt(0);
            foreach (MysteriousPortal portal in mysteriousPortals[0]) //for 
            {
                if (portal != null) portal.deactivate();
            }
            mysteriousPortals.RemoveAt(0);
        }

        public override void ResetConditions()
        {
            base.ResetConditions();
            mysteriousPortals.Clear();
            coroutines.Clear();
        }

        private IEnumerator spawnPortals(int count)
        {
            int baseCount = count;

            float minCooldown = 2f;
            float maxCooldown = 3f;

            MysteriousPortal[] portals = new MysteriousPortal[count];
            mysteriousPortals.Add(portals);

            while (count > 0)
            {
                float time = UnityEngine.Random.Range(minCooldown, maxCooldown);
                while (time > 0)
                {
                    time -= Time.deltaTime * ec.EnvironmentTimeScale;
                    yield return null;
                }

                time = UnityEngine.Random.Range(minCooldown, maxCooldown);

                Vector3 position1 = ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).FloorWorldPosition + Vector3.up * 5f;
                MysteriousPortal portal1 = spawnPortal(position1);

                Vector3 position2 = ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).FloorWorldPosition + Vector3.up * 5f;
                MysteriousPortal portal2 = spawnPortal(position2);

                portal1.connectTo(portal2);
                portal2.connectTo(portal1);

                portal1.activate();
                portal2.activate();

                portals[baseCount - count] = portal1;
                count--;
                portals[baseCount - count] = portal2;
                count--;
                yield return null;
            }
            yield break;
        }

        private MysteriousPortal spawnPortal(Vector3 position)
        {
            /*GameObject gameObject = new GameObject("Mysterious portal");
            gameObject.transform.position = position;

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(5f, 5f, 5f);
            collider.isTrigger = true;

            GameObject rendererBase = ObjectsCreator.createSpriteRendererBillboard(CachedAssets.sprites["adv_portal"]);
            rendererBase.transform.parent = gameObject.transform;
            rendererBase.transform.localPosition = Vector3.zero;

            SpriteRenderer renderer = rendererBase.GetComponentInChildren<SpriteRenderer>();

            MysteriousPortal portal = gameObject.AddComponent<MysteriousPortal>();*/
            MysteriousPortal portal = Instantiate(ObjectsStorage.Objects["mysterious_portal"].GetComponent<MysteriousPortal>());
            portal.transform.position = position;
            portal.postInitialize(ec, true);
            return portal;
        }


    }
}
