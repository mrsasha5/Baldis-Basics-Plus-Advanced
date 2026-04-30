using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.GameManager;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI
{
    public class CreditsScreen : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private AudioManager musMan;

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private TMP_Text tmpText;

        [SerializeField]
        private StandardMenuButton exitButton, pauseButton, skipBackButton, skipForwardButton;

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
        private float switchBgTime;

        [SerializeField]
        private float arrowKeyboardSpeed;

        [SerializeField]
        private float skipBackButtonHeight, skipForwardButtonHeight;

        [SerializeField]
        private float offset;

        [SerializeField]
        private float maxHeight;

        private int soundObjectIndex, midiIndex, spriteIndex;

        private float time;

        private bool switchingBg;

        private int pauses;

        private bool paused;

        private bool pausedByKeyboardSkip;

        private Vector3 _position;

        public Action onScreenClose;

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
            transparentPlaySprite = AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_play_transparent.png");
            playSprite = AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_play.png");
            transparentPauseSprite = AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_pause_transparent.png");
            pauseSprite = AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_pause.png");

            skipBackButtonHeight = 500f;
            skipForwardButtonHeight = 1000f;
            arrowKeyboardSpeed = 150f;
            switchBgTime = 10f;
            fadeSpeed = 0.5f;

            musMan = ObjectCreator.CreateAudioManager(Vector3.zero, distanceIndependent: true, transform);
            musMan.ignoreListenerPause = true;
            musMan.useUnscaledPitch = true;
            musMan.ReflectionSetValue("usesSfx", false);
            musMan.ReflectionSetValue("usesMusic", true);

            audMusics = new List<SoundObject>()
            {
                AssetHelper.LoadAsset<SoundObject>("Mus_Party"),
                AssetStorage.sounds["creepy_old_computer"]
            };
            midiNames = new List<string>();

            speed = 30f;

            canvas = ObjectCreator.CreateCanvas(gameObject, setGlobalCam: true);
            canvas.transform.SetParent(transform, false);

            backgroundSprites = new List<Sprite>();

            foreach (string path in Directory.GetFiles(AssetHelper.modPath + "Textures/Backgrounds/Credits", 
                "*.*", SearchOption.AllDirectories))
            {
                backgroundSprites.Add(AssetHelper.SpriteFromFile(path.Replace(AssetHelper.modPath, "")));
            }

            backgroundImage = UIHelpers.CreateImage(backgroundSprites[0], canvas.transform, Vector3.zero, false);
            backgroundImage.ToCenter();
            backgroundImage.rectTransform.sizeDelta = new Vector2(640f, 360f);
            backgroundImage.color = new Color(1f, 1f, 1f, 0.5f);

            string creditsText = "";

            int linesCount = 0;

            using (StreamReader reader = new StreamReader(AssetHelper.modPath + "Language/English/Credits.txt"))
            {
                offset = float.Parse(reader.ReadLine().Replace("Offset: ", ""), CultureInfo.InvariantCulture);
                maxHeight = float.Parse(reader.ReadLine().Replace("Max Height: ", ""), CultureInfo.InvariantCulture);
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
            tmpText.rectTransform.sizeDelta = new Vector2(480, 50);
            tmpText.alignment = TextAlignmentOptions.Center;

            exitButton = ObjectCreator.CreateSpriteButton(
                AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_door_exit_transparent.png"), 
                new Vector3(208, 148), canvas.transform, 
                AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_door_exit.png"),
                AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_door_exit_pressed.png")
            );

            pauseButton = ObjectCreator.CreateSpriteButton(
                transparentPauseSprite,
                new Vector3(208, -85), canvas.transform,
                pauseSprite,
                pauseSprite
            );

            skipForwardButton = ObjectCreator.CreateSpriteButton(
                AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_acceleration_transparent.png"),
                new Vector3(208, -148), canvas.transform,
                AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_acceleration.png")
            );

            skipBackButton = ObjectCreator.CreateSpriteButton(
                AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_acceleration_transparent.png"),
                new Vector3(145, -148), canvas.transform,
                AssetHelper.SpriteFromFile("Textures/UI/Buttons/adv_button_acceleration.png")
            );
            skipBackButton.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

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
                if (!paused && !pausedByKeyboardSkip)
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
                if (!paused && !pausedByKeyboardSkip)
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
            paused = true;
            LockInterface(true);
            StartCoroutine(Initializer());
        }

        private IEnumerator Initializer()
        {
            while (GlobalCam.Instance.TransitionActive) yield return null;

            paused = false;
            LockInterface(false);

            CoreManagerPausePatch.SetPauseDisable(true);
            GlobalCam.Instance.Transition(UiTransition.Dither, 0.01666667f);
            exitButton.InitializeAllEvents();
            pauseButton.InitializeAllEvents();
            skipBackButton.InitializeAllEvents();
            skipForwardButton.InitializeAllEvents();

            exitButton.OnRelease.AddListener(
                delegate 
                {
                    Close();
                }
            );

            pauseButton.OnPress.AddListener(
                delegate
                {
                    Pause(!paused);
                }
            );

            skipBackButton.OnPress.AddListener(
                delegate
                {
                    StartCoroutine(MoveText(skipBackButton, tmpText.transform.position.y - skipBackButtonHeight));
                }
            );

            skipForwardButton.OnPress.AddListener(
                delegate
                {
                    StartCoroutine(MoveText(skipForwardButton, tmpText.transform.position.y + skipForwardButtonHeight));
                }
            );
        }

        private IEnumerator MoveText(StandardMenuButton button, float newHeight)
        {
            newHeight = Mathf.Clamp(newHeight, offset, maxHeight);

            float baseHeight = tmpText.transform.position.y;

            float redColorSpeed = 2f;

            Pause(true);
            LockInterface(true);

            bool setButtonToRed = true;
            Color color = Color.white;

            float percent = 0f;
            Vector3 position = tmpText.transform.position;

            float val = 0f;

            while (percent < 1f)
            {
                if (button != null)
                {
                    if (setButtonToRed)
                    {
                        color.g = color.b -= Time.unscaledDeltaTime * redColorSpeed;
                        if (color.g < 0f)
                        {
                            color.g = color.b = 0f;
                            setButtonToRed = false;
                        }
                        button.image.color = color;
                    }
                    else
                    {
                        color.g = color.b += Time.unscaledDeltaTime * redColorSpeed * 2f;
                        if (color.g > 1f)
                        {
                            color.g = color.b = 1f;
                            setButtonToRed = true;
                        }
                        button.image.color = color;
                    }
                }

                val += Time.unscaledDeltaTime;

                percent += Time.unscaledDeltaTime * val;
                if (percent > 1f) percent = 1f;

                position.y = baseHeight + (newHeight - baseHeight) * percent;
                tmpText.transform.position = position;
                yield return null;
            }

            Pause(false);
            LockInterface(false);
            if (button != null) button.image.color = Color.white;
        }

        private void Start()
        {
            spriteIndex = 1;
            time = switchBgTime;
            MusicManager.Instance.StopMidi();
            canvas.worldCamera.backgroundColor = Color.black;
            GetComponentInChildren<CursorController>().transform.SetSiblingIndex(7);
        }

        private void LockInterface(bool state)
        {
            pauseButton.enabled = !state;
            skipBackButton.enabled = !state;
            skipForwardButton.enabled = !state;

            pauseButton.gameObject.tag = state ? "Untagged" : "Button";
            skipBackButton.gameObject.tag = state ? "Untagged" : "Button";
            skipForwardButton.gameObject.tag = state ? "Untagged" : "Button";
        }

        public void Pause(bool state)
        {
            if (state) pauses++;
            else pauses--;

            if (pauses < 0) pauses = 0;

            paused = pauses > 0;

            if (paused) musMan.audioSourceManager.Pause();
            else musMan.audioSourceManager.GetAudioSource(SoundType.Music).UnPause();

            MusicManager.Instance.PauseMidi(paused);

            if (paused)
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
            _position.x = tmpText.transform.position.x;
            _position.y = tmpText.transform.position.y;
            _position.z = tmpText.transform.position.z;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A))
            {
                if (!pausedByKeyboardSkip)
                {
                    Pause(true);
                    LockInterface(true);
                }
                pausedByKeyboardSkip = true;
                _position.y -= Time.unscaledDeltaTime * arrowKeyboardSpeed;
                if (_position.y < offset) _position.y = offset;
                tmpText.transform.position = _position;

                skipBackButton.image.color = Color.red;
                skipForwardButton.image.color = Color.white;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                if (!pausedByKeyboardSkip)
                {
                    Pause(true);
                    LockInterface(true);
                }
                pausedByKeyboardSkip = true;
                _position.y += Time.unscaledDeltaTime * arrowKeyboardSpeed;
                if (_position.y > maxHeight) _position.y = maxHeight;
                tmpText.transform.position = _position;

                skipBackButton.image.color = Color.white;
                skipForwardButton.image.color = Color.red;
            }
            else if (pausedByKeyboardSkip)
            {
                pausedByKeyboardSkip = false;
                Pause(false);
                LockInterface(false);
                skipBackButton.image.color = Color.white;
                skipForwardButton.image.color = Color.white;
            }

            if (!paused && !pausedByKeyboardSkip)
            {
                if (time > 0f && !switchingBg)
                {
                    time -= Time.unscaledDeltaTime;
                    if (time <= 0f)
                    {
                        StartCoroutine(BackgroundSwitcher());
                        time = switchBgTime;
                    }
                }

                if (!musMan.audioSourceManager.isPlaying && !MusicManager.Instance.MidiPlaying)
                {
                    if (soundObjectIndex <= audMusics.Count - 1)
                    {
                        musMan.audioSourceManager.GetAudioSource(SoundType.Music).clip = audMusics[soundObjectIndex].soundClip;
                        musMan.audioSourceManager.Play();
                        soundObjectIndex++;
                        //isMidiPlaying = false;
                    }
                    else if (midiIndex <= midiNames.Count - 1)
                    {
                        MusicManager.Instance.PlayMidi(midiNames[midiIndex], false);
                        midiIndex++;
                        //isMidiPlaying = true;
                    }
                    else
                    {
                        soundObjectIndex = 0;
                        midiIndex = 0;
                    }
                }

                tmpText.transform.position += Vector3.up * Time.unscaledDeltaTime * speed;

                if (tmpText.transform.position.y > maxHeight) Close();
            }
        }

        private void Close()
        {
            GlobalCam.Instance.Transition(UiTransition.Dither, 0.01666667f);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            canvas.worldCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);

            CoreManagerPausePatch.SetPauseDisable(false);
            MusicManager.Instance.StopMidi();
            onScreenClose?.Invoke();
        }

    }
}