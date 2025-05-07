using BaldisBasicsPlusAdvanced.Attributes;
using BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings;
using BBE;
using BBE.CustomClasses;
using HarmonyLib;
using MTM101BaldAPI;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Patches.Game.FunSettings
{
    [HarmonyPatch(typeof(EnvironmentController))]
    [ConditionalPatchIntegratedMod(ModsIntegration.extraId, ModsIntegration.extraVersion)]
    internal class FunSettingsInitiator
    {

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        private static void OnBeginPlay()
        {
            PrepareFunSetting<IceBootsFunSetting>("IceBoots");
            PrepareFunSetting<TeleportationChaosFunSetting>("TeleportationChaos");
        }

        private static void PrepareFunSetting<T>(string @enum) where T : BaseFunSetting
        {
            if (EnumExtensions.GetFromExtendedName<FunSettingsType>(@enum).IsActive())
            {
                Singleton<BaseGameManager>.Instance.gameObject.AddComponent<T>()
                    .Initialize();
            }
        }

    }
}
