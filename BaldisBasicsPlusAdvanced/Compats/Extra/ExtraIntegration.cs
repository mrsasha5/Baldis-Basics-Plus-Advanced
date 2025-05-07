using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using BBE;
using BBE.API;
using BBE.CustomClasses;
using BBE.Extensions;

namespace BaldisBasicsPlusAdvanced.Compats.Extra
{
    public class ExtraIntegration
    {

        public static bool IsIntegrable()
        {
            return ModsIntegration.ExtraInstalled && AdvancedCore.extraIntegrationEnabled;
        }

        public static void Initialize()
        {
            new FunSettingBuilder(AdvancedCore.Instance.Info, "adv_cursed_ice_boots")
                .SetName("Adv_FunSetting_IceBoots")
                .SetDescription("Adv_FunSetting_IceBoots_Desc")
                .SetEnum("IceBoots")
                .SetNotAllowed(FunSettingsType.Hook)
                .Build<IceBootsFunSetting>();

            //Adv_FunSetting_VacuumCleanerInvasion_Desc

            new FunSettingBuilder(AdvancedCore.Instance.Info, "adv_teleportation_chaos")
                .SetName("Adv_FunSetting_TeleportationChaos")
                .SetDescription("Adv_FunSetting_TeleportationChaos_Desc")
                .SetEnum("TeleportationChaos")
                .Build<TeleportationChaosFunSetting>();

            ModsIntegration.AddPluginAsIntegrated(BasePlugin.Instance);
        }

    }
}
