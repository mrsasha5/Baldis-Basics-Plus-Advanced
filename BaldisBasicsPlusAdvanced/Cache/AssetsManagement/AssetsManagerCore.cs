using BaldisBasicsPlusAdvanced.Helpers;

namespace BaldisBasicsPlusAdvanced.Cache.AssetsManagement
{
    internal class AssetsManagerCore
    {

        public static void InitializePre()
        {
            AssetsStorage.InitializePre();
        }

        public static void Initialize()
        {
            LayersHelper.Initialize();
            AssetsStorage.Initialize();
        }

    }
}
