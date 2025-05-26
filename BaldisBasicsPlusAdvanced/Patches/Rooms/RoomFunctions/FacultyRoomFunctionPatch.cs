/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using HarmonyLib;
using System;

namespace BaldisBasicsPlusAdvanced.Patches.Rooms.RoomFunctions
{
    [HarmonyPatch(typeof(RoomFunction))]
    internal class FacultyRoomFunctionPatch
    {

        private static System.Random rng;

        private static LevelBuilder builder;

        private static int buildingPermits;

        //[HarmonyPatch("Build")]
        //[HarmonyPrefix]
        //private static void OnBuild(RoomFunction __instance, LevelBuilder builder, System.Random rng)
        //{
        //    if (__instance is FacultyTrigger && rng.Next(0, 100) >= 0)
        //    {
        //        Debug.Log("Doors count: " + __instance.Room.doors.Count);
        //        foreach (Door door in __instance.Room.doors)
        //        {
        //            builder.InstatiateEnvironmentObject(ObjectsStorage.GameButtons["noisy_plate"].gameObject,
        //                door.aTile, Direction.North);//cell.doorDirs[0]);
        //        }
        //    }
        //}

        [HarmonyPatch("Build")]
        [HarmonyPrefix]
        private static void OnBuild(RoomFunction __instance, LevelBuilder builder, System.Random rng)
        {
            FacultyRoomFunctionPatch.builder = builder;
            FacultyRoomFunctionPatch.rng = rng;
            if (builder != null && builder is LevelGenerator) buildingPermits++;
        }

        [HarmonyPatch("OnGenerationFinished")]
        [HarmonyPrefix]
        private static void OnGenerationFinished(RoomFunction __instance)
        {
            if (buildingPermits > 0
                && __instance is FacultyTrigger
                && rng.Next(0, 101) >= 90
                )
            {
                buildingPermits--;
                try
                {
                    foreach (Door door in __instance.Room.doors)
                    {
                        NoisyPlate plate = builder.InstatiateEnvironmentObject(ObjectsStorage.GameButtons["noisy_plate"].gameObject,
                            door.aTile, door.direction.GetOpposite())
                            .GetComponent<NoisyPlate>();
                        plate.SetIgnoreCooldown(true);
                        plate.SetNoReward(true);
                    }
                } catch (Exception e)
                {
                    buildingPermits = 0;
                    throw e;
                }
                
            }
        }

    }
}
*/