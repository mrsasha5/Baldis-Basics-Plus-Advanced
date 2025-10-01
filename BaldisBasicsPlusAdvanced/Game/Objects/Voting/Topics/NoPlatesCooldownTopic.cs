using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Builders;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class NoPlatesCooldownTopic : BaseTopic
    {
        public override string Desc => "Adv_SC_Topic_PlatesCooldown".Localize();

        public override string BasicInfo => "Adv_SC_Topic_PlatesCooldown_BasicInfo".Localize();

        public override BaseTopic Clone()
        {
            NoPlatesCooldownTopic topic = new NoPlatesCooldownTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }

        public override bool IsAvailable()
        {
            return base.IsAvailable() && GameObject.FindObjectOfType<BaseStructure_Plate>() != null
                && Array.Find(GameObject.FindObjectsOfType<BasePlate>(), x => x.Data.initiallyHasCooldown) != null;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            if (isWin) OverrideCooldownPlates();
        }

        private void OverrideCooldownPlates()
        {
            foreach (BasePlate plate in Array.FindAll(GameObject.FindObjectsOfType<BasePlate>(), x => x.Data.initiallyHasCooldown))
            {
                plate.SetIgnoreCooldown(true);
                plate.OnCooldownEnded();
            }
        }

    }
}
