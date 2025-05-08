using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.SaveSystem.Data;
using MTM101BaldAPI.SaveSystem;
using Newtonsoft.Json;
using System.IO;
using UnityCipher;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SaveSystem
{
    
    public class PlayerDataManager
    {
        //1f
        private static float version = 1f;

        private static string path;

        private static string file = "player.dat";

        private static PlayerSaveData data;

        public static PlayerSaveData Data => data;

        public static void Load()
        {
            PrepareFolder();
            if (!File.Exists(path + file))
            {
                data = new PlayerSaveData();
                Save();
            }
            else
            {
                data = JsonConvert.DeserializeObject<PlayerSaveData>(RijndaelEncryption.Decrypt(File.ReadAllText(path + file), 
                    Singleton<PlayerFileManager>.Instance.fileName));
                //if (data.version != version) 
            }
        }

        public static void Save()
        {
            PrepareFolder();
            if (data != null)
            {
                data.version = version;
                File.WriteAllText(path + file, RijndaelEncryption.Encrypt(JsonConvert.SerializeObject(data), Singleton<PlayerFileManager>.Instance.fileName));
            }
        }

        private static void PrepareFolder()
        {
            path = ModdedSaveSystem.GetSaveFolder(AdvancedCore.Instance, Singleton<PlayerFileManager>.Instance.fileName) + "/";
            Directory.CreateDirectory(path);
        }

    }
}
