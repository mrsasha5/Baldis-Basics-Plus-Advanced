using BaldisBasicsPlusAdvanced.Helpers;

namespace BaldisBasicsPlusAdvanced.Cache.AssetsManagement
{
    internal class AssetsManagerCore
    {

        public static void PreInitialize()
        {
            AssetsStorage.InitializeCriticalResources();
        }

        public static void Initialize()
        {
            LayersHelper.Initialize();
            AssetsStorage.Initialize();
        }

    }
}
