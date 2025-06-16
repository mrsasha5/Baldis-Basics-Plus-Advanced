using BaldisBasicsPlusAdvanced.Patches;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class OpenVentsTopic : BaseTopic
    {

        public override string Desc => "Adv_Text_School_Council_Topic9".Localize();

        public override BaseTopic Clone()
        {
            OpenVentsTopic topic = new OpenVentsTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
        }
    }
}
