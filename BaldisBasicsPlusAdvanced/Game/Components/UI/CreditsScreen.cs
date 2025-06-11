using System;
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
        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private TMP_Text tmpText;

        [SerializeField]
        private StandardMenuButton exitButton;

        [SerializeField]
        private float speed;

        [SerializeField]
        private AudioManager musMan;

        [SerializeField]
        private List<SoundObject> audMusics;

        [SerializeField]
        private List<string> midiNames;

        [SerializeField]
        private float maxHeight;

        private int soundObjectIndex, midiIndex;

        private bool paused;

        public Action onScreenClose;

        private void InitializeMidisList()
        {
            List<string> postMidis = new List<string>();
            foreach (List<string> midiNames in MusicPatch.musicNames.Values)
            {
                foreach (string midiName in midiNames)
                {
                    if (midiName.Contains("Adv_Excavator"))
                    {
                        this.midiNames.Add(midiName);
                    }
                    else postMidis.Add(midiName);
                }
            }

            this.midiNames.AddRange(postMidis);
        }

        public void InitializePrefab(int variant)
        {
            musMan = ObjectsCreator.CreateAudMan(Vector3.zero);
            musMan.transform.SetParent(transform, false);

            audMusics = new List<SoundObject>()
            {
                AssetsHelper.LoadAsset<SoundObject>("Mus_Party"),
                AssetsStorage.sounds["creepy_old_computer"]
            };
            midiNames = new List<string>();

            speed = 30f;

            canvas = ObjectsCreator.CreateCanvas(gameObject, setGlobalCam: true);
            canvas.transform.SetParent(transform, false);

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

            Image exitImage = UIHelpers.CreateImage(AssetsStorage.sprites["adv_exit_transparent"], canvas.transform,
                Vector3.zero, correctPosition: false);
            exitImage.name = "Exit";

            exitImage.ToCenter();

            exitImage.transform.localPosition = new Vector3(210, -150);
            exitImage.tag = "Button";

            exitButton = exitImage.gameObject.AddComponent<StandardMenuButton>();

            exitButton.image = exitImage;
            exitButton.heldSprite = AssetsStorage.sprites["adv_exit"];
            exitButton.unhighlightedSprite = AssetsStorage.sprites["adv_exit_transparent"];
            exitButton.highlightedSprite = AssetsStorage.sprites["adv_exit"];

            exitButton.swapOnHold = true; //on press
            exitButton.swapOnHigh = true; //on high

            canvas.SetCursorInitiator();

            enabled = true;

            ApiManager.RegisterOnModAssetsLoading(InitializeMidisList, post: true);
        }

        private void Start()
        {
            exitButton.InitializeAllEvents();
            exitButton.OnPress.AddListener(delegate { Destroy(gameObject); });
            MusicManager.Instance.StopMidi();
            canvas.worldCamera.backgroundColor = Color.black;
            GetComponentInChildren<CursorController>().transform.SetSiblingIndex(3);
        }

        public void Pause(bool state)
        {
            paused = state;
            musMan.Pause(paused);
            MusicManager.Instance.PauseMidi(paused);
        }

        private void Update()
        {
            if (!paused)
            {
                if (!musMan.AnyAudioIsPlaying && !MusicManager.Instance.MidiPlaying)
                {
                    if (soundObjectIndex <= audMusics.Count - 1)
                    {
                        musMan.PlaySingle(audMusics[soundObjectIndex]);
                        soundObjectIndex++;
                    }
                    else if (midiIndex <= midiNames.Count - 1)
                    {
                        MusicManager.Instance.PlayMidi(midiNames[midiIndex], false);
                        midiIndex++;
                    } else
                    {
                        soundObjectIndex = 0;
                        midiIndex = 0;
                    }
                }

                tmpText.transform.position += Vector3.up * Time.unscaledDeltaTime * speed;

                if (tmpText.transform.position.y > maxHeight) Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            MusicManager.Instance.StopMidi();
            canvas.worldCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            onScreenClose?.Invoke();
        }

    }
}
