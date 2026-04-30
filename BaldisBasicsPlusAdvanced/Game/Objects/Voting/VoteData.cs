using System;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting
{
    [Obsolete("Voting event is removed.")]
    public class VoteData
    {
        public NPC npc;

        public PlayerManager pm;

        public bool isNPC;

        public bool isPlayer;

        public bool value;

    }
}
