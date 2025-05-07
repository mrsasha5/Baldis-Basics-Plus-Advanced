using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.Movement
{
    public class RotatableTransformBehaviour : MonoBehaviour
    {

        public Vector3 addend;

        public float multiplier = 1f;

        public bool updating = true;

        public bool unscaledTime;

        private void Update()
        {
            if (!updating) return;
            Quaternion quaternion = transform.rotation;
            if (unscaledTime) quaternion.eulerAngles += addend * multiplier * Time.unscaledDeltaTime;
            else quaternion.eulerAngles += addend * multiplier * Time.deltaTime;
            transform.rotation = quaternion;
        }

    }
}
