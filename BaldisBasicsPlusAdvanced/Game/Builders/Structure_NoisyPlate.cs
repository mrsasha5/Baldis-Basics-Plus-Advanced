using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
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

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);

            pointsPerFaculty = 30;
            facultyCooldown = 60f;

            includeHalls = false;
            roomPrefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["noisy_plate"],
                    weight = 100
                }
            };
        }

        public override void Load(List<StructureData> data)
        {
            base.Load(data);

            List<int> extraData = new List<int>();

            for (int i = 0; i < data.Count; i++)
            {
                List<NoisyPlate> currentPlates =
                    BuildInRoom(ec.rooms[data[i].data], data[i].prefab.GetComponent<NoisyPlate>(), ignoreCoverage: true);

                for (int i2 = i + 1; i2 < data.Count; i2++)
                {
                    if (data[i2].prefab == null)
                    {
                        extraData.Add(data[i2].data);
                        i = i2;
                    }
                    else
                    {
                        i = i2 - 1;
                        break;
                    }
                }

                foreach (NoisyPlate plate in currentPlates)
                {
                    if (extraData.Count > 0)
                        plate.SetCooldown(extraData[0]);

                    if (extraData.Count > 1)
                        plate.SetGenerosity(extraData[1]);
                }

                extraData.Clear();
            }
        }

        public override void OnGenerationFinished(LevelBuilder lb)
        {
            if (!(lb is LevelGenerator)) return;

            System.Random rng = lb.controlledRNG;

            int faculties = rng.Next(parameters.minMax[0].x, parameters.minMax[0].z + 1);

            NoisyPlate platePre = WeightedGameObject.ControlledRandomSelection(roomPrefabs, rng)
                .GetComponent<NoisyPlate>();

            foreach (RoomController room in ec.rooms)
            {
                if (room.category != RoomCategory.Faculty) continue;

                if (faculties > 0) faculties--;
                else break;

                BuildInRoom(room, platePre, ignoreCoverage: false);
            }

            generatedPlates.Clear();
        }

        public List<NoisyPlate> BuildInRoom(RoomController room, NoisyPlate prefab, bool ignoreCoverage)
        {
            List<NoisyPlate> facultyPlates = new List<NoisyPlate>();

            for (int i = 0; i < room.doors.Count; i++)
            {
                if (ignoreCoverage || room.doors[i].aTile.HardCoverageFits(CellCoverage.Down))
                {
                    facultyPlates.Add((NoisyPlate)BuildPrefab(prefab, room.doors[i].aTile, room.doors[i].aTile.doorDirs[0].GetOpposite()));
                }
            }

            for (int i = 0; i < facultyPlates.Count; i++)
            {
                facultyPlates[i].ConnectRange(facultyPlates);
                facultyPlates[i].SetCallsPrincipal(true);
                facultyPlates[i].OverrideCooldown(facultyCooldown);
                facultyPlates[i].SetPointsReward(pointsPerFaculty);
                facultyPlates[i].SetGenerosity(1);
            }

            return facultyPlates;
        }

    }
}
