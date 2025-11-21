using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
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
                Image image = UIHelpers.CreateImage(AssetStorage.sprites["about_notif"], __instance.transform, 
                    Vector3.zero, correctPosition: false);
                image.ToCenter();

                image.transform.localPosition = new Vector3(-13f, 33f, 0f);

                image.transform.SetSiblingIndex(CursorController.Instance.transform.GetSiblingIndex());

                Quaternion quaternion = image.transform.rotation;
                quaternion.eulerAngles = new Vector3(0f, 180f, 0f);
                image.transform.rotation = quaternion;

                __instance.gameObject.AddComponent<NotifiedMainMenu>().notifImage = image;
            }

            /*if (AdvancedCore.updateChecksEnabled)
            {
                StandardMenuButton updatesCenterButton = ObjectsCreator.CreateSpriteButton(
                    AssetsStorage.sprites["adv_arrows"], 
                    new Vector3(-217f, 153f, 0f), __instance.transform);
                updatesCenterButton.transform.localScale = Vector3.one * 0.5f;
                updatesCenterButton.OnPress.AddListener(delegate ()
                {
                    
                });
                updatesCenterButton.transform.SetSiblingIndex(__instance.transform.childCount - 3);
            }*/
        }

    }
}
