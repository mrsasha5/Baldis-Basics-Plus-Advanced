using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using System.Collections.Generic;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class LightsEconomyTopic : BaseTopic
    {
        private static RoomCategory[] categories = new RoomCategory[]
        {
            RoomCategory.Faculty,
            RoomCategory.Office,
            EnumExtensions.GetFromExtendedName<RoomCategory>("SchoolCouncil"),
            RoomCategory.Special
        };

        private static RoomCategory[] forcedCategories = new RoomCategory[]
        {
            RoomCategory.Special
        };

        public override string Desc => "Adv_SC_Topic_Electricity".Localize();

        public override string BasicInfo => "Adv_SC_Topic_Electricity_BasicInfo".Localize();

        public override bool IsAvailable()
        {
            List<RoomCategory> _categories = new List<RoomCategory>();
            for (int i = 0; i < ec.rooms.Count; i++)
            {
                if (categories.Contains(ec.rooms[i].category)
                    && !_categories.Contains(ec.rooms[i].category))
                {
                    _categories.Add(ec.rooms[i].category);
                }
            }
            if (_categories.Count == forcedCategories.Length) return base.IsAvailable();
            return false;
        }

        public override BaseTopic Clone()
        {
            LightsEconomyTopic topic = new LightsEconomyTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            if (isWin)
            {
                foreach (RoomController room in ec.rooms.FindAll(x => categories.Contains(x.category)))
                {
                    room.SetPower(false);
                }
            }
            
        }

    }
}
