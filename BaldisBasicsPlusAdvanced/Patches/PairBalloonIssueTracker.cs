#warning DELETE THIS
#if BETA
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using HarmonyLib;
using System.Diagnostics;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Patches
{
    [HarmonyPatch(typeof(Entity))]
    internal class PairBalloonIssueTracker
    {
        [HarmonyPatch("Override")]
        [HarmonyPostfix]
        private static void OnOverride(Entity __instance, ref bool __result)
        {
            if (__result && __instance.name == "PairBalloon_0(Clone)")
            {
                MethodBase method = new StackTrace().GetFrame(2).GetMethod();
                string name = $"{method.DeclaringType.FullName}::{method.Name}";
                AdvancedCore.Logging.LogWarning($"{name} is added an overrider for entity \"{__instance.name}\"!");
                NotificationManager.Instance.Queue("Pair balloon issue is catched! Send logs on Advanced Edition server.", 
                    AssetStorage.sounds["buzz_elv"], 10f, true);
            }
        }

        [HarmonyPatch("Release")]
        [HarmonyPostfix]
        private static void OnRelease(Entity __instance)
        {
            if (__instance.name == "PairBalloon_0(Clone)")
            {
                MethodBase method = new StackTrace().GetFrame(2).GetMethod();
                string name = $"{method.DeclaringType.FullName}::{method.Name}";
                AdvancedCore.Logging.LogWarning($"{name} is released entity \"{__instance.name}\"!");
                NotificationManager.Instance.Queue("Pair balloon issue is catched! Send logs on Advanced Edition server.",
                    AssetStorage.sounds["buzz_elv"], 10f, true);
            }
        }

        [HarmonyPatch("SetFrozen")]
        [HarmonyPostfix]
        private static void OnSetFrozen(Entity __instance, bool value)
        {
            if (__instance.name == "PairBalloon_0(Clone)" && !value)
            {
                MethodBase method = new StackTrace().GetFrame(2).GetMethod();
                string name = $"{method.DeclaringType.FullName}::{method.Name}";
                AdvancedCore.Logging.LogWarning($"{name} is set frozen state {value} for entity \"{__instance.name}\"!");
                NotificationManager.Instance.Queue("Pair balloon issue is catched! Send logs on Advanced Edition server.",
                    AssetStorage.sounds["buzz_elv"], 10f, true);
            }
        }
    }
}
#endif