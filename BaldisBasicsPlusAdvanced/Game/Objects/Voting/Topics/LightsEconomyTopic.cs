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

        public override string Desc => "Adv_Text_School_Council_Topic1".Localize();

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

        public override void OnVotingEndedPost(bool isWin)
        {
            base.OnVotingEndedPost(isWin);
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
