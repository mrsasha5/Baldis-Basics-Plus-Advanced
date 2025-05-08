using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class TemporaryGameObject : MonoBehaviour
    {

        private EnvironmentController ec;

        private GameObject target;

        private float time;

        public void Initialize(EnvironmentController ec, GameObject target, float livingTime)
        {
            this.ec = ec;
            this.target = target;
            this.time = livingTime;
        }

        private void Update()
        {
            if (time >= 0)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
            } else
            {
                Destroy(target);
            }
        }

    }
}
