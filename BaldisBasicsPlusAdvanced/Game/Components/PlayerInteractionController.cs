using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class PlayerInteractionController : MonoBehaviour
    {

        private static PlayerInteractionController instance;

        private int playerEntitiesInteractionDisables;

        public static PlayerInteractionController Instance => instance;

        private void Awake() {
            if (instance != null) Destroy(instance);
            instance = this;
        }

        public void SetIgnorePlayerEntitiesInteraction(bool state)
        {
            if (state)
            {
                playerEntitiesInteractionDisables++;
            } else
            {
                playerEntitiesInteractionDisables--;
            }

            if (playerEntitiesInteractionDisables < 0) playerEntitiesInteractionDisables = 0;

            LayerHelper.SetIgnoreCollisionForPlayer(playerEntitiesInteractionDisables > 0);
        }

        private void OnDestroy()
        {
            LayerHelper.SetIgnoreCollisionForPlayer(false);
        }

    }
}
