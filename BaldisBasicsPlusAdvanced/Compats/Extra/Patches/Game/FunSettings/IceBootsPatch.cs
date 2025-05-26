using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Game.GameItems;
using BBE.Extensions;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Patches.Game.FunSettings
{
    [HarmonyPatch(typeof(IceBootsItem))]
    [ConditionalPatchIntegratedMod(ModsIntegration.extraId, ModsIntegration.extraVersion)]
    internal class IceBootsPatch
    {
        [HarmonyPatch("Use")]
        [HarmonyPostfix]
        private static void OnUse(PlayerManager pm, ref bool __result)
        {
            if (MTM101BaldAPI.EnumExtensions.GetFromExtendedName<FunSettingsType>("IceBoots").IsActive() && pm.itm.selectedItem == 0)
            {
                __result = false;
            }
        }

    }
}
