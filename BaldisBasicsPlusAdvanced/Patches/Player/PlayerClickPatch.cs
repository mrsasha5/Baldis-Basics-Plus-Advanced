using HarmonyLib;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Player
{
    [HarmonyPatch(typeof(PlayerClick))]
    internal class PlayerClickPatch
    {
        private static PlayerClick instance;

        private static bool enabled = true;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool OnUpdate()
        {
            return instance == null || (instance != null && enabled);
        }

        public static void SetEnabled(bool state)
        {
            enabled = state;
            if (!state)
            {
                if (instance != null)
                {
                    instance.clickedThisFrame = null;
                    if (instance.seesClickable)
                    {
                        Singleton<CoreGameManager>.Instance.GetHud(0).UpdateReticle(active: false);
                    }
                    instance.seesClickable = false;
                } else
                {
                    instance = Singleton<CoreGameManager>.Instance.GetPlayer(0).GetComponent<PlayerClick>();
                    instance.clickedThisFrame = null;
                    if (instance.seesClickable)
                    {
                        Singleton<CoreGameManager>.Instance.GetHud(0).UpdateReticle(active: false);
                    }
                    instance.seesClickable = false;
                }
            }
            
        }

    }
}
