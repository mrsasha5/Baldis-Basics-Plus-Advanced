using BaldiLevelEditor;
using HarmonyLib;
using MTM101BaldAPI;
using PlusLevelFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.Patches
{
    [HarmonyPatch(typeof(PlusLevelEditor))]
    [ConditionalPatchMod(ModsIntegration.LevelEditorId)]
    internal class PlusLevelEditorPatch
    {
        [HarmonyPatch("LoadTempPlay")]
        [HarmonyPrefix]
        private static void onLoadTempPlay(PlusLevelEditor __instance)
        {
            PrefabLocation authenticEditor = __instance.level.prefabs.Find(x => x.prefab == "adv_authentic_mode_label");
            if (authenticEditor != null) {
                Singleton<PlayerFileManager>.Instance.authenticMode = true;
            }
        }

        /*[HarmonyPatch("CompileLevel")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> test(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int startIndex = -1;
            int endIndex = -1;

            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instruction = codes[i];
                if (instruction.opcode == OpCodes.Ldloc_1)
                {
                    for (int j = i; j < codes.Count; j++)
                    {

                    }
                }
            }

            if (startIndex > -1 && endIndex > -1)
            {
                for (int i = 0; i < codes.Count; i++)
                {

                }
            }

            return codes.AsEnumerable();
        }*/
    }
}
