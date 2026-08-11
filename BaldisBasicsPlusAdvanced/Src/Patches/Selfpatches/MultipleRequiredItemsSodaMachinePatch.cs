using BaldisBasicsPlusAdvanced.Game.Objects.SodaMachines;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.Selfpatches
{
    [HarmonyPatch(typeof(SodaMachine))]
    internal class MultipleRequiredItemsSodaMachinePatch
    {

        [HarmonyPatch("InsertItem")]
        [HarmonyPrefix]
        private static bool OnInsert(SodaMachine __instance)
        {
            if (!(__instance is MultipleRequiredItemsSodaMachine)) return true;

            MultipleRequiredItemsSodaMachine machine = (MultipleRequiredItemsSodaMachine)__instance;

            machine.insertedPerTime++;

            if (machine.insertedPerTime >= machine.requiredItemsAmmount)
            {
                machine.insertedPerTime = 0;
                return true;
            }

            return false;
        }

    }
}
