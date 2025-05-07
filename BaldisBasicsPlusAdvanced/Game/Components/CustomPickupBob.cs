using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class CustomPickupBob : MonoBehaviour
    {
        public float addendY;

        private void Update()
        {
            base.transform.localPosition = new Vector3(0f, PickupBobValue.bobVal + addendY, 0f);
        }

        private void OnEnable()
        {
            base.transform.localPosition = new Vector3(0f, PickupBobValue.bobVal + addendY, 0f);
        }

    }
}
