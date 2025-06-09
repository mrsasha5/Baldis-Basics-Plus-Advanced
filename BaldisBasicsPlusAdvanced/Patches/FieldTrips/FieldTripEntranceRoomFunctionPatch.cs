using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.FieldTrips
{
    [HarmonyPatch(typeof(FieldTripEntranceRoomFunction))]
    internal class FieldTripEntranceRoomFunctionPatch
    {
        private static FieldTripEntranceRoomFunction instance;

        private static FieldTripData data;

        public static FieldTripEntranceRoomFunction Instance => instance;

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInitialize(FieldTripEntranceRoomFunction __instance)
        {
            instance = __instance;
            SpecialTripsRegistryManager.InitializeRng();
            data = SpecialTripsRegistryManager.GetRandomTripData();
            SpecialTripsRegistryManager.ResetRng();
        }

        [HarmonyPatch("StartFieldTrip")]
        [HarmonyPrefix]
        private static bool OnStartFieldTrip(PlayerManager player, ref bool ___unlocked)
        {
            if (player.itm.Has(ObjectsStorage.ItemObjects["MysteriousBusPass"].itemType) || ___unlocked)
            {
                if (!___unlocked)
                {
                    player.itm.Remove(ObjectsStorage.ItemObjects["MysteriousBusPass"].itemType);
                    ___unlocked = true;
                }

                FieldTripsLoader.LoadFieldTrip(data);
                return false;
            }
            return true;
        }

    }
}
