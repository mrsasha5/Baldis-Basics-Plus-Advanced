using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.FakePlate
{
    //TODO: finish the balance
    public class ObstacleTrick : BaseTrick
    {
        private SpriteRenderer renderer;

        private ObstacleTrick_Sign sign;

        private float speed = 10f;//10f;

        private float time;

        public override bool IsSelectable()
        {
            Cell cell = Plate.Ec.CellFromPosition(transform.position);
            if (cell != null)
            {
                return base.IsSelectable() && ((cell.HasWallInDirection(Direction.North) && cell.HasWallInDirection(Direction.South))
                    || (cell.HasWallInDirection(Direction.East) && cell.HasWallInDirection(Direction.West)));
            }

            return false;
        }

        public override void OnPostInitialization()
        {
            base.OnPostInitialization();
            renderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_obstacle_trick"]);
            renderer.transform.parent.SetParent(Plate.transform, false);
            renderer.transform.localPosition = Vector3.up * -5f;
            renderer.gameObject.SetActive(false);
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (activated && time > 0f)
            {
                time -= Time.deltaTime * Plate.Timescale;
                if (time <= 0f && sign != null) sign.Break();
                else if (sign == null) Plate.EndTrick();
            }
        }

        public override void Reset()
        {
            base.Reset();
            //Plate.SetSurprizeVisual(false, true);
            Plate.SetRandomCooldown();
        }

        public override void OnEntityStayTargetZone(Entity entity)
        {
            base.OnEntityStayTargetZone(entity);
            if (entity.CompareTag("Player") && !activated)
            {
                activated = true;
                StartCoroutine(SetupSign());
            }
        }

        private IEnumerator SetupSign()
        {
            renderer.gameObject.SetActive(true);
            Plate.SetSurprizeVisual(true, playAudio: true, playPressingAudio: false);

            float maxHeight = 9f;

            float addendHeight = 0f;

            while (addendHeight < maxHeight)
            {
                addendHeight += Time.deltaTime * Plate.Timescale * speed;
                renderer.transform.localPosition = Vector3.up * (-5f + addendHeight);
                yield return null;
            }

            addendHeight = maxHeight;
            renderer.transform.localPosition = Vector3.up * (-5f + addendHeight);

            Plate.SetSurprizeVisual(false, playAudio: true, playPressingAudio: true);
            CreateObstacle();
            time = Random.Range(15f, 30f);

            yield break;
        }

        private void CreateObstacle()
        {
            BoxCollider collider = new GameObject("SignCollider").AddComponent<BoxCollider>();
            sign = collider.gameObject.AddComponent<ObstacleTrick_Sign>();
            sign.Initialize(Plate, collider, renderer);

            collider.transform.SetParent(transform, false);
            collider.transform.localPosition = Vector3.up * 5f;
            collider.size = new Vector3(10f, 10f, 10f);

            NavMeshObstacle obstacle = collider.gameObject.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(10f, 10f, 10f);
            //obstacle.carveOnlyStationary = false; //it carves mesh even it doesn't move
            obstacle.carving = true; //let's cut out nav mesh!
        }

    }
}
