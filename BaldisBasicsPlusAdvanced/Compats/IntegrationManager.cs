﻿using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BaldisBasicsPlusAdvanced.Compats
{
    public class IntegrationManager
    {
        public static bool LevelLoaderInstalled => AssetsHelper.ModInstalled(levelLoaderId);

        public const string levelLoaderId = "mtm101.rulerp.baldiplus.levelloader";

        private static List<CompabilityModule> modules = new List<CompabilityModule>();

        public static List<CompabilityModule> Modules => modules;

        public static bool IsActive<T>() where T : CompabilityModule
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
            List<Type> types;
            try
            {
                types = Assembly.GetAssembly(typeof(AdvancedCore)).GetTypes().ToList();
            } 
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.ToList();
            }
            
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i] == null || !types[i].IsSubclassOf(typeof(CompabilityModule)))
                {
                    types.RemoveAt(i);
                    i--;
                }
            }
            foreach (Type type in types)
            {
                Object module = Activator.CreateInstance(type, true);
                modules.Add((CompabilityModule)module);
            }
            types.Clear();

            modules.Sort((m1, m2) => m2.Priority.CompareTo(m1.Priority));

            for (int i = 0; i < modules.Count; i++)
            {
                typeof(CompabilityModule)
                    .GetMethod("InitializePre", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Invoke(modules[i], null);
            }
        }

        internal static void Initialize()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].IsIntegrable())
                {
                    modules[i].GetType().GetMethod("Initialize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Invoke(modules[i], null);
                } else if (!modules[i].IsIntegrable() && modules[i].IsForced)
                {
                    ObjectsCreator.CauseCrash($"Baldi's Basics Plus Advanced Edition. Required dependency is missing!" +
                        $"\nGUID of the required mod: {modules[i].Guid}", AssetsStorage.weirdErrorSound);
                }
            }
        }

    }
}
