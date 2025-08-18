namespace BaldisBasicsPlusAdvanced.Cache
{
    internal class TagsStorage
    {

        #region Development API 

        //NPCs
        public const string teacher = "teacher";
        public const string faculty = "faculty";
        public const string student = "student";

        //Items
        public const string food = "food";
        public const string drink = "drink";
        public const string shopItem = "shop_dummy";

        #endregion

        #region Internal

        public const string firstPrizeImmunity = "adv_first_prize_immunity";

        //Expel Hammer only
        public const string expelHammerImmunity = "adv_exclusion_hammer_immunity";
        public const string expelHammerWeakness = "adv_exclusion_hammer_weakness";

        //Events
        public const string coldSchoolEventImmunity = "adv_ev_cold_school_immunity";
        public const string disappearingCharactersEventImmunity = "adv_ev_disappearing_characters_immunity";

        //Symbol Machine
        public const string symbolMachinePotentialReward = "adv_sm_potential_reward";

        //Present plate
        public const string forbiddenPresent = "adv_forbidden_present";

        //Universal tags
        public const string perfectRate = "adv_perfect";
        public const string goodRate = "adv_good";
        public const string normalRate = "adv_normal";
        public const string commonRate = "adv_common";
        internal const string noneRate = "adv_none";

        //Not sure yet if these supposed to be used by other mods
        internal const string narrowlyFunctional = "adv_narrowly_functional";
        internal const string repairTool = "adv_repair_tool";

        #endregion

        #region Criminal Pack

        public const string criminal_contraband = "crmp_contraband";

        #endregion

    }
}
