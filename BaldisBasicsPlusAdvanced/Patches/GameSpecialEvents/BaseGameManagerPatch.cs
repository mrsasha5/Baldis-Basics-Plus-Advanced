using BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using System;

namespace BaldisBasicsPlusAdvanced.Patches.GameSpecialEvents
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class BaseGameManagerPatch
    {

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void OnInit(BaseGameManager __instance)
        {
            ElevatorExpelHammerPatch.OnGameManagerInit(__instance);

            //Part of compatibility
            SpatialChalkboard.OnGameManagerInit(__instance);
        }

        [HarmonyPatch("LoadSceneObject", new Type[] { typeof(SceneObject), typeof(bool) })]
        [HarmonyPostfix]
        private static void OnLoadSceneObject(SceneObject sceneObject, bool restarting)
        {
            LevelDataManager.LevelData.OnLoadSceneObject(sceneObject, restarting);
        }

        [HarmonyPatch("LoadNextLevel")]
        [HarmonyPostfix]
        private static void OnLoadNextLevel(BaseGameManager __instance)
        {
            LevelDataManager.LevelData.OnLoadNextLevel(__instance is PitstopGameManager);
        }

    }
}
