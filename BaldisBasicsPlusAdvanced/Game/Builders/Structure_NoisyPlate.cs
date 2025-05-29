using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_NoisyPlate : BaseStructure_Plate
    {
        [SerializeField]
        private int pointsPerFaculty;

        [SerializeField]
        private float facultyCooldown;

        [SerializeField]
        private int minTrollingFaculties;

        [SerializeField]
        private int maxTrollingFaculties;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);

            minTrollingFaculties = 1;
            maxTrollingFaculties = 2;
            pointsPerFaculty = 30;
            facultyCooldown = 60f;

            hallPrefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["noisy_plate"],
                    weight = 100
                }
            };
            roomPrefabs = hallPrefabs;
        }

        public override void OnGenerationFinished(LevelBuilder lb)
        {
            System.Random rng = lb.controlledRNG;

            int faculties = rng.Next(minTrollingFaculties, maxTrollingFaculties + 1);

            foreach (RoomController room in ec.rooms)
            {
                List<NoisyPlate> facultyPlates = new List<NoisyPlate>();

                if (room.category == RoomCategory.Faculty)
                {
                    List<IntVector2> positions = room.builtDoorPositions;
                    List<Cell> cells = room.GetNewTileList();
                    for (int i = 0; i < positions.Count; i++)
                    {
                        Cell cell = ec.CellFromPosition(positions[i]);//cells.Find(x => x.position == positions[i]);

                        if (cell != null && cell.HardCoverageFits(CellCoverage.Center | CellCoverage.Down))
                        {
                            facultyPlates.Add((NoisyPlate)BuildPrefab(cell, rng, inRoom: true));

                            Direction dir = cell.doorDirs[0].GetOpposite();

                            facultyPlates[facultyPlates.Count - 1].transform.rotation = dir.ToRotation();
                        }
                    }
                }

                if (facultyPlates.Count > 0) faculties--;

                for (int i = 0; i < facultyPlates.Count; i++)
                {
                    facultyPlates[i].ConnectRange(facultyPlates);
                    facultyPlates[i].SetCallsPrincipal(true);
                    facultyPlates[i].OverrideCooldown(facultyCooldown);
                    facultyPlates[i].SetPointsReward(pointsPerFaculty);
                    facultyPlates[i].SetGenerosityCount(1);
                }
            }

            generatedPlates.Clear();
        }

    }
}
