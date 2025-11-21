using BaldisBasicsPlusAdvanced.Helpers;

namespace BaldisBasicsPlusAdvanced.Cache
{
    internal class AssetManagerCore
    {

        public static void PreInitialize()
        {
            AssetStorage.InitializeCriticalResources();
        }

        public static void Initialize()
        {
            LayerHelper.Initialize();
            AssetStorage.Initialize();
        }

    }
}
