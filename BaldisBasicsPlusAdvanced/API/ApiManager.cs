using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using BaldisBasicsPlusAdvanced.SavedData;
using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaldisBasicsPlusAdvanced.API
{
    public static class ApiManager
    {
        internal static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("BB+ Advanced Edition API");

        internal static Action onAssetsPreLoading;

        internal static Action onAssetsPostLoading;

        internal static Action onExtraSettingPreLoading;

        internal static Action onExtraSettingsPostLoading;

        //private static int elevatorTopTextDisables = 0;

        //public static bool ElevatorTopText => elevatorTopTextDisables <= 0;

        /// <summary>
        /// It contains data from options menu "Extra Settings".
        /// Please note that the value may be null.
        /// </summary>
        public static ExtraSettingsData ExtraSettings => DataManager.ExtraSettings;

        [Obsolete("Use RegisterOnExtraSettingsLoading!")]
        public static void RegisterOnExtraSettingsLoadingPost(Action action)
        {
            RegisterOnExtraSettingsLoading(action, true);
        }

        [Obsolete("Use RegisterOnModAssetsLoading!")]
        public static void RegisterOnModAssetsLoadingPre(Action action)
        {
            RegisterOnModAssetsLoading(action, true);
        }

        [Obsolete("Use RegisterOnModAssetsLoading!")]
        public static void RegisterOnModAssetsLoadingPost(Action action)
        {
            RegisterOnModAssetsLoading(action, true);
        }

        /*/// <summary>
        /// You can dynamically override the text in the elevator!
        /// </summary>
        /// <param name="key">Localization key</param>
        /// <param name="asTip">If true, then your text will appear as a tip.</param>
        /// <returns>True, if the action was successful.</returns>
        public static bool SetElevatorText(bool state, string key, bool asTip)
        {
            ElevatorTipsPatch.SetOverride(state, key, asTip);
        }*/

        /// <summary>
        /// Invokes transmitted delegate before/after loading Extra Settings.
        /// </summary>
        /// <param name="action">Delegate that will be invoked when needed.</param>
        /// <param name="post">If true, then the action will be invoked after loading.</param>
        public static void RegisterOnExtraSettingsLoading(Action action, bool post)
        {
            if (post)
                onExtraSettingsPostLoading += action;
            else onExtraSettingPreLoading += action;
        }

        /// <summary>
        /// Invokes transmitted delegate before/after loading mod assets.
        /// </summary>
        /// <param name="action">Delegate that will be invoked when needed.</param>
        /// <param name="post">If true, then the action will be invoked after loading.</param>
        public static void RegisterOnModAssetsLoading(Action action, bool post)
        {
            if (post)
                onAssetsPostLoading += action;
            else onAssetsPreLoading += action;
        }

        /// <summary>
        /// Returns a list of the Symbol Machine words from the desired mod.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <returns>Current list reference. Changes to this list lead to changes in the display of words. Result may be null!</returns>
        public static List<string> GetAllSymbolMachineWordsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo))
            {
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return null;
            }

            List<string> tips = ObjectsStorage.TipKeys[pluginInfo];

            return tips;
        }

        /// <summary>
        /// Returns a list of all the Symbol Machine words. Not a valid reference.
        /// </summary>
        /// <returns>Any changes to the list will not affect the words displayed.</returns>
        public static List<string> GetAllSymbolMachineWords()
        {
            List<string> words = new List<string>();

            foreach (List<string> _words in ObjectsStorage.SymbolMachineWords.Values.ToArray())
            {
                words.AddRange(_words);
            }

            return words;
        }

        /// <summary>
        /// Adds a new words for the Symbol Machine. Transmitting words that contain more than 5 characters is unacceptable.
        /// Words that do not meet the allowed number of characters will simply be ignored.
        /// Add words, not localization keys.
        /// Add words using the API!
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <param name="words">String array that contains words.</param>
        /// <returns>Current list reference. Changes to this list lead to changes in the display of tips.
        /// It recommended using only for analysis.</returns>
        public static List<string> AddNewSymbolMachineWords(PluginInfo pluginInfo, params string[] words)
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

            return ObjectsStorage.SymbolMachineWords[pluginInfo];
        }

        /// <summary>
        /// Unloads words for the Symbol Machine, that was added by current mod.
        /// Words are not localization keys, you must specify an English word.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <param name="words">String array that contains words.</param>
        /// <returns>Current list reference. Changes to this list lead to changes in the display of tips.
        /// It recommended using only for analysis.</returns>
        public static List<string> UnloadSymbolMachineWords(PluginInfo pluginInfo, params string[] words)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo))
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");

            for (int i = 0; i < words.Length; i++)
            {
                if (!ObjectsStorage.SymbolMachineWords[pluginInfo].Contains(words[i]))
                {
                    logger.LogWarning("Word " + words[i] + " doesn't exist in collection!");
                    continue;
                }
                ObjectsStorage.SymbolMachineWords[pluginInfo].Remove(words[i]);
            }

            return ObjectsStorage.SymbolMachineWords[pluginInfo];
        }

        /// <summary>
        /// Unloads all words for the Symbol Machine, that was added by current mod.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <returns>True, if the action was successful.</returns>
        public static bool UnloadAllSymbolMachineWordsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo))
            {
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return false;
            }
            ObjectsStorage.SymbolMachineWords.Remove(pluginInfo);
            return true;
        }

        /// <summary>
        /// Unloads absolutely all words for the Symbol Machine.
        /// </summary>
        public static void UnloadAllSymbolMachineWords()
        {
            ObjectsStorage.SymbolMachineWords.Clear();
        }

        /// <summary>
        /// Unloads all words for the Symbol Machine from mods that are not listed in the array.
        /// </summary>
        /// <param name="pluginInfos">Current mods info.</param>
        /// <returns>It contains mods that have registered their words and their words have been deleted. It is useful for analysis.</returns>
        public static List<PluginInfo> UnloadAllSymbolMachineWordsExcept(params PluginInfo[] pluginInfos)
        {
            List<PluginInfo> exceptedPlugins = new List<PluginInfo>();

            foreach (PluginInfo info in ObjectsStorage.SymbolMachineWords.Keys.ToArray())
            {
                if (!pluginInfos.Contains(info))
                {
                    ObjectsStorage.SymbolMachineWords.Remove(info);
                }
                else
                {
                    exceptedPlugins.Add(info);
                }
            }

            return exceptedPlugins;
        }

        /// <summary>
        /// Returns a list of tips (localization keys) from the desired mod.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <returns>Current list reference. Changes to this list lead to changes in the display of tips.
        /// It recommended using only for analysis. Result may be null!</returns>
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

        /// <summary>
        /// Returns a list of all tips (localization keys). Not a valid reference.
        /// </summary>
        /// <returns>Any changes to the list will not affect the tips displayed.</returns>
        public static List<string> GetAllTips()
        {
            List<string> tips = new List<string>();

            foreach (List<string> partOfTips in ObjectsStorage.TipKeys.Values.ToArray())
            {
                tips.AddRange(partOfTips);
            }

            return tips;
        }

        /// <summary>
        /// Adds a new tips for the elevator.
        /// Add localization keys, not translated tips!
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <param name="localizationKeys">String array that contains localization keys.</param>
        /// <returns>Current list reference. Changes to this list lead to changes in the display of tips.
        /// It recommended using only for analysis.</returns>
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

        /// <summary>
        /// Unloads a tips for the elevator, that was added by current mod.
        /// Unload localization keys, not translated tips!
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <param name="localizationKeys">String array that contains localization keys.</param>
        /// <returns>Current list reference. Changes to this list lead to changes in the display of tips.
        /// It recommended using only for analysis. Result may be null!</returns>
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

        /// <summary>
        /// Unloads absolutely all tips from the elevator!
        /// </summary>
        public static void UnloadAllTips()
        {
            ObjectsStorage.TipKeys.Clear();
        }

        /// <summary>
        /// Unloads absolutely all tips from the elevator, that was added by current mod.
        /// Returns true, if the action was successful.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <returns>Returns true, if the action was successful.</returns>
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

        /// <summary>
        /// Unloads all tips for the elevator from mods that are not listed in the array.
        /// </summary>
        /// <param name="pluginInfos">Current mods info.</param>
        /// <returns>It contains mods that have registered their tips and their tips have been deleted. It is useful for analysis.</returns>
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
    }
}
