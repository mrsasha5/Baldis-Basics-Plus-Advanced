using QualityOfPlus;
using QualityOfPlus.BetterElevator;
using QualityOfPlus.BetterElevator.BackButtons;

namespace BaldisBasicsPlusAdvanced.Compats.QualityOfPlus
{
    internal class QualityOfPlusIntegration : CompatibilityModule
    {
        public QualityOfPlusIntegration() : base()
        {
            guid = IntegrationManager.QOP_ID;
            versionInfo = new VersionInfo(this)
                .SetMinVersion("2.0", exceptCurrent: false);
            requiresCorrectVersion = true;

            CreateConfigValue("Quality of Plus",
                "Enables specific patches for QOP mod. Highly is not recommended to turn off.");
        }

        public static void LockStart(bool value)
        {
            if (value)
                QOPManager.Instance.GetFeature<BackElevatorButtonsFeature>().AddForce(true);
            else QOPManager.Instance.GetFeature<BackElevatorButtonsFeature>().RemoveForce(true);
        }
    }
}