using System;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;

namespace BaldisBasicsPlusAdvanced.API
{
    /// <summary>
    /// Helps you in managing PIT Stop's stuff
    /// </summary>
    public class PitOverrides
    {

        private static bool kitchenStoveDisabled;

        private static bool englishClassDisabled;

        private static bool accelerationPlateDisabled;

        private static int hammerPickupDisables;

        /// <summary>
        /// If you turn it off, you won't be able to turn it on again.
        /// If enabled, the pit level will be overridden.
        /// </summary>
        public static bool KitchenStoveDisabled => kitchenStoveDisabled;

        /// <summary>
        /// If you turn it off, you won't be able to turn it on again.
        /// If enabled, the pit level will be overridden.
        /// </summary>
        public static bool EnglishClassDisabled => englishClassDisabled;

        /// <summary>
        /// If you turn it off, you won't be able to turn it on again.
        /// If enabled, the pit level will be overridden.
        /// </summary>
        public static bool AccelerationPlateDisabled => accelerationPlateDisabled;

        /// <summary>
        /// This can be easily turned on and off any time, but be aware of other mods!
        /// </summary>
        public static bool ExpelHammerPickupDisabled => hammerPickupDisables > 0;

        [Obsolete("No longer exists since it's 0.11 addition!")]
        public static bool RefreshPickupDisabled => true;

        /// <summary>
        /// It must be called only before mod assets will be loaded.
        /// </summary>
        public static void DisableAllContent()
        {
            if (AssetsStorage.Overridden)
            {
                ApiManager.logger.LogWarning("DisableAllOverrides() doesn't work on assets post load! Assets are overridden already.");
                return;
            }

            DisableAccelerationPlate();
            DisableKitchenStove();
            DisableEnglishClass();
            SetExpelHammerPickup(active: false);
        }

        /// <summary>
        /// It must be called only before mod assets will be loaded.
        /// </summary>
        public static void DisableAccelerationPlate()
        {
            if (AssetsStorage.Overridden && !kitchenStoveDisabled)
            {
                ApiManager.logger.LogWarning("Acceleration Plate cannot be disabled! Assets are overridden already.");
                return;
            }
            accelerationPlateDisabled = true;
        }

        /// <summary>
        /// It must be called only before mod assets will be loaded.
        /// </summary>
        public static void DisableKitchenStove()
        {
            if (AssetsStorage.Overridden && !kitchenStoveDisabled)
            {
                ApiManager.logger.LogWarning("Kitchen Stove cannot be disabled! Assets are overridden already.");
                return;
            }
            kitchenStoveDisabled = true;
        }

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


        [Obsolete("No longer exists since it's 0.11 addition!")]
        public static void SetRefreshPickup(bool active) { }


    }
}
