using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Menu;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.SavedData;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Menu
{
    public class ModSettingsMenu
    {
        private static MenuToggle tipsButton;

        private static MenuToggle firstPrizeFeatureButton;

        private static MenuToggle particlesButton;

        public static void OnMenuInit(OptionsMenu menu)
        {
            bool gameInitialized = Singleton<BaseGameManager>.Instance != null;

            GameObject modCategory = CustomOptionsCore.CreateNewCategory(menu, "Extra\nSettings");

            tipsButton = CustomOptionsCore.CreateToggleButton(menu, new Vector2(70, 35), "SettingAdv_Tips", DataManager.ExtraSettings.tipsEnabled, "SettingAdv_Tips_Desc");

            tipsButton.transform.SetParent(modCategory.transform, false);

            particlesButton = CustomOptionsCore.CreateToggleButton(menu, new Vector2(70, -5), "SettingAdv_Particles", DataManager.ExtraSettings.particlesEnabled, "SettingAdv_Particles_Desc");

            particlesButton.transform.SetParent(modCategory.transform, false);

            firstPrizeFeatureButton = CustomOptionsCore.CreateToggleButton(menu, new Vector2(70, -45), "SettingAdv_FirstPrizeFeature", DataManager.ExtraSettings.firstPrizeFeaturesEnabled, "SettingAdv_FirstPrizeFeature_Desc");

            firstPrizeFeatureButton.transform.SetParent(modCategory.transform, false);

            if (gameInitialized)
            {
                firstPrizeFeatureButton.Disable(true);
            }

            if (DataManager.ExtraSettings.showNotif)
            {
                Image notif = 
                    UIHelpers.CreateImage(AssetsStorage.sprites["exclamation_point_sheet0"], modCategory.transform,
                    Vector3.zero, correctPosition: false);
                notif.ToCenter();

                notif.transform.localScale = Vector3.one * 0.25f;
                notif.transform.localPosition = new Vector3(50f, 120f, 0f);

                modCategory.gameObject.AddComponent<NotifiedExtraMenu>().notifImage = notif;
            }
        }

        public static void OnMenuClose(OptionsMenu menu)
        {
            DataManager.ExtraSettings.tipsEnabled = tipsButton.Value;
            DataManager.ExtraSettings.firstPrizeFeaturesEnabled = firstPrizeFeatureButton.Value;
            DataManager.ExtraSettings.particlesEnabled = particlesButton.Value;
            DataManager.SaveMenu();
        }
    }
}
