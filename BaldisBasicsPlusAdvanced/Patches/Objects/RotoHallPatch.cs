using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Objects
{
    [HarmonyPatch(typeof(RotoHall))]
    internal class RotoHallPatch
    {

        private static float oldSpeed = 25f;

        [HarmonyPatch("Setup")]
        [HarmonyPostfix]
        private static void OnSetup(RotoHall __instance, ref float ___speed, ref SoundObject ___audTurn, ref MeshRenderer ___cylinder)
        {
            if (ControlledRNG.GenAvailable && ControlledRNG.Object.Next(0, 101) <= 50)
            {
                ___speed = oldSpeed;
                ___audTurn = AssetsStorage.sounds["adv_rotohall_start"];

                Material[] materials = ___cylinder.materials;
                materials[0].SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall"]);
                materials[1].SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_sign_left"]);
                materials[2].SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_sign_right"]);
                ___cylinder.materials = materials;

                __instance.GetComponent<MeshRenderer>().material.SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_blank"]);

                Array.Find(__instance.GetComponentsInChildren<MeshRenderer>(), x => x.name == "CylinderFloor_Model").material.
                    SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_floor"]);
            }
        }

        [HarmonyPatch("Rotate")]
        [HarmonyPrefix]
        private static void OnRotate(RotoHall __instance, ref bool ___moving, ref AudioManager ___audMan, ref float ___speed,
            ref SoundObject ___audTurn)
        {
            if (___moving || ___speed != oldSpeed) return;
            ___audMan.FlushQueue(endCurrent: true);
            __instance.StartCoroutine(WaitToOverrideAudMan(___audMan, ___audTurn));
        }

        /*[HarmonyPatch("Rotator")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RotatorAudioPatch(IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Transpiler patches...");
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;
            }
        }*/

        private static IEnumerator WaitToOverrideAudMan(AudioManager audMan, SoundObject soundObj)
        {
            while (!audMan.QueuedAudioIsPlaying)
            {
                yield return null;
            }

            audMan.QueueAudio(AssetsStorage.sounds["adv_rotohall_loop"]);
            audMan.SetLoop(true);

            yield break;
        }


        [HarmonyPatch("Unblock")]
        [HarmonyPostfix]
        private static void OnUnblock(ref AudioManager ___audMan, ref float ___speed)
        {
            if (___speed != oldSpeed) return;
            ___audMan.FlushQueue(endCurrent: true);
            ___audMan.QueueAudio(AssetsStorage.sounds["adv_rotohall_end"]);
        }

    }
}
