using BaldisBasicsPlusAdvanced.Compats.Extra;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor;
using BaldisBasicsPlusAdvanced.Exceptions;
using BaldisBasicsPlusAdvanced.Helpers;
using BepInEx;
using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Compats
{
    public class ModsIntegration
    {
        public static bool EndlessInstalled => AssetsHelper.ModInstalled(endlessFloorsId);

        public static bool ExtraInstalled => AssetsHelper.ModInstalled(extraId);

        //public static bool CarnellInstalled => AssetsHelper.ModInstalled("alexbw145.baldiplus.bcarnellchars");

        //public static bool CarnivalInstalled => AssetsHelper.ModInstalled("mtm101.rulerp.bbplus.carnivalpackroot");

        public static bool LevelEditorInstalled => AssetsHelper.ModInstalled(levelEditorId);

        public static bool LevelLoaderInstalled => AssetsHelper.ModInstalled(levelLoaderId);

        public const string endlessFloorsId = "mtm101.rulerp.baldiplus.endlessfloors";

        public const string levelEditorId = "mtm101.rulerp.baldiplus.leveleditor";

        public const string levelLoaderId = "mtm101.rulerp.baldiplus.levelloader";

        public const string extraId = "rost.moment.baldiplus.extramod";

        public const string extraVersion = "2.1.9.2";

        private static List<string> pluginsToIntegration = new List<string>();

        private static Dictionary<string, BaseUnityPlugin> integratedPlugins = new Dictionary<string, BaseUnityPlugin>();

        public static Dictionary<string, BaseUnityPlugin> IntegratedPlugins => integratedPlugins;

        public static List<string> PluginsToIntegration => pluginsToIntegration;

        internal static void AddPluginAsIntegrated(BaseUnityPlugin plugin)
        {
            IntegratedPlugins.Add(plugin.Info.Metadata.GUID, plugin);
        }

        public static void CheckPotentialModIntegrations()
        {
            if (LevelEditorIntegration.IsIntegratable())
            {
                PluginsToIntegration.Add(levelEditorId);
            }

            if (ExtraIntegration.IsIntegrable() && Chainloader.PluginInfos[extraId].Metadata.Version >= new Version(extraVersion))
            {
                PluginsToIntegration.Add(extraId);
            }


        }

        public static void MakeIntegration()
        {
            if (LevelEditorIntegration.IsIntegratable())
            {
                LevelEditorIntegration.Initialize();
            }

            if (ExtraIntegration.IsIntegrable())
            {
                if (Chainloader.PluginInfos[extraId].Metadata.Version >= new Version(extraVersion))
                {
                    ExtraIntegration.Initialize();
                } else
                {
                    AdvancedCore.Logging.LogWarning("Extra integration is impossible because you are using outdated version!");
                }
                
            }
        }

        public static void CheckCompabilities()
        {
            if (AssetsHelper.ModInstalled("baldi.basics.plus.advanced.endless.mod"))
            {
                throw new MessageException("Please uninstall baldi.basics.plus.advanced.endless.mod! This is obsolete.");
            }

            if (AssetsHelper.ModInstalled("baldi.basics.plus.advanced.editor.mod"))
            {
                throw new MessageException("Please uninstall baldi.basics.plus.advanced.editor.mod! This is obsolete.");
            }
        }

        

    }
}
