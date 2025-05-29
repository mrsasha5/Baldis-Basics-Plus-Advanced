using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Rooms.Functions
{
    public class CorruptedLightsFunction : RoomFunction
    {
        private float time;

        private List<Cell> targetedCells = new List<Cell>();

        private int blinks;

        public override void AfterAllRoomsPlaced(LevelBuilder builder, System.Random rng)
        {
            int lightsCount = (int)(room.standardLightCells.Count * 0.35f);
            if (lightsCount == 0 && room.standardLightCells.Count > 0) lightsCount = 1;

            List<Cell> potentialTargets = room.cells.FindAll(x => x.hasLight);
            for (int i = 0; i < lightsCount; i++)
            {
                targetedCells.Add(potentialTargets[rng.Next(0, potentialTargets.Count)]);
            }

            time = 1f;
        }

        private void Update()
        {
            if (room.Powered && time > 0f && blinks <= 0 && targetedCells.Count > 0)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                if (time <= 0f)
                {
                    time = Random.Range(20f, 45f);
                    StartCoroutine(BlinkLights());
                }
            }
        }

        private IEnumerator BlinkLights()
        {
            blinks++;

            Cell cellToBlink = targetedCells[Random.Range(0, targetedCells.Count)];

            float time = Random.Range(3f, 6f);
            float cooldownTime = Random.Range(0.1f, 0.15f);
            float enableLightTime = Random.Range(0.1f, 0.25f);

            while (time > 0f && room.Powered)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                if (enableLightTime <= 0f) cooldownTime -= Time.deltaTime * room.ec.EnvironmentTimeScale;

                if (enableLightTime > 0f)
                {
                    enableLightTime -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                    if (enableLightTime <= 0f)
                    {
                        cellToBlink.SetLight(true);
                    }
                }

                if (cooldownTime <= 0f)
                {
                    cooldownTime = Random.Range(0.1f, 0.15f);

                    if (cellToBlink.lightOn)
                    {
                        cellToBlink.SetLight(false);
                        enableLightTime = Random.Range(0.1f, 0.25f);
                    }
                }

                yield return null;
            }

            if (enableLightTime > 0f && room.Powered)
            {
                cellToBlink.SetLight(true);
                enableLightTime = 0f;
            }

            if (!room.Powered) cellToBlink.SetLight(false);

            blinks--;
            yield break;
        }

    }
}
