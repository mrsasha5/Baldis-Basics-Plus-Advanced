using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class TurnOffFacultyNoisyPlatesTopic : BaseTopic
    {
        public override string Desc => "Adv_SC_Topic_NoisyPlates".Localize();

        public override string BasicInfo => "Adv_SC_Topic_NoisyPlates_BasicInfo".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && 
                Array.Find(GameObject.FindObjectsOfType<NoisyPlate>(), x => x.CallsPrincipal) != null; //...
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            if (isWin)
            {
                foreach (NoisyPlate plate in GameObject.FindObjectsOfType<NoisyPlate>())
                {
                    if (plate.CallsPrincipal) plate.SetTurnOff(true);
                }
            }
        }

        public override BaseTopic Clone()
        {
            TurnOffFacultyNoisyPlatesTopic topic = new TurnOffFacultyNoisyPlatesTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }
    }
}
