using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using HarmonyLib;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Patches.Characters
{
    [HarmonyPatch(typeof(Principal))]
    internal class PrincipalPatch
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
            if (!VotingEvent.TopicIsActive<PrincipalIgnoresSomeRulesTopic>()) return true;
            return !allowedRulesWhenTopicActive.Contains(player.ruleBreak);
        }

    }
}
