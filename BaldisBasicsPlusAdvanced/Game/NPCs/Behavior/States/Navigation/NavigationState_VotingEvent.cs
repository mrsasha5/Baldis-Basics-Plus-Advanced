using BaldisBasicsPlusAdvanced.Game.Objects.Voting;

namespace BaldisBasicsPlusAdvanced.Game.NPCs.Behavior.States.Navigation
{
    public class NavigationState_VotingEvent : NavigationState
    {
        private RoomController votingRoom;

        private VotingBallot ballot;

        private bool ended;

        public bool Entered { get; private set; }

        public NavigationState_VotingEvent(NPC npc, int priority, RoomController votingRoom)
            : base(npc, priority)
        {
            this.votingRoom = votingRoom;
            ballot = votingRoom.GetComponentInChildren<VotingBallot>();
        }

        public override void Enter()
        {
            base.Enter();
            npc.Navigator.FindPath(votingRoom.ec.CellFromPosition(ballot.transform.position).CenterWorldPosition);
            Entered = true;
        }

        public override void DestinationEmpty()
        {
            npc.Navigator.FindPath(votingRoom.ec.CellFromPosition(ballot.transform.position).CenterWorldPosition);
        }

        public void End()
        {
            if (ended) return;
            ended = true;
            priority = 0;
            npc.behaviorStateMachine.RestoreNavigationState();
        }

    }
}
