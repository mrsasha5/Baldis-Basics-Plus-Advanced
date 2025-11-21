using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.Player;
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

        public void SetGameTip(int player, string tooltip = null)
        {
            if (OptionsDataManager.ExtraSettings.GetValue<bool>("tips_during_game"))
            {
                if (tooltip != null)
                {
                    Singleton<CoreGameManager>.Instance.GetHud(player).SetTooltip(tooltip);
                } else
                {
                    Singleton<CoreGameManager>.Instance.GetHud(player).CloseTooltip();
                }
            }     
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
