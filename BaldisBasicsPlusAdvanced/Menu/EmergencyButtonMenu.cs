using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Menu
{
    public class EmergencyButtonMenu
    {
        private static StandardMenuButton emergencyButton;

        private static TMP_Text emergencybuttonDesc;

        private static OptionsMenu menu;

        public static bool available;

        public static void OnMenuInit(OptionsMenu menu)
        {
            available = true;
            //Debug.Log("Menu initialized!");
            bool buttonSpawn = Singleton<CoreGameManager>.Instance != null;

            EmergencyButtonMenu.menu = menu;

            GameObject modCategory = CustomOptionsCore.CreateNewCategory(menu, "Emergency\nButton");

            if (buttonSpawn)
            {
                emergencyButton = ObjectsCreator.CreateSpriteButton(AssetsStorage.sprites["elv_button_up"], out Image image, AssetsStorage.sprites["elv_button_down"], false, modCategory.transform);
                emergencyButton.swapOnHold = true;
                emergencyButton.heldSprite = AssetsStorage.sprites["elv_button_down"];

                RectTransform rect = image.rectTransform;

                rect.sizeDelta = new Vector2(140, 130);

                emergencyButton.transform.localPosition = new Vector3(0, -120, 0);

                TMP_Text buttonTitle = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_EmergencyButton_SpriteTitle"),
                    modCategory.transform, new Vector3(0, -104, 0), false);

                buttonTitle.fontSize = 12;
                buttonTitle.color = Color.black;
                buttonTitle.alignment = TextAlignmentOptions.Top;
            }

            emergencybuttonDesc = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "",
                    modCategory.transform, new Vector3(0, 0, 0), false);

            if (buttonSpawn)
            {
                SetButtonState(ButtonState.InGame);
            } else
            {
                SetButtonState(ButtonState.InMenu);
            }

            emergencybuttonDesc.fontSize = 12;
            emergencybuttonDesc.color = Color.black;
            emergencybuttonDesc.alignment = TextAlignmentOptions.Top;

            emergencybuttonDesc.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 50);

            if (buttonSpawn)
            {
                emergencyButton.OnPress.AddListener(OnEmergencyButtonPress);
            }

        }

        public static void SetButtonState(ButtonState state)
        {
            if (emergencybuttonDesc == null) return;
            if (state == ButtonState.InMenu)
            {
                EmergencyButtonMenu.available = false;
                emergencybuttonDesc.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_EmergencyButton_Desc_In_MainMenu");
            }
            if (state == ButtonState.InGame)
            {
                EmergencyButtonMenu.available = true;
                emergencybuttonDesc.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_EmergencyButton_Desc");
            }
            if (state == ButtonState.Pressed)
            {
                EmergencyButtonMenu.available = false;
                emergencybuttonDesc.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_EmergencyButton_Desc_Reloading");
            }
        }

        public static void OnMenuClose(OptionsMenu menu)
        {
            
        }

        private static void OnEmergencyButtonPress()
        {
            if (!available)
            {
                emergencyButton.image.sprite = AssetsStorage.sprites["elv_button_up"];
                return;
            }

            if (Singleton<BaseGameManager>.Instance is PitstopGameManager)
            {
                Singleton<BaseGameManager>.Instance.StartCoroutine(Waiter());
            }
            else
            {
                LevelReloader reloader = new GameObject("Level reloader").AddComponent<LevelReloader>();
                reloader.Initialize("Adv_EmergencyButton_Pressed_Notif", 4f);
            }

            if (available)
            {
                AudioManager audMan = ReflectionHelper.GetValue<AudioManager>(menu, "audMan");
                audMan.PlaySingle(AssetsStorage.sounds["button_press"]);
                SetButtonState(ButtonState.Pressed);
            }
        }

        private static IEnumerator Waiter()
        {
            while (Time.timeScale == 0f)
            {
                yield return null;
            }
            Singleton<BaseGameManager>.Instance.LoadNextLevel();
            SetButtonState(ButtonState.InGame);
            yield break;
        }

    }
}
