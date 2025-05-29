using BaldisBasicsPlusAdvanced.Game.Events;
using System;

namespace BaldisBasicsPlusAdvanced.Game.Rooms.Functions
{
    public class SchoolCouncilFunction : RoomFunction
    {
        private VotingEvent eventInstance;

        private LevelBuilder levelBuilder;

        public LevelBuilder LevelBuilder => levelBuilder;

        public void Assign(VotingEvent votingEvent)
        {
            this.eventInstance = votingEvent;
        }

        public override void Build(LevelBuilder builder, Random rng)
        {
            base.Build(builder, rng);
            this.levelBuilder = builder;
        }


    }
}
