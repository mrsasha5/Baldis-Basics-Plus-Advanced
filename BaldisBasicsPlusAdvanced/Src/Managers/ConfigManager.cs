using BepInEx.Configuration;

namespace BaldisBasicsPlusAdvanced.Managers
{
    internal class ConfigManager
    {

        public static ConfigEntry<bool> notificationsEnabled;

        public static void Initialize()
        {
            notificationsEnabled = AdvancedCore.Instance.Config
                .Bind("Settings", "Notifications", defaultValue: true,
                    "Disables/enables notifications.");
        }

    }
}
