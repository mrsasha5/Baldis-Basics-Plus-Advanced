using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;

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

        public void InitializePrefab(int variant)
        {
            coverage = CellCoverage.Up;// | CellCoverage.Center;
            shapes = TileShapeMask.Corner | TileShapeMask.End | TileShapeMask.Open | TileShapeMask.Straight;
            includeOpen = true;
            minLength = 9;
        }

        public override void OnGenerationFinished(LevelBuilder lb)
        {
            base.OnGenerationFinished(lb);

            if (!(lb is LevelGenerator)) return;

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

                        int length = AnalysePath(cells[i], cells[j], setCoverage: false);

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

                    Build(lb, straightPaths[index]);
                }
                else break;

                count--;
            }

            straightPaths.Clear();
            lengths.Clear();
        }

        public void Build(LevelBuilder lb, KeyValuePair<Cell, Cell> cells)
        {
            AnalysePath(cells.Key, cells.Value, setCoverage: true);

            GameObject gm = new GameObject("Zipline");

            gm.transform.parent = ec.transform;

            LineRenderer lineRenderer = new GameObject("LineRenderer").AddComponent<LineRenderer>();
            lineRenderer.transform.parent = gm.transform;
            lineRenderer.startWidth = 0.1977f;
            lineRenderer.endWidth = 0.1977f;
            lineRenderer.allowOcclusionWhenDynamic = false;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.material = new Material(AssetsStorage.materials["black_behind"]);

            lineRenderer.SetPositions(new Vector3[]
            {
                cells.Key.TileTransform.position + Vector3.up * 9f,
                cells.Value.TileTransform.position + Vector3.up * 9f
            });

            Vector3 direction = (cells.Key.TileTransform.position - cells.Value.TileTransform.position).normalized;

            ZiplineHanger hanger = Instantiate(WeightedGameObject.ControlledRandomSelection(parameters.prefab, lb.controlledRNG).GetComponent<ZiplineHanger>());
            hanger.transform.parent = gm.transform;
            hanger.Initialize(ec, lb.controlledRNG, new KeyValuePair<Vector3, Vector3>(cells.Key.TileTransform.position - direction * hanger.Offset,
                cells.Value.TileTransform.position + direction * hanger.Offset));

            CreateCubeDecoration(cells.Key.TileTransform.position + Vector3.up * 10f, gm.transform);
            CreateCubeDecoration(cells.Value.TileTransform.position + Vector3.up * 10f, gm.transform);
        }

        private void CreateCubeDecoration(Vector3 pos, Transform parent)
        {
            GameObject gm = GameObject.CreatePrimitive(PrimitiveType.Cube);

            gm.transform.parent = parent;
            gm.transform.position = pos;
            gm.transform.localScale = Vector3.one + Vector3.up * 2f;

            MeshRenderer renderer = gm.GetComponent<MeshRenderer>();
            renderer.material = new Material(AssetsStorage.materials["belt"]);
            //renderer.material.SetMainTexture(AssetsStorage.textures["white"]);
            //renderer.material.SetColor(Color.gray);

            Destroy(gm.GetComponent<Collider>());
        }

        private int AnalysePath(Cell cell1, Cell cell2, bool setCoverage)
        {
            Vector3 direction = (cell1.TileTransform.position - cell2.TileTransform.position).normalized;

            //Debug.Log($"Direction. x: {direction.x} y: {direction.y} z: {direction.z}");
            if (direction.x != 0f && direction.x != 1f) return -1; //do not spam with a lot of logs about null direction
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

            //start - cell2
            //goal - cell1
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

                if (setCoverage) cell.HardCover(coverage);

                length++;

                if (cell == cell1) break;

                pos += new IntVector2((int)direction.x, (int)direction.z);
            }

            return length;
        }
    }
}
