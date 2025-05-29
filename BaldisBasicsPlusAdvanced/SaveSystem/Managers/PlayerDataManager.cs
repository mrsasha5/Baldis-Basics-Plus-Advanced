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

        private static string file = "player.dat";

        private static PlayerSaveData data;

        public static PlayerSaveData Data => data;

        public static void Load()
        {
            if (!File.Exists(SaveSystemCore.Path + file))
            {
                data = new PlayerSaveData();
                Save();
            }
            else
            {
                data = JsonConvert.DeserializeObject<PlayerSaveData>(
                    RijndaelEncryption.Decrypt(File.ReadAllText(SaveSystemCore.Path + file), 
                    SaveSystemCore.FileName));
            }
        }

        public static void Save()
        {
            if (data != null)
            {
                data.version = version;
                File.WriteAllText(SaveSystemCore.Path + file, RijndaelEncryption.Encrypt(JsonConvert.SerializeObject(data),
                    SaveSystemCore.FileName));
            }
        }

    }
}
