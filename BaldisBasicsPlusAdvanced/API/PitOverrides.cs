using BaldisBasicsPlusAdvanced.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.API
{
    /// <summary>
    /// It's second API class that provides methods for you.
    /// And also this is a container of fields which tells you what is available and what is not.
    /// </summary>
    public class PitOverrides
    {

        private static bool englishClassDisabled;

        private static int hammerPickupDisables;

        private static int refreshPickupDisables;

        /// <summary>
        /// If you turn it off, you won't be able to turn it on again.
        /// If enabled, the pit level will be overridden.
        /// </summary>
        public static bool EnglishClassDisabled => englishClassDisabled;

        /// <summary>
        /// This can be easily turned on and off any time, but be aware of other mods!
        /// </summary>
        public static bool ExpelHammerPickupDisabled => hammerPickupDisables > 0;

        /// <summary>
        /// This can be easily turned on and off any time, but be aware of other mods!
        /// </summary>
        public static bool RefreshPickupDisabled => refreshPickupDisables > 0;

        /// <summary>
        /// It must be called only before mod assets will be loaded.
        /// </summary>
        public static void DisableEnglishClass()
        {
            if (AssetsStorage.Overridden && !englishClassDisabled)
            {
                ApiManager.logger.LogWarning("English class cannot be disabled! Assets are overridden already.");
                return;
            }
            englishClassDisabled = true;
        }

        public static void SetExpelHammerPickup(bool active)
        {
            if (active)
                hammerPickupDisables--;
            else
                hammerPickupDisables++;

            if (hammerPickupDisables < 0) hammerPickupDisables = 0;
        }

        public static void SetRefreshPickup(bool active)
        {
            if (active)
                refreshPickupDisables--;
            else
                refreshPickupDisables++;

            if (refreshPickupDisables < 0) refreshPickupDisables = 0;
        }


    }
}
