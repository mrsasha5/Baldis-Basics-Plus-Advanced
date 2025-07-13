using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using BaldisBasicsPlusAdvanced.Game.WeightedSelections;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BaldisBasicsPlusAdvanced.SaveSystem.Data;
using BaldisBasicsPlusAdvanced.SerializableData;
using BepInEx;
using BepInEx.Logging;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.API
{
    public static class ApiManager
    {
        internal static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("BB+ Advanced Edition API");

        internal static Action onAssetsPreLoading;

        internal static Action onAssetsPostLoading;

        internal static Action onExtraSettingsPreLoading;

        internal static Action onExtraSettingsPostLoading;

        /// <summary>
        /// It contains data from options menu "Extra Settings".
        /// Please note that the value will be nullable until player's profile will be loaded.
        /// </summary>
        public static ExtraSettingsData ExtraSettings => OptionsDataManager.ExtraSettings;

        #region Kitchen Stove recipes

        /// <summary>
        /// Loads recipes from JSON files which should contain structure like <see cref="FoodRecipeSerializableData"/> class has it.
        /// Overloading of LoadKitchenStoveRecipesFromFolder(string, bool, bool, bool) with recommended parameters (still free to use it!).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="includeSubdirectories"></param>
        public static void LoadKitchenStoveRecipesFromFolder(string path, bool includeSubdirectories)
        {
            LoadKitchenStoveRecipesFromFolder(path, includeSubdirectories, logWarnings: true, sendWarningNotifications: false);
        }

        /// <summary>
        /// Loads recipes from JSON files which should contain structure like <see cref="FoodRecipeSerializableData"/> class has it.
        /// </summary>
        /// <param name="path">Folder's path.</param>
        /// <param name="includeSubdirectories"></param>
        /// <param name="logWarnings">Logs if some recipes loading was failed (each recipe will be showed in console + exception if it exists).</param>
        /// <param name="sendWarningNotifications">They let user to know if something went wrong during recipes loading (without logging which recipes caused that)!</param>
        public static void LoadKitchenStoveRecipesFromFolder(string path, bool includeSubdirectories, 
            bool logWarnings, bool sendWarningNotifications)
        {
            KitchenStove.LoadRecipesFromAssets(path, includeSubdirectories, logWarnings, sendWarningNotifications);
        }

        /// <summary>
        /// Return a list of food recipe datas from all mods except specified.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns></returns>
        public static List<FoodRecipeData> GetAllKitchenStoveRecipesExcept(params PluginInfo[] pluginInfos)
        {
            List<FoodRecipeData> datas = new List<FoodRecipeData>();
            foreach (FoodRecipeData data in KitchenStove.Datas)
            {
                foreach (PluginInfo pluginInfo in pluginInfos)
                {
                    if (!data.pluginInfos.Contains(pluginInfo))
                    {
                        datas.Add(data);
                        break;
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// Return a list of food recipe datas from specified mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns></returns>
        public static List<FoodRecipeData> GetAllKitchenStoveRecipesFrom(params PluginInfo[] pluginInfos)
        {
            List<FoodRecipeData> datas = new List<FoodRecipeData>();
            foreach (FoodRecipeData data in KitchenStove.Datas)
            {
                foreach (PluginInfo pluginInfo in pluginInfos)
                {
                    if (data.pluginInfos.Contains(pluginInfo))
                    {
                        datas.Add(data);
                        break;
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// Returns all recipes that have been added to the Kitchen Stove.
        /// </summary>
        /// <returns>New instance of the original reference.</returns>
        public static List<FoodRecipeData> GetAllKitchenStoveRecipes()
        {
            return new List<FoodRecipeData>(KitchenStove.Datas);
        }

        /// <summary>
        /// Creates a new Kitchen Stove recipe! Also instead of using this, you can invoke RegisterRecipe() from the FoodRecipeData!
        /// </summary>
        /// <param name="data"></param>
        /// <returns>False, if raw food components already used by other recipe and your recipe has different cooked components.</returns>
        public static bool CreateKitchenStoveRecipe(FoodRecipeData data)
        {
            for (int i = 0; i < KitchenStove.Datas.Count; i++)
            {
                if (KitchenStove.Datas[i].IsEqual(data))
                {
                    if (!KitchenStove.Datas[i].pluginInfos.Contains(data.pluginInfos[0])) KitchenStove.Datas[i].pluginInfos.Add(data.pluginInfos[0]);
                    return true;
                } else if (KitchenStove.Datas[i].IsIdentical(data))
                {
                    return false;
                }
            }
            KitchenStove.Datas.Add(data);
            return true;
        }

        /// <summary>
        /// Removes Kitchen Stove recipe by reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>True, if the action was successful.</returns>
        public static bool RemoveKitchenStoveRecipe(FoodRecipeData data)
        {
            return KitchenStove.Datas.Remove(data);
        }

        /// <summary>
        /// Remove all kitchen recipes from mods that array contains.
        /// </summary>
        /// <param name="pluginInfos"></param>
        public static void RemoveAllKitchenStoveRecipesFrom(params PluginInfo[] pluginInfos)
        {
            foreach (PluginInfo plugin in pluginInfos)
            {
                for (int i = 0; i < KitchenStove.Datas.Count; i++)
                {
                    if (KitchenStove.Datas[i].pluginInfos.Contains(plugin))
                    {
                        RemoveKitchenStoveRecipe(KitchenStove.Datas[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Remove all kitchen recipes from mods that array not contains.
        /// </summary>
        /// <param name="pluginInfos"></param>
        public static void RemoveAllKitchenStoveRecipesExcept(params PluginInfo[] pluginInfos)
        {
            foreach (PluginInfo plugin in pluginInfos)
            {
                for (int i = 0; i < KitchenStove.Datas.Count; i++)
                {
                    if (!KitchenStove.Datas[i].pluginInfos.Contains(plugin))
                    {
                        RemoveKitchenStoveRecipe(KitchenStove.Datas[i]);
                    }
                }
            }
        }
        #endregion

        #region Objects (plates, Spelloons)

        /// <summary>
        /// Just creates a plate prefab.
        /// Remember that classes outside of the API can be updated without any saving of old parts of the code!
        /// If you find it difficult to figure out how this works, then study the code of existing slabs!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T CreatePlate<T>(string name) where T : BasePlate
        {
            return PrefabsCreator.CreatePlate<T>(name, putInMemory: false);
        }

        /// <summary>
        /// Registers new symbol and creates a Spelloon with sprite!
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sprite"></param>
        /// <returns>False if Spelloon already exists!</returns>
        public static bool CreateNewSpelloon(string symbol, Sprite sprite)
        {
            string _symbol = symbol.ToLower();//symbol.ToString().ToLower();

            if (ObjectsStorage.Spelloons.ContainsKey("spelloon_" + _symbol))
            {
                logger?.LogWarning($"Spelloon \"{symbol}\" already exists!");
                return false;
            }

            MathMachineNumber mathNumComp = GameObject.Instantiate(AssetsHelper.LoadAsset<MathMachineNumber>("MathNum_0"));
            GameObject.Destroy(mathNumComp);
            mathNumComp.gameObject.ConvertToPrefab(true);

            Spelloon spelloon = mathNumComp.gameObject.AddComponent<Spelloon>();
            spelloon.name = "Spelloon_" + _symbol;
            spelloon.InitializePrefab(1);
            spelloon.InitializePrefabPost(_symbol, sprite);

            SymbolMachine[] machines = AssetsHelper.LoadAssets<SymbolMachine>();

            for (int i = 0; i < machines.Length; i++)
            {
                if (!machines[i].potentialSymbols.Contains(spelloon.Value)) machines[i].potentialSymbols.Add(spelloon.Value);
            }
            
            ObjectsStorage.Spelloons.Add("spelloon_" + _symbol, spelloon);
            return true;
        }

        /// <summary>
        /// Unloads Spelloon and all words that contain its symbol!
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool UnloadSpelloon(string symbol)
        {
            if (!ObjectsStorage.Spelloons.ContainsKey("spelloon_" + symbol))
            {
                logger?.LogWarning($"Spelloon \"{symbol}\" doesn't exist!");
                return false;
            }

            Spelloon spelloon = ObjectsStorage.Spelloons["spelloon_" + symbol];

            List<string> words = GetAllSymbolMachineWords();
            for (int i = 0; i < words.Count; i++)
            {
                if (!words[i].Contains(symbol))
                {
                    words.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            SymbolMachine[] machines = AssetsHelper.LoadAssets<SymbolMachine>();

            for (int i = 0; i < machines.Length; i++)
            {
                if (machines[i].potentialSymbols.Contains(spelloon.Value)) machines[i].potentialSymbols.Remove(spelloon.Value);
            }

            UnloadSymbolMachineWordsFromAllMods(words.ToArray());

            ObjectsStorage.Spelloons.Remove("spelloon_" + symbol);
            spelloon.gameObject.RemoveUnloadMark();
            GameObject.Destroy(spelloon.gameObject);

            return true;
        }

        /// <summary>
        /// Replaces Spelloon's sprite!
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sprite"></param>
        /// <returns>Returns false if Spelloon doesn't exist!</returns>
        public static bool UpdateSpelloonSprite(string symbol, Sprite sprite)
        {
            if (!ObjectsStorage.Spelloons.ContainsKey("spelloon_" + symbol))
            {
                logger?.LogWarning($"Spelloon \"{symbol}\" doesn't exist!");
                return false;
            }

            Spelloon spelloon = ObjectsStorage.Spelloons["spelloon_" + symbol];

            spelloon.GetComponentInChildren<SpriteRenderer>().sprite = sprite;

            return true;
        }

        #endregion

        #region School Council

        /// <summary>
        /// Returns a list (new instance) of the specific mod's weighted topics.
        /// </summary>
        /// <param name="pluginInfo"></param>
        /// <returns></returns>
        public static List<WeightedCouncilTopic> GetAllWeigthedSchoolCouncilTopicsFrom(PluginInfo pluginInfo)
        {
            return ObjectsStorage.Topics[pluginInfo];
        }

        /// <summary>
        /// Returns a list (new instance) of the specific mod's topics.
        /// </summary>
        /// <param name="pluginInfo"></param>
        /// <returns></returns>
        public static List<BaseTopic> GetAllSchoolCouncilTopicsFrom(PluginInfo pluginInfo)
        {
            List<BaseTopic> topics = new List<BaseTopic>();
            List<WeightedCouncilTopic> _topics = ObjectsStorage.Topics[pluginInfo];

            for (int i = 0; i < _topics.Count; i++)
            {
                topics.Add(_topics[i].selection);
            }

            return topics;
        }

        public static List<BaseTopic> GetAllSchoolCouncilTopics()
        {
            List<BaseTopic> topics = new List<BaseTopic>();

            foreach (List<WeightedCouncilTopic> _topics in ObjectsStorage.Topics.Values)
            {
                for (int i = 0; i < _topics.Count; i++)
                {
                    topics.Add(_topics[i].selection);
                }
            }

            return topics;
        }

        /// <summary>
        /// Creates a new topic that can appear on the voting event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pluginInfo"></param>
        /// <param name="weight"></param>
        public static void CreateSchoolCouncilTopic<T>(PluginInfo pluginInfo, int weight = 100) where T : BaseTopic, new()
        {
            if (!ObjectsStorage.Topics.ContainsKey(pluginInfo)) ObjectsStorage.Topics.Add(pluginInfo, new List<WeightedCouncilTopic>());
            ObjectsStorage.Topics[pluginInfo].Add(new WeightedCouncilTopic()
            {
                selection = new T(),
                weight = weight
            });
        }

        /// <summary>
        /// Unloads all School Council's topics except specific mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>List that contains plugins from which topics were removed.</returns>
        public static List<PluginInfo> UnloadAllSchoolCouncilTopicsExcept(params PluginInfo[] pluginInfos)
        {
            List<PluginInfo> exceptedPlugins = new List<PluginInfo>();
            foreach (PluginInfo pluginInfo in ObjectsStorage.Topics.Keys.ToArray())
            {
                if (!pluginInfos.Contains(pluginInfo))
                {
                    ObjectsStorage.Topics.Remove(pluginInfo);
                    exceptedPlugins.Add(pluginInfo);
                }
            }
            return exceptedPlugins;
        }

        /// <summary>
        /// Unloads all School Council's topics from specific mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>List that contains plugins from which topics were removed.</returns>
        public static List<PluginInfo> UnloadAllSchoolCouncilTopicsFrom(params PluginInfo[] pluginInfos)
        {
            List<PluginInfo> exceptedPlugins = new List<PluginInfo>();
            foreach (PluginInfo pluginInfo in pluginInfos)
            {
                if (ObjectsStorage.Topics.ContainsKey(pluginInfo))
                {
                    ObjectsStorage.Topics.Remove(pluginInfo);
                    exceptedPlugins.Add(pluginInfo);
                }
            }
            return exceptedPlugins;
        }

        /// <summary>
        /// Unloads all School Council's topics.
        /// </summary>
        public static void UnloadAllSchoolCouncilTopics()
        {
            ObjectsStorage.Topics.Clear();
        }

        /// <summary>
        /// Unloads School Council's topics by reference.
        /// </summary>
        /// <param name="topics"></param>
        public static void UnloadSchoolCouncilTopics(params BaseTopic[] topics)
        {
            foreach (List<WeightedCouncilTopic> _topics in ObjectsStorage.Topics.Values)
            {
                for (int i = 0; i < _topics.Count; i++)
                {
                    if (topics.Contains(_topics[i].selection))
                    {
                        _topics.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Invokes transmitted delegate before/after loading Extra Settings.
        /// </summary>
        /// <param name="action">Delegate that will be invoked when needed.</param>
        /// <param name="post">If true, then the action will be invoked after loading.</param>
        public static void RegisterOnExtraSettingsLoading(Action action, bool post)
        {
            if (post)
                onExtraSettingsPostLoading += action;
            else onExtraSettingsPreLoading += action;
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

        #endregion

        #region Symbol Machine words

        /// <summary>
        /// Returns a list of the Symbol Machine words from the desired mod.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <returns>Current list reference. Changes to this list lead to changes in the display of words. Result may be null!</returns>
        public static List<string> GetAllSymbolMachineWordsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo))
            {
                logger?.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
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

            foreach (List<string> _words in ObjectsStorage.SymbolMachineWords.Values)
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
                    logger?.LogWarning("Word " + words[i] + " skipped, because max length is 5 symbols!");
                    continue;
                }
                ObjectsStorage.SymbolMachineWords[pluginInfo].Add(words[i]);
            }

            return ObjectsStorage.SymbolMachineWords[pluginInfo];
        }

        /// <summary>
        /// Unloads words for the Symbol Machine, that were added by any mods.
        /// Words are not localization keys, you must specify an English word.
        /// </summary>
        /// <param name="words"></param>
        public static void UnloadSymbolMachineWordsFromAllMods(params string[] words)
        {
            foreach (List<string> _words in ObjectsStorage.SymbolMachineWords.Values)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    if (_words.Contains(words[i]))
                    {
                        _words.Remove(words[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads words for the Symbol Machine, that were added by current mod.
        /// Words are not localization keys, you must specify an English word.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <param name="words">String array that contains words.</param>
        /// <returns>Current list reference. Changes to this list lead to changes in the display of tips.
        /// It recommended using only for analysis.</returns>
        public static List<string> UnloadSymbolMachineWords(PluginInfo pluginInfo, params string[] words)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo))
            {
                logger?.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return null;
            }

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
        /// Unloads all words for the Symbol Machine, that were added by current mod.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <returns>True, if the action was successful.</returns>
        public static bool UnloadAllSymbolMachineWordsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo))
            {
                logger?.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
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

            foreach (PluginInfo info in ObjectsStorage.SymbolMachineWords.Keys)
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

        #endregion

        #region Tips

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
                logger?.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
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

            foreach (List<string> partOfTips in ObjectsStorage.TipKeys.Values)
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
        /// Unloads a tips for the elevator, that were added by current mod.
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
                logger?.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return null;
            }

            List<string> tips = ObjectsStorage.TipKeys[pluginInfo];

            for (int i = 0; i < localizationKeys.Length; i++)
            {
                if (!tips.Contains(localizationKeys[i]))
                {
                    logger?.LogWarning("Tip " + localizationKeys[i] + " doesn't exist in collection!");
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
        /// Unloads absolutely all tips from the elevator, that were added by current mod.
        /// Returns true, if the action was successful.
        /// </summary>
        /// <param name="pluginInfo">Current mod info.</param>
        /// <returns>Returns true, if the action was successful.</returns>
        public static bool UnloadAllTipsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.TipKeys.ContainsKey(pluginInfo))
            {
                logger?.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
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

            foreach (PluginInfo info in ObjectsStorage.TipKeys.Keys)
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

        #endregion
    }
}
