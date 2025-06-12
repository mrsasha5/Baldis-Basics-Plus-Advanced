using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Menu;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.GameManager;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI
{
    public class CreditsScreen : MonoBehaviour, IPrefab
    {

        private class FrameState
        {

            public Vector3 textPosition;

            public int audioIndex = -1;

            public int midiIndex = -1;

            public int bgIndex;

            public float audioTime;

            public float midiTime;

            public Color backgroundColor;

            public float time;

            public bool switchingBg;

        }


        [SerializeField]
        private AudioManager musMan;

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private TMP_Text tmpText;

        [SerializeField]
        private StandardMenuButton exitButton, pauseButton;

        [SerializeField]
        private float speed;

        [SerializeField]
        private Sprite transparentPlaySprite, transparentPauseSprite, playSprite, pauseSprite;

        [SerializeField]
        private List<Sprite> backgroundSprites;

        [SerializeField]
        private List<SoundObject> audMusics;

        [SerializeField]
        private List<string> midiNames;

        [SerializeField]
        private float fadeSpeed;

        [SerializeField]
        private float switchTime;

        [SerializeField]
        private float maxHeight;

        private List<FrameState> frames = new List<FrameState>();

        private int soundObjectIndex, midiIndex, spriteIndex;

        private float time;

        private bool isMidiPlaying;

        private bool frameLoaded;

        private bool switchingBg;

        private bool paused;

        public Action onScreenClose;

        private void LoadFrame(FrameState frame)
        {
            tmpText.transform.position = frame.textPosition;
            time = frame.time;
            spriteIndex = frame.bgIndex;

            musMan.FlushQueue(true);
            MusicManager.Instance.StopMidi();

            if (frame.audioIndex != -1)
            {
                soundObjectIndex = frame.audioIndex;
                musMan.audioDevice.clip = audMusics[frame.audioIndex].soundClip;
                musMan.audioDevice.time = frame.audioTime;
            }

            if (frame.midiIndex != -1)
            {
                midiIndex = frame.midiIndex;
                MusicManager.Instance.PlayMidi(midiNames[midiIndex], false);
                MusicManager.Instance.MidiPlayer.MPTK_Position = frame.midiTime;
            }

            backgroundImage.sprite = backgroundSprites[spriteIndex - 1];
            if (frame.switchingBg)
            {
                StopAllCoroutines();
                backgroundImage.color = frame.backgroundColor;
                StartCoroutine(BackgroundSwitcher());
            }
            else
            {
                StopAllCoroutines();
                backgroundImage.color = new Color(1f, 1f, 1f, 0.5f);
                switchingBg = false;
            }

            //frameLoaded = true;
        }

        private void SaveFrame()
        {
            FrameState frame = new FrameState();

            frame.time = time;
            frame.bgIndex = spriteIndex;

            if (!isMidiPlaying)
            {
                frame.audioIndex = soundObjectIndex - 1;
                frame.audioTime = musMan.audioDevice.time;
            }
            else
            {
                frame.midiIndex = midiIndex - 1;
                frame.midiTime = (float)MusicManager.Instance.MidiPlayer.MPTK_Position;
            }

            frame.textPosition = tmpText.transform.position;

            if (switchingBg)
            {
                frame.switchingBg = true;
                frame.backgroundColor = backgroundImage.color;
            }

            frames.Add(frame);
        }

        private void InitializeMidisList()
        {
            List<string> postMidis = new List<string>();
            foreach (List<string> midiNames in MusicPatch.musicNames.Values)
            {
                foreach (string midiName in midiNames)
                {
                    if (this.midiNames.Contains(midiName)) continue;

                    if (midiName.Contains("Adv_Excavator"))
                    {
                        this.midiNames.Add(midiName);
                    }
                    else postMidis.Add(midiName);
                }
            }

            string maintenanceTheme = midiNames.Find(x => x.Contains("Adv_Excavator_JanitorsOnly"));
            string factoryTheme = midiNames.Find(x => x.Contains("Adv_Excavator_GradesAreImportant"));

            if (maintenanceTheme != null)
            {
                midiNames.Remove(maintenanceTheme);
                midiNames.Insert(0, maintenanceTheme);
            }

            if (factoryTheme != null)
            {
                midiNames.Remove(factoryTheme);
                midiNames.Insert(1, factoryTheme);
            }

            this.midiNames.AddRange(postMidis);
        }

        public void InitializePrefab(int variant)
        {
            transparentPlaySprite = AssetsHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_play_transparent.png");
            playSprite = AssetsHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_play.png");
            transparentPauseSprite = AssetsHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_pause_transparent.png");
            pauseSprite = AssetsHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_pause.png");

            switchTime = 10f;
            fadeSpeed = 0.5f;

            musMan = ObjectsCreator.CreateAudMan(Vector3.zero);
            musMan.transform.SetParent(transform, false);
            musMan.ignoreListenerPause = true;
            musMan.useUnscaledPitch = true;

            audMusics = new List<SoundObject>()
            {
                AssetsHelper.LoadAsset<SoundObject>("Mus_Party"),
                AssetsStorage.sounds["creepy_old_computer"]
            };
            midiNames = new List<string>();

            speed = 30f;

            canvas = ObjectsCreator.CreateCanvas(gameObject, setGlobalCam: true);
            canvas.transform.SetParent(transform, false);

            backgroundSprites = new List<Sprite>();

            foreach (string path in Directory.GetFiles(AssetsHelper.modPath + "Textures/CreditsBackgrounds", 
                "*.*", SearchOption.AllDirectories))
            {
                backgroundSprites.Add(AssetsHelper.SpriteFromFile(path.Replace(AssetsHelper.modPath, "")));
            }

            backgroundImage = UIHelpers.CreateImage(backgroundSprites[0], canvas.transform, Vector3.zero, false);
            backgroundImage.rectTransform.sizeDelta = new Vector2(640f, 360f);
            backgroundImage.color = new Color(1f, 1f, 1f, 0.5f);

            string creditsText = "";

            int linesCount = 0;
            float offset = 0;

            using (StreamReader reader = new StreamReader(AssetsHelper.modPath + "Language/English/Credits.txt"))
            {
                offset = float.Parse(reader.ReadLine().Replace("Offset: ", ""));
                maxHeight = float.Parse(reader.ReadLine().Replace("Max Height: ", ""));
                string line = reader.ReadLine();
                while (line != null)
                {
                    creditsText += line + "\n";
                    line = reader.ReadLine();
                    linesCount++;
                }
            }

            tmpText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, creditsText, canvas.transform, Vector3.zero);
            tmpText.transform.position += Vector3.up * offset;
            tmpText.rectTransform.sizeDelta = new Vector2(500, 50);
            tmpText.alignment = TextAlignmentOptions.Center;

            exitButton = ObjectsCreator.CreateSpriteButton(
                AssetsHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_door_exit_transparent.png"), 
                new Vector3(210, 150), canvas.transform, 
                AssetsHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_door_exit.png"),
                AssetsHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_door_exit_pressed.png")
            );

            pauseButton = ObjectsCreator.CreateSpriteButton(
                transparentPauseSprite,
                new Vector3(210, -150), canvas.transform,
                pauseSprite
            );

            //new Vector3(146, -150)

            canvas.SetCursorInitiator();

            enabled = true;

            ApiManager.RegisterOnModAssetsLoading(InitializeMidisList, post: true);
        }

        private IEnumerator BackgroundSwitcher()
        {
            switchingBg = true;
            Color color = backgroundImage.color;
            float baseVal = 0.5f;
            while (color.a > 0f)
            {
                if (!paused)
                {
                    color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime * fadeSpeed);
                    backgroundImage.color = color;
                }
                yield return null;
            }
            backgroundImage.sprite = backgroundSprites[spriteIndex];
            spriteIndex++;
            if (spriteIndex > backgroundSprites.Count - 1) spriteIndex = 0;
            while (color.a < baseVal)
            {
                if (!paused)
                {
                    color.a = Mathf.Clamp(color.a + Time.unscaledDeltaTime * fadeSpeed, 0f, baseVal);
                    backgroundImage.color = color;
                }
                yield return null;
            }
            switchingBg = false;
        }

        private void Awake()
        {
            CoreManagerPausePatch.SetPauseDisable(true);
            GlobalCam.Instance.Transition(UiTransition.Dither, 0.01666667f);
            exitButton.InitializeAllEvents();
            pauseButton.InitializeAllEvents();

            exitButton.OnRelease.AddListener(
                delegate 
                { 
                    Destroy(gameObject);
                }
            );

            pauseButton.OnPress.AddListener(
                delegate
                {
                    Pause(!paused);
                }
            );
        }

        private void Start()
        {
            spriteIndex = 1;
            time = switchTime;
            MusicManager.Instance.StopMidi();
            canvas.worldCamera.backgroundColor = Color.black;
            GetComponentInChildren<CursorController>().transform.SetSiblingIndex(5);
        }

        private void LockInterface(bool state)
        {
            exitButton.enabled = !state;
            pauseButton.enabled = !state;
        }

        public void Pause(bool state)
        {
            paused = state;

            //AW MYSTMAN!!! USE UNPAUSE!!!!
            //musMan.Pause(paused);
            if (state) musMan.audioDevice.Pause();
            else musMan.audioDevice.UnPause();

            MusicManager.Instance.PauseMidi(paused);

            if (state)
            {
                pauseButton.unhighlightedSprite = transparentPlaySprite;
                pauseButton.highlightedSprite = playSprite;

                if (pauseButton.WasHighlighted)
                pauseButton.image.sprite = pauseButton.unhighlightedSprite;
            } else
            {
                pauseButton.unhighlightedSprite = transparentPauseSprite;
                pauseButton.highlightedSprite = pauseSprite;
                pauseButton.image.sprite = pauseButton.unhighlightedSprite;
            }
        }

        private void Update()
        {
            if (!paused)
            {
                if (time > 0f && !switchingBg)
                {
                    time -= Time.unscaledDeltaTime;
                    if (time <= 0f)
                    {
                        StartCoroutine(BackgroundSwitcher());
                        time = switchTime;
                    }
                }

                if (!frameLoaded && !musMan.audioDevice.isPlaying && !MusicManager.Instance.MidiPlaying)
                {
                    if (soundObjectIndex <= audMusics.Count - 1)
                    {
                        musMan.audioDevice.clip = audMusics[soundObjectIndex].soundClip;
                        musMan.audioDevice.Play();
                        soundObjectIndex++;
                        isMidiPlaying = false;
                    }
                    else if (midiIndex <= midiNames.Count - 1)
                    {
                        MusicManager.Instance.PlayMidi(midiNames[midiIndex], false);
                        midiIndex++;
                        isMidiPlaying = true;
                    } else
                    {
                        soundObjectIndex = 0;
                        midiIndex = 0;
                    }
                }

                tmpText.transform.position += Vector3.up * Time.unscaledDeltaTime * speed;

                if (tmpText.transform.position.y > maxHeight) Destroy(gameObject);

                frameLoaded = false;
            }
        }

        private void OnDestroy()
        {
            CoreManagerPausePatch.SetPauseDisable(false);
            MusicManager.Instance.StopMidi();
            canvas.worldCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            onScreenClose?.Invoke();
            GlobalCam.Instance.FadeIn(UiTransition.Dither, 0.01666667f);
        }

    }
}
