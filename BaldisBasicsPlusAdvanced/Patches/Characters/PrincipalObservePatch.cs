using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using HarmonyLib;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{
    [HarmonyPatch(typeof(Principal))]
    internal class PrincipalObservePatch
    {
        private static string[] allowedRulesWhenTopicActive = new string[] {
            "Running",
            "Drinking",
            "AfterHours"
        };

        [HarmonyPatch("ObservePlayer")]
        [HarmonyPrefix]
        private static bool OnObservePlayer(PlayerManager player)
        {
            if (!VotingEvent.IsTopicActive<PrincipalIgnoresSomeRulesTopic>()) return true;
            return !allowedRulesWhenTopicActive.Contains(player.ruleBreak);
        }

    }
}
