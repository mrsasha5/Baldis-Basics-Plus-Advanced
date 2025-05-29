using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    // 0 min/max - for hall plates
    // 1 min/max - for room plates (not forced value)
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

        //[SerializeField]
        //private bool avoidCloseTiles;

        //protected int roomPlates;

        [SerializeField]
        protected WeightedGameObject[] hallPrefabs;

        [SerializeField]
        protected WeightedGameObject[] roomPrefabs;

        protected List<BasePlate> generatedPlates;

        public virtual void InitializePrefab(int variant)
        {
            tileShapes = TileShapeMask.Open | TileShapeMask.Single | TileShapeMask.Corner | TileShapeMask.Straight | TileShapeMask.End;
            plateCoverage = CellCoverage.Down | CellCoverage.South | CellCoverage.North | CellCoverage.East | CellCoverage.West;
            coverageToFit = CellCoverage.None;
            includeHalls = true;
            roomPlatesCoverageMustHaveWalls = true;
            //avoidCloseTiles = true;
        }

        public override void Initialize(EnvironmentController ec, StructureParameters parameters)
        {
            base.Initialize(ec, parameters);
            generatedPlates = new List<BasePlate>();
        }

        public override void PostOpenCalcGenerate(LevelGenerator lg, System.Random rng)
        {
            base.PostOpenCalcGenerate(lg, rng);
            if (includeHalls) Build(lg, rng, isRoomCells: false);
        }

#warning TODO: invent calculation algorithm to avoid plates accumulation
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

        public virtual void Build(LevelBuilder lb, System.Random rng, bool isRoomCells)
        {
            List<Cell> cells = isRoomCells ? GetRoomCells() : GetCells();

            int num = isRoomCells ? rng.Next(parameters.minMax[1].x, parameters.minMax[1].z + 1) :
                                    rng.Next(parameters.minMax[0].x, parameters.minMax[0].z + 1);
            for (int i = 0; i < num; i++)
            {
                if (cells.Count <= 0)
                {
                    break;
                }

                int index = 0;

                /*if (isRoomCells || !avoidCloseTiles)*/ index = rng.Next(0, cells.Count);

                BuildPrefab(cells[index], rng, isRoomCells);

                cells.RemoveAt(index);
            }
        }

        public override void OnGenerationFinished(LevelBuilder lb)
        {
            base.OnGenerationFinished(lb);
            if (lb is LevelGenerator)
            {
                if (includeRooms) Build(lb, lb.controlledRNG, isRoomCells: true);
                generatedPlates.Clear();
            }
            
        }

        public virtual BasePlate BuildPrefab(Cell cell, System.Random rng, bool inRoom)
        {
            BasePlate plate = UnityEngine.Object.Instantiate(
                WeightedGameObject.ControlledRandomSelection(inRoom ? roomPrefabs : hallPrefabs, rng).GetComponent<BasePlate>(),
                cell.room.objectObject.transform);
            plate.transform.position = cell.FloorWorldPosition;

            Direction dir = RandomBuildDirection(cell, plateCoverage, useWallDirection: true, rng);//cell.RandomUncoveredDirection(rng);

            if (dir != Direction.Null) plate.transform.rotation = dir.ToRotation();
            
            cell.HardCover(plateCoverage);
            generatedPlates.Add(plate);
            return plate;
        }
    }
}
