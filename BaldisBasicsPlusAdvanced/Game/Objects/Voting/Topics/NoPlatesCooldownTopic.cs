using BaldisBasicsPlusAdvanced.Game.Builders;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class NoPlatesCooldownTopic : BaseTopic
    {
        public override string Desc => "Adv_Text_School_Council_Topic2".Localize();

        public override BaseTopic Clone()
        {
            NoPlatesCooldownTopic topic = new NoPlatesCooldownTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }

        public override bool IsAvailable()
        {
            return base.IsAvailable() && GameObject.FindObjectOfType<BaseStructure_Plate>() != null
                && GameObject.FindObjectOfType<BaseCooldownPlate>() != null;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            if (isWin) OverrideCooldownPlates();
        }

        private void OverrideCooldownPlates()
        {
            foreach (BaseCooldownPlate plate in GameObject.FindObjectsOfType<BaseCooldownPlate>())
            {
                plate.SetIgnoreCooldown(true);
                plate.OnCooldownEnded();
            }
        }

    }
}
