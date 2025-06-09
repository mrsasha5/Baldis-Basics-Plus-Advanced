using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using BaldisBasicsPlusAdvanced.Helpers;
using System.Linq;
using AlmostEngine;
using System;
using System.Collections.Generic;
using MTM101BaldAPI.Reflection;
using BaldisBasicsPlusAdvanced.Patches.FieldTrips;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Managers;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips
{
    public class FieldTripsLoader
    {
        public static BaseGameManager PrevGameMan { get; private set; }

        public static DijkstraMap PrevDijkstra { get; private set; }

        public static Cubemap PrevSkyboxCubemap { get; private set; }

        public static Material PrevSkybox { get; private set; }

        public static Color PrevSkyboxColor { get; private set; }

        public static Vector3[] PlayerPrevPositions { get; private set; }

        public static Quaternion[] PlayerPrevRotations { get; private set; }

        //public static List<MovementModifier>[] MoveMods { get; private set; } //Todo: implement that

        public static List<Entity> PrevActiveEntities { get; private set; }

        public static float PrevTimeScale { get; private set; }

        public static string PrevMidi { get; private set; }

        public static double PrevMidiPos { get; private set; }

        public static float PrevMidiSpeed { get; private set; }

        public static EnvironmentController PrevEc { get; private set; }

        public static SceneObject PrevSceneObject { get; private set; }

        public static Action onGameLoadedBack;

        public static Scene PrevScene { get; private set; }

        private static Canvas fieldTripLoadingScreen;

        private static Scene fieldTripScene;

        //FieldTripLoad

        public static void LoadFieldTrip(FieldTripData data)
        {
            Singleton<CoreGameManager>.Instance.StartCoroutine(FieldTripLoader(data));
        }

        private static IEnumerator FieldTripLoader(FieldTripData data)
        {
            fieldTripLoadingScreen = UnityEngine.Object.Instantiate(
                AssetsHelper.LoadAsset<Canvas>("FieldTripLoad"));
            Singleton<GlobalCam>.Instance.FadeIn(UiTransition.Dither, 0.01666667f);

            PrevTimeScale = Time.timeScale;
            PrevDijkstra = GameCamera.dijkstraMap;
            PrevEc = Singleton<BaseGameManager>.Instance.Ec;
            PrevGameMan = Singleton<BaseGameManager>.Instance;
            PrevSkybox = RenderSettings.skybox;
            PrevSkyboxCubemap = (Cubemap)Shader.GetGlobalTexture("_Skybox");
            PrevSkyboxColor = Shader.GetGlobalColor("_SkyboxColor");
            PrevScene = SceneManager.GetActiveScene();
            PrevActiveEntities = new List<Entity>();

            Time.timeScale = 0f;
            AudioListener.pause = true;
            PropagatedAudioManager.paused = true;

            while (Singleton<GlobalCam>.Instance.TransitionActive)
            {
                yield return null;
            }

            //allEntities
            foreach (Entity entity in UnityEngine.Object.FindObjectsOfType<Entity>())
            {
                if (!entity.CompareTag("Player"))
                {
                    entity.SetActive(false);
                    PrevActiveEntities.Add(entity);
                }
            }

            PrevEc.PauseEnvironment(true);
            PrevEc.PauseEvents(true);
            PrevEc.Active = false;
            PrevEc.gameObject.SetActive(false);

            Singleton<CoreGameManager>.Instance.disablePause = true;
            Singleton<CoreGameManager>.Instance.readyToStart = false;

            Singleton<BaseGameManager>.Instance.gameObject.SetActive(false);

            if (Singleton<MusicManager>.Instance.MidiPlaying)
            {
                PrevMidi = Singleton<MusicManager>.Instance.MidiPlayer.MPTK_MidiName;
                PrevMidiPos = Singleton<MusicManager>.Instance.MidiPlayer.MPTK_Position;
                PrevMidiSpeed = Singleton<MusicManager>.Instance.MidiPlayer.MPTK_Speed;
                Singleton<MusicManager>.Instance.StopMidi();
            }

            PrevSceneObject = Singleton<CoreGameManager>.Instance.sceneObject;
            Singleton<CoreGameManager>.Instance.sceneObject = data.sceneObject;

            ReflectionHelper.Static_SetValue<Singleton<BaseGameManager>>("m_Instance", null); //to singleton wouldn't wanted to destroy GameMan

            PlayerPrevPositions = new Vector3[Singleton<CoreGameManager>.Instance.setPlayers];
            PlayerPrevRotations = new Quaternion[Singleton<CoreGameManager>.Instance.setPlayers];
            for (int i = 0; i < Singleton<CoreGameManager>.Instance.setPlayers; i++)
            {
                PlayerPrevPositions[i] = Singleton<CoreGameManager>.Instance.GetPlayer(i).transform.position;
                PlayerPrevRotations[i] = Singleton<CoreGameManager>.Instance.GetPlayer(i).transform.localRotation;
            }

            foreach (SwingDoor door in UnityEngine.Object.FindObjectsOfType<SwingDoor>(true))
            {
                if (door.open) door.Shut();
            }

            foreach (StandardDoor door in UnityEngine.Object.FindObjectsOfType<StandardDoor>(true))
            {
                if (door.open) door.Shut();
            }

            fieldTripScene = SceneManager.CreateScene(data.sceneName);
            SceneManager.SetActiveScene(fieldTripScene);

            RenderSettings.skybox = data.skyboxMaterial;

            GameObject gameObject = new GameObject("GameInitializer");
            gameObject.SetActive(false);

            GameInitializer gameInitializer = gameObject.AddComponent<GameInitializer>();

            //LevelGenerator
            //LevelLoader

            ReflectionHelper.SetValue(gameInitializer, "ecPre",
                AssetsHelper.LoadAsset<EnvironmentController>("Environment Controller"));
            ReflectionHelper.SetValue(gameInitializer, "generatorPre",
                AssetsHelper.LoadAsset<LevelGenerator>("LevelGenerator"));
            ReflectionHelper.SetValue(gameInitializer, "loaderPre",
                AssetsHelper.LoadAsset<LevelLoader>("LevelLoader"));
            ReflectionHelper.SetValue(gameInitializer, "sceneObject", data.sceneObject);

            gameObject.SetActive(true);
            //gameInitializer.StartCoroutine(WaitForTripReady());
        }

        public static void OnTripReady()
        {
            Singleton<CoreGameManager>.Instance.disablePause = false;
            Singleton<CoreGameManager>.Instance.readyToStart = true;
            Singleton<GlobalCam>.Instance.Transition(UiTransition.Dither, 0.01666667f);
            UnityEngine.Object.Destroy(fieldTripLoadingScreen.gameObject);
            UnityEngine.Object.FindObjectOfType<BaseFieldTripManager>().OnLoadingScreenDestroying();
        }

        public static void CloseFieldTrip(bool showFieldTripScreen)
        {
            Singleton<CoreGameManager>.Instance.StartCoroutine(CloseTrip(showFieldTripScreen));
        }

        private static IEnumerator CloseTrip(bool showFieldTripScreen)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            PropagatedAudioManager.paused = true;
            Singleton<CoreGameManager>.Instance.disablePause = true;

            if (showFieldTripScreen)
            {
                fieldTripLoadingScreen = UnityEngine.Object.Instantiate(
                AssetsHelper.LoadAsset<Canvas>("FieldTripLoad"));
                SceneManager.MoveGameObjectToScene(fieldTripLoadingScreen.gameObject, PrevScene);
            }

            AsyncOperation unloader = SceneManager.UnloadSceneAsync(fieldTripScene);

            while (!unloader.isDone)
            {
                yield return null;
            }
            
            if (showFieldTripScreen)
                UnityEngine.Object.Destroy(fieldTripLoadingScreen.gameObject);

            Singleton<CoreGameManager>.Instance.sceneObject = PrevSceneObject;

            RenderSettings.skybox = PrevSkybox;
            Shader.SetGlobalTexture("_Skybox", PrevSkyboxCubemap);
            Shader.SetGlobalColor("_SkyboxColor", PrevSkyboxColor);

            ReflectionHelper.Static_SetValue<Singleton<BaseGameManager>>("m_Instance", PrevGameMan); //to singleton wouldn't wanted to destroy GameMan

            PrevGameMan.gameObject.SetActive(true);
            
            Singleton<CoreGameManager>.Instance.tripPlayed = true;

            Singleton<CoreGameManager>.Instance.disablePause = false;

            Time.timeScale = PrevTimeScale;
            GameCamera.dijkstraMap = PrevDijkstra;

            if (GameCamera.dijkstraMap.UpdateIsNeeded())
            {
                GameCamera.dijkstraMap.QueueUpdate();
            }

            //prevEc = Singleton<BaseGameManager>.Instance.Ec;
            //prevGameMan = Singleton<BaseGameManager>.Instance;

            PrevEc.PauseEnvironment(false);
            PrevEc.PauseEvents(false);
            PrevEc.Active = true;
            PrevEc.gameObject.SetActive(true);

            for (int i = 0; i < Singleton<CoreGameManager>.Instance.setPlayers; i++)
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(i).plm.Entity.Teleport(PlayerPrevPositions[i]);
                Singleton<CoreGameManager>.Instance.GetPlayer(i).transform.localRotation = PlayerPrevRotations[i];
                Singleton<CoreGameManager>.Instance.GetPlayer(i).ec = PrevEc;
                Singleton<CoreGameManager>.Instance.GetPlayer(i).plm.Entity.Initialize(
                    PrevEc, PlayerPrevPositions[i]);
                //Singleton<CoreGameManager>.Instance.GetPlayer(i).itm.Disable(false);
                Singleton<CoreGameManager>.Instance.GetCamera(i).SetControllable(true);
                Singleton<CoreGameManager>.Instance.GetCamera(i).matchTargetRotation = true;
                Singleton<CoreGameManager>.Instance.GetCamera(i).mapCam = PrevEc.map.cams[0];
            }

            foreach (Entity entity in PrevActiveEntities)
            {
                if (!entity.CompareTag("Player"))
                {
                    entity.SetActive(true);
                    entity.Initialize(PrevEc, entity.transform.position);
                }    
            }

            if (PrevEc.elevators.Count > 0)
            {
                for (int j = 0; j < PrevEc.elevators.Count; j++)
                {
                    if (PrevEc.elevators[j].IsSpawn)
                    {
                        PrevGameMan.StartCoroutine("WaitToExitSpawn", PrevEc.elevators[j].ColliderGroup);
                        break;
                    }
                }
            }

            Singleton<CoreGameManager>.Instance.ResetCameras();
            Singleton<CoreGameManager>.Instance.ResetShaders();
            Singleton<CoreGameManager>.Instance.readyToStart = true;
            Singleton<BaseGameManager>.Instance.CollectNotebooks(0);
            foreach (Cell cell in PrevEc.AllCells())
            {
                PrevEc.UpdateLightingAtCell(cell);
            }

            Singleton<MusicManager>.Instance.StopMidi();
            if (!string.IsNullOrEmpty(PrevMidi))
            {
                Singleton<MusicManager>.Instance.PlayMidi(PrevMidi, true);
                Singleton<MusicManager>.Instance.MidiPlayer.MPTK_Position = PrevMidiPos;
                Singleton<MusicManager>.Instance.SetSpeed(PrevMidiSpeed);
            }

            Time.timeScale = PrevTimeScale;
            AudioListener.pause = false;
            PropagatedAudioManager.paused = false;

            FieldTripEntranceRoomFunctionPatch.Instance.OnTripPlayed();

            onGameLoadedBack?.Invoke();
            onGameLoadedBack = null;

            ResetAllValues();
            GC.Collect();
        }

        private static void ResetAllValues()
        {
            PrevDijkstra = null;
            PrevGameMan = null;
            PrevEc = null;
            PrevActiveEntities = null;
            PrevMidi = null;
            PrevMidiPos = 0;
            PrevScene = default;
            PrevSceneObject = null;
            PrevSkybox = null;
            PrevSkyboxColor = default;
            PrevSkyboxCubemap = null;
            PrevTimeScale = 1f;
            PlayerPrevPositions = null;
            PlayerPrevRotations = null;
        }
    }
}
