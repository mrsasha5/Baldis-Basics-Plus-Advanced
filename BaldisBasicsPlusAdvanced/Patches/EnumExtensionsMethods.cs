using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches
{
    public static class EnumExtensionsMethods
    {
        public static string ConvertToTag(this RewardType rewardType)
        {
            switch (rewardType)
            {
                case RewardType.PerfectItem:
                    return "adv_perfect";
                case RewardType.GoodItem:
                    return "adv_good";
                case RewardType.NormalItem:
                    return "adv_normal";
                case RewardType.CommonItem:
                    return "adv_common";
                default:
                    return "adv_none";
            }
        }
    }
}
