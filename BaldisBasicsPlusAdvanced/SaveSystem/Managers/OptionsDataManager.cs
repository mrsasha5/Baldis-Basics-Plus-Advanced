using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.SaveSystem.Data;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityCipher;

namespace BaldisBasicsPlusAdvanced.SaveSystem
{
    internal class OptionsDataManager
    {
        private static string extraSettingsFile = "extraSettings.dat";

        public const string keyAccess = "AdvancedEdition";

        private static ExtraSettingsData extraSettings;

        public static ExtraSettingsData ExtraSettings => extraSettings;

        //1f
        //1.01f
        //2f
        //3f - array instead of many fields + a new way of serialization

        private const float extraSettingsVersion = 3f;

        public static void Load()
        {
            ApiManager.onExtraSettingsPreLoading?.Invoke();

            if (!File.Exists(SaveSystemCore.Path + extraSettingsFile))
            {
                extraSettings = new ExtraSettingsData();
                extraSettings.CheckValues();
                Save();
            }
            else
            {
                try
                {
                    extraSettings = JsonConvert.DeserializeObject<ExtraSettingsData>
                        (RijndaelEncryption.Decrypt(File.ReadAllText(SaveSystemCore.Path + extraSettingsFile), keyAccess));
                    extraSettings.CheckValues();
                    if (extraSettings.saveVersion != extraSettingsVersion) extraSettings.showNotif = true;
                }
                catch
                {
                    AdvancedCore.Logging.LogError("OptionsDataManager cannot load options data! Creating new one...");

                    extraSettings = new ExtraSettingsData();
                    extraSettings.CheckValues();
                    Save();
                }
                
            }

            ApiManager.onExtraSettingsPostLoading?.Invoke();
        }

        public static void Save()
        {
            if (extraSettings != null)
            {
                extraSettings.saveVersion = extraSettingsVersion;
                File.WriteAllText(SaveSystemCore.Path + extraSettingsFile, RijndaelEncryption.Encrypt(JsonConvert.SerializeObject(extraSettings), keyAccess));
            }
        }
    }
}
