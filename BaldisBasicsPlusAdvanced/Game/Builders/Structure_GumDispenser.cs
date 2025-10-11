using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_GumDispenser : StructureBuilder, IPrefab
    {
        [SerializeField]
        private WeightedGameObject[] prefabs;

        [SerializeField]
        private GameButtonBase buttonPre;

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
            buttonPre = AssetsStorage.gameButton;
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
                    if (BuildDispenserRandomly(lg, cell2, startWallDir)) count--;
                    
                } else if (cell1 != null && cell1.AllCoverageFits(coverage | endWallDir.ToCoverage()))
                {
                    if (BuildDispenserRandomly(lg, cell1, endWallDir)) count--;
                }

                if (count <= 0) break;
            }

        }

        public override void Load(List<StructureData> data)
        {
            base.Load(data);
            for (int i = 0; i < data.Count; i += 2)
            {
                GumDispenser dispenser =
                    BuildDispenser(data[i].prefab.GetComponent<GumDispenser>(), ec.CellFromPosition(data[i].position), data[i].direction);

                StructureData buttonData = data[i + 1];

                GameButton.Build(buttonPre, ec, buttonData.position, buttonData.direction).SetUp(dispenser);

                ushort uses = (ushort)(data[i].data >> 16);
                ushort cooldown = (ushort)data[i].data;

                dispenser.OverrideParameters(uses, cooldown);
            }
        }

        public GumDispenser BuildDispenser(GumDispenser dispenserPre, Cell cell, Direction dir)
        {
            GumDispenser dispenser = Instantiate(dispenserPre, cell.ObjectBase);

            cell.HardCoverWall(dir, covered: true);

            dispenser.transform.rotation = dir.ToRotation();
            dispenser.ec = ec;
            dispenser.position = cell.position;
            dispenser.bOffset = dir.ToIntVector2();
            dispenser.direction = dir;

            return dispenser;
        }

        public bool BuildDispenserRandomly(LevelGenerator lg, Cell cell, Direction dir)
        {
            GumDispenser dispenser = Instantiate(WeightedGameObject.ControlledRandomSelection(prefabs, lg.controlledRNG)
                        .GetComponent<GumDispenser>(), cell.ObjectBase);

            cell.HardCoverWall(dir, covered: true); //Based on game logic (Rotohalls uses that logic lol)
                                                                  //And I mean that it will cover cell even if button wasn't built

            GameButtonBase button = GameButton.BuildInArea(ec, cell.position, buttonRange, dispenser.gameObject, buttonPre, lg.controlledRNG);

            if (button != null)
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
