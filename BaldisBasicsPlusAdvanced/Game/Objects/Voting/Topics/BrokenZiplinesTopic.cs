using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class BrokenZiplinesTopic : BaseTopic
    {
        public override string Desc => "Adv_Text_School_Council_Topic8".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && GameObject.FindObjectOfType<ZiplineHanger>() != null;
        }

        public override BaseTopic Clone()
        {
            BrokenZiplinesTopic topic = new BrokenZiplinesTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }

        public override void OnBringUp(System.Random rng)
        {
            base.OnBringUp(rng);
            foreach (ZiplineHanger hanger in GameObject.FindObjectsOfType<ZiplineHanger>())
            {
                hanger.Break(false);
            }
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            if (isWin)
            {
                foreach (ZiplineHanger hanger in GameObject.FindObjectsOfType<ZiplineHanger>())
                {
                    hanger.Restore();
                }
            }
        }

    }
}
