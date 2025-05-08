using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BBE.CustomClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings
{
    public class TeleportationChaosFunSetting : FunSetting
    {
        private Vector2 minMaxTime = new Vector2(10f, 40f);

        private bool teleporterActive;

        private float time;

        private bool initialized;

        private EnvironmentController ec;

        private TeleportationChaosFunSetting activeFunSetting;

        //public override bool UnlockConditional => true;

        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            base.OnBaseGameManagerInitialize(baseGameManager);
            activeFunSetting = baseGameManager.gameObject.AddComponent<TeleportationChaosFunSetting>(); //...
            activeFunSetting.ec = baseGameManager.Ec;
        }

        public override void OnSpoopModeBegin()
        {
            activeFunSetting.initialized = true;
            activeFunSetting.time = 5f;
        }

        private void Update()
        {
            if (initialized && !teleporterActive && time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                if (time <= 0f)
                {
                    TeleportEveryone(UnityEngine.Random.Range(1, 3));
                    time = UnityEngine.Random.Range(minMaxTime.x, minMaxTime.y);
                }
            }
        }

        private void TeleportEveryone(int variant)
        {
            switch (variant)
            {
                case 1:
                    StartCoroutine(GenericTeleporter());
                    break;
                case 2:
                    StartCoroutine(Mixer());
                    break;
            }
            
        }

        private IEnumerator Mixer()
        {
            List<Entity> allEntities = new List<Entity>(ReflectionHelper.Static_GetValue<Entity, List<Entity>>("allEntities"));

            float time = 0f;

            List<Entity> entitiesToMix = new List<Entity>();

            teleporterActive = true;

            while (allEntities.Count > 0)
            {
                while (time > 0f)
                {
                    time -= Time.deltaTime * ec.EnvironmentTimeScale;
                    yield return null;
                }

                for (int j = 0; j < 2; j++)
                {
                    int i = UnityEngine.Random.Range(0, allEntities.Count);
                    //fuck you Numballoon
                    while (allEntities[i] == null || (allEntities[i] != null && 
                        (allEntities[i].TryGetComponent(out MathMachineNumber _) || allEntities[i].CurrentRoom == null)))
                    {
                        allEntities.RemoveAt(i);
                        i = UnityEngine.Random.Range(0, allEntities.Count);
                        if (allEntities.Count == 0)
                        {
                            teleporterActive = false;
                            yield break;
                        }
                    }

                    entitiesToMix.Add(allEntities[i]);
                    allEntities.RemoveAt(i);
                }

                for (int j = 0; j < entitiesToMix.Count; j += 2)
                {
                    if (j + 2 > entitiesToMix.Count)
                    {
                        if (j + 1 <= entitiesToMix.Count) entitiesToMix[j].RandomTeleport();
                        break;
                    }
                    Vector3 firstEntityPos = entitiesToMix[j].transform.position;
                    entitiesToMix[j].SpectacularTeleport(entitiesToMix[j + 1].transform.position);
                    entitiesToMix[j + 1].SpectacularTeleport(firstEntityPos);
                }
                entitiesToMix.Clear();

                time = 0.3f;

                yield return null;
            }

            teleporterActive = false;
        }

        private IEnumerator GenericTeleporter()
        {
            List<Entity> allEntities = new List<Entity>(ReflectionHelper.Static_GetValue<Entity, List<Entity>>("allEntities"));

            float time = 0f;

            teleporterActive = true;

            while (allEntities.Count > 0)
            {
                while (time > 0f)
                {
                    time -= Time.deltaTime * ec.EnvironmentTimeScale;
                    yield return null;
                }

                int i = UnityEngine.Random.Range(0, allEntities.Count);
                while (allEntities[i] == null || (allEntities[i] != null && allEntities[i].TryGetComponent(out MathMachineNumber _)))
                {
                    allEntities.RemoveAt(i);
                    i = UnityEngine.Random.Range(0, allEntities.Count);
                    if (allEntities.Count == 0)
                    {
                        teleporterActive = false;
                        yield break;
                    }
                }

                allEntities[i].RandomTeleport();
                allEntities.RemoveAt(i);

                time = 0.5f;

                yield return null;
            }

            teleporterActive = false;
        }

    }
}
