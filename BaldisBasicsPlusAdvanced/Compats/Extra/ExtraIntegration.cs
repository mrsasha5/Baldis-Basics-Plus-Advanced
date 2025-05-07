using BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings;
using BBE;
using BBE.API;
using BBE.CustomClasses;

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
            new FunSettingBuilder(AdvancedCore.Instance.Info)
                .SetName("Adv_FunSetting_IceBoots")
                .SetDescription("Adv_FunSetting_IceBoots_Desc")
                .SetEnum("IceBoots")
                .SetNotAllowed(FunSettingsType.Hook)
                .SetActionOnEnabling(() =>
                    Singleton<BaseGameManager>.Instance.gameObject.AddComponent<IceBootsFunSetting>()
                    .Initialize()
                )
                .SetActionOnDisabling(() =>
                    Singleton<BaseGameManager>.Instance.gameObject.DeleteComponent<IceBootsFunSetting>()
                )
                .Build();

            //Adv_FunSetting_VacuumCleanerInvasion_Desc

            new FunSettingBuilder(AdvancedCore.Instance.Info)
                .SetName("Adv_FunSetting_TeleportationChaos")
                .SetDescription("Adv_FunSetting_TeleportationChaos_Desc")
                .SetEnum("TeleportationChaos")
                .SetActionOnEnabling(delegate()
                {
                    Singleton<BaseGameManager>.Instance.gameObject.AddComponent<TeleportationChaosFunSetting>();
                }
                )
                .SetActionOnDisabling(delegate()
                {
                    Singleton<BaseGameManager>.Instance.gameObject.DeleteComponent<TeleportationChaosFunSetting>();
                }
                )
                .Build();

            ModsIntegration.AddPluginAsIntegrated(BasePlugin.Instance);
        }

    }
}
