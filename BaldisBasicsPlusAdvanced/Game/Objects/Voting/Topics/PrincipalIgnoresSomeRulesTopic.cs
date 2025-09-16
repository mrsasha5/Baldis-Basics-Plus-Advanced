using BaldisBasicsPlusAdvanced.Extensions;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{

    public class PrincipalIgnoresSomeRulesTopic : BaseTopic
    {

        public override string Desc => "Adv_SC_Topic_Rules".Localize();

        public override string BasicInfo => "Adv_SC_Topic_Rules_BasicInfo".Localize();

        public override BaseTopic Clone()
        {
            PrincipalIgnoresSomeRulesTopic topic = new PrincipalIgnoresSomeRulesTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }
    }
}
