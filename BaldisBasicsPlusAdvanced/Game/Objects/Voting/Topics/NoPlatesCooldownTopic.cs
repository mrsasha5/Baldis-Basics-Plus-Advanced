using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class NoPlatesCooldownTopic : TopicBase
    {
        private bool cooldownOverriden;

        public override string Desc => "Adv_Text_School_Council_Topic2".Localize();

        public override bool SaveToNextFloors => true;

        public override void OnEnvBeginPlay(EnvironmentController ec)
        {
            base.OnEnvBeginPlay(ec);
            cooldownOverriden = false;
            OverrideCooldownPlates();
        }

        public override void OnVotingEndedSuccessfully()
        {
            base.OnVotingEndedSuccessfully();
            OverrideCooldownPlates();
        }

        private void OverrideCooldownPlates()
        {
            if (cooldownOverriden) return;
            cooldownOverriden = true;
            foreach (CooldownPlateBase plate in GameObject.FindObjectsOfType<CooldownPlateBase>())
            {
                plate.SetIgnoreCooldown(true);
            }
        }

    }
}
