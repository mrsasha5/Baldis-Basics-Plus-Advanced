using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Portals;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class PortalChaosEvent : RandomEvent, IPrefab
    {
        private static List<MysteriousPortal[]> mysteriousPortals = new List<MysteriousPortal[]>();

        private static List<Coroutine> coroutines = new List<Coroutine>();

        public void InitializePrefab(int variant)
        {
            
        }

        public override void Begin()
        {
            base.Begin();
            coroutines.Add(StartCoroutine(SpawnPortals(90)));
        }

        public override void End()
        {
            base.End();
            StopCoroutine(coroutines[0]);
            coroutines.RemoveAt(0);
            foreach (MysteriousPortal portal in mysteriousPortals[0]) //for 
            {
                if (portal != null) portal.Deactivate();
            }
            mysteriousPortals.RemoveAt(0);
        }

        public override void ResetConditions()
        {
            base.ResetConditions();
            mysteriousPortals.Clear();
            coroutines.Clear();
        }

        private IEnumerator SpawnPortals(int count)
        {
            int baseCount = count;

            float minCooldown = 2f;
            float maxCooldown = 5f;

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
                MysteriousPortal portal1 = SpawnPortal(position1);

                Vector3 position2 = ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true).FloorWorldPosition + Vector3.up * 5f;
                MysteriousPortal portal2 = SpawnPortal(position2);

                portal1.ConnectTo(portal2);
                portal2.ConnectTo(portal1);

                portal1.Activate();
                portal2.Activate();

                portals[baseCount - count] = portal1;
                count--;
                portals[baseCount - count] = portal2;
                count--;
                yield return null;
            }
            yield break;
        }

        private MysteriousPortal SpawnPortal(Vector3 position)
        {
            CrazyMysteriousPortal portal = 
                Instantiate(ObjectStorage.Objects["crazy_mysterious_portal"].GetComponent<CrazyMysteriousPortal>());
            portal.transform.position = position;
            portal.PostInitialize(ec, false);
            portal.StartTimer();
            return portal;
        }


    }
}
