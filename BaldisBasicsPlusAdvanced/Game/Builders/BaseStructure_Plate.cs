using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    //0 min/max - for hall plates
    //1 min/max - for room plates (not forced value)
    public class BaseStructure_Plate : StructureBuilder, IPrefab
    {

        [SerializeField]
        protected TileShapeMask tileShapes;

        [SerializeField]
        protected CellCoverage coverageToFit;

        [SerializeField]
        protected CellCoverage plateCoverage;

        [SerializeField]
        protected bool includeOpenTiles;

        [SerializeField]
        protected bool includeHalls;

        [SerializeField]
        protected bool includeRooms;

        [SerializeField]
        protected bool roomPlatesCoverageMustHaveWalls;

        [SerializeField]
        protected WeightedGameObject[] hallPrefabs;

        [SerializeField]
        protected WeightedGameObject[] roomPrefabs;

        [SerializeField]
        protected float minPlatesDistance;

        protected List<BasePlate> generatedPlates;

        public virtual void InitializePrefab(int variant)
        {
            tileShapes = TileShapeMask.Open | TileShapeMask.Single | TileShapeMask.Corner | TileShapeMask.Straight | TileShapeMask.End;
            plateCoverage = CellCoverage.Down | CellCoverage.South | CellCoverage.North | CellCoverage.East | CellCoverage.West;
            coverageToFit = CellCoverage.None;
            includeHalls = true;
            roomPlatesCoverageMustHaveWalls = true;
            minPlatesDistance = 50f;
        }

        public override void Initialize(EnvironmentController ec, StructureParameters parameters)
        {
            base.Initialize(ec, parameters);
            generatedPlates = new List<BasePlate>();
        }

        public override void PostOpenCalcGenerate(LevelGenerator lg, System.Random rng)
        {
            base.PostOpenCalcGenerate(lg, rng);
            if (includeHalls) Build(lg, rng, roomCells: false);
        }

        protected virtual List<Cell> GetCells()
        {
            List<Cell> tilesOfShape = ec.mainHall.GetTilesOfShape(tileShapes, coverageToFit, includeOpenTiles);

            return tilesOfShape;
        }

        protected virtual List<Cell> GetRoomCells()
        {
            List<Cell> potentialDoorTiles = new List<Cell>();

            foreach (RoomController room in ec.rooms)
            {
                potentialDoorTiles.AddRange(room.GetPotentialDoorCells());
            }

            potentialDoorTiles.FilterOutCellsThatDontFitCoverage(plateCoverage, CellCoverageType.All, coverageMustHaveWalls: roomPlatesCoverageMustHaveWalls);

            return potentialDoorTiles;
        }

        public virtual void Build(LevelBuilder lb, System.Random rng, bool roomCells)
        {
            List<Cell> ignoredCells = new List<Cell>();

            List<Cell> cellsToAvoid = new List<Cell>();

            foreach (BasePlate plate in FindObjectsOfType<BasePlate>())
            {
                cellsToAvoid.Add(ec.CellFromPosition(plate.transform.position));
            }

            List<Cell> cells = roomCells ? GetRoomCells() : GetCells();

            int num = roomCells ? rng.Next(parameters.minMax[1].x, parameters.minMax[1].z + 1) :
                                    rng.Next(parameters.minMax[0].x, parameters.minMax[0].z + 1);

            int i = 0;

            for (; i < num; i++)
            {
                if (cells.Count <= 0)
                {
                    break;
                }

                int index = rng.Next(0, cells.Count);

                bool ignored = false;

                for (int i2 = 0; i2 < cellsToAvoid.Count; i2++)
                {
                    if (!IsEnoughDistance(cells[index], cellsToAvoid[i2]))
                    {
                        ignored = true;
                        break;
                    }
                }

                if (!ignored)
                {
                    BuildPrefab(cells[index], rng, roomCells);

                    cellsToAvoid.Add(cells[index]);

                    cells.RemoveAt(index);
                }
                else
                {
                    ignoredCells.Add(cells[index]);
                }
            }

            for (; i < num; i++)
            {
                if (ignoredCells.Count <= 0)
                {
                    break;
                }

                int index = rng.Next(0, ignoredCells.Count);

                BuildPrefab(ignoredCells[index], rng, roomCells);

                ignoredCells.RemoveAt(index);
            }
        }

        public override void OnGenerationFinished(LevelBuilder lb)
        {
            base.OnGenerationFinished(lb);
            if (lb is LevelGenerator)
            {
                if (includeRooms) Build(lb, lb.controlledRNG, roomCells: true);
                generatedPlates.Clear();
            }

        }

        public virtual BasePlate BuildPrefab(Cell cell, System.Random rng, bool inRoom)
        {
            BasePlate plate = UnityEngine.Object.Instantiate(
                WeightedGameObject.ControlledRandomSelection(inRoom ? roomPrefabs : hallPrefabs, rng).GetComponent<BasePlate>(),
                cell.room.objectObject.transform);
            plate.transform.position = cell.FloorWorldPosition;

            Direction dir = RandomBuildDirection(cell, plateCoverage, useWallDirection: true, rng);

            if (dir != Direction.Null) plate.transform.rotation = dir.ToRotation();

            cell.HardCover(plateCoverage);
            generatedPlates.Add(plate);
            return plate;
        }

        protected bool IsEnoughDistance(Cell cell1, Cell cell2)
        {
            if (Vector3.Distance(cell1.CenterWorldPosition, cell2.CenterWorldPosition) > minPlatesDistance)
            {
                return true;
            }
            return false;
        }
    }
}
