using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
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

        #region Kitchen Stove Recipes

        /// <summary>
        /// Loads recipes from JSON files which should contain structure like <see cref="FoodRecipeSerializableData"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="path">JSON recipes folder location.</param>
        /// <param name="includeSubdirectories">Should the mod check files in subdirectories from path.</param>
        /// <returns>List of loaded and registered recipes.</returns>
        public static List<FoodRecipeData> LoadKitchenStoveRecipesFromFolder(PluginInfo info, string path, bool includeSubdirectories)
        {
            return LoadKitchenStoveRecipesFromFolder(info, path, includeSubdirectories, out _);
        }

        /// <summary>
        /// Loads recipes from JSON files which should contain structure like <see cref="FoodRecipeSerializableData"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="path">JSON recipes folder location.</param>
        /// <param name="includeSubdirectories">Should the mod check files in subdirectories from path.</param>
        /// <param name="failedRecipes">Recipes which were loaded, but not registered by some reason.</param>
        /// <returns>List of loaded and registered recipes.</returns>
        public static List<FoodRecipeData> LoadKitchenStoveRecipesFromFolder(PluginInfo info, string path, bool includeSubdirectories, 
            out List<FoodRecipeData> failedRecipes)
        {
            return LoadKitchenStoveRecipesFromFolder(info, path, includeSubdirectories,
                out failedRecipes, logWarnings: true, sendWarningNotifications: true);
        }

        /// <summary>
        /// Loads recipes from JSON files which should contain structure like <see cref="FoodRecipeSerializableData"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="path">JSON recipes folder location.</param>
        /// <param name="includeSubdirectories">Should the mod check files in subdirectories from path.</param>
        /// <param name="failedRecipes">Recipes which were loaded, but not registered by some reason.</param>
        /// <param name="logWarnings">Logs if some recipes loading was failed.</param>
        /// <param name="sendWarningNotifications">They let user to know if something went wrong during 
        /// recipes loading (without logging which recipes caused that)!</param>
        /// <returns>List of loaded and registered recipes.</returns>
        public static List<FoodRecipeData> LoadKitchenStoveRecipesFromFolder(PluginInfo info, string path, bool includeSubdirectories,
            out List<FoodRecipeData> failedRecipes, bool logWarnings, bool sendWarningNotifications)
        {
            return KitchenStove.LoadRecipesFromAssets(info, path, includeSubdirectories, 
                out failedRecipes, logWarnings, sendWarningNotifications);
        }

        /// <summary>
        /// Finds all Kitchen Stove's recipes by condition in <see cref="Predicate{T}"/>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>List (new instance).</returns>
        public static List<FoodRecipeData> FindKitchenStoveRecipes(Predicate<FoodRecipeData> predicate)
        {
            List<FoodRecipeData> recipes = new List<FoodRecipeData>();
            for (int i = 0; i < KitchenStove.RecipeData.Count; i++)
            {
                if (predicate.Invoke(KitchenStove.RecipeData[i]))
                {
                    recipes.Add(KitchenStove.RecipeData[i]);
                }
            }
            return recipes;
        }

        /// <summary>
        /// Returns a list of food recipe datas except provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>List (new instance).</returns>
        public static List<FoodRecipeData> GetAllKitchenStoveRecipesExcept(params PluginInfo[] pluginInfos)
        {
            List<FoodRecipeData> datas = new List<FoodRecipeData>();
            for (int i = 0; i < KitchenStove.RecipeData.Count; i++)
            {
                bool getRecipe = true;
                for (int j = 0; j < pluginInfos.Length; j++)
                {
                    if (KitchenStove.RecipeData[i].pluginInfos.Contains(pluginInfos[j]))
                    {
                        getRecipe = false;
                        break;
                    }
                }
                if (getRecipe) datas.Add(KitchenStove.RecipeData[i]);
            }
            return datas;
        }

        /// <summary>
        /// Returns a list of food recipe data from provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>List (new instance).</returns>
        public static List<FoodRecipeData> GetAllKitchenStoveRecipesFrom(params PluginInfo[] pluginInfos)
        {
            List<FoodRecipeData> datas = new List<FoodRecipeData>();
            for (int i = 0; i < KitchenStove.RecipeData.Count; i++)
            {
                for (int j = 0; j < pluginInfos.Length; j++)
                {
                    if (KitchenStove.RecipeData[i].pluginInfos.Contains(pluginInfos[j]))
                    {
                        datas.Add(KitchenStove.RecipeData[i]);
                        break;
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// Returns all recipes that have been added to the Kitchen Stove.
        /// </summary>
        /// <returns>List (new instance).</returns>
        public static List<FoodRecipeData> GetAllKitchenStoveRecipes()
        {
            return new List<FoodRecipeData>(KitchenStove.RecipeData);
        }

        /// <summary>
        /// Creates a new Kitchen Stove recipe! Also instead of using this, you can invoke RegisterRecipe() from the FoodRecipeData!
        /// Register this on pre-loading! Not after invoking generation changes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>False, if raw food components already used by other recipe and your recipe has different cooked components.</returns>
        public static bool CreateKitchenStoveRecipe(FoodRecipeData data)
        {
            data.CreateRecipePoster();

            for (int i = 0; i < KitchenStove.RecipeData.Count; i++)
            {
                if (KitchenStove.RecipeData[i].IsEqual(data))
                {
                    if (!KitchenStove.RecipeData[i].pluginInfos.Contains(data.pluginInfos[0]))
                        KitchenStove.RecipeData[i].pluginInfos.Add(data.pluginInfos[0]);
                    return true;
                } else if (KitchenStove.RecipeData[i].IsIdentical(data))
                {
                    return false;
                }
            }
            KitchenStove.RecipeData.Add(data);
            return true;
        }

        /// <summary>
        /// Removes Kitchen Stove recipe by reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>True, if the action was successful.</returns>
        public static bool RemoveKitchenStoveRecipe(FoodRecipeData data)
        {
            return KitchenStove.RecipeData.Remove(data);
        }

        /// <summary>
        /// Removes all Kitchen Stove's recipes by condition in <see cref="Predicate{T}"/>.
        /// </summary>
        /// <param name="predicate"></param>
        public static void RemoveKitchenStoveRecipesBy(Predicate<FoodRecipeData> predicate)
        {
            for (int i = 0; i < KitchenStove.RecipeData.Count; i++)
            {
                if (predicate.Invoke(KitchenStove.RecipeData[i]) && RemoveKitchenStoveRecipe(KitchenStove.RecipeData[i])) i--;
            }
        }

        /// <summary>
        /// Removes all Kitchen Stove's recipes from provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        public static void RemoveAllKitchenStoveRecipesFrom(params PluginInfo[] pluginInfos)
        {
            for (int i = 0; i < KitchenStove.RecipeData.Count; i++)
            {
                for (int j = 0; j < pluginInfos.Length; j++)
                {
                    if (KitchenStove.RecipeData[i].pluginInfos.Contains(pluginInfos[j]))
                    {
                        RemoveKitchenStoveRecipe(KitchenStove.RecipeData[i]);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all Kitchen Stove's recipes except provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        public static void RemoveAllKitchenStoveRecipesExcept(params PluginInfo[] pluginInfos)
        {
            for (int i = 0; i < KitchenStove.RecipeData.Count; i++)
            {
                bool removeRecipe = true;
                for (int j = 0; j < pluginInfos.Length; j++)
                {
                    if (KitchenStove.RecipeData[i].pluginInfos.Contains(pluginInfos[j]))
                    {
                        removeRecipe = false;
                        break;
                    }
                }
                if (removeRecipe)
                {
                    RemoveKitchenStoveRecipe(KitchenStove.RecipeData[i]);
                    i--;
                }
            }
        }

        #endregion

        #region Objects (plates, Spelloons)

        [Obsolete]
        public static T CreatePlate<T>(string name) where T : BasePlate
        {
            return PrefabsCreator.CreatePlate<T>(name, putInMemory: false);
        }

        /// <summary>
        /// Registers new symbol and creates a Spelloon with sprite!
        /// </summary>
        /// <param name="symbol">If it is provided as upper case, it will be converted to lower case.</param>
        /// <param name="sprite"></param>
        /// <returns>False if Spelloon already exists!</returns>
        public static bool CreateNewSpelloon(string symbol, Sprite sprite)
        {
            symbol = symbol.ToLower();

            if (ObjectsStorage.Spelloons.ContainsKey("spelloon_" + symbol))
            {
                logger.LogWarning($"Spelloon \"{symbol}\" already exists!");
                return false;
            }

            MathMachineNumber mathNumComp = GameObject.Instantiate(
                AssetsStorage.gameObjects["math_num_0"].GetComponent<MathMachineNumber>());
            GameObject.Destroy(mathNumComp);
            mathNumComp.gameObject.ConvertToPrefab(true);

            Spelloon spelloon = mathNumComp.gameObject.AddComponent<Spelloon>();
            spelloon.name = "Spelloon_" + symbol;
            spelloon.InitializePrefab(1);
            spelloon.InitializePrefabPost(symbol, sprite);

            SymbolMachine[] machines = AssetsHelper.LoadAssets<SymbolMachine>();

            for (int i = 0; i < machines.Length; i++)
            {
                if (!machines[i].potentialSymbols.Contains(spelloon.Value)) machines[i].potentialSymbols.Add(spelloon.Value);
            }
            
            ObjectsStorage.Spelloons.Add("spelloon_" + symbol, spelloon);
            return true;
        }

        /// <summary>
        /// Unloads Spelloon and all words that contain its symbol!
        /// </summary>
        /// <param name="symbol">If it is provided as upper case, it will be converted to lower case.</param>
        /// <returns></returns>
        public static bool UnloadSpelloon(string symbol)
        {
            symbol = symbol.ToLower();

            if (!ObjectsStorage.Spelloons.ContainsKey("spelloon_" + symbol))
            {
                logger.LogWarning($"Spelloon \"{symbol}\" doesn't exist!");
                return false;
            }

            Spelloon spelloon = ObjectsStorage.Spelloons["spelloon_" + symbol];

            List<string> words = GetAllSymbolMachineWords();
            for (int i = 0; i < words.Count; i++)
            {
                if (!words[i].Contains(symbol) && !words[i].Contains(symbol.ToUpper()))
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
        /// <param name="symbol">If it is provided as upper case, it will be converted to lower case.</param>
        /// <param name="sprite"></param>
        /// <returns>Returns false if Spelloon doesn't exist!</returns>
        public static bool UpdateSpelloonSprite(string symbol, Sprite sprite)
        {
            symbol = symbol.ToLower();
            if (!ObjectsStorage.Spelloons.ContainsKey("spelloon_" + symbol))
            {
                logger.LogWarning($"Spelloon \"{symbol}\" doesn't exist!");
                return false;
            }

            Spelloon spelloon = ObjectsStorage.Spelloons["spelloon_" + symbol];

            spelloon.GetComponentInChildren<SpriteRenderer>().sprite = sprite;

            return true;
        }

        #endregion

        #region School Council

        /// <param name="pluginInfo"></param>
        /// <returns>List (new instance).</returns>
        public static List<WeightedCouncilTopic> GetAllWeightedSchoolCouncilTopicsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.Topics.ContainsKey(pluginInfo))
                return null;

            return new List<WeightedCouncilTopic>(ObjectsStorage.Topics[pluginInfo]);
        }

        [Obsolete("Use GetAllWeightedSchoolCouncilTopicsFrom!")]
        public static List<WeightedCouncilTopic> GetAllWeigthedSchoolCouncilTopicsFrom(PluginInfo pluginInfo)
        {
            return GetAllWeightedSchoolCouncilTopicsFrom(pluginInfo);
        }

        [Obsolete("Use GetAllWeightedSchoolCouncilTopicsFrom!")]
        public static List<BaseTopic> GetAllSchoolCouncilTopicsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.Topics.ContainsKey(pluginInfo))
                return null;

            List<BaseTopic> topics = new List<BaseTopic>();
            List<WeightedCouncilTopic> _topics = ObjectsStorage.Topics[pluginInfo];

            for (int i = 0; i < _topics.Count; i++)
            {
                topics.Add(_topics[i].selection);
            }

            return topics;
        }

        /// <summary>
        /// Gets all topics from all mods.
        /// </summary>
        /// <returns>List (new instance).</returns>
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
            if (!ObjectsStorage.Topics.ContainsKey(pluginInfo)) 
                ObjectsStorage.Topics.Add(pluginInfo, new List<WeightedCouncilTopic>());

            ObjectsStorage.Topics[pluginInfo].Add(new WeightedCouncilTopic()
            {
                selection = new T(),
                weight = weight
            });
        }

        /// <summary>
        /// Unloads all voting topics except provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>Plugins which topics were removed.</returns>
        public static List<PluginInfo> UnloadAllSchoolCouncilTopicsExcept(params PluginInfo[] pluginInfos)
        {
            List<PluginInfo> unloadedPlugins = new List<PluginInfo>();
            foreach (PluginInfo pluginInfo in ObjectsStorage.Topics.Keys.ToArray())
            {
                if (!pluginInfos.Contains(pluginInfo))
                {
                    ObjectsStorage.Topics.Remove(pluginInfo);
                    unloadedPlugins.Add(pluginInfo);
                }
            }
            return unloadedPlugins;
        }

        /// <summary>
        /// Unloads all voting topics from provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>Plugins which topics were removed.</returns>
        public static List<PluginInfo> UnloadAllSchoolCouncilTopicsFrom(params PluginInfo[] pluginInfos)
        {
            List<PluginInfo> unloadedPlugins = new List<PluginInfo>();
            foreach (PluginInfo pluginInfo in pluginInfos)
            {
                if (ObjectsStorage.Topics.ContainsKey(pluginInfo))
                {
                    ObjectsStorage.Topics.Remove(pluginInfo);
                    unloadedPlugins.Add(pluginInfo);
                }
            }
            return unloadedPlugins;
        }

        /// <summary>
        /// Unloads all voting topics.
        /// </summary>
        public static void UnloadAllSchoolCouncilTopics()
        {
            ObjectsStorage.Topics.Clear();
        }

        /// <summary>
        /// Unloads voting topics by references.
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
        /// Returns a list of the Symbol Machine words from the provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>List (new instance).</returns>
        public static List<string> GetAllSymbolMachineWordsFrom(params PluginInfo[] pluginInfos)
        {
            List<string> words = new List<string>();
            foreach (KeyValuePair<PluginInfo, List<string>> pair in ObjectsStorage.SymbolMachineWords)
            {
                words.AddRange(pair.Value);
            }

            return words;
        }

        [Obsolete("Use overload with array argument!")]
        public static List<string> GetAllSymbolMachineWordsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo))
            {
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return null;
            }

            return new List<string>(ObjectsStorage.SymbolMachineWords[pluginInfo]);
        }

        /// <summary>
        /// Returns all Symbol Machine words.
        /// </summary>
        /// <returns>List (new instance).</returns>
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
        /// Adds a new words for the Symbol Machine.
        /// Max length - 5.
        /// Do not use localization keys.
        /// </summary>
        /// <param name="pluginInfo"></param>
        /// <param name="words">Words to add.</param>
        /// <returns>List (reference). Do not modify.</returns>
        public static List<string> AddNewSymbolMachineWords(PluginInfo pluginInfo, params string[] words)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo)) 
                ObjectsStorage.SymbolMachineWords.Add(pluginInfo, new List<string>());

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 5)
                {
                    logger.LogWarning("Word " + words[i] + " is skipped, because max length is 5 symbols!");
                    continue;
                }

                foreach (char symbol in words[i])
                {
                    if (!ObjectsStorage.Spelloons.ContainsKey("spelloon_" + symbol.ToString().ToLower()))
                    {
                        logger.LogWarning("Word " + words[i] + " is skipped, because it contains not existing symbol(s).");
                        goto EndAndDontAdd;
                    }
                }
                
                ObjectsStorage.SymbolMachineWords[pluginInfo].Add(words[i]);
            EndAndDontAdd:;
            }

            return ObjectsStorage.SymbolMachineWords[pluginInfo];
        }

        /// <summary>
        /// Unloads provided words from the Symbol Machine from all mods.
        /// Words are not localization keys.
        /// </summary>
        /// <param name="words">Words to remove.</param>
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
        /// Unloads words from the Symbol Machine from provided mod.
        /// </summary>
        /// <param name="pluginInfo"></param>
        /// <param name="words">Words to unload.</param>
        /// <returns>List (new instance). May return null if plugin is not found.</returns>
        public static List<string> UnloadSymbolMachineWords(PluginInfo pluginInfo, params string[] words)
        {
            if (!ObjectsStorage.SymbolMachineWords.ContainsKey(pluginInfo))
            {
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
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

            return new List<string>(ObjectsStorage.SymbolMachineWords[pluginInfo]);
        }

        /// <summary>
        /// Unloads all words from the Symbol Machine from provided mod.
        /// </summary>
        /// <param name="pluginInfo"></param>
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
        /// Unloads all words from the Symbol Machine.
        /// </summary>
        public static void UnloadAllSymbolMachineWords()
        {
            ObjectsStorage.SymbolMachineWords.Clear();
        }

        /// <summary>
        /// Unloads all words from the Symbol Machine except ones from provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>Mods that have registered their words and their words have been deleted.</returns>
        public static List<PluginInfo> UnloadAllSymbolMachineWordsExcept(params PluginInfo[] pluginInfos)
        {
            List<PluginInfo> unloadedPlugins = new List<PluginInfo>();

            foreach (PluginInfo info in ObjectsStorage.SymbolMachineWords.Keys.ToArray())
            {
                if (!pluginInfos.Contains(info))
                {
                    ObjectsStorage.SymbolMachineWords.Remove(info);
                    unloadedPlugins.Add(info);
                }
            }

            return unloadedPlugins;
        }

        #endregion

        #region Tips

        /// <summary>
        /// Returns a list of tips (localization keys) from provided mod.
        /// </summary>
        /// <param name="pluginInfo"></param>
        /// <returns>List (new instance).</returns>
        public static List<string> GetAllTipsFrom(PluginInfo pluginInfo)
        {
            if (!ObjectsStorage.TipKeys.ContainsKey(pluginInfo))
            {
                logger.LogWarning("Requested PluginInfo doesn't exist in dictionary!");
                return null;
            }

            return new List<string>(ObjectsStorage.TipKeys[pluginInfo]);
        }

        /// <summary>
        /// Returns a list of all tips (localization keys).
        /// </summary>
        /// <returns>List (new instance).</returns>
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
        /// </summary>
        /// <param name="pluginInfo"></param>
        /// <param name="localizationKeys">Tips.</param>
        /// <returns>List of tips (new instance) from provided mod.</returns>
        public static List<string> AddNewTips(PluginInfo pluginInfo, params string[] localizationKeys)
        {
            if (!ObjectsStorage.TipKeys.ContainsKey(pluginInfo)) ObjectsStorage.TipKeys.Add(pluginInfo, new List<string>());

            List<string> tips = ObjectsStorage.TipKeys[pluginInfo];

            for (int i = 0; i < localizationKeys.Length; i++)
            {
                tips.Add(localizationKeys[i]);
            }

            return new List<string>(tips);
        }

        /// <summary>
        /// Unloads a tips from the elevator from provided mod.
        /// </summary>
        /// <param name="pluginInfo"></param>
        /// <param name="localizationKeys">Tips.</param>
        /// <returns>List of tips (new instance) from provided mod. Do not modify! Use for analyze.</returns>
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
        /// Unloads all tips from the elevator!
        /// </summary>
        public static void UnloadAllTips()
        {
            ObjectsStorage.TipKeys.Clear();
        }

        /// <summary>
        /// Unloads all tips from the elevator from provided mod.
        /// </summary>
        /// <param name="pluginInfo"></param>
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
        /// Unloads all tips from the elevator except provided mods.
        /// </summary>
        /// <param name="pluginInfos"></param>
        /// <returns>Mods that have registered their words and their words have been deleted.</returns>
        public static List<PluginInfo> UnloadAllTipsExcept(params PluginInfo[] pluginInfos)
        {
            List<PluginInfo> unloadedPlugins = new List<PluginInfo>();

            foreach (PluginInfo info in ObjectsStorage.TipKeys.Keys.ToArray())
            {
                if (!pluginInfos.Contains(info))
                {
                    ObjectsStorage.TipKeys.Remove(info);
                    unloadedPlugins.Add(info);
                }
            }
            
            return unloadedPlugins;
        }

        #endregion
    }
}
