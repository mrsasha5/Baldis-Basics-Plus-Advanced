using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.Objects;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class DisabledConveyorsTopic : BaseTopic
    {

        public override string Desc => "Adv_Text_School_Council_Topic7".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && GameObject.FindObjectOfType<LevelBuilder>().ld.type == LevelType.Factory;
        }

        public override BaseTopic Clone()
        {
            DisabledConveyorsTopic topic = new DisabledConveyorsTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            if (isWin)
            {
                foreach (BeltRoomFunction func in GameObject.FindObjectsOfType<BeltRoomFunction>())
                {
                    foreach (BeltManager belt in func.builtConveyorBelts)
                    {
                        belt.SetRunning(false);
                        BeltManagerRunningPatch.belts.Add(belt);
                    }
                }

                foreach (Structure_ConveyorBelt beltBuilder in GameObject.FindObjectsOfType<Structure_ConveyorBelt>())
                {
                    foreach (BeltManager belt in beltBuilder.builtBelts)
                    {
                        belt.SetRunning(false);
                        BeltManagerRunningPatch.belts.Add(belt);
                    }
                    
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            BeltManagerRunningPatch.belts.Clear();
        }

    }
}
