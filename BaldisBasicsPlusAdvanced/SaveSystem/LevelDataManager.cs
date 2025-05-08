using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem.Data;
using BepInEx;

namespace BaldisBasicsPlusAdvanced.SaveSystem
{
    internal class LevelDataManager : ModdedSaveGameIOJsonNet<LevelSaveData>
    {
        private static LevelDataManager instance;

        private static LevelSaveData levelData = new LevelSaveData();

        private static LevelSaveData levelDataLoaded = new LevelSaveData();

        public static LevelSaveData LevelData => levelDataLoaded;

        public override PluginInfo pluginInfo => AdvancedCore.Instance.Info;

        public static LevelDataManager Instance
        {
            get
            {
                if (instance == null) instance = new LevelDataManager();
                return instance;
            }
        }

        public override LevelSaveData GetObjectToSave()
        {
            levelDataLoaded.OnSave();
            levelData.LoadFrom(levelDataLoaded);
            return levelDataLoaded;
        }

        public override void OnObjectLoaded(LevelSaveData loadedObject)
        {
            levelData = loadedObject;
            levelDataLoaded.LoadFrom(levelData);
            levelDataLoaded.OnLoad();
        }

        public override void Reset()
        {
            levelData = new LevelSaveData();
            levelDataLoaded = new LevelSaveData();
        }

        public override void OnCGMCreated(CoreGameManager instance, bool isFromSavedGame)
        {
            if (isFromSavedGame)
            {
                SetLastSave();
            }
            else
            {
                ClearLevelDataLoaded();
            }
        }

        public static void ClearLevelDataLoaded()
        {
            levelDataLoaded = new LevelSaveData();
        }

        public static void SetLastSave()
        {
            levelDataLoaded.LoadFrom(levelData);
        }
    }
}
