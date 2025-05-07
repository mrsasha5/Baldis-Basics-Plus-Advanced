using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_GumDispenser : StructureBuilder, IPrefab
    {
        [SerializeField]
        private WeightedGameObject[] prefabs;

        [SerializeField]
        private WeightedGameObject[] buttons;

        [SerializeField]
        private CellCoverage coverage;

        [SerializeField]
        private int buttonRange;

        public void InitializePrefab(int variant)
        {
            coverage = CellCoverage.Center;
            prefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["gum_dispenser"],
                    weight = 100
                }
            };
            buttons = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = AssetsStorage.gameButton.gameObject,
                    weight = 100
                }
            };
            buttonRange = 6;
        }

        public override void PostOpenCalcGenerate(LevelGenerator lg, System.Random rng)
        {
            base.PostOpenCalcGenerate(lg, rng);

            List<List<Cell>> halls = lg.Ec.FindHallways();
            halls.Sort((a, b) => b.Count - a.Count);

            int count = rng.Next(parameters.minMax[0].x, parameters.minMax[0].z + 1);

            for (int i = 0; i < halls.Count; i++)
            {
                List<Cell> cells = halls[i];
                Cell cell1 = cells[0]; //end cell for calculations
                Cell cell2 = cells[cells.Count - 1]; //start cell for calculations

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

                if (cell2 != null && cell2.AllCoverageFits(coverage | startWallDir.ToCoverage()))
                {
                    if (BuildDispenser(lg, cell2, startWallDir.GetOpposite())) count--;
                    
                } else if (cell1 != null && cell1.AllCoverageFits(coverage | endWallDir.ToCoverage()))
                {
                    if (BuildDispenser(lg, cell1, endWallDir.GetOpposite())) count--;
                }

                if (count <= 0) break;
            }

        }

        public bool BuildDispenser(LevelGenerator lg, Cell cell, Direction dir)
        {
            GumDispenser dispenser = Instantiate(WeightedGameObject.ControlledRandomSelection(prefabs, lg.controlledRNG)
                        .GetComponent<GumDispenser>(), cell.ObjectBase);

            cell.HardCoverWall(dir.GetOpposite(), covered: true); //based on game logic (Rotohalls uses that logic lol)

            if (GameButton.BuildInArea(ec, cell.position, cell.position, buttonRange, dispenser.gameObject,
                WeightedGameObject.ControlledRandomSelection(buttons, lg.controlledRNG).GetComponent<GameButton>(), lg.controlledRNG) != null)
            {
                dispenser.transform.rotation = dir.ToRotation();
                dispenser.ec = ec;
                dispenser.position = cell.position;
                dispenser.bOffset = dir.ToIntVector2();
                dispenser.direction = dir;

                return true;
            };

            Destroy(dispenser.gameObject);
            
            return false;
        }
    }
}
