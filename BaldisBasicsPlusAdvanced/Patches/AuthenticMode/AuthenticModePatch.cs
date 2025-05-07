using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Patches.AuthenticMode
{
    [HarmonyPatch(typeof(CoreGameManager))]
    internal class AuthenticModePatch
    {
        private static AuthenticScreenManager authenticScreen;

        [HarmonyPatch("PlayBegins")]
        [HarmonyPostfix]
        private static void onPlay(CoreGameManager __instance)
        {
            if (Singleton<PlayerFileManager>.Instance.authenticMode && ApiManager.AuthenticModeExtended)
            {
                Vector2[] positions = new Vector2[] {
                    new Vector2(28f, 139f),
                    new Vector2(74f, 139f),
                    new Vector2(118f, 139f),
                    new Vector2(164f, 139f),
                    new Vector2(210f, 139f),
                };

                authenticScreen = __instance.authenticScreen;

                RectTransform itemsParent = Array.Find(authenticScreen.GetComponentsInChildren<RectTransform>(), x => x.name == "Items");

                StandardMenuButton basePrefabButtonInstance = Array.Find(authenticScreen.GetComponentsInChildren<StandardMenuButton>(), x => x.name == "ItemButton_0");

                for (int n = 3; n <= 4; n++)
                {
                    StandardMenuButton button = UnityEngine.Object.Instantiate(basePrefabButtonInstance);
                    int counter = n;
                    button.name = "ItemButton_" + n;
                    button.transform.SetParent(itemsParent.transform, false);
                    button.OnPress = new UnityEvent();
                    button.OnPress.AddListener(delegate {
                        setItem(counter);
                    });
                    button.transform.localPosition = positions[n];
                }
            }
        }

        private static void setItem(int num)
        {
            Debug.Log(num);
            authenticScreen.UseItem(num);
        }
    }
}
