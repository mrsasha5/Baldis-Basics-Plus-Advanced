using System.Runtime.CompilerServices;
using BaldisBasicsPlusAdvanced.Helpers;

namespace BaldisBasicsPlusAdvanced.Cache.AssetsManagment
{
    public class AssetsManagerCore
    {

        public static void Initialize()
        {
            //Load layers
            RuntimeHelpers.RunClassConstructor(typeof(LayersHelper).TypeHandle);
            //Initialize assets
            RuntimeHelpers.RunClassConstructor(typeof(AssetsStorage).TypeHandle);

        }

    }
}
