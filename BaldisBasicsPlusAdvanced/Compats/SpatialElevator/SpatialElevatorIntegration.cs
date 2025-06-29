using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using The3DElevator.Extensions;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator
{
    public class SpatialElevatorIntegration : CompatibilityModule
    {

        public static GameObject monitorPre;

        public override bool IsIntegrable()
        {
            return base.IsIntegrable() && AdvancedCore.spatialElevatorIntegrationEnabled;
        }

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
