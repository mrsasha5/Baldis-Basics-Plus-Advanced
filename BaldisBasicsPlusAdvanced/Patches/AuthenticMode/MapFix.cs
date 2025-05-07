using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BaldisBasicsPlusAdvanced.Patches.AuthenticMode
{
    [HarmonyPatch(typeof(CoreGameManager))]
    internal class MapFix
    {
        private static bool mapIsOpened;

        public static bool MapIsOpened => mapIsOpened;

        //fix when authentic mode active
        [HarmonyPatch("OpenMap")]
        [HarmonyPostfix]
        private static void onOpenMap(CoreGameManager __instance)
        {
            if (!__instance.disablePause && Singleton<PlayerFileManager>.Instance.authenticMode && ApiManager.AuthenticModeExtended)
            {
                mapIsOpened = true;

                __instance.GetHud(0).Hide(val: false);
                Map map = __instance.GetPlayer(0).ec.map;
                Canvas padCanvas = ReflectionHelper.getValue<Canvas>(map, "padCanvas");
                padCanvas.gameObject.SetActive(false);

                /*Camera camera = Singleton<BaseGameManager>.Instance.Ec.map.cams[0];
                UniversalAdditionalCameraData additionalCameraData = camera.GetUniversalAdditionalCameraData();

                additionalCameraData.renderType = CameraRenderType.Base;*/
            }
        }

        [HarmonyPatch("CloseMap")]
        [HarmonyPostfix]
        private static void onCloseMap(CoreGameManager __instance)
        {
            if (!__instance.disablePause && Singleton<PlayerFileManager>.Instance.authenticMode)
            {
                mapIsOpened = false;
                //Camera camera = Singleton<BaseGameManager>.Instance.Ec.map.cams[0];
                //UniversalAdditionalCameraData additionalCameraData = camera.GetUniversalAdditionalCameraData();

                //additionalCameraData.renderType = CameraRenderType.Overlay;
            }
        }

    }
}
