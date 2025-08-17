using System.Collections.Generic;
using FundakaExtender.FundakaStructure;
using MonoMod.Utils;
using MTM101BaldAPI;

namespace BaldisBasicsPlusAdvanced.Compats.FundakaPack
{
    internal class FundakaIntegration : CompatibilityModule
    {

        public FundakaIntegration()
        {
            guid = "whiydev.plusmod.fundakaextender";
            versionInfo = new VersionInfo(this);

            CreateConfigValue("Fundaka",
                "Adds integration for that mod! Like text for the Event Poster...");
        }

        protected override void Initialize()
        {
            base.Initialize();
            Structure_EventPoster.eventTypeKey
                .AddRange(new Dictionary<RandomEventType, string>()
                {
                    { EnumExtensions.GetFromExtendedName<RandomEventType>("ColdSchool"), "New Cold Technology" },
                    { EnumExtensions.GetFromExtendedName<RandomEventType>("DisappearingCharacters"), "Invisible Characters" },
                    { EnumExtensions.GetFromExtendedName<RandomEventType>("PortalChaos"), "Mysterious Portals" },
                    { EnumExtensions.GetFromExtendedName<RandomEventType>("Voting"), "Democracy Time!" }
                });
        }

    }
}
