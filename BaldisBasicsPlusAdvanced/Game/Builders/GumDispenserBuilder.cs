using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class GumDispenserBuilder : ObjectBuilder, IPrefab
    {
        [SerializeField]
        private int buttonRange;

        private GumDispenser dispenser;

        public void InitializePrefab()
        {
            buttonRange = 6;
        }

        public override void Build(EnvironmentController ec, LevelBuilder builder, RoomController room, System.Random cRng)
        {
            List<Cell> list = new List<Cell>();
            List<TileShape> list2 = new List<TileShape>();
            list2.Add(TileShape.End);
            list = room.GetTilesOfShape(list2, includeOpen: false);

            Cell cell = null;
            while (list.Count > 0 && cell == null)
            {
                int index = cRng.Next(0, list.Count);
                if (list[index].HasAnyHardCoverage)
                {
                    list.RemoveAt(index);
                }
                else
                {
                    cell = list[index];
                }
            }

            if (cell != null)
            {
                //dispenser = UnityEngine.Object.Instantiate(ObjectsStorage.Objects["gum_dispenser"].GetComponent<GumDispenser>(), cell.ObjectBase);
                dispenser = UnityEngine.Object.Instantiate(ObjectsStorage.Objects["gum_dispenser"].GetComponent<GumDispenser>(), room.transform);

                dispenser.transform.position = cell.FloorWorldPosition;
                dispenser.ChooseBestRotation();

                cell.HardCoverEntirely();

                if (GameButton.BuildInArea(ec, cell.position, cell.position, buttonRange, dispenser.gameObject, AssetsStorage.gameButton, cRng) == null)
                {
                    //Debug.LogWarning("No suitable location for a Gum Dispenser button was found. Destroying the Gum Dispenser");
                    UnityEngine.Object.Destroy(dispenser.gameObject);
                }

                dispenser.ec = ec;
                dispenser.position = cell.position;

                Quaternion[] rotations = new Quaternion[4]
                {
                    Quaternion.identity,
                    Quaternion.Euler(0f, 90f, 0f),
                    Quaternion.Euler(0f, 180f, 0f),
                    Quaternion.Euler(0f, 270f, 0f)
                };

                Quaternion rotation = Array.Find(rotations, x => x == dispenser.transform.rotation);

                dispenser.direction = (Direction)Array.IndexOf(rotations, rotation);
                dispenser.bOffset = new IntVector2(0, 0);
            }
        }

    }
}
