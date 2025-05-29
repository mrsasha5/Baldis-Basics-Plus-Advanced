using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using HarmonyLib;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Items
{
    [HarmonyPatch(typeof(ITM_Scissors))]
    internal class ScissorsPatch
    {

        [HarmonyPatch("Use")]
        [HarmonyPostfix]
        private static void OnUse(ref bool __result)
        {
            foreach (GumProjectile gum in GameObject.FindObjectsOfType<GumProjectile>())
            {
                if (gum.AttachedToPlayer)
                {
                    gum.Cut();
                    __result = true;
                }
            }
            
        }

    }
}
