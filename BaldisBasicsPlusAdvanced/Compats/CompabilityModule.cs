using BaldisBasicsPlusAdvanced.Helpers;
using BepInEx;
using BepInEx.Bootstrap;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Compats
{
    public class CompabilityModule
    {
        protected bool isForced;

        protected BaseUnityPlugin plugin;

        protected string guid = "OHNO!";

        protected VersionInfo versionInfo;

        protected int priority;

        public int Priority => priority;

        public bool IsForced => isForced;

        public string Guid => guid;

        protected CompabilityModule()
        {

        }

        public virtual bool IsIntegrable()
        {
            return AssetsHelper.ModInstalled(guid) && versionInfo.IsPluginCorrect();
        }

        private void InitializePre()
        {
            if (!Chainloader.PluginInfos.ContainsKey(guid)) return;
            plugin = Chainloader.PluginInfos[guid].Instance;
        }

        protected virtual void Initialize()
        {

        }

        public class VersionInfo
        {
            private string minVersion;

            private List<string> versionsIgnored = new List<string>();

            private List<string> bannedVersions = new List<string>();

            private CompabilityModule module;

            public VersionInfo(CompabilityModule module)
            {
                this.module = module;
            }

            public bool IsPluginCorrect()
            {
                if (module.plugin == null) return false;

                if (minVersion != null)
                {
                    if (!versionsIgnored.Contains(module.plugin.Info.Metadata.Version.ToString()) 
                        && module.plugin.Info.Metadata.Version < new System.Version(minVersion))
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

            public VersionInfo SetMinVersion(string version)
            {
                if (minVersion == null) minVersion = version;
                return this;
            }

        }

    }
}
