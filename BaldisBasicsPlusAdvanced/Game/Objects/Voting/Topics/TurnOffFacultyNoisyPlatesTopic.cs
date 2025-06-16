using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class TurnOffFacultyNoisyPlatesTopic : BaseTopic
    {
        public override string Desc => "Adv_Text_School_Council_Topic4".Localize();

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
