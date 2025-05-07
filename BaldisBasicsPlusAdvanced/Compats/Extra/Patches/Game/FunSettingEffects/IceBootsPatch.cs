using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.GameItems;
using BBE;
using BBE.CustomClasses;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.PlusExtensions;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Patches.Game.FunSettingEffects
{
    [HarmonyPatch(typeof(BaseGameManager))]
    [ConditionalPatchMod(ModsIntegration.extraId)]
    internal class IceBootsFunSettingPatch
    {

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInitialize()
        {
            if ("IceBoots".ToEnum<FunSettingsType>().IsActive())
            {
                PlayerMovementStatModifier moveModifier = Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier();
                moveModifier.AddModifier("walkSpeed", new ValueModifier(0f));
                moveModifier.AddModifier("runSpeed", new ValueModifier(0f));

                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.SetItem(ObjectsStorage.ItemsObjects["IceBoots"], 0);
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(0, val: true);
            }
        }

    }

    [HarmonyPatch(typeof(IceBootsItem))]
    [ConditionalPatchMod(ModsIntegration.extraId)]
    internal class IceBootsPatch
    {

        [HarmonyPatch("Use")]
        [HarmonyPostfix]
        private static void OnUse(PlayerManager pm, ref bool __result)
        {
            if ("IceBoots".ToEnum<FunSettingsType>().IsActive() && pm.itm.selectedItem == 0)
            {
                __result = false;
            }
        }

    }
}
