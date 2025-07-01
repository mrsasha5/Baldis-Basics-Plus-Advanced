using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.AutoUpdate;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Managers;
using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace BaldisBasicsPlusAdvanced
{
    [BepInPlugin(modId, modName, version)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", DependencyFlags.HardDependency)]
    [BepInDependency(IntegrationManager.levelLoaderId, DependencyFlags.HardDependency)]
    public class AdvancedCore : BaseUnityPlugin
    {
        public const string modId = "mrsasha5.baldi.basics.plus.advanced";

        public const string modName = "Baldi's Basics Plus Advanced Edition";

        public const string version = "0.2.5.2";

        internal static string tempPath;

        internal static bool editorIntegrationEnabled;

        internal static bool spatialElevatorIntegrationEnabled;

        //internal static float updateCheckIntervalTime;

        internal static bool updateChecksEnabled;

        internal static bool notificationsEnabled;

        internal static bool preparedToInstalling;

        private static AdvancedCore instance;

        public static AdvancedCore Instance => instance;

        public static string GameVersion => Application.version;

        internal static ManualLogSource Logging => Instance.Logger;

        private static Harmony harmony;

        private void OnApplicationQuit()
        {
            if (AutoUpdateManager.thread != null) AutoUpdateManager.thread.Abort();

            //I think in the future I'll make something like bat files and etc if they will be working like I want
            if (preparedToInstalling)
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo();
                process.StartInfo.WorkingDirectory = tempPath;
                process.StartInfo.FileName = "AutoUpdater.exe";
                process.StartInfo.Arguments = System.AppDomain.CurrentDomain.Id.ToString();
                process.Start();
            }
        }

        private void Awake()
        {
            harmony = new Harmony(modId);

            instance = this;

            tempPath = Application.persistentDataPath + "/Modded/" + modId + "/TEMP/";
            if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);

            editorIntegrationEnabled = Config.Bind("Integration", "Editor", defaultValue: true,
                "If disabled, then items and other things from this mod will not load in the editor!").Value;
            spatialElevatorIntegrationEnabled = Config.Bind("Integration", "Spatial Elevator", defaultValue: true,
                "If disabled, then integration with 3D Elevator will be disabled! " +
                "Not recommended to disable since you'll lose access to the content like this: tips & Hammer of Force.").Value;

            notificationsEnabled = Config.Bind("Settings", "Notifications", defaultValue: true, 
                "Disables/enables notifications.").Value;
            
            updateChecksEnabled = 
                Config.Bind("Updates", "Check Updates", defaultValue: true, 
                "Checks releases from Github repo and install them if you give permission for this!").Value;

            //updateCheckIntervalTime =
            //    Config.Bind("Updates", "Interval Time", defaultValue: 600f,
            //    "How many seconds should go until mod will check releases again. " +
            //    "If you want to turn off these cyclic checks, then set value less than 0. For example: -1").Value;

            AssetLoader.LoadLocalizationFolder(AssetLoader.GetModPath(this) + "/Language/English", Language.English);
            PrepareSettingsMenu();
            ModdedSaveGame.AddSaveHandler(LevelDataManager.Instance);
            GeneratorManagement.Register(this, GenerationModType.Addend, GenerationPatchingManager.RegisterMainLevelData);
            LoadingEvents.RegisterOnAssetsLoaded(Info, ModLoader(), false);
            LoadingEvents.RegisterOnAssetsLoaded(Info, OnAssetsLoadedPost(), true);

            /*MTM101BaldiDevAPI.AddWarningScreen(
                "<color=#FF0000>Advanced Edition BETA BUILD\n</color>" +
                "Remember about main conditions for the beta testers. " +
                "You must observe them until they are declared obsolete by me.\n" +
                "<color=#FFFF00>The main points you should remember are:\n" +
                " * Do not leak build & assets\n" +
                "<color=#00FF00> * You are allowed to show a new content</color></color>",
                //" * Do not disclose information about new/planned content in any way to non-beta testers</color>", 
                false);
            MTM101BaldiDevAPI.AddWarningScreen(
                "<color=#FF0000>Advanced Edition BETA BUILD\n</color>" +
                "If this build was leaked without permission and you have installed it... " +
                "Please note that as a NON-BETA TESTER YOU WILL NOT RECEIVE FEEDBACK IN CASE OF A BROKEN GAME. " +
                "You can close game until it will be launched fully.",
                false);*/

            GameRegisterManager.InitializeDoNotDestroyOnLoadObjects();
        }

        private static IEnumerator ModLoader()
        {
            IntegrationManager.Prepare();            

            harmony.PatchAllConditionals();

            IEnumerator assetsLoading = OnAssetsLoadedPre();
            bool move = true;
            while (move)
            {
                try
                {
                    move = assetsLoading.MoveNext();
                } catch (Exception e)
                {
                    Logging.LogWarning($"Exception occured on state: {assetsLoading.Current.ToString()}");
                    ObjectsCreator.CauseCrash(e);
                    
                    move = false;
                }
                yield return assetsLoading.Current;
            }
            yield break;
        }

        private static IEnumerator OnAssetsLoadedPost()
        {
            yield return 3;

            yield return "Loading Kitchen Stove recipes...";

            KitchenStove.LoadRecipesFromAssets();

            yield return "Initializing new MIDIs...";

            GameRegisterManager.InitializeMidisPost();

            yield return "Initializing Kitchen Stove posters...";
            GameRegisterManager.InitializeKitchenStovePosters();

            if (ApiManager.onAssetsPostLoading != null)
            {
                ApiManager.onAssetsPostLoading.Invoke();
                ApiManager.onAssetsPostLoading = null;
            }

            GC.Collect();
        }

        private static IEnumerator OnAssetsLoadedPre()
        {
            if (ApiManager.onAssetsPreLoading != null)
            {
                ApiManager.onAssetsPreLoading.Invoke();
                ApiManager.onAssetsPreLoading = null;
            }

            int count = 20;

            yield return count;
            yield return "Caching game assets...";
            AssetsManagerCore.Initialize();
            GameRegisterManager.CreateDoorMats();
            yield return "Initializing vending machines...";
            GameRegisterManager.InitializeVendingMachines();
            yield return "Initializing UI...";
            GameRegisterManager.InitializeUi();
            yield return "Initializing entities...";
            GameRegisterManager.InitializeEntities();
            yield return "Initializing items...";
            GameRegisterManager.InitializeGameItems();
            GameRegisterManager.InitializeMultipleUsableItems();
            yield return "Initializing objects...";
            GameRegisterManager.InitializeObjects();
            yield return "Initializing NPCs...";
            GameRegisterManager.InitializeNPCs();
            yield return "Initializing room basics...";
            GameRegisterManager.InitializeRoomBasics();
            yield return "Loading extensions for the Level Loader...";
            LevelLoaderIntegration.Initialize();
            yield return "Initializing events...";
            GameRegisterManager.InitializeRandomEvents();
            yield return "Initializing builders...";
            GameRegisterManager.InitializeObjectBuilders();
            yield return "Initializing posters...";
            GameRegisterManager.InitializePosters();
            yield return "Initializing other things...";
            GameRegisterManager.InitializeApiThings();
            yield return "Adding some tags...";
            GameRegisterManager.SetTags();
            yield return "Initializing room assets...";
            GameRegisterManager.InitializeRoomAssets();
            yield return "Overriding game prefabs...";
            AssetsStorage.OverrideAssetsProperties();
            yield return "Correcting patches...";
            GameRegisterManager.CorrectPatches();
            yield return "Initializing scene objects...";
            GameRegisterManager.InitializeSceneObjects();
            yield return "Initializing trips...";
            GameRegisterManager.InitializeTrips();
            yield return "Loading new MIDIs...";
            GameRegisterManager.InitializeMidis();
            yield return "Integrating with other mods...";
            IntegrationManager.Initialize();

            GC.Collect();

            yield break;
        }

        private void PrepareSettingsMenu()
        {
            CustomOptionsCore.OnMenuInitialize += delegate (OptionsMenu menu, CustomOptionsHandler handler)
            {
                handler.AddCategory<ExtraOptionsMenu>("Adv_Options_Menu_Extra_Settings");
                handler.AddCategory<EmergencyOptionsMenu>("Adv_Options_Menu_Emergency_Options");
                handler.AddCategory<KeyBindingsOptionsMenu>("Adv_Options_Menu_Key_Bindings_Settings");
            };
        }

    }
}
