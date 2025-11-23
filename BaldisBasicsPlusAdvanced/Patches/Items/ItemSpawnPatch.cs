using BaldisBasicsPlusAdvanced.Cache;
using HarmonyLib;
using BaldisBasicsPlusAdvanced.Game.Spawning;
using BaldisBasicsPlusAdvanced.Generation;

namespace BaldisBasicsPlusAdvanced.Patches.Items
{
    [HarmonyPatch(typeof(PartyEvent))]
    internal class ItemSpawnPatch
    {
        //Well actually here is only single prefab of event which means that item set doesn't depend on floor
        //But actually why not support it, lol
        [HarmonyPatch(typeof(PartyEvent), "Initialize")]
        [HarmonyPrefix]
        private static void AddNewItems(ref WeightedItemObject[] ___potentialItems)
        {
            ___potentialItems = ___potentialItems.AddRangeToArray(
                GenerationManager.GetPartyItems(BaseGameManager.Instance.CurrentLevel + 1).ToArray());
        }

        [HarmonyPatch(typeof(MysteryRoom), "AssignRoom")]
        [HarmonyPrefix]
        private static void AddNewMysteryItems(ref WeightedItemObject[] ___items)
        {
            ___items = ___items.AddRangeToArray(
                GenerationManager.GetMysteryRoomItems(BaseGameManager.Instance.CurrentLevel + 1).ToArray());
        }
    }
}
