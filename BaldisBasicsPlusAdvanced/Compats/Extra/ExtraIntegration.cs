using BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings;
using BBE.API;
using BBE.Extensions;

namespace BaldisBasicsPlusAdvanced.Compats.Extra
{
    public class ExtraIntegration : CompabilityModule
    {

        protected ExtraIntegration() : base()
        {
            guid = "rost.moment.baldiplus.extramod";
            versionInfo = new VersionInfo(this)
                .SetMinVersion("2.2");
        }

        public override bool IsIntegrable()
        {
            return base.IsIntegrable() && AdvancedCore.extraIntegrationEnabled;
        }

        protected override void Initialize()
        {
            base.Initialize();
            new FunSettingBuilder(AdvancedCore.Instance.Info, "adv_cursed_ice_boots")
                .SetName("Adv_FunSetting_IceBoots")
                .SetDescription("Adv_FunSetting_IceBoots_Desc")
                .SetEnum("IceBoots")
                .SetNotAllowed(FunSettingsType.Hook)
                .Build<IceBootsFunSetting>();

            new FunSettingBuilder(AdvancedCore.Instance.Info, "adv_teleportation_chaos")
                .SetName("Adv_FunSetting_TeleportationChaos")
                .SetDescription("Adv_FunSetting_TeleportationChaos_Desc")
                .SetEnum("TeleportationChaos")
                .Build<TeleportationChaosFunSetting>();
        }

    }
}
