using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects.Controllers;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Menu
{
    internal class EmergencyOptionsMenu : CustomOptionsCategory
    {
        private static StandardMenuButton emergencyButton;

        private static TextMeshProUGUI description;

        private static bool available;

        public override void Build()
        {
            bool buttonSpawn = Singleton<CoreGameManager>.Instance != null;

            if (buttonSpawn)
            {
                emergencyButton = ObjectsCreator.CreateSpriteButton(AssetsStorage.sprites["elv_button_up"], out Image image, AssetsStorage.sprites["elv_button_down"], false, transform);
                emergencyButton.swapOnHold = true;
                emergencyButton.heldSprite = AssetsStorage.sprites["elv_button_down"];

                RectTransform rect = image.rectTransform;

                rect.sizeDelta = new Vector2(140, 130);

                emergencyButton.transform.localPosition = new Vector3(0, -120, 0);

                TMP_Text buttonTitle = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_EmergencyButton_SpriteTitle"),
                    transform, new Vector3(0, -104, 0), false);

                buttonTitle.fontSize = 12;
                buttonTitle.color = Color.black;
                buttonTitle.alignment = TextAlignmentOptions.Top;
            }

            description = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "",
                    transform, new Vector3(0, 0, 0), false);

            if (buttonSpawn)
            {
                SetButtonState(ButtonState.InGame);
            }
            else
            {
                SetButtonState(ButtonState.InMenu);
            }

            description.fontSize = 12;
            description.color = Color.black;
            description.alignment = TextAlignmentOptions.Top;

            description.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 50);

            if (buttonSpawn)
            {
                emergencyButton.OnPress.AddListener(OnEmergencyButtonPress);
            }
        }

        public static void SetButtonState(ButtonState state)
        {
            if (description == null) return;
            if (state == ButtonState.InMenu)
            {
                available = false;
                description.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_EmergencyButton_Desc_In_MainMenu");
            }
            if (state == ButtonState.InGame)
            {
                available = true;
                description.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_EmergencyButton_Desc");
            }
            if (state == ButtonState.Pressed)
            {
                available = false;
                description.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_EmergencyButton_Desc_Reloading");
            }
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
                AudioManager audMan = FindObjectOfType<OptionsMenu>().GetComponent<AudioManager>();//ReflectionHelper.GetValue<AudioManager>(menu, "audMan");
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
