using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.SaveSystem.Data;
using MTM101BaldAPI.SaveSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityCipher;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SaveSystem
{
    internal class OptionsDataManager
    {
        private static string path;

        private static string extraSettingsFile = "extraSettings.dat";

        private static string keyBindingsFile = "bindings.txt";

        public const string keyAccess = "AdvancedEdition";

        private static Dictionary<string, KeyBindingData> keyBindings = new Dictionary<string, KeyBindingData>();

        private static ExtraSettingsData extraSettings;

        //public static LevelSaveData LevelData => levelDataLoaded;

        public static ExtraSettingsData ExtraSettings => extraSettings;

        public static Dictionary<string, KeyBindingData> KeyBindings => keyBindings;

        //1f
        //1.01f
        //2f
        //3f - array instead of many fields + a new way of serialization

        private const float extraSettingsVersion = 3f;

        public static void SetBindingDefaultValues()
        {
            PrepareKeyBinding("wind_blower_switch", "Adv_KeyBind_WindBlowerSwitch", "Adv_KeyBind_WindBlowerSwitch_Desc", KeyCode.B);
            PrepareKeyBinding("balloon_pop_action", "Adv_KeyBind_BalloonPopAction", "Adv_KeyBind_BalloonPopAction_Desc", KeyCode.P);
            PrepareKeyBinding("exit_from_trapdoor", "Adv_KeyBind_ExitFromTrapdoor", "Adv_KeyBind_ExitFromTrapdoor_Desc", KeyCode.Mouse0);
            /*for (int i = 0; i < 10; i++)
            {
                PrepareKeyBinding("test" + i, "TEST" + i, "Test?", KeyCode.T);
            }*/
        }

        public static void RewriteBindings()
        {
            List<KeyBindingData> datas = keyBindings.Values.ToList();
            string text = "";
            for (int i = 0; i < datas.Count; i++)
            {
                text += $"{datas[i].Id} {datas[i].Button}\n";
            }
            File.WriteAllText(path + keyBindingsFile, text);
        }

        public static void Load()
        {
            ApiManager.onExtraSettingsPreLoading?.Invoke();

            PrepareFolder();

            if (!File.Exists(path + extraSettingsFile))
            {
                extraSettings = new ExtraSettingsData();
                extraSettings.CheckValues();
                Save();
            }
            else
            {
                extraSettings = JsonConvert.DeserializeObject<ExtraSettingsData>(RijndaelEncryption.Decrypt(File.ReadAllText(path + extraSettingsFile), keyAccess));
                //extraSettings = JsonUtility.FromJson<ExtraSettingsData>(RijndaelEncryption.Decrypt(File.ReadAllText(path + extraSettingsFile), keyAccess));
                extraSettings.CheckValues();
                if (extraSettings.saveVersion != extraSettingsVersion) extraSettings.showNotif = true;
            }

            SetBindingDefaultValues();

            if (File.Exists(path + keyBindingsFile))
            {
                string[] lines = File.ReadAllLines(path + keyBindingsFile);
                foreach (string line in lines)
                {
                    //0 - id
                    //1 - button
                    string[] values = line.Split(new char[] { ' ' });
                    if (keyBindings.ContainsKey(values[0]))
                    {
                        keyBindings[values[0]].OverrideButton((KeyCode)Enum.Parse(typeof(KeyCode), values[1]));
                    }
                }
            }
            else
            {
                RewriteBindings();
            }

            ApiManager.onExtraSettingsPostLoading?.Invoke();
        }

        public static void Save()
        {
            if (extraSettings != null)
            {
                extraSettings.saveVersion = extraSettingsVersion;
                File.WriteAllText(path + extraSettingsFile, RijndaelEncryption.Encrypt(JsonConvert.SerializeObject(extraSettings), keyAccess));
                //File.WriteAllText(path + extraSettingsFile, RijndaelEncryption.Encrypt(JsonUtility.ToJson(extraSettings), keyAccess));
            }
        }

        private static void PrepareKeyBinding(string id, string locName, string locDesc, KeyCode key)
        {
            if (!KeyBindings.ContainsKey(id))
            {
                KeyBindingData data = new KeyBindingData(id, locName, locDesc, key);
                keyBindings.Add(id, data);
            }
            else
            {
                keyBindings[id].OverrideButton(key);
            }
        }

        private static void PrepareFolder()
        {
            path = ModdedSaveSystem.GetSaveFolder(AdvancedCore.Instance, Singleton<PlayerFileManager>.Instance.fileName) + "/";
            Directory.CreateDirectory(path);
        }
    }
}
