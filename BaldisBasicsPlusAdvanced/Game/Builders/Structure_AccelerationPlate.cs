using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_AccelerationPlate : BaseStructure_Plate
    {
        [SerializeField]
        private WeightedGameObject[] buttonsPre;

        [SerializeField]
        private int buttonRange;

        [SerializeField]
        private int minHallLength;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);
            buttonRange = 6;
            buttonsPre = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = AssetsStorage.gameButton.gameObject,
                    weight = 100
                }
            };

            hallPrefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["acceleration_plate"],
                    weight = 100
                }
            };

            minHallLength = 6;
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
                            plate.SetAngleIndexByAngle((float)tiles[j].Value * 90f);
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
                GameButton.BuildInArea(ec, cell.position, buttonRange, plate.gameObject,
                    WeightedGameObject.ControlledRandomSelection(buttonsPre, rng).GetComponent<GameButton>(), rng);
            }
            return plate;
        }

    }
}
