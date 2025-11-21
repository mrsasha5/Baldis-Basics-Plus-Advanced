using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_Pulley : StructureBuilder, IPrefab
    {
        [SerializeField]
        private Pulley pulleyPre;

        [SerializeField]
        private float offsetFromCenter;

        [SerializeField]
        private CellCoverage coverage;

        [SerializeField]
        private TileShapeMask shapes;

        [SerializeField]
        private bool includeOpen;

        public void InitializePrefab(int variant)
        {
            pulleyPre = ObjectStorage.Objects["pulley"].GetComponent<Pulley>();
            offsetFromCenter = 2f;
            shapes = TileShapeMask.Corner | TileShapeMask.Straight | TileShapeMask.End | TileShapeMask.Single;
            coverage = CellCoverage.Center;
            includeOpen = true;
        }

        public override void Load(List<StructureData> data)
        {
            base.Load(data);

            for (int i = 0; i < data.Count; i += 5)
            {
                Pulley pulley = Build(data[i].prefab.GetComponent<Pulley>(), ec.cells[data[i].position.x, data[i].position.z], 
                    data[i].direction.GetOpposite());

                pulley.uses = data[i + 1].data;
                pulley.points = data[i + 2].data;
                pulley.finalPoints = data[i + 3].data;
                pulley.maxDistance = data[i + 4].data.ConvertToFloatNoRecast();
            }
        }

        public override void PostOpenCalcGenerate(LevelGenerator lg, System.Random rng)
        {
            base.PostOpenCalcGenerate(lg, rng);

            List<Cell> cells = ec.mainHall.GetTilesOfShape(shapes, includeOpen);

            int num = rng.Next(parameters.minMax[0].x, parameters.minMax[0].z + 1);

            Cell cell;

            Direction chosenDir = Direction.Null;

            for (int i = 0; i < num; i++)
            {
                chosenDir = Direction.Null;

                if (cells.Count <= 0)
                {
                    break;
                }

                while (chosenDir == Direction.Null && cells.Count > 0)
                {
                    cell = cells[rng.Next(0, cells.Count)];

                    foreach (Direction dir in Directions.All())
                    {
                        if (cell.HasWallInDirection(dir.GetOpposite()) && 
                            cell.AllCoverageFits(coverage | dir.GetOpposite().ToCoverage()))
                        {
                            cell.HardCover(coverage | dir.GetOpposite().ToCoverage());
                            chosenDir = dir;
                            break;
                        }
                    }

                    if (chosenDir != Direction.Null)
                        Build(pulleyPre, cell, chosenDir);
                    
                    cells.Remove(cell);
                }
            }
        }

        public Pulley Build(Pulley pulleyPre, Cell cell, Direction dir)
        {
            Pulley pulley = Instantiate(pulleyPre, ec.transform);

            MeshRenderer backgroundRenderer = ObjectCreator.CreateQuadRenderer();
            backgroundRenderer.name = "PulleyBackgroundRenderer";
            backgroundRenderer.transform.SetParent(cell.room.objectObject.transform, false);
            backgroundRenderer.transform.position = cell.CenterWorldPosition - Directions.ToVector3(dir) * 5f;

            MeshRenderer meshRenderer = ObjectCreator.CreateQuadRenderer();
            meshRenderer.name = "PulleyRenderer";
            meshRenderer.transform.SetParent(backgroundRenderer.transform.parent, false);
            meshRenderer.transform.localScale = new Vector3(3f, 3f, 1f);
            meshRenderer.transform.position = backgroundRenderer.transform.position + Directions.ToVector3(dir) * 0.1f;
            meshRenderer.material.mainTexture = pulley.SpriteRenderer.sprite.texture;

            Quaternion rotation = meshRenderer.transform.rotation;
            rotation.eulerAngles = new Vector3(0f, Directions.ToDegrees(dir.GetOpposite()), 0f);

            meshRenderer.transform.rotation = rotation;
            backgroundRenderer.transform.rotation = rotation;

            pulley.Initialize(ec, cell.CenterWorldPosition - Directions.ToVector3(dir) * 5f, Directions.ToVector3(dir) * offsetFromCenter,
                new MeshRenderer[] { backgroundRenderer, meshRenderer } );

            return pulley;
        }

    }
}
