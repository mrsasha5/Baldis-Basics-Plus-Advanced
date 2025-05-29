using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class CustomPickupBob : MonoBehaviour
    {
        public float addendY;

        public float bobVal;

        public float speed = 5f;

        public float divider = 2f;

        public float val;

        private void Update()
        {
            val += Time.deltaTime;
            bobVal = Mathf.Sin(val * speed) / divider;
            base.transform.localPosition = new Vector3(0f, bobVal + addendY, 0f);
        }

        /*private void OnEnable()
        {
            base.transform.localPosition = new Vector3(0f, bobVal + addendY, 0f);
        }*/

    }
}
