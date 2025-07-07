using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
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

        private static void InvokeCrash(CompatibilityModule module)
        {
            ObjectsCreator.CauseCrash(
                $"Advanced Edition integration error!\nFailed by: {module.GetType().Name}\n" +
                $"You can turn off integation in config ({module.ConfigValue.ToString()})");
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
                //Singleton<NotificationManager>.Instance.Queue(
                //    "Some types are missing in the Integration Manager!", AssetsStorage.sounds["elv_buzz"]);
                //AdvancedCore.Logging.LogError("Can't load some types! Exception:");
                //AdvancedCore.Logging.LogError(e.ToString());

                //The most funny is that it happens not on all devices in same situation (even on same OS)
            }
            
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i] == null || !types[i].IsSubclassOf(typeof(CompatibilityModule)))
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

            for (int i = 0; i < modules.Count; i++)
            {
                typeof(CompatibilityModule)
                    .GetMethod("InitializePre", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Invoke(modules[i], null);
            }
        }

        internal static void Initialize()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                try
                {
                    bool isIntegrable = modules[i].IsIntegrable();

                    if (isIntegrable)
                    {
                        modules[i].GetType().GetMethod("Initialize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Invoke(modules[i], null);
                    }
                    else if (!isIntegrable && modules[i].IsForced)
                    {
                        ObjectsCreator.CauseCrash($"Baldi's Basics Plus Advanced Edition. Required dependency is missing!" +
                            $"\nGUID of the integrable mod: {modules[i].Guid}", AssetsStorage.weirdErrorSound);
                    }
                    else
                    {
                        modules.RemoveAt(i);
                        i--;
                    }
                }
                catch
                {
                    InvokeCrash(modules[i]);
                }
                
            }
        }

        internal static void InvokeOnAssetsLoadPost()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                try
                {
                    modules[i].GetType().GetMethod("InitializeOnAssetsLoadPost",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Invoke(modules[i], null);
                }
                catch
                {
                    InvokeCrash(modules[i]);
                }
                
            }
        }
    }
}
