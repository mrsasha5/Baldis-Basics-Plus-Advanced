using BaldisBasicsPlusAdvanced.Helpers;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Compats
{
    public class CompatibilityModule
    {
        protected bool isForced;

        protected BaseUnityPlugin plugin;

        protected string guid = "OHNO!";

        protected VersionInfo versionInfo;

        protected ConfigEntry<bool> configValue;

        protected int priority;

        public int Priority => priority;

        public bool IsForced => isForced;

        public string Guid => guid;

        protected CompatibilityModule()
        {

        }

        public virtual bool IsIntegrable()
        {
            return AssetsHelper.ModInstalled(guid) && versionInfo.IsPluginCorrect() && configValue.Value;
        }

        protected void CreateConfigValue(string name, string desc, bool defValue = true)
        {
            configValue = AdvancedCore.Instance.Config.Bind("Integration", name, defaultValue: defValue, desc);
        }

        protected virtual void InitializePre()
        {
            if (!Chainloader.PluginInfos.ContainsKey(guid)) return;
            plugin = Chainloader.PluginInfos[guid].Instance;
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void InitializeOnAssetsLoadPost()
        {

        }

        public class VersionInfo
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

            public bool IsPluginCorrect()
            {
                if (module.plugin == null) return false;

                if (minVersion != null)
                {
                    if (!versionsIgnored.Contains(module.plugin.Info.Metadata.Version.ToString()) && 
                        ((exceptMinVersion && module.plugin.Info.Metadata.Version <= new System.Version(minVersion)) ||
                        (!exceptMinVersion && module.plugin.Info.Metadata.Version < new System.Version(minVersion))))
                    {
                        return false;
                    }
                }

                for (int i = 0; bannedVersions.Count < i; i++)
                {
                    if (new System.Version(bannedVersions[i]) == module.plugin.Info.Metadata.Version)
                    {
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
}
