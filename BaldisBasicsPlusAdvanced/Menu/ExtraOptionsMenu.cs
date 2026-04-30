using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using BaldisBasicsPlusAdvanced.Game.Components.UI.MainMenu;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Menu
{
    public class ExtraOptionsMenu : CustomOptionsCategory
    {
        private Dictionary<string, MenuToggle> synchronizatedToggles = new Dictionary<string, MenuToggle>();

        private MenuToggle legacyElevator;

        internal static GameObject disableCover;

        public override void Build()
        {
            bool gameInitialized = BaseGameManager.Instance != null;

            CreateSynchronizatedToggleButton(
                synchronizableName: "tips",
                nameKey: "Adv_Option_Tips",
                descKey: "Adv_Option_Tips_Desc",
                pos: new Vector3(70f, 35f, 0f),
                width: 250f,
                setDisabledCover: true
            ).Disable(!ExtraSettingsManager.ExtraSettings.GetValue<bool>("legacy_elevator"));

            legacyElevator = CreateSynchronizatedToggleButton(
                synchronizableName: "legacy_elevator",
                nameKey: "Adv_Option_LegacyElevator",
                descKey: "Adv_Option_LegacyElevator_Desc",
                pos: new Vector3(70f, -5f, 0f),
                width: 250f
            );

            CreateSynchronizatedToggleButton(
                synchronizableName: "particles",
                nameKey: "Adv_Option_Particles",
                descKey: "Adv_Option_Particles_Desc",
                pos: new Vector3(70f, -45f, 0f),
                width: 250f
            );

            CreateSynchronizatedToggleButton(
                synchronizableName: "elevator_animations",
                nameKey: "Adv_Option_Elv_Animations",
                descKey: "Adv_Option_Elv_Animations_Desc",
                pos: new Vector3(70f, -85f, 0f),
                width: 250f,
                setDisabledCover: true
            ).Disable(!ExtraSettingsManager.ExtraSettings.GetValue<bool>("legacy_elevator"));

            CreateSynchronizatedToggleButton(
                synchronizableName: "first_prize_extensions",
                nameKey: "Adv_Option_FirstPrizeFeature",
                descKey: "Adv_Option_FirstPrizeFeature_Desc",
                pos: new Vector3(70f, -125f, 0f),
                width: 250f,
                setDisabledCover: true
            ).Disable(gameInitialized);

            StandardMenuButton creditsButton = CreateButton(LoadCreditsScreen, AssetHelper.LoadAsset<Sprite>("QMark_Sheet_1"),
                "CreditsButton", new Vector3(155f, 70f, 0f));
            creditsButton.swapOnHigh = true;
            creditsButton.image = creditsButton.GetComponent<Image>();
            creditsButton.highlightedSprite = AssetHelper.LoadAsset<Sprite>("QMark_Sheet_0");
            creditsButton.unhighlightedSprite = creditsButton.image.sprite;

            AddTooltip(creditsButton, "Adv_Option_Credits_Desc");

            CreateApplyButton(OnApply);

            if (ExtraSettingsManager.ExtraSettings.showNotif)
            {
                Image notif =
                    UIHelpers.CreateImage(AssetStorage.sprites["exclamation_point_sheet0"], transform,
                    Vector3.zero, correctPosition: false);
                notif.ToCenter();

                notif.transform.localScale = Vector3.one * 0.25f;
                notif.transform.localPosition = new Vector3(50f, 120f, 0f);

                gameObject.AddComponent<NotifiedExtraMenu>().notifImage = notif;
            }
        }

        private void LoadCreditsScreen()
        {
            CreditsScreen screen = Instantiate(ObjectStorage.Objects["credits_screen"].GetComponent<CreditsScreen>());
            screen.onScreenClose += delegate
            {
                transform.parent.gameObject.SetActive(true);
            };
            transform.parent.gameObject.SetActive(false);
        }

        private void OnApply()
        {
            string[] keys = synchronizatedToggles.Keys.ToArray();
            if (!legacyElevator.Value)
            {
                synchronizatedToggles["tips"].Disable(true);
                synchronizatedToggles["elevator_animations"].Disable(true);
                if (synchronizatedToggles["tips"].Value) synchronizatedToggles["tips"].Toggle();
                if (synchronizatedToggles["elevator_animations"].Value) synchronizatedToggles["elevator_animations"].Toggle();
            }
            else
            {
                synchronizatedToggles["tips"].Disable(false);
                synchronizatedToggles["elevator_animations"].Disable(false);
            }
            for (int i = 0; i < synchronizatedToggles.Count; i++)
            {
                ExtraSettingsManager.ExtraSettings.SetValue(keys[i], synchronizatedToggles[keys[i]].Value);
            }
            ExtraSettingsManager.Save();
            GetComponentInParent<AudioManager>().PlaySingle(AssetStorage.sounds["bell"]);
        }

        private void DisableToggleOnPress(MenuToggle toggle)
        {
            GetComponentInParent<AudioManager>().PlaySingle(AssetStorage.sounds["elv_buzz"]);
            toggle.Set(false);
        }

        private MenuToggle CreateSynchronizatedToggleButton(string synchronizableName, string nameKey, string descKey, Vector3 pos, float width, bool setDisabledCover = false)
        {
            MenuToggle toggle = CreateToggle(synchronizableName, nameKey, ExtraSettingsManager.ExtraSettings.GetValue<bool>(synchronizableName), pos, width);

            if (!string.IsNullOrEmpty(descKey)) AddTooltip(toggle, descKey);
            if (!string.IsNullOrEmpty(synchronizableName)) synchronizatedToggles.Add(synchronizableName, toggle);

            //DisabledCover
            if (setDisabledCover) {
                GameObject obj = Instantiate(disableCover);
                ReflectionHelper.SetValue(toggle, "disableCover", obj);
                obj.transform.SetParent(toggle.transform, false);
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 48f);
                obj.transform.localPosition = Vector3.zero;
            }
            return toggle;
        }
    }
}
