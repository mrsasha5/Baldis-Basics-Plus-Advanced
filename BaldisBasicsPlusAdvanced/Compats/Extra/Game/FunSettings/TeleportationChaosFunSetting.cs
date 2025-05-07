using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings
{
    public class TeleportationChaosFunSetting : BaseFunSetting
    {

        private float time;

        public override void Initialize()
        {
            base.Initialize();
            time = 30f;
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (initialized && time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                if (time <= 0f)
                {
                    TeleportEveryone();
                    time = Random.Range(15f, 40f);
                }
            }
        }

        private void TeleportEveryone()
        {
            StartCoroutine(Teleporter());
        }

        private IEnumerator Teleporter()
        {
            List<Entity> allEntities = new List<Entity>(ReflectionHelper.GetValue<Entity, List<Entity>>("allEntities"));

            float time = 0f;

            while (allEntities.Count > 0)
            {
                while (time > 0f)
                {
                    time -= Time.deltaTime * ec.EnvironmentTimeScale;
                    yield return null;
                }

                int i = Random.Range(0, allEntities.Count);
                while (allEntities[i] == null || (allEntities[i] != null && allEntities[i].TryGetComponent(out MathMachineNumber _)))
                {
                    allEntities.RemoveAt(i);
                    i = Random.Range(0, allEntities.Count);
                    if (allEntities.Count == 0) yield break;
                }
                
                allEntities[i].SpectacularTeleport(
                    ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true)
                                    .FloorWorldPosition + Vector3.up * 5f);
                allEntities.RemoveAt(i);

                time = 0.2f;

                yield return null;
            }

            yield break;
        }

    }
}
