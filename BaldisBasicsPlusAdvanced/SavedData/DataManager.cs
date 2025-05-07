using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Helpers;
using BepInEx;
using MTM101BaldAPI.SaveSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityCipher;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SavedData
{
    internal class DataManager : ModdedSaveGameIOBinary
    {
        private static string path;

        private static string extraSettingsFile = "extraSettings.dat";

        public const string keyAccess = "AdvancedEdition";

        private static LevelSaveData levelData = new LevelSaveData();

        private static LevelSaveData levelDataLoaded = new LevelSaveData();

        private static ExtraSettingsData extraSettings;

        public static LevelSaveData LevelDataLoaded => levelDataLoaded;

        public static LevelSaveData LevelData => levelData;

        public static ExtraSettingsData ExtraSettings => extraSettings;

        public override PluginInfo pluginInfo => AdvancedCore.Instance.Info;

        public static void initialize()
        {
            prepareFolder();
            load();
            ApiManager.onExtraSettingsLoadingPost?.Invoke();
        }

        private static void prepareFolder()
        {
            path = ModdedSaveSystem.GetSaveFolder(AdvancedCore.Instance, Singleton<PlayerFileManager>.Instance.fileName) + "/";
            Directory.CreateDirectory(path);
        }

        private static void load()
        {
            if (!File.Exists(path + extraSettingsFile))
            {
                extraSettings = new ExtraSettingsData();
                extraSettings.setDefaults();
                //Console.WriteLine("Extra settings data created!");
                saveMenu();
            } else
            {
                extraSettings = JsonUtility.FromJson<ExtraSettingsData>(RijndaelEncryption.Decrypt(File.ReadAllText(path + extraSettingsFile), keyAccess));
                //Console.WriteLine("Extra settings data loaded!");
            }
        }

        public static void saveMenu(float version = 1.01f)
        {
            if (extraSettings != null)
            {
                extraSettings.saveVersion = version;
                File.WriteAllText(path + extraSettingsFile, RijndaelEncryption.Encrypt(JsonUtility.ToJson(extraSettings), keyAccess));
                //Console.WriteLine("Extra settings saved!");
            }
        }

        public override void Save(BinaryWriter writer)
        {
            levelDataLoaded.save(writer);
            levelData.loadFrom(levelDataLoaded);
        }

        public override void Load(BinaryReader reader)
        {
            levelData.load(reader);
            levelDataLoaded.loadFrom(levelData);
        }

        public override void Reset()
        {
            levelData = new LevelSaveData();
            levelDataLoaded = new LevelSaveData();
        }

        public static void ResetAllData()
        {
            levelData = new LevelSaveData();
            levelDataLoaded = new LevelSaveData();
        }

        public static void setLastSave()
        {
            levelDataLoaded.loadFrom(levelData);
        }

        public static bool saveIsAvailable()
        {
            return ReflectionHelper.getValue<bool>(Singleton<CoreGameManager>.Instance, "saveEnabled");
        }
    }
}
