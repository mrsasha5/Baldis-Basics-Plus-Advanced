using BaldisBasicsPlusAdvanced.Patches;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{

    public class PrincipalIgnoresSomeRulesTopic : BaseTopic
    {

        public override string Desc => "Adv_Text_School_Council_Topic3".Localize();

        public override BaseTopic Clone()
        {
            PrincipalIgnoresSomeRulesTopic topic = new PrincipalIgnoresSomeRulesTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }
    }
}
