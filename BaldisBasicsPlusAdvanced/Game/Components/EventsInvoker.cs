using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class EventsInvoker : MonoBehaviour
    {
        public Action onStart;

        public Action onDestroy;

        private void OnStart()
        {
            onStart?.Invoke();
        }

        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }

    }
}
