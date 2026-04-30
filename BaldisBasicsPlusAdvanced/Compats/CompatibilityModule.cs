using BaldisBasicsPlusAdvanced.Helpers;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Compats
{
    internal class CompatibilityModule
    {
        protected bool isForced;

        protected BaseUnityPlugin plugin;

        protected string guid = "OHNO!";

        // Crashes the game if forced/non-forced mod version is not compatible
        protected bool requiresCorrectVersion;

        protected VersionInfo versionInfo;

        protected ConfigEntry<bool> configValue;

        protected int priority;

        public int Priority => priority;

        public bool IsForced => isForced;

        public string Guid => guid;

        public VersionInfo VersionInfo => versionInfo;

        public bool RequiresCorrectVersion => requiresCorrectVersion;

        public ConfigEntry<bool> ConfigValue => configValue;

        public BaseUnityPlugin Plugin => plugin;

        protected CompatibilityModule()
        {

        }

        public virtual bool IsIntegrable()
        {
            return AssetHelper.ModInstalled(guid) && versionInfo.IsPluginCorrect(out _, out _) && configValue.Value;
        }

        protected void CreateConfigValue(string name, string desc, bool defValue = true)
        {
            configValue = AdvancedCore.Instance.Config.Bind("Integration", name, defaultValue: defValue, desc);
        }

        protected void PreInitialize()
        {
            if (!Chainloader.PluginInfos.ContainsKey(guid)) return;
            plugin = Chainloader.PluginInfos[guid].Instance;
        }

        protected virtual void OnModLoadingStarted()
        {

        }

        protected virtual void Initialize()
        {

        }

        protected virtual void OnAssetsPostLoad()
        {

        }
    }

    internal class VersionInfo
    {
        private string minVersion;

        private bool exceptMinVersion;

        private List<string> versionsIgnored = new List<string>();

        private List<string> bannedVersions = new List<string>();

        private CompatibilityModule module;

        public VersionInfo(CompatibilityModule module)
        {
            this.module = module;
        }

        public bool IsPluginCorrect(out bool versionChecked, out string error)
        {
            error = "";
            versionChecked = false;
            if (module.Plugin == null) return false;
            versionChecked = true;

            if (minVersion != null)
            {
                if (!versionsIgnored.Contains(module.Plugin.Info.Metadata.Version.ToString()))
                {
                    if (exceptMinVersion && module.Plugin.Info.Metadata.Version <= new System.Version(minVersion))
                    {
                        error = $"{module.Plugin.Info.Metadata.GUID} must have version that is higher than {minVersion}.";
                        return false;
                    }
                    if (!exceptMinVersion && module.Plugin.Info.Metadata.Version < new System.Version(minVersion))
                    {
                        error = $"{module.Plugin.Info.Metadata.GUID} must have version that is higher than {minVersion} or equals this value.";
                        return false;
                    } 
                }
            }

            for (int i = 0; bannedVersions.Count < i; i++)
            {
                if (new System.Version(bannedVersions[i]) == module.Plugin.Info.Metadata.Version)
                {
                    error = $"{module.Plugin.Info.Metadata.GUID} is installed with forbidden version {bannedVersions[i]}.";
                    return false;
                }
            }
            return true;
        }

        public VersionInfo AddAllowedVersion(string version)
        {
            versionsIgnored.Add(version);
            return this;
        }

        public VersionInfo BanVersion(string version)
        {
            bannedVersions.Add(version);
            return this;
        }

        public VersionInfo SetMinVersion(string version, bool exceptCurrent)
        {
            if (minVersion == null) minVersion = version;
            exceptMinVersion = exceptCurrent;
            return this;
        }
    }
}
