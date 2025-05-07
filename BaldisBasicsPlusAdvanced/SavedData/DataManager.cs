using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Helpers;
using BepInEx;
using MTM101BaldAPI.SaveSystem;
using System;
using System.IO;
using UnityCipher;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SavedData
{
    internal class DataManager
    {
        private static string path;

        private static string extraSettingsFile = "extraSettings.dat";

        public const string keyAccess = "AdvancedEdition";

        private static ExtraSettingsData extraSettings;

        //public static LevelSaveData LevelData => levelDataLoaded;

        public static ExtraSettingsData ExtraSettings => extraSettings;

        //1f
        //1.01f
        //2f

        private const float extraSettingsVersion = 2f;

        public static void Initialize()
        {
            PrepareFolder();

            ApiManager.onExtraSettingPreLoading?.Invoke();
            Load();
            ApiManager.onExtraSettingsPostLoading?.Invoke();
        }

        private static void PrepareFolder()
        {
            path = ModdedSaveSystem.GetSaveFolder(AdvancedCore.Instance, Singleton<PlayerFileManager>.Instance.fileName) + "/";
            Directory.CreateDirectory(path);
        }

        private static void Load()
        {
            if (!File.Exists(path + extraSettingsFile))
            {
                extraSettings = new ExtraSettingsData();
                extraSettings.SetDefaults();
                SaveMenu();
            } else
            {
                extraSettings = JsonUtility.FromJson<ExtraSettingsData>(RijndaelEncryption.Decrypt(File.ReadAllText(path + extraSettingsFile), keyAccess));
                if (extraSettings.saveVersion != extraSettingsVersion) extraSettings.showNotif = true;
            }
        }

        public static void SaveMenu()
        {
            if (extraSettings != null)
            {
                extraSettings.saveVersion = extraSettingsVersion;
                File.WriteAllText(path + extraSettingsFile, RijndaelEncryption.Encrypt(JsonUtility.ToJson(extraSettings), keyAccess));
            }
        }

        

    }
}
