using QualityOfPlus.BetterElevator;

namespace BaldisBasicsPlusAdvanced.Compats.QualityOfPlus
{
    internal class QualityOfPlusIntegration : CompatibilityModule
    {
        public QualityOfPlusIntegration() : base()
        {
            guid = IntegrationManager.QOP_ID;
            versionInfo = new VersionInfo(this)
                .SetMinVersion("1.8.1", exceptCurrent: true);
            requiresCorrectVersion = true;

            CreateConfigValue("Quality of Plus",
                "Enables specific patches for QOP mod. Highly is not recommended to turn off.");
        }

        public static void LockStart(bool value)
        {
            if (value)
                BackButtonsInElevator.LockStart();
            else BackButtonsInElevator.UnlockStart();
        }
    }
}