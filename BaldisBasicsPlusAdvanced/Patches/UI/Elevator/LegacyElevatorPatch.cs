using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.QualityOfPlus;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using MTM101BaldAPI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.UI.Elevator
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    [ConditionalPatchNoMod(IntegrationManager.QOP_ID)]
    internal class LegacyElevatorPatch
    {
        private static FieldInfo _buttonAnimator = AccessTools.Field(typeof(ElevatorScreen), "buttonAnimator");
        private static FieldInfo _buttonLabel = AccessTools.Field(typeof(ElevatorScreen), "buttonLabel");
        private static FieldInfo _skipButton = AccessTools.Field(typeof(ElevatorScreen), "skipButton");
        private static MethodInfo _startGame = AccessTools.Method(typeof(ElevatorScreen), "StartGame");

        [HarmonyPatch("AwakeFunction")]
        [HarmonyPriority(Priority.LowerThanNormal)] // Must be called after Expel Hammer
        [HarmonyPostfix]
        private static void ControlElevatorLock(ElevatorScreen __instance)
        {
            if (ElevatorExpelHammerPatch.Available && IntegrationManager.IsActive<QualityOfPlusIntegration>())
            {
                QualityOfPlusIntegration.LockStart(true);
                __instance.GetComponent<ElevatorAdditionsPatch.UnityEventsTracker>().onDestroy += () => 
                    QualityOfPlusIntegration.LockStart(false);
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> DoNotStartGameAndInjectUpdate(IEnumerable<CodeInstruction> instructions)
        {
            bool successful = false;
            List<CodeInstruction> _instructions = instructions.Select((CodeInstruction c) => new CodeInstruction(c)).ToList();
            for (int i = 0; i < _instructions.Count; i++)
            {
                if (_instructions[i].opcode == OpCodes.Ldarg_0 && i + 1 < _instructions.Count &&
                    _instructions[i + 1].operand?.ToString() == "Void StartGame()")
                {
                    AdvancedCore.Logging.LogInfo("LegacyElevatorPatch: StartGame is found, removing this method.");
                    _instructions.RemoveAt(i);
                    _instructions.RemoveAt(i);
                    _instructions.Insert(i, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LegacyElevatorPatch), 
                        "RevealPlayButton")));
                    successful = true;
                }
            }
            if (successful)
            {
                AdvancedCore.Logging.LogInfo("LegacyElevatorPatch: defects are not detected. Guessing everything was patched well.");
                return _instructions;
            }
            else
            {
                AdvancedCore.Logging.LogWarning("LegacyElevatorPatch is failed to patch elevator! Reverting changes.");
                return instructions;
            }
        }

        private static void RevealPlayButton()
        {
            if (ExtraSettingsManager.ExtraSettings.GetValue<bool>("legacy_elevator") || ElevatorExpelHammerPatch.Available)
            {
                Animator buttonAnimator = (Animator)_buttonAnimator.GetValue(ElevatorAdditionsPatch.elvScreen);
                TMP_Text buttonLabel = (TMP_Text)_buttonLabel.GetValue(ElevatorAdditionsPatch.elvScreen);
                buttonAnimator.Play("ButtonRise", -1, 0f);
                buttonLabel.text = LocalizationManager.Instance.GetLocalizedText("But_Play!");
                ((GameObject)_skipButton.GetValue(ElevatorAdditionsPatch.elvScreen))
                    .SetActive(CoreGameManager.Instance.sceneObject.skippable);
            }
            else
            {
                _startGame.Invoke(ElevatorAdditionsPatch.elvScreen, null);
            }
        }
    }
}
