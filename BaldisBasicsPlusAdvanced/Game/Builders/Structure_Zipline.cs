using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections.Generic;
using UnityEngine;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using MTM101BaldAPI;
using System;
using UnityEngine.Rendering;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_Zipline : StructureBuilder, IPrefab
    {
        [SerializeField]
        private TileShapeMask shapes;

        [SerializeField]
        private CellCoverage coverage;

        [SerializeField]
        private int minLength;

        [SerializeField]
        private bool includeOpen;

        internal static GameObject ceilingPillarPre;

        public void InitializePrefab(int variant)
        {
            coverage = CellCoverage.Up;// | CellCoverage.Center;
            shapes = TileShapeMask.Corner | TileShapeMask.End | TileShapeMask.Open | TileShapeMask.Straight;
            includeOpen = true;
            minLength = 9;

            ceilingPillarPre = new GameObject("CeilingPillar");
            ceilingPillarPre.gameObject.ConvertToPrefab(true);

            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "CeilingPillar";
            pillar.transform.localScale = Vector3.one - Vector3.up * 0.25f;
            MeshRenderer renderer = pillar.GetComponent<MeshRenderer>();
            renderer.material = new Material(AssetsStorage.materials["belt"]);
            Destroy(pillar.GetComponent<Collider>());

            pillar.transform.SetParent(ceilingPillarPre.transform, false);
            pillar.transform.localPosition = Vector3.up * 9.25f;
        }

        public override void OnGenerationFinished(LevelBuilder lb)
        {
            base.OnGenerationFinished(lb);

            if (lb is LevelGenerator)
            {
                GenerateZiplines(lb);
            }
            else
            {
                AdvancedCore.Logging.LogWarning("Ziplines Builder OnGenerationFinished: unknown Level Bulder type.");
            }
        }

        public override void Load(List<StructureData> data)
        {
            base.Load(data);

            for (int i = 0; i < data.Count; i += 2)
            {
                Cell start = ec.CellFromPosition(data[i].position);
                Cell end = ec.CellFromPosition(data[i + 1].position);

                ZiplineHanger hanger =
                    Build(new KeyValuePair<Cell, Cell>(start, end), data[i].prefab.GetComponent<ZiplineHanger>());

                ushort uses = (ushort)(data[i].data >> 16);
                ushort percentageDistanceToBreak = (ushort)data[i].data;

                hanger.OverrideParameters(uses, percentageDistanceToBreak / 100f);

                hanger.PostInitialization();
            }
        }

        public LineRenderer GetLineRenderer()
        {
            LineRenderer lineRenderer = new GameObject("LineRenderer").AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1977f;
            lineRenderer.endWidth = 0.1977f;
            lineRenderer.allowOcclusionWhenDynamic = false;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.material = new Material(AssetsStorage.materials["black_behind"]);
            return lineRenderer;
        }

        public void UpdateRenderer(LineRenderer lineRenderer, Vector3 start, Vector3 end)
        {
            start.y = 9f;
            end.y = start.y;
            lineRenderer.SetPositions(new Vector3[]
            {
                start,
                end
            });
        }

        public ZiplineHanger Build(KeyValuePair<Cell, Cell> cells, ZiplineHanger ziplinePre)
        {
            CoverPath(cells.Key, cells.Value);

            GameObject gm = new GameObject("Zipline");

            gm.transform.parent = ec.transform;

            LineRenderer lineRenderer = GetLineRenderer();
            lineRenderer.transform.parent = gm.transform;

            UpdateRenderer(lineRenderer, cells.Key.TileTransform.position, cells.Value.TileTransform.position);

            Vector3 direction = (cells.Key.TileTransform.position - cells.Value.TileTransform.position).normalized;

            ZiplineHanger hanger = Instantiate(ziplinePre);
            hanger.transform.parent = gm.transform;
            hanger.Initialize(ec, new KeyValuePair<Vector3, Vector3>(cells.Key.TileTransform.position - direction * hanger.Offset,
                cells.Value.TileTransform.position + direction * hanger.Offset));

            CreatePillarDecoration(cells.Key.TileTransform.position, gm.transform);
            CreatePillarDecoration(cells.Value.TileTransform.position, gm.transform);
            return hanger;
        }

        private void CreatePillarDecoration(Vector3 pos, Transform parent)
        {
            GameObject gm = Instantiate(ceilingPillarPre);

            gm.transform.parent = parent;
            pos.y = 0f;
            gm.transform.position = pos;
        }

        private void GenerateZiplines(LevelBuilder lb)
        {
            List<Cell> cells = ec.mainHall.GetTilesOfShape(shapes, coverage, includeOpen);

            List<KeyValuePair<Cell, Cell>> straightPaths = new List<KeyValuePair<Cell, Cell>>();
            List<int> lengths = new List<int>();

            int count = lb.controlledRNG.Next(parameters.minMax[0].x, parameters.minMax[0].z + 1);

            while (count > 0)
            {
                straightPaths.Clear();
                lengths.Clear();
                for (int i = 0; i < cells.Count; i++)
                {
                    for (int j = 0; j < cells.Count; j++)
                    {
                        if (cells[i] == cells[j])
                        {
                            continue;
                        }

                        int length = AnalysePath(cells[i], cells[j]);

                        if (length >= minLength)
                        {
                            straightPaths.Add(new KeyValuePair<Cell, Cell>(cells[i], cells[j]));
                            lengths.Add(length);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                if (lengths.Count > 0)
                {
                    int max = MathHelper.FindMaxValue(lengths.ToArray());

                    int index = lengths.IndexOf(max);

                    Build(straightPaths[index],
                        WeightedGameObject.ControlledRandomSelection(parameters.prefab, lb.controlledRNG).GetComponent<ZiplineHanger>())
                            .PostInitialization();
                }
                else break;

                count--;
            }

            straightPaths.Clear();
            lengths.Clear();
        }

        private void CoverPath(Cell cell1, Cell cell2)
        {
            Vector3 direction = (cell2.FloorWorldPosition - cell1.FloorWorldPosition).normalized;

            Cell cell = cell1;
            Vector3 pos = cell1.FloorWorldPosition;

            cell1.HardCover(coverage);

            while (true)
            {
                pos += direction * 10f;

                if (ec.ContainsCoordinates(pos)) cell = ec.CellFromPosition(pos);
                else
                {
                    AdvancedCore.Logging.LogWarning($"Ziplines Builder: cell is not found at ({pos.x}; {pos.y}; {pos.z})!");
                    break;
                }

                cell.HardCover(coverage);

                if (cell == cell2) break;
            }
        }

        //Invented only for 0, 90, 180, 270 angles
        private int AnalysePath(Cell cell1, Cell cell2)
        {
            Vector3 direction = (cell1.FloorWorldPosition - cell2.FloorWorldPosition).normalized;

            if (direction.x != 0f && direction.x != 1f) return -1; //Do not spam with a lot of logs about null direction
            if (direction.z != 0f && direction.z != 1f) return -1;

            Cell cell = cell2;
            IntVector2 pos = cell.position;
            int length = 0;

            Direction dir = Directions.DirFromVector3(direction, 0f);

            if (dir == Direction.Null) return -1;

            CellCoverage startCover = this.coverage | dir.ToCoverage();
            CellCoverage cover = this.coverage | dir.ToCoverage() | dir.GetOpposite().ToCoverage();
            CellCoverage endCover = this.coverage | dir.GetOpposite().ToCoverage();

            if (cell2.Null || cell1.Null || cell2.HasWallInDirection(dir) || cell1.HasWallInDirection(dir.GetOpposite()) ||
                !cell2.AllCoverageFits(startCover) || !cell1.AllCoverageFits(endCover)) return -1;

            //Start - cell2
            //Goal - cell1
            while (true)
            {
                if (ec.ContainsCoordinates(pos))
                {
                    cell = ec.CellFromPosition(pos);
                }
                else
                {
                    return -1;
                }

                if (cell != cell1 && cell != cell2)
                {
                    if (cell.Null || !cell.AllCoverageFits(cover) || cell.HasWallInDirection(dir.GetOpposite()) ||
                    cell.HasWallInDirection(dir))
                    {
                        return -1;
                    }
                }

                length++;

                if (cell == cell1) break;

                pos += new IntVector2((int)direction.x, (int)direction.z);
            }

            return length;
        }
    }
}
