using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.FieldTrips.Managers
{
    public class BaseFieldTripManager : BaseGameManager
    {

        protected virtual void CloseFieldTrip(bool showFieldTripScreen = true)
        {
            FieldTripsLoader.CloseFieldTrip(showFieldTripScreen);
        }

        protected override void LoadSceneObject(SceneObject sceneObject, bool restarting)
        {
            CloseFieldTrip();
        }

        public override void EndGame(Transform player, Baldi baldi)
        {
            EndGame(player, baldi.transform, baldi.loseSounds);
        }

        public virtual void EndGame(Transform player, Transform killer, WeightedSelection<SoundObject>[] loseSounds)
        {
            Time.timeScale = 0f;
            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<CoreGameManager>.Instance.disablePause = true;
            Singleton<CoreGameManager>.Instance.GetCamera(0).UpdateTargets(killer.transform, 0);
            Singleton<CoreGameManager>.Instance.GetCamera(0).offestPos = (player.position - killer.transform.position).normalized * 2f + Vector3.up;
            Singleton<CoreGameManager>.Instance.GetCamera(0).SetControllable(value: false);
            Singleton<CoreGameManager>.Instance.GetCamera(0).matchTargetRotation = false;
            Singleton<CoreGameManager>.Instance.audMan.volumeModifier = 0.6f;
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(WeightedSelection<SoundObject>.RandomSelection(loseSounds));
            Singleton<CoreGameManager>.Instance.StartCoroutine(EndSequence());
            Singleton<InputManager>.Instance.Rumble(1f, 2f);
            Singleton<HighlightManager>.Instance.Highlight("steam_x", 
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Steam_Highlight_Lose"), 
                string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Steam_Highlight_Lose_Desc"), 
                Singleton<LocalizationManager>.Instance.GetLocalizedText(managerNameKey), 
                Singleton<LocalizationManager>.Instance.GetLocalizedText(Singleton<CoreGameManager>.Instance.sceneObject.nameKey)), 
                2u, 0f, 0f, TimelineEventClipPriority.Standard);
        }

        protected virtual IEnumerator EndSequence()
        {
            float time2 = 1f;
            Shader.SetGlobalColor("_SkyboxColor", Color.black);
            while (time2 > 0f)
            {
                time2 -= Time.unscaledDeltaTime;
                Singleton<CoreGameManager>.Instance.GetCamera(0).camCom.farClipPlane = 500f * time2;
                Singleton<CoreGameManager>.Instance.GetCamera(0).billboardCam.farClipPlane = 500f * time2;
                yield return null;
            }

            Singleton<CoreGameManager>.Instance.GetCamera(0).camCom.farClipPlane = 1000f;
            Singleton<CoreGameManager>.Instance.GetCamera(0).billboardCam.farClipPlane = 1000f;
            Singleton<CoreGameManager>.Instance.GetCamera(0).StopRendering(val: true);
            Singleton<CoreGameManager>.Instance.audMan.FlushQueue(endCurrent: true);
            AudioListener.pause = true;
            time2 = 2f;
            while (time2 > 0f)
            {
                time2 -= Time.unscaledDeltaTime;
                yield return null;
            }

            RestartLevel();
        }

        public override void Initialize()
        {
            ec.AssignPlayers();
            for (int i = 0; i < Singleton<CoreGameManager>.Instance.setPlayers; i++)
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(i).plm.Entity.Initialize(ec, 
                    Singleton<CoreGameManager>.Instance.GetPlayer(i).transform.position);
            }
            base.Initialize();
        }

        public override void BeginPlay()
        {
            Time.timeScale = 1f;
            Singleton<InputManager>.Instance.ActivateActionSet("InGame");
            Singleton<CoreGameManager>.Instance.PlayBegins();
            ec.Active = true;
            ec.BeginPlay();
            playStarted = true;
            AudioListener.pause = false;
            PropagatedAudioManager.paused = false;
        }

        public override void RestartLevel()
        {
            LoadSceneObject(FieldTripsLoader.PrevSceneObject, false);
        }

        public virtual void OnLoadingScreenDestroying()
        {

        }

    }
}
