namespace BaldisBasicsPlusAdvanced.Cache
{
    internal class TagStorage
    {

        #region Development API 

        // NPCs
        public const string TEACHER = "teacher";
        public const string FACULTY = "faculty";
        public const string STUDENT = "student";

        // Items
        public const string FOOD = "food";
        public const string DRINK = "drink";
        public const string SHOP_ITEM = "shop_dummy";

        #endregion

        #region Internal

        public const string FIRST_PRIZE_IMMUNITY = "adv_first_prize_immunity";

        // Expel Hammer only
        public const string EXPEL_HAMMER_IMMUNITY = "adv_exclusion_hammer_immunity";
        public const string EXPEL_HAMMER_WEAKNESS = "adv_exclusion_hammer_weakness";

        // Events
        public const string COLD_SCHOOL_IMMUNIY = "adv_ev_cold_school_immunity";
        public const string DISAPPEARING_CHARACTERS_IMMUNITY = "adv_ev_disappearing_characters_immunity";

        // Symbol Machine
        public const string SYMBOL_MACHINE_POTENTIAL_REWARD = "adv_sm_potential_reward";

        // Present plate
        public const string FORBIDDEN_PRESENT = "adv_forbidden_present";

        // Universal tags
        public const string PERFECT_RATE = "adv_perfect";
        public const string GOOD_RATE = "adv_good";
        public const string NORMAL_RATE = "adv_normal";
        public const string COMMON_RATE = "adv_common";
        internal const string NONE_RATE = "adv_none";

        // Not sure yet if these supposed to be used by other mods
        internal const string NARROWLY_FUNCTIONAL = "adv_narrowly_functional";
        internal const string REPAIR_TOOL = "adv_repair_tool";

        #endregion

        #region Criminal Pack

        public const string COMPAT_CRIMINAL_CONTRABAND = "crmp_contraband";

        #endregion

    }
}
