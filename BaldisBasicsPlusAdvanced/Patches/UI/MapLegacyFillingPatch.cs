using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.UI
{
    //It was removed in 0.10 btw
    [HarmonyPatch(typeof(Map))]
    internal class MapLegacyFillingPatch
    {

        private static Map map;

        private static IntVector2 _gridPosition;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void OnAwake(Map __instance)
        {
            map = __instance;
        }

        [HarmonyPatch("Update")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> FillOnlyOpenAreas(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher();
            ReflectionHelper.SetValue(
                matcher, "codes", 
                instructions.Select((CodeInstruction c) => new CodeInstruction(c)).ToList()); //For context:
                                                                                              //I can't use constructor
                                                                                              //Since it contains ILGenerator variable
            CodeInstruction hackyInstruction = matcher
                .MatchForward(false, new CodeMatch(OpCodes.Beq))
                .InstructionAt(0);
            ReflectionHelper.SetValue(hackyInstruction, "opcode", OpCodes.Brfalse_S); //For context: I can't use Label class

            matcher
                .MatchBack(false, new CodeMatch(OpCodes.Ldarg_0))
                .Advance(-6);
                //.MatchBack(false, new CodeMatch(OpCodes.Ldarg_0)) //I SPENT MANY TIME ON DEBUGGING TO FIND THAT THIS SHIT IS NOT WORKING
                //.MatchBack(false, new CodeMatch(OpCodes.Ldarg_0)); //AS I THOUGHT

            if (matcher.IsInvalid)
            {
                AdvancedCore.Logging.LogWarning("Cancelling transpiler since matcher is invalid!");
                return instructions;
            }

            matcher.RemoveInstructions(14);

            if (matcher.IsInvalid)
            {
                AdvancedCore.Logging.LogWarning("Cancelling transpiler since matcher is invalid!");
                return instructions;
            }

            for (int i = 0; i < 12; i++)
            {
                matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Nop));
            }

            MethodInfo staticMethod = AccessTools.Method(typeof(MapLegacyFillingPatch), "CanFillAllTiles");

            matcher
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, staticMethod))
                .InsertAndAdvance(hackyInstruction);

            if (matcher.Length != 874)
            {
                AdvancedCore.Logging.LogWarning(
                    "Detected wrong instructions length in MapLegacyFillingPatch!\n" +
                    "Is game codebase updated or somebody joined to my party?");
            }

            return matcher.InstructionEnumeration();
        }

        private static bool CanFillAllTiles()
        {
            _gridPosition = IntVector2.GetGridPosition(map.targets[0].transform.position);

            return map.Ec.cells[_gridPosition.x, _gridPosition.z].room.type != RoomType.Hall 
                && map.Ec.cells[_gridPosition.x, _gridPosition.z].room.functions.name != "CornFieldFunctionContainer(Clone)";
        }

    }
}
