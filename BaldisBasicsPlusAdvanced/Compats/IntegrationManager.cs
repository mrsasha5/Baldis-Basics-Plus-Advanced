using BaldisBasicsPlusAdvanced.Exceptions;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Compats
{
#warning TODO: add special screen when any module breaks
    internal class IntegrationManager
    {
        public static bool LevelLoaderInstalled => AssetHelper.ModInstalled(LEVEL_LOADER_ID);

        public const string LEVEL_LOADER_ID = "mtm101.rulerp.baldiplus.levelstudioloader";
        public const string REC_CHARS_ID = "io.github.uncertainluei.baldiplus.recommendedchars";
        public const string CARNIVAL_PACK_ID = "mtm101.rulerp.bbplus.carnivalpackroot";
        public const string CRIMINAL_PACK_ID = "mtm101.rulerp.baldiplus.criminalpackroot";
        public const string PIRATE_PACK_ID = "mtm101.rulerp.baldiplus.piratepack";
        public const string QOP_ID = "rost.moment.baldiplus.qop";

        private static List<CompatibilityModule> modules = new List<CompatibilityModule>();

        public static List<CompatibilityModule> Modules => modules;

        public static bool IsActive<T>() where T : CompatibilityModule
        {
            Type type = typeof(T);
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].GetType() == type)
                {
                    return modules[i].IsIntegrable();
                }
            }
            return false;
        }

        internal static void Prepare()
        {
            List<Type> types = Assembly.GetAssembly(typeof(AdvancedCore)).TryGetAllTypes().ToList();
            for (int i = 0; i < types.Count; i++)
            {
                if (!types[i].IsSubclassOf(typeof(CompatibilityModule)))
                {
                    types.RemoveAt(i);
                    i--;
                }
            }
            foreach (Type type in types)
            {
                object module = Activator.CreateInstance(type, true);
                modules.Add((CompatibilityModule)module);
            }
            types.Clear();
            modules.Sort((m1, m2) => m2.Priority.CompareTo(m1.Priority));
            MethodInfo method = typeof(CompatibilityModule)
                .GetMethod("PreInitialize", AccessTools.all);
            for (int i = 0; i < modules.Count; i++)
            {
                method.Invoke(modules[i], null);
            }
        }

        internal static void OnModLoadingStarted()
        {
            MethodInfo method = typeof(CompatibilityModule)
                .GetMethod("OnModLoadingStarted", AccessTools.all);
            for (int i = 0; i < modules.Count; i++)
            {
                bool isIntegrable = modules[i].IsIntegrable();
                if (isIntegrable)
                {
                    method.Invoke(modules[i], null);
                }
                else if (!isIntegrable && modules[i].IsForced)
                {
                    throw new MessageException($"Required dependency is missing!" +
                        $"\nGUID of the integrable mod: {modules[i].Guid}");
                }
                else if (modules[i].RequiresCorrectVersion && 
                    !modules[i].VersionInfo.IsPluginCorrect(out bool versionChecked, out string error) && versionChecked)
                {
                    throw new MessageException(error);
                }
                else
                {
                    modules.RemoveAt(i);
                    i--;
                }
            }
        }

        internal static void Initialize()
        {
            MethodInfo method = typeof(CompatibilityModule)
                .GetMethod("Initialize", AccessTools.all);
            for (int i = 0; i < modules.Count; i++)
            {
                method.Invoke(modules[i], null);
            }
        }

        internal static void InvokeOnAssetsPostLoad()
        {
            MethodInfo method = typeof(CompatibilityModule).GetMethod("OnAssetsPostLoad", AccessTools.all);
            for (int i = 0; i < modules.Count; i++)
            {
                method.Invoke(modules[i], null);
            }
        }
    }
}
