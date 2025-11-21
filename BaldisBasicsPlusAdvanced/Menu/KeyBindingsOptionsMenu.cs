using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Patches.GameManager;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BaldisBasicsPlusAdvanced.SaveSystem.Data;
using BaldisBasicsPlusAdvanced.SaveSystem.Managers;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Menu
{
    public class KeyBindingsOptionsMenu : CustomOptionsCategory
    {
        private List<TMP_Text> redColors = new List<TMP_Text>();

        private KeyCode[] keyCodes;

        private Transform[] moveableElements;

        private int clicks;

        private int maxClicks;

        private void OnEnable()
        {
            for (int i = 0; i < redColors.Count; i++)
            {
                redColors[i].color = Color.black;
            }
        }

        public override void Build()
        {
            //Basics

            int maxElementsCount = 3; //on the one page

            float distanceBetweenBindings = 65f;

            keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode)); //Unity why I should do that???? Are you joking Unity's devs?????

            Image background = CreateImage(null, "ButtonsBackground", Vector3.up * -40f + Vector3.left * 12f, new Vector2(325f, 195f));
            //background.color = Color.yellow; //for tests
            background.gameObject.AddComponent<Mask>();

            StandardMenuButton arrowUpButton =
                CreateButton(delegate
                {
                    if (clicks <= 0) return;
                    for (int i = 0; i < moveableElements.Length; i++)
                    {
                        moveableElements[i].position -= Vector3.up * distanceBetweenBindings;
                    }
                    clicks--;
                }, AssetStorage.sprites["menuArrow2"], "MoveUpUIButton", new Vector3(168f, 50f, 0f));
            arrowUpButton.transform.rotation = Quaternion.Euler(Vector3.forward * 270f);
            arrowUpButton.swapOnHigh = true;
            arrowUpButton.highlightedSprite = AssetStorage.sprites["menuArrow0"];
            arrowUpButton.unhighlightedSprite = AssetStorage.sprites["menuArrow2"];

            StandardMenuButton arrowDownButton =
                CreateButton(delegate
                {
                    if (clicks >= maxClicks) return;
                    for (int i = 0; i < moveableElements.Length; i++)
                    {
                        moveableElements[i].position += Vector3.up * distanceBetweenBindings;
                    }
                    clicks++;
                }, AssetStorage.sprites["menuArrow2"], "MoveDownUIButton", new Vector3(168f, -130f, 0f));
            arrowDownButton.transform.rotation = Quaternion.Euler(Vector3.forward * 90f);
            arrowDownButton.swapOnHigh = true;
            arrowDownButton.highlightedSprite = AssetStorage.sprites["menuArrow0"];
            arrowDownButton.unhighlightedSprite = AssetStorage.sprites["menuArrow2"];

            //Bindings buttons and reset button

            Vector3 position = Vector3.up * 25f + Vector3.left * 75f;

            List<KeyBindingData> bindings = KeyBindingsManager.Keys.Values.ToList();
            List<StandardMenuButton> textButtons = new List<StandardMenuButton>();
            List<StandardMenuButton> keyButtons = new List<StandardMenuButton>();

            for (int i = 0; i < bindings.Count; i++)
            {
                KeyBindingData binding = bindings[i];

                StandardMenuButton textButton = CreateTextButton(null, "KeyBindingNameTextButton" + (i + 1), bindings[i].LocNameKey, position,
                    BaldiFonts.ComicSans18, TextAlignmentOptions.Left, new Vector2(190f, 45f), Color.black);
                textButton.transform.SetParent(background.transform, worldPositionStays: true);
                textButton.OnPress.RemoveAllListeners();
                textButton.underlineOnHigh = false; //don't needed
                AddTooltip(textButton, bindings[i].LocDescKey);
                textButtons.Add(textButton);

                StandardMenuButton keyButton = CreateTextButton(null, "KeyBindingTextButton" + (i + 1), bindings[i].Button.ToString(),
                    position + Vector3.right * 160f, BaldiFonts.BoldComicSans24, TextAlignmentOptions.Center, new Vector2(125f, 45f), Color.black);
                keyButton.transform.SetParent(background.transform, worldPositionStays: true);
                keyButton.OnPress.RemoveAllListeners();
                
                keyButton.OnPress.AddListener(delegate
                {
                    OnPressKeyButton(keyButton, binding);
                });

                keyButtons.Add(keyButton);

                textButton.OnPress.AddListener(delegate
                {
                    OnPressTextButton(keyButton, binding);
                });

                position -= Vector3.up * distanceBetweenBindings;
            }

            moveableElements = new Transform[textButtons.Count + keyButtons.Count];
            int counter = 0;
            for (int i = 0; i < moveableElements.Length; i += 2)
            {
                moveableElements[i] = textButtons[counter].transform;
                moveableElements[i + 1] = keyButtons[counter].transform;
                counter++;
            }

            maxClicks = textButtons.Count - maxElementsCount; 

            if (keyButtons.Count > 0)
            {
                CreateTextButton(delegate
                {
                    List<KeyBindingData> oldBindings = new List<KeyBindingData>();

                    for (int i = 0; i < bindings.Count; i++)
                    {
                        oldBindings.Add((KeyBindingData)bindings[i].Clone());
                    }

                    KeyBindingsManager.SetBindingDefaultValues();
                    KeyBindingsManager.RewriteBindings();

                    for (int i = 0; i < keyButtons.Count; i++)
                    {
                        keyButtons[i].text.text = bindings[i].Button.ToString();

                        if (oldBindings[i].Button != bindings[i].Button)
                        {
                            keyButtons[i].StopAllCoroutines();
                            keyButtons[i].StartCoroutine(ColorAnimator(keyButtons[i].text));
                        }
                        
                    }

                    GetComponentInParent<AudioManager>().PlaySingle(AssetStorage.sounds["teleport"]);
                }, "Reset", "Adv_Reset", new Vector3(130f, -162f, 0f),
                        BaldiFonts.ComicSans24, TextAlignmentOptions.TopRight, new Vector2(100f, 32f), Color.black);
            }
            
        }

        private void OnPressTextButton(StandardMenuButton keyButton, KeyBindingData data)
        {
            keyButton.GetComponentInParent<AudioManager>().PlaySingle(AssetStorage.sounds["error_maybe"]);
            if (!redColors.Contains(keyButton.text))
            {
                redColors.Add(keyButton.text);
                
            }
            keyButton.StopAllCoroutines();
            keyButton.StartCoroutine(ColorAnimator(keyButton.text));
        }

        private void OnPressKeyButton(StandardMenuButton button, KeyBindingData data)
        {
            CoreManagerPausePatch.SetPauseDisable(true);
            StartCoroutine(Waiter(button, data));
            CursorController.Instance.Hide(true);
            button.text.text = "???";
        }

        private IEnumerator ColorAnimator(TMP_Text text)
        {
            Color color = Color.red;
            text.color = color;

            float value = 1f;

            while (value > 0f)
            {
                value -= Time.unscaledDeltaTime;

                if (value < 0f) value = 0f;

                color.r = value;
                text.color = color;
                yield return null;
            }

            redColors.Remove(text);

            yield break;
        }

        private IEnumerator Waiter(StandardMenuButton button, KeyBindingData data)
        {
            yield return null; //lol yeah. I just will skip the one frame.

            KeyCode pressed = KeyCode.None;

            while (pressed == KeyCode.None)
            {
                for (int i = 0; i < keyCodes.Length; i++)
                {
                    if (Input.GetKeyDown(keyCodes[i]))
                    {
                        pressed = keyCodes[i];
                        break;
                    }
                }
                yield return null;
            }

            CoreManagerPausePatch.SetPauseDisable(false);

            button.text.text = pressed.ToString();
            CursorController.Instance.Hide(false);

            data.OverrideButton(pressed);
            KeyBindingsManager.RewriteBindings();

            button.GetComponentInParent<AudioManager>().PlaySingle(AssetStorage.sounds["slap"]);

            yield break;
        }
    }
}
