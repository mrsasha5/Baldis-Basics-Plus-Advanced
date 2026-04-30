using System.IO;
using BaldisBasicsPlusAdvanced.SaveSystem.Managers;
using MTM101BaldAPI.SaveSystem;

namespace BaldisBasicsPlusAdvanced.SaveSystem
{
    public class SaveSystemCore
    {
        public static string Path
        {
            get
            {
                string _path = ModdedSaveSystem.GetSaveFolder(AdvancedCore.Instance, FileName) + "/";
                Directory.CreateDirectory(_path);
                return _path;
            }
        }

        public static string FileName => PlayerFileManager.Instance.fileName;

        public static void Load()
        {
            // Options
            ExtraSettingsManager.Load();
            KeyBindingsManager.Load();
            // Player data
            PlayerDataManager.Load();
        }

        public static void Save()
        {
            // Only player since options themselves manage saving
            PlayerDataManager.Save();
        }
    }
}
