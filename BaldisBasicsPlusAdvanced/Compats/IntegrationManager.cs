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
        public static bool LevelLoaderInstalled => AssetHelper.ModInstalled(levelLoaderId);

        public const string levelLoaderId = "mtm101.rulerp.baldiplus.levelstudioloader";

        public const string recommendedCharactersId = "io.github.uncertainluei.baldiplus.recommendedchars";

        public const string carnivalPackId = "mtm101.rulerp.bbplus.carnivalpackroot";

        public const string criminalPackId = "mtm101.rulerp.baldiplus.criminalpackroot";

        public const string piratePackId = "mtm101.rulerp.baldiplus.piratepack";

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
                Object module = Activator.CreateInstance(type, true);
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

        internal static void Initialize()
        {
            MethodInfo method = typeof(CompatibilityModule)
                .GetMethod("Initialize", AccessTools.all);
            for (int i = 0; i < modules.Count; i++)
            {
                bool isIntegrable = modules[i].IsIntegrable();

                if (isIntegrable)
                {
                    method.Invoke(modules[i], null);
                }
                else if (!isIntegrable && modules[i].IsForced)
                {
                    throw new Exception($"Baldi's Basics Plus Advanced Edition. Required dependency is missing!" +
                        $"\nGUID of the integrable mod: {modules[i].Guid}");
                }
                else
                {
                    modules.RemoveAt(i);
                    i--;
                }
            }
        }

        internal static void InvokeOnAssetsPosrLoad()
        {
            MethodInfo method = typeof(CompatibilityModule).GetMethod("OnAssetsPostLoad", AccessTools.all);
            for (int i = 0; i < modules.Count; i++)
            {
                method.Invoke(modules[i], null);
            }
        }

    }
}
