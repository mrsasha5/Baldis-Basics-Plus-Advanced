using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.GameEventsProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using tripolygon.UModeler;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class NoisyPlatesBuilder : GenericHallBuilder, IPrefab, IGameManagerEventsReceiver
    {
        [SerializeField]
        private int maxTrollingFacultiesCount;

        [SerializeField]
        private int minTrollingFacultiesCount;

        [SerializeField]
        private int pointsPerFaculty;

        private List<PlateBase> plates = new List<PlateBase>();

        private int maxValue;

        public void InitializePrefab()
        {
            minTrollingFacultiesCount = 1;
            maxTrollingFacultiesCount = 3;
            pointsPerFaculty = 30;
        }

        public override void Build(EnvironmentController ec, LevelBuilder builder, RoomController room, System.Random cRng)
        {
            base.Build(ec, builder, room, cRng);

            //this.ec = ec;
            maxValue = cRng.Next(minTrollingFacultiesCount, maxTrollingFacultiesCount + 1);

            int facultiesCounter = 0;
            List<Cell> ignorableCells = new List<Cell>();
            for (int i = 0; i < ec.rooms.Count; i++)
            {
                if (facultiesCounter >= maxValue) break;

                List<NoisyPlate> currentPlates = new List<NoisyPlate>();

                if (ec.rooms[i].category == RoomCategory.Faculty)
                {
                    for (int j = 0; j < ec.rooms[i].doors.Count; j++)
                    {
                        Cell cell = ec.rooms[i].doors[j].aTile;
                        Direction dir = ec.rooms[i].doors[j].direction;

                        if (cell.room != ec.rooms[i])
                        {
                            cell = ec.rooms[i].doors[j].bTile;
                            dir = ec.rooms[i].doors[j].direction.GetOpposite();
                        }

                        if (ignorableCells.Contains(cell)) continue;
                        ignorableCells.Add(cell);

                        NoisyPlate plate = builder.InstatiateEnvironmentObject(ObjectsStorage.GameButtons["noisy_plate"].gameObject,
                            cell,
                            dir)
                            .GetComponent<NoisyPlate>();

                        plate.SetIgnoreCooldown(true);
                        plate.SetGenerosityCount(1);
                        plates.Add(plate);
                        currentPlates.Add(plate);
                    }
                    facultiesCounter++;
                }

                int pointsDistributed = 0;

                for (int j = 0; j < currentPlates.Count; j++)
                {
                    if (j == currentPlates.Count - 1 && pointsDistributed != pointsDistributed + pointsPerFaculty / currentPlates.Count)
                    {
                        currentPlates[j].SetPointsReward(pointsPerFaculty - pointsDistributed);
                        pointsDistributed += pointsPerFaculty - pointsDistributed;
                    } else
                    {
                        currentPlates[j].SetPointsReward(pointsPerFaculty / currentPlates.Count);
                        pointsDistributed += pointsPerFaculty / currentPlates.Count;
                    }
                }
            }

            BaseGameManagerEvents.Register(this);
        }

        public void OnManagerInitPost()
        {
            BaseGameManagerEvents.Unregister(this);
            foreach (PlateBase plate in plates)
            {
                plate.transform.position = plate.transform.localPosition;
            }
        }
    }
}
