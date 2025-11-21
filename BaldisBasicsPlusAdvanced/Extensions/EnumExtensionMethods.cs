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
                    return TagStorage.perfectRate;
                case RewardType.GoodItem:
                    return TagStorage.goodRate;
                case RewardType.NormalItem:
                    return TagStorage.normalRate;
                case RewardType.CommonItem:
                    return TagStorage.commonRate;
                default:
                    return TagStorage.noneRate;
            }
        }
    }
}
