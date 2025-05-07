/*using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI.PlusExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches
{

    [HarmonyPatch(typeof(Elevator))]
    internal class TestPatch
    {
        public static SoundObject intro;

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void onInitialize(Elevator __instance)
        {
            Sprite sprite = AssetsHelper.spriteFromFile("Textures/modLogo.png", 150f);
            //__instance.transform.position + __instance.transform.forward * 40f + Vector3.up * 5f;
            Vector3 pos = __instance.transform.position + __instance.transform.forward * -5f + Vector3.up * 5f;
            SpriteRenderer renderer = ObjectsCreator.createSpriteRenderer(sprite);
            renderer.transform.position = pos;
            renderer.transform.rotation = __instance.transform.rotation;

            if (intro == null) intro = AssetsHelper.loadAsset<SoundObject>("BAL_HideSeek");
        }

    }

    [HarmonyPatch(typeof(EnvironmentController))]
    internal class PlayerTestPatch
    {

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        private static void onBeginPlay()
        {
            Singleton<CoreGameManager>.Instance.GetHud(0).Hide(true);

            PlayerMovementStatModifier modifier = Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier();
            modifier.baseStats["walkSpeed"] = 7f;

            BaldiIntroRepeater repeater = new GameObject("BaldiIntroRepeater").AddComponent<BaldiIntroRepeater>();

            HappyBaldi baldi = GameObject.FindObjectOfType<HappyBaldi>();
            ReflectionHelper.setValue<SoundObject>(baldi, "audIntro", TestPatch.intro);
        }

    }

    internal class BaldiIntroRepeater : MonoBehaviour
    {

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                HappyBaldi baldi = FindObjectOfType<HappyBaldi>();
                ReflectionHelper.getValue<Animator>(baldi, "animator").Play("BAL_Wave", -1, 0f);
            }
        }

    }
}
*/