using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.LevelLoadingSystem;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio;
using BaldisBasicsPlusAdvanced.Game.Activities;
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
using System.Globalization;
using System.IO;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace BaldisBasicsPlusAdvanced
{
    [BepInPlugin(modId, modName, version)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", DependencyFlags.HardDependency)]
    [BepInDependency(IntegrationManager.levelLoaderId, DependencyFlags.HardDependency)]
    [BepInDependency(LevelStudioIntegration.GUID, DependencyFlags.SoftDependency)]
    public class AdvancedCore : BaseUnityPlugin
    {
        public const string modId = "mrsasha5.baldi.basics.plus.advanced";

        public const string modName = "BB+ Advanced Edition";

        public const string version = "0.3.1.1";

        private static AdvancedCore instance;

        public static AdvancedCore Instance => instance;

        internal static ManualLogSource Logging => Instance.Logger;

        internal static CultureInfo StandardCultureInfo => CultureInfo.GetCultureInfo("eu");

        private static Harmony harmony;

        private void Awake()
        {
            harmony = new Harmony(modId);

            instance = this;

            ConfigManager.Initialize();

            PrepareSettingsMenu();
            ModdedSaveGame.AddSaveHandler(LevelDataManager.Instance);
            GeneratorManagement.Register(this, GenerationModType.Addend, GenerationPatchingManager.RegisterMainLevelData);
            LoadingEvents.RegisterOnAssetsLoaded(Info, ModLoader(), LoadingEventOrder.Pre);
            LoadingEvents.RegisterOnAssetsLoaded(Info, ModPostLoader(), LoadingEventOrder.Post);

            if (Application.version == "0.12" || Application.version == "0.12.1")
            {
                MTM101BaldiDevAPI.AddWarningScreen(
                    "Modification is not compatible with versions lower than 0.12.2 due of the updated Unity Engine. " +
                    "Also it's highly recommended to use 0.12.2 because old versions" +
                    " contain critical security issue.",
                        fatal: true);
            }

#if BETA
            MTM101BaldiDevAPI.AddWarningScreen(
                "<color=#FF0000>Advanced Edition BETA BUILD\n</color>" +
                "Remember about main conditions for the beta testers. " +
                "You must observe them until they are declared obsolete by me.\n" +
                "<color=#FFFF00>The main points you should remember are:\n" +
                " * Do not leak build & assets\n" +
                //"<color=#FF0000> * You cannot show a new content</color></color>",
                "<color=#00FF00> * You are allowed to show a new content</color></color>",
                //" * Do not disclose information about new/planned content in any way to non-beta testers</color>", 
                false);

            MTM101BaldiDevAPI.AddWarningScreen(
                "<color=#FF0000>Advanced Edition BETA BUILD\n</color>" +
                "If you are not a beta tester... " +
                "Please note that as a NON-BETA TESTER YOU WILL NOT RECEIVE FEEDBACK IN CASE OF A BROKEN GAME. " +
                "You can close game until it will be launched fully.",
                false);
#endif

            GameRegisterManager.InitializeDoNotDestroyOnLoadObjects();

            AssetLoader.LoadLocalizationFolder(AssetLoader.GetModPath(this) + "/Language/English", Language.English);
            AssetLoader.LoadLocalizationFolder(AssetLoader.GetModPath(this) + "/Language/English/Compats", Language.English);
        }

        private static IEnumerator ModPostLoader()
        {
            IEnumerator assetsLoading = OnAssetsPostLoad();
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

        private IEnumerator ModLoader()
        {
            if (!Directory.Exists(AssetsHelper.modPath))
            {
                ObjectsCreator.CauseCrash(new Exception("Assets folder is missing!"));
            }

            AssetsManagerCore.PreInitialize();

            IEnumerator assetsLoading = OnAssetsPreLoad();
            bool move = true;
            while (move)
            {
                try
                {
                    move = assetsLoading.MoveNext();
                }
                catch (Exception e)
                {
                    if (assetsLoading.Current != null)
                    {
                        Logging.LogWarning($"Exception occured on state: {assetsLoading.Current.ToString()}");   
                    } else Logging.LogWarning($"Exception occured on null state!");

                    ObjectsCreator.CauseCrash(e);

                    //move = false;
                }

                yield return assetsLoading.Current;
            }
        }

        private static IEnumerator OnAssetsPostLoad()
        {
            yield return 3;

            yield return "Initializing new MIDIs...";

            GameRegisterManager.PostInitializeMidis();

            yield return "Initializing posters...";

            GameRegisterManager.PostInitializePosters();

            yield return "Invoking OnAssetsLoadPost for modules...";

            IntegrationManager.InvokeOnAssetsLoadPost();

            if (ApiManager.onAssetsPostLoading != null)
            {
                ApiManager.onAssetsPostLoading.Invoke();
                ApiManager.onAssetsPostLoading = null;
            }

            GC.Collect();
        }

        private IEnumerator OnAssetsPreLoad()
        {
            if (ApiManager.onAssetsPreLoading != null)
            {
                ApiManager.onAssetsPreLoading.Invoke();
                ApiManager.onAssetsPreLoading = null;
            }

            NotificationManager.Notification notif = CheckAssetsMarker();

            int count = 25;

            if (notif != null) count++;

            yield return count;

            if (notif != null)
            {
                yield return "Waiting until you'll read...";
                while (!InputManager.Instance.AnyButton(onDown: true)) yield return 0;
                notif.time = 1f;
            }

            yield return "Preparing Integration Manager...";
            IntegrationManager.Prepare();
            yield return "Patching game...";
            harmony.PatchAllConditionals();

            yield return "Caching game assets...";
            AssetsManagerCore.Initialize();
            GameRegisterManager.CreateDoorMats();
            yield return "Initializing textures for cells...";
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
            yield return "Connecting to Level Loading System...";
            LevelLoaderIntegration.Initialize();
            yield return "Initiailizing room assets in prefabs...";
            GameRegisterManager.InitializeRoomAssetsInPrefabs();
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
            yield return "Initializing integration modules...";
            IntegrationManager.Initialize();

            GC.Collect();

            yield break;
        }

        private void PrepareSettingsMenu()
        {
            CustomOptionsCore.OnMenuInitialize += delegate (OptionsMenu menu, CustomOptionsHandler handler)
            {
                handler.AddCategory<ExtraOptionsMenu>("Adv_Options_Menu_Extra_Settings");
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
