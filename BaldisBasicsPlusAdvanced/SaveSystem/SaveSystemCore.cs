using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BaldisBasicsPlusAdvanced.AutoUpdate;
using BaldisBasicsPlusAdvanced.SaveSystem.Managers;
using MTM101BaldAPI.SaveSystem;

namespace BaldisBasicsPlusAdvanced.SaveSystem
{
    public class SaveSystemCore
    {

        private static string path;

        public static string Path => path;

        public static string FileName => Singleton<PlayerFileManager>.Instance.fileName;

        public static void Load()
        {
            PrepareFolder();

            OptionsDataManager.Load();
            PlayerDataManager.Load();
            KeyBindingsManager.Load();

            //AutoUpdateManager.Instance?.Check();
        }

        public static void Save()
        {
            PlayerDataManager.Save();
        }

        private static void PrepareFolder()
        {
            path = ModdedSaveSystem.GetSaveFolder(AdvancedCore.Instance, Singleton<PlayerFileManager>.Instance.fileName) + "/";
            Directory.CreateDirectory(path);
        }

    }
}
