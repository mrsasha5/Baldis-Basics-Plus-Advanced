/*using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class EntityCheckerTriggerObject : MonoBehaviour
    {

        public Action<PlayerManager> onPlayerEnter;

        public Action<PlayerManager> onPlayerExit;

        public Action<Entity> onEntityEnter;

        public Action<Entity> onEntityExit;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                onPlayerEnter?.Invoke(collider.GetComponent<PlayerManager>());
            } else if (collider.TryGetComponent(out Entity entity))
            {
                onEntityEnter?.Invoke(entity);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                onPlayerExit?.Invoke(collider.GetComponent<PlayerManager>());
            }
            else if (collider.TryGetComponent(out Entity entity))
            {
                onEntityExit?.Invoke(entity);
            }
        }

    }
}
*/