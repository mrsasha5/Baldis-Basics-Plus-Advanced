using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;

namespace BaldisBasicsPlusAdvanced.Extensions
{
    public static class EnumExtensionMethods
    {
        public static string ConvertToTag(this RewardType rewardType)
        {
            switch (rewardType)
            {
                case RewardType.PerfectItem:
                    return TagStorage.PERFECT_RATE;
                case RewardType.GoodItem:
                    return TagStorage.GOOD_RATE;
                case RewardType.NormalItem:
                    return TagStorage.NORMAL_RATE;
                case RewardType.CommonItem:
                    return TagStorage.COMMON_RATE;
                default:
                    return TagStorage.NONE_RATE;
            }
        }
    }
}
