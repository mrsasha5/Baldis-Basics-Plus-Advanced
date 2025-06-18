using BaldisBasicsPlusAdvanced.Cache;
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
                    return TagsStorage.perfectRate;
                case RewardType.GoodItem:
                    return TagsStorage.goodRate;
                case RewardType.NormalItem:
                    return TagsStorage.normalRate;
                case RewardType.CommonItem:
                    return TagsStorage.commonRate;
                default:
                    return TagsStorage.noneRate;
            }
        }
    }
}
