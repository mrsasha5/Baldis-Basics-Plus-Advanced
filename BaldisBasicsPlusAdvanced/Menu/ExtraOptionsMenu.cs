using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using BaldisBasicsPlusAdvanced.Game.Components.UI.MainMenu;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
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

        public override void Build()
        {
            bool gameInitialized = Singleton<BaseGameManager>.Instance != null;

            CreateSynchronizatedToggleButton(
                synchronizableName: "tips",
                nameKey: "Adv_Option_Tips",
                descKey: "Adv_Option_Tips_Desc",
                pos: new Vector3(70f, 35f, 0f),
                width: 250f
            );

            CreateSynchronizatedToggleButton(
                synchronizableName: "tips_during_game",
                nameKey: "Adv_Option_Tips_During_Game",
                descKey: "Adv_Option_Tips_During_Game_Desc",
                pos: new Vector3(70f, -5f, 0f),
                width: 250f,
                setDisabledCover: true
            ).Disable(gameInitialized);

            CreateSynchronizatedToggleButton(
                synchronizableName: "particles",
                nameKey: "Adv_Option_Particles",
                descKey: "Adv_Option_Particles_Desc",
                pos: new Vector3(70f, -45f, 0f),
                width: 250f
            );

            CreateSynchronizatedToggleButton(
                synchronizableName: "first_prize_extensions",
                nameKey: "Adv_Option_FirstPrizeFeature",
                descKey: "Adv_Option_FirstPrizeFeature_Desc",
                pos: new Vector3(70f, -85f, 0f),
                width: 250f,
                setDisabledCover: true
            ).Disable(gameInitialized);

            StandardMenuButton creditsButton = CreateButton(LoadCreditsScreen, AssetsHelper.LoadAsset<Sprite>("QMark_Sheet_1"),
                "CreditsButton", new Vector3(155f, 70f, 0f));
            creditsButton.swapOnHigh = true;
            creditsButton.image = creditsButton.GetComponent<Image>();
            creditsButton.highlightedSprite = AssetsHelper.LoadAsset<Sprite>("QMark_Sheet_0");
            creditsButton.unhighlightedSprite = creditsButton.image.sprite;

            AddTooltip(creditsButton, "Adv_Option_Credits_Desc");

            CreateApplyButton(OnApply);

            if (OptionsDataManager.ExtraSettings.showNotif)
            {
                Image notif =
                    UIHelpers.CreateImage(AssetsStorage.sprites["exclamation_point_sheet0"], transform,
                    Vector3.zero, correctPosition: false);
                notif.ToCenter();

                notif.transform.localScale = Vector3.one * 0.25f;
                notif.transform.localPosition = new Vector3(50f, 120f, 0f);

                gameObject.AddComponent<NotifiedExtraMenu>().notifImage = notif;
            }
        }

        private void LoadCreditsScreen()
        {
            CreditsScreen screen = Instantiate(ObjectsStorage.Objects["credits_screen"].GetComponent<CreditsScreen>());
            screen.onScreenClose += delegate
            {
                transform.parent.gameObject.SetActive(true);
            };
            transform.parent.gameObject.SetActive(false);
        }

        private void OnApply()
        {
            string[] keys = synchronizatedToggles.Keys.ToArray();
            for (int i = 0; i < synchronizatedToggles.Count; i++)
            {
                OptionsDataManager.ExtraSettings.SetValue(keys[i], synchronizatedToggles[keys[i]].Value);
            }
            OptionsDataManager.Save();
            GetComponentInParent<AudioManager>().PlaySingle(AssetsStorage.sounds["bell"]);
        }

        private void DisableToggleOnPress(MenuToggle toggle)
        {
            GetComponentInParent<AudioManager>().PlaySingle(AssetsStorage.sounds["elv_buzz"]);
            toggle.Set(false);
        }

        private MenuToggle CreateSynchronizatedToggleButton(string synchronizableName, string nameKey, string descKey, Vector3 pos, float width, bool setDisabledCover = false)
        {
            MenuToggle toggle = CreateToggle(synchronizableName, nameKey, OptionsDataManager.ExtraSettings.GetValue<bool>(synchronizableName), pos, width);

            if (!string.IsNullOrEmpty(descKey)) AddTooltip(toggle, descKey);

            if (!string.IsNullOrEmpty(synchronizableName)) synchronizatedToggles.Add(synchronizableName, toggle);

            //DisabledCover
            if (setDisabledCover) {
                GameObject obj = Instantiate(AssetsHelper.LoadAsset<GameObject>("DisabledCover"));
                ReflectionHelper.SetValue(toggle, "disableCover", obj);
                obj.transform.SetParent(toggle.transform, false);
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 48f);
                obj.transform.localPosition = Vector3.zero;
            }

            return toggle;
        }
    }
}
