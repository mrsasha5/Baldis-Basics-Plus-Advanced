using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.SavedData;
using BepInEx;
using BepInEx.Logging;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.API
{
    public static class ApiManager
    {
        internal static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("BB+ Advanced Edition API");

        internal static Action onAssetsLoadingPre;

        internal static Action onAssetsLoadingPost;

        internal static Action onExtraSettingsLoadingPost;

        //private static int elevatorTopTextDisables = 0;

        private static bool authenticModeExtended = true;

        //public static bool ElevatorTopText => elevatorTopTextDisables <= 0;

        public static ExtraSettingsData ExtraSettings => DataManager.ExtraSettings;

        public static bool AuthenticModeExtended => authenticModeExtended;

        public static void RegisterOnExtraSettingsLoadingPost(Action action)
        {
            onExtraSettingsLoadingPost += action;
        }

        public static void RegisterOnModAssetsLoadingPre(Action action)
        {
            onAssetsLoadingPre += action;
        }

        public static void RegisterOnModAssetsLoadingPost(Action action)
        {
            onAssetsLoadingPost += action;
        }

        public static void AddNewSymbolMachineWords(PluginInfo pluginInfo, params string[] words)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo)) ObjectsStorage.SymbolMachineWords.Add(pluginInfo, new List<string>());

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 5)
                {
                    logger.LogWarning("Word " + words[i] + " skipped, because max length is 5 symbols!");
                    continue;
                }
                ObjectsStorage.SymbolMachineWords[pluginInfo].Add(words[i]);
            }
            
        }

        public static void UnloadSymbolMachineWords(PluginInfo pluginInfo, params string[] words)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo)) return;

            for (int i = 0; i < words.Length; i++)
            {
                if (!ObjectsStorage.SymbolMachineWords[pluginInfo].Contains(words[i]))
                {
                    logger.LogWarning("Word " + words[i] + " doesn't exist in collection!");
                    continue;
                }
                ObjectsStorage.SymbolMachineWords[pluginInfo].Remove(words[i]);
            }
        }

        public static List<string> GetAllTipsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.TipKeys.ContainsKey(pluginInfo))
            {
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return null;
            }

            List<string> tips = ObjectsStorage.TipKeys[pluginInfo];

            return tips;
        }

        public static List<string> GetAllTips()
        {
            List<string> tips = new List<string>();

            foreach (List<string> partOfTips in ObjectsStorage.TipKeys.Values.ToArray())
            {
                tips.AddRange(partOfTips);
            }

            return tips;
        }

        public static List<string> AddNewTips(PluginInfo pluginInfo, params string[] localizationKeys)
        {
            if (!ObjectsStorage.TipKeys.ContainsKey(pluginInfo)) ObjectsStorage.TipKeys.Add(pluginInfo, new List<string>());

            List<string> tips = ObjectsStorage.TipKeys[pluginInfo];

            for (int i = 0; i < localizationKeys.Length; i++)
            {
                tips.Add(localizationKeys[i]);
            }

            return tips;
        }

        public static List<string> UnloadTips(PluginInfo pluginInfo, params string[] localizationKeys)
        {
            if (!ObjectsStorage.TipKeys.ContainsKey(pluginInfo))
            {
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return null;
            }

            List<string> tips = ObjectsStorage.TipKeys[pluginInfo];

            for (int i = 0; i < localizationKeys.Length; i++)
            {
                if (!tips.Contains(localizationKeys[i]))
                {
                    logger.LogWarning("Tip " + localizationKeys[i] + " doesn't exist in collection!");
                    continue;
                }
                tips.Remove(localizationKeys[i]);
            }

            return tips;
        }

        public static void UnloadAllTips()
        {
            ObjectsStorage.TipKeys.Clear();
        }

        public static bool UnloadAllTipsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.TipKeys.ContainsKey(pluginInfo))
            {
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return false;
            }

            ObjectsStorage.TipKeys.Remove(pluginInfo);
            return true;
        }

        public static List<PluginInfo> UnloadAllTipsExcept(params PluginInfo[] pluginInfos)
        {
            List<PluginInfo> exceptedPlugins = new List<PluginInfo>();

            foreach (PluginInfo info in ObjectsStorage.TipKeys.Keys.ToArray())
            {
                if (!pluginInfos.Contains(info))
                {
                    ObjectsStorage.TipKeys.Remove(info);
                } else
                {
                    exceptedPlugins.Add(info);
                }
            }
            
            return exceptedPlugins;
        }

        /*public static void SetElevatorTopTextActivity(bool active)
        {
            if (active)
            {
                elevatorTopTextDisables--;
            } else
            {
                elevatorTopTextDisables++;
            }

            if (elevatorTopTextDisables < 0) elevatorTopTextDisables = 0;
        }*/

        public static bool CancelAuthenticModeOverriding()
        {
            if (AssetsStorage.Overridden)
            {
                logger.LogWarning("Authentic Mode extensions can't be cancelled! It is already overridden.");
                return false;
            }
            authenticModeExtended = false;
            return true;
        }

    }
}
