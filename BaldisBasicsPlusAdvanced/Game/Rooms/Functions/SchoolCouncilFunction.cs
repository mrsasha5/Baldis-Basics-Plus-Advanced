using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.Objects;

namespace BaldisBasicsPlusAdvanced.Game.Rooms.Functions
{
    public class SchoolCouncilFunction : RoomFunction
    {

        private VotingEvent voting;

        public void Assign(VotingEvent votingEvent)
        {
            voting = votingEvent;
        }

        public override void OnGenerationFinished()
        {
            base.OnGenerationFinished();
            if (voting != null && voting.Screens.Count == 0)
            {
                foreach (VotingCeilingScreen screen in FindObjectsOfType<VotingCeilingScreen>())
                {
                    voting.Screens.Add(screen);
                }

                voting.UpdateTexts();
            }
        }

    }
}
