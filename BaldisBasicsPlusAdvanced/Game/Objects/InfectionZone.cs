using BaldisBasicsPlusAdvanced.Cache;
using MTM101BaldAPI;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class InfectionZone : MonoBehaviour, IPrefab
    {
        private float time;

        private EnvironmentController ec;

        public void InitializePrefab(int variant)
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 30f;

            ParticleSystem particleSystem = gameObject.AddComponent<ParticleSystem>();

            MainModule main = particleSystem.main;
            EmissionModule emission = particleSystem.emission;
            ShapeModule shape = particleSystem.shape;
            SizeOverLifetimeModule sizeOverLifetime = particleSystem.sizeOverLifetime;
            ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;

            colorOverLifetime.enabled = true;

            main.loop = true;
            main.startSpeedMultiplier = 0f; //don't want to speed
            main.startLifetime = 3f;
            main.startSize = 1f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.playOnAwake = true; //useless if isn't used by prefab.
            main.startColor = Color.white;

            emission.rateOverTime = 100f;
            /*emission.SetBursts(new Burst[] {
                    new Burst(0f, 200f)
                }
            );*/

            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = collider.radius;

            ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(AssetsStorage.materials["adv_white"]);
            renderer.material.SetColor(Color.green);
            //renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }

        public void Initialize(EnvironmentController ec, float time)
        {
            this.ec = ec;
            this.time = time;
        }

        private void Update()
        {
            if (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (time <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }

        //todo: stay
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("NPC") || other.CompareTag("Player"))
            {
                
            }
        }

        /*private void OnTriggerExit(Collider other)
        {

        }*/
    }
}
