using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.LevelLoadingSystem;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
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
using System.IO;
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

        public const string version = "0.2.6";

        internal static string tempPath;

        internal static bool notificationsEnabled;

        private static AdvancedCore instance;

        public static AdvancedCore Instance => instance;

        public static string GameVersion => Application.version;

        internal static ManualLogSource Logging => Instance.Logger;

        private static Harmony harmony;


        private void Awake()
        {
            harmony = new Harmony(modId);

            instance = this;

            tempPath = Application.persistentDataPath + "/Modded/" + modId + "/TEMP/";
            if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);

            notificationsEnabled = Config.Bind("Settings", "Notifications", defaultValue: true, 
                "Disables/enables notifications.").Value;

            PrepareSettingsMenu();
            ModdedSaveGame.AddSaveHandler(LevelDataManager.Instance);
            GeneratorManagement.Register(this, GenerationModType.Addend, GenerationPatchingManager.RegisterMainLevelData);
            LoadingEvents.RegisterOnAssetsLoaded(Info, ModLoader(), false);
            LoadingEvents.RegisterOnAssetsLoaded(Info, ModLoaderPost(), true);
            AssetLoader.LoadLocalizationFolder(AssetLoader.GetModPath(this) + "/Language/English", Language.English);

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

        private static IEnumerator ModLoaderPost()
        {
            IEnumerator assetsLoading = OnAssetsLoadedPost();
            bool move = true;
            while (move)
            {
                try
                {
                    move = assetsLoading.MoveNext();
                }
                catch (Exception e)
                {
                    Logging.LogWarning($"Exception occured on state: {assetsLoading.Current.ToString()}");
                    ObjectsCreator.CauseCrash(e);
                }
                yield return assetsLoading.Current;
            }
        }

        private static IEnumerator ModLoader()
        {
            if (!Directory.Exists(AssetsHelper.modPath))
            {
                ObjectsCreator.CauseCrash(new Exception("Mod assets folder is missing!"));
            }

            IntegrationManager.Prepare();            

            harmony.PatchAllConditionals();

            IEnumerator assetsLoading = OnAssetsLoadedPre();
            bool move = true;
            while (move)
            {
                try
                {
                    move = assetsLoading.MoveNext();
                }
                catch (Exception e)
                {
                    Logging.LogWarning($"Exception occured on state: {assetsLoading.Current.ToString()}");
                    ObjectsCreator.CauseCrash(e);
                    move = false;
                }

                yield return assetsLoading.Current;
            }
        }

        private static IEnumerator OnAssetsLoadedPost()
        {
            yield return 4;

            yield return "Loading Kitchen Stove recipes...";

            ApiManager.LoadKitchenStoveRecipesFromFolder(Instance.Info, 
                AssetsHelper.modPath + "Premades/Recipes/KitchenStove/", true);

            yield return "Initializing new MIDIs...";

            GameRegisterManager.InitializeMidisPost();

            yield return "Initializing Kitchen Stove posters...";
            GameRegisterManager.InitializeKitchenStovePosters();

            yield return "Invoking OnAssetsLoadPost for modules...";

            IntegrationManager.InvokeOnAssetsLoadPost();

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

            NotificationManager.Notification notif = CheckAssetsMarker();

            int count = 21;

            if (notif != null) count++;

            yield return count;

            if (notif != null)
            {
                yield return "Waiting until you'll read...";
                while (!InputManager.Instance.AnyButton(onDown: true)) yield return 0;
                notif.time = 1f;
            }

            yield return "Caching game assets...";
            AssetsManagerCore.Initialize();
            if (AssetsStorage.exception != null) throw AssetsStorage.exception;

            GameRegisterManager.CreateDoorMats();
            yield return "Initializing and setting new cell's textures...";
            GameRegisterManager.InitializeCellTextures();
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

        private static NotificationManager.Notification CheckAssetsMarker()
        {
            NotificationManager.Notification notif = null;

            string[] versionMarkers =
                Directory.GetFiles(AssetsHelper.modPath, "*.ver", SearchOption.TopDirectoryOnly);

            if (versionMarkers.Length > 1)
                notif =
                    NotificationManager.Instance.Queue("Adv_Notif_AssetsWarning", AssetsStorage.sounds["buzz_elv"], time: 0f);
            else if (versionMarkers.Length == 1)
            {
                if (Path.GetFileNameWithoutExtension(versionMarkers[0]) != AdvancedCore.version)
                {
                    notif =
                        NotificationManager.Instance.Queue("Adv_Notif_IncorrectAssetsWarning", AssetsStorage.sounds["buzz_elv"], time: 0f);
                }
            }
            else
            {
                notif =
                    NotificationManager.Instance.Queue("Adv_Notif_UnknownAssetsVersion", AssetsStorage.sounds["buzz_elv"], time: 0f);
            }
            return notif;
        }

    }
}
