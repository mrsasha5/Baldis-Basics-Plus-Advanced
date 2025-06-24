using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator
{
    public class SpatialElevatorIntegration : CompatibilityModule
    {

#warning add config value

        public static GameObject monitorPre;

        public SpatialElevatorIntegration() : base()
        {
            guid = "pixelguy.pixelmodding.baldiplus.3delevator";
            versionInfo = new VersionInfo(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
            monitorPre = AssetLoader.ModelFromFile(AssetsHelper.modPath + "Models/TipScreen.obj");
            monitorPre.ConvertToPrefab(true); //Is it on purpose it does not convert model instance to the prefab???
        }

    }
}
