using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.SavedData;
using MTM101BaldAPI.OptionsAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Menu
{
    public class ModSettingsMenu
    {
        private static MenuToggle authenticModeButton;

        private static MenuToggle tipsButton;

        private static MenuToggle firstPrizeFeatureButton;

        public static void onMenuInit(OptionsMenu menu)
        {
            bool gameInitialized = Singleton<BaseGameManager>.Instance != null;

            GameObject modCategory = CustomOptionsCore.CreateNewCategory(menu, "Extra\nSettings");
            
            authenticModeButton = CustomOptionsCore.CreateToggleButton(menu, new Vector2(70, 35), "SettingAdv_AuthenticMode", Singleton<PlayerFileManager>.Instance.authenticMode, "SettingAdv_AuthenticMode_Desc");

            authenticModeButton.transform.SetParent(modCategory.transform, false);

            tipsButton = CustomOptionsCore.CreateToggleButton(menu, new Vector2(70, -5), "SettingAdv_Tips", DataManager.ExtraSettings.tipsEnabled, "SettingAdv_Tips_Desc");

            tipsButton.transform.SetParent(modCategory.transform, false);

            firstPrizeFeatureButton = CustomOptionsCore.CreateToggleButton(menu, new Vector2(70, -45), "SettingAdv_FirstPrizeFeature", DataManager.ExtraSettings.firstPrizeFeaturesEnabled, "SettingAdv_FirstPrizeFeature_Desc");

            firstPrizeFeatureButton.transform.SetParent(modCategory.transform, false);

            if (gameInitialized)
            {
                firstPrizeFeatureButton.Disable(true);
            }

        }

        public static void onMenuClose(OptionsMenu menu)
        {
            if (authenticModeButton.Value && !DataManager.ExtraSettings.authenticMode)
            {
                Singleton<PlayerFileManager>.Instance.authenticMode = true;
                DataManager.ExtraSettings.authenticMode = true; //mark
                if (Singleton<CoreGameManager>.Instance != null)
                {
                    Singleton<CoreGameManager>.Instance.Quit();
                }
            } else if (!authenticModeButton.Value && DataManager.ExtraSettings.authenticMode)
            {
                Singleton<PlayerFileManager>.Instance.authenticMode = false;
                DataManager.ExtraSettings.authenticMode = false; //mark
                if (Singleton<CoreGameManager>.Instance != null)
                {
                    Singleton<CoreGameManager>.Instance.Quit();
                }
            }

            DataManager.ExtraSettings.tipsEnabled = tipsButton.Value;
            DataManager.ExtraSettings.firstPrizeFeaturesEnabled = firstPrizeFeatureButton.Value;
            DataManager.saveMenu();
        }
    }
}
