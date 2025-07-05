using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Components.UI.MainMenu;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using MTM101BaldAPI.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Patches.UI.Menu
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class MainMenuPatches
    {

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void OnStart(MainMenu __instance)
        {
            if (OptionsDataManager.ExtraSettings.showNotif && !__instance.TryGetComponent(out NotifiedMainMenu _))
            {
                Image image = UIHelpers.CreateImage(AssetsStorage.sprites["about_notif"], __instance.transform, 
                    Vector3.zero, correctPosition: false);
                image.ToCenter();

                image.transform.localPosition = new Vector3(-13f, 33f, 0f);

                image.transform.SetSiblingIndex(CursorController.Instance.transform.GetSiblingIndex());

                Quaternion quaternion = image.transform.rotation;
                quaternion.eulerAngles = new Vector3(0f, 180f, 0f);
                image.transform.rotation = quaternion;

                __instance.gameObject.AddComponent<NotifiedMainMenu>().notifImage = image;
            }

            if (AdvancedCore.updateChecksEnabled)
            {
                Image updateCenterImage = UIHelpers.CreateImage(AssetsStorage.sprites["adv_arrows"], __instance.transform,
                    Vector3.zero, correctPosition: false);
                updateCenterImage.ToCenter();

                updateCenterImage.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
        }

    }
}
