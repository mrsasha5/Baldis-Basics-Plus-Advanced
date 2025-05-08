using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Game;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Managers;
using BaldisBasicsPlusAdvanced.Menu;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BaldisBasicsPlusAdvanced.SerializableData;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
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

        public const string version = "0.2.3.6";

        internal static bool editorIntegrationEnabled;

        internal static bool extraIntegrationEnabled;

        internal static bool notificationsEnabled;

        private static AdvancedCore instance;

        public static AdvancedCore Instance => instance;

        internal static ManualLogSource Logging => Instance.Logger;

        private static Harmony harmony;

        private static IEnumerator CheckGamebananaVersion()
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(
                "https://api.gamebanana.com/Core/Item/Data?itemtype=Mod&itemid=504169&fields=Updates().aGetLatestUpdates()");
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                AdvancedCore.Logging.LogWarning("Unable to connect to the Gamebanana's API!");
                yield break;
            }
            string json = webRequest.downloadHandler.text;
            JToken gbResults = JToken.Parse(json); //parse as jtoken instead of JObject

            string version = gbResults[0][0]["_sVersion"].Value<string>();

            if (Instance.Info.Metadata.Version < new System.Version(version))
            {
                Singleton<NotificationManager>.Instance.Queue(
                $"Baldi's Basics Plus Advanced Edition:\nNewest {version} version is available on the Gamebanana now!",
                AssetsStorage.sounds["bal_wow"], time: 10f);
            }
        }

        private void Awake()
        {
            harmony = new Harmony(modId);

            instance = this;

            editorIntegrationEnabled = Config.Bind("Integration", "Editor", defaultValue: true, "If disabled, then items and other things from this mod will not load in the editor!").Value;
            extraIntegrationEnabled = Config.Bind("Integration", "Extra", defaultValue: true, "If disabled, then integration (Fun Settings and etc) will be disabled.").Value;
            notificationsEnabled = Config.Bind("Settings", "Notifications", defaultValue: true, "Disables/enables notifications.").Value;

            AssetLoader.LoadLocalizationFolder(AssetsHelper.modPath + "Language/English", Language.English);
            PrepareSettingsMenu();
            ModdedSaveGame.AddSaveHandler(LevelDataManager.Instance);
            GeneratorManagement.Register(this, GenerationModType.Addend, GenerationPatchingManager.RegisterMainLevelData);
            LoadingEvents.RegisterOnAssetsLoaded(Info, ModLoader(), false);
            LoadingEvents.RegisterOnAssetsLoaded(Info, OnAssetsLoadedPost(), true);

            GameRegisterManager.InitializeDoNotDestroyOnLoadObjects();
        }

        private static IEnumerator ModLoader()
        {
            IntegrationManager.Prepare();

            harmony.PatchAllConditionals(); //I can't check installed some mods, because priority is different and this is solution of problem

            IEnumerator assetsLoading = OnAssetsLoadedPre();
            bool move = true;
            while (move)
            {
                try
                {
                    move = assetsLoading.MoveNext();
                } catch (Exception e)
                {
                    ObjectsCreator.CauseCrash(e);
                    move = false;
                }
                yield return assetsLoading.Current;
            }
            yield break;
        }

        private static IEnumerator OnAssetsLoadedPost()
        {
            yield return 1;

            yield return "Loading Kitchen Stove recipes...";

            KitchenStove.LoadRecipesFromAssets();

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

            int count = 18; //+1 ?

            yield return count;
            yield return "Caching game assets...";
            AssetsStorage.Cache();
            if (AssetsStorage.exception != null) throw AssetsStorage.exception;
            Instance.StartCoroutine(CheckGamebananaVersion());
            GameRegisterManager.CreateDoorMats();
            yield return "Initializing vending machines...";
            GameRegisterManager.InitializeVendingMachines();
            yield return "Initializing UI...";
            GameRegisterManager.InitializeUI();
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
            GameRegisterManager.InitializeGameEvents();
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
            //+3 to count
            //for (int i = 0; i < 3; i++)
            //{
            //    yield return "Invoking changes for MainLevel" + (i + 1);
            //    RegisterMainLevelData(AssetsHelper.loadAsset<SceneObject>("MainLevel_" + (i + 1)), i);
            //}
            yield return "Overriding game prefabs...";
            AssetsStorage.OverrideAssetsProperties();
            yield return "Correcting patches...";
            GameRegisterManager.CorrectPatches();
            yield return "Initializing scene objects...";
            GameRegisterManager.InitializeSceneObjects();
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
