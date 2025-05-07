using BaldisBasicsPlusAdvanced.Compats;
using BepInEx.Bootstrap;
using MTM101BaldAPI;
using System;

namespace BaldisBasicsPlusAdvanced.Attributes
{
    public class ConditionalPatchIntegratedMod : ConditionalPatchMod
    {

        private string GUID;

        private string version;

        public ConditionalPatchIntegratedMod(string mod) : base(mod)
        {
            this.GUID = mod;
            this.version = "any";
        }

        public ConditionalPatchIntegratedMod(string mod, string version) : base(mod)
        {
            this.GUID = mod;
            this.version = version;
        }

        public override bool ShouldPatch()
        {
            return Chainloader.PluginInfos.ContainsKey(GUID) && ModsIntegration.PluginsToIntegration.Contains(GUID) && (version == "any" || Chainloader.PluginInfos[GUID].Metadata.Version >= new Version(version));
        }
    }
}
