using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_AccelerationPlate : BaseStructure_Plate
    {
        [SerializeField]
        private GameButtonBase buttonPre;

        [SerializeField]
        private int buttonRange;

        [SerializeField]
        private int minHallLength;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);
            buttonRange = 6;
            buttonPre = AssetStorage.gameButton;

            hallPrefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectStorage.Objects["acceleration_plate"],
                    weight = 100
                }
            };

            minHallLength = 6;
        }

        public override void Load(List<StructureData> data)
        {
            base.Load(data);

            for (int i = 0; i < data.Count; i += 9)
            {
                AccelerationPlate plate = (AccelerationPlate)BuildPrefab(data[i].prefab.GetComponent<AccelerationPlate>(), 
                    ec.cells[data[i].position.x, data[i].position.z], data[i].direction);

                float firstAngle = data[i].direction.ToDegrees();

                for (int j = 1; j <= data[i].data; j++)
                {
                    plate.DefineRotateDirection(firstAngle + data[i + j].data.ConvertToFloatNoRecast());
                }
                i += data[i].data;

                plate.UpdateAngleIndex(firstAngle);
                plate.SetRotation(firstAngle);

                plate.Data.showsUses = data[i + 6].data.ToBool();
                plate.Data.showsCooldown = data[i + 7].data.ToBool();

                plate.SetMaxUses(data[i + 1].data);

                if (data[i + 2].data >= 0f)
                    plate.ForcefullyPatchCooldown(data[i + 2].data.ConvertToFloatNoRecast());

                plate.Data.timeToUnpress = data[i + 3].data.ConvertToFloatNoRecast();
                plate.initialSpeed = data[i + 4].data.ConvertToFloatNoRecast();
                plate.acceleration = data[i + 5].data.ConvertToFloatNoRecast();

                if (data.Count - 1 > i + 7 && data[i + 8].prefab == null)
                {
                    GameButtonBase button = GameButton.Build(buttonPre, ec, data[i + 8].position, data[i + 8].direction);
                    button.SetUp(plate);
                }
                else i--;
            }
        }

        public override void Build(LevelBuilder lb, System.Random rng, bool isRoomCells)
        {
            if (!isRoomCells)
            {
                int count = rng.Next(parameters.minMax[0].x, parameters.minMax[0].z + 1);

                List<List<Cell>> halls = ec.FindHallways();

                for (int i = 0; i < halls.Count; i++)
                {
                    if (halls[i].Count < minHallLength)
                    {
                        halls.RemoveAt(i);
                        i--;
                    }
                }

                halls.Sort((a, b) => b.Count - a.Count);

                List<KeyValuePair<Cell, Direction>> tiles;

                for (int i = 0; i < halls.Count; i++)
                {
                    if (count == 0) break;

                    tiles = GetAppropriateTiles(halls[i]);

                    for (int j = 0; j < tiles.Count; j++)
                    {
                        if (count == 0) break;
                        AccelerationPlate plate = (AccelerationPlate)RandomlyBuildPrefab(tiles[j].Key, rng, inRoom: false);
                        plate.transform.rotation = tiles[j].Value.ToRotation();
                        if (plate.IsRotatable)
                            plate.UpdateAngleIndex((float)tiles[j].Value * 90f);
                        count--;
                    }

                }

            }
        }

        public List<KeyValuePair<Cell, Direction>> GetAppropriateTiles(List<Cell> hall)
        {
            List<KeyValuePair<Cell, Direction>> tiles = new List<KeyValuePair<Cell, Direction>>();

            Cell cell1 = hall[0]; //end cell for calculations
            Cell cell2 = hall[hall.Count - 1]; //start cell for calculations

            Vector3 direction = -(cell1.TileTransform.position - cell2.TileTransform.position).normalized;

            Direction startWallDir = Directions.DirFromVector3(direction, buffer: 0f);
            Direction endWallDir = Directions.DirFromVector3(-direction, buffer: 0f);

            if (ec.ContainsCoordinates(cell2.position + new IntVector2((int)direction.x, (int)direction.z)))
            {
                Cell _cell2 = ec.CellFromPosition(cell2.position + new IntVector2((int)direction.x, (int)direction.z));
                if (!_cell2.Null && _cell2.HasWallInDirection(startWallDir) && ec.GetCellNeighbors(cell2.position).Contains(_cell2)
                    && _cell2.room == ec.mainHall)
                {
                    cell2 = _cell2;
                }
                else cell2 = null;
            }
            else cell2 = null;

            if (ec.ContainsCoordinates(cell1.position - new IntVector2((int)direction.x, (int)direction.z)))
            {
                Cell _cell1 = ec.CellFromPosition(cell1.position - new IntVector2((int)direction.x, (int)direction.z));
                if (!_cell1.Null && _cell1.HasWallInDirection(endWallDir) && ec.GetCellNeighbors(cell1.position).Contains(_cell1)
                    && _cell1.room == ec.mainHall)
                {
                    cell1 = _cell1;
                }
                else cell1 = null;
            }
            else cell1 = null;

            if (cell2 != null && cell2.AllCoverageFits(plateCoverage))
            {
                tiles.Add(new KeyValuePair<Cell, Direction>(cell2, startWallDir.GetOpposite()));
            }
            else if (cell1 != null && cell1.AllCoverageFits(plateCoverage))
            {
                tiles.Add(new KeyValuePair<Cell, Direction>(cell1, endWallDir.GetOpposite()));
            }

            return tiles;
        }

        public override BasePlate RandomlyBuildPrefab(Cell cell, System.Random rng, bool inRoom)
        {
            AccelerationPlate plate = (AccelerationPlate)base.RandomlyBuildPrefab(cell, rng, inRoom);
            plate.InitializePotentialDirections();
            if (rng.Next(0, 101) <= 50 && plate.IsRotatable)
            {
                GameButton.BuildInArea(ec, cell.position, buttonRange, plate.gameObject, buttonPre, rng);
            }
            return plate;
        }

    }
}
