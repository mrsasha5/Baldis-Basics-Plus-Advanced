using BaldiLevelEditor;
using BaldisBasicsPlusAdvanced.Attributes;
using HarmonyLib;
using PlusLevelFormat;

namespace BaldisBasicsPlusAdvanced.Compats.LevelEditor.Patches
{
    [HarmonyPatch(typeof(EditorLevel))]
    [ConditionalPatchIntegratedMod(ModsIntegration.levelEditorId)]
    internal class EditorLevelPatch
    {

        [HarmonyPatch("InitializeDefaultTextures")]
        [HarmonyPostfix]
        private static void OnInitializeDefaultTextures(EditorLevel __instance)
        {
            __instance.defaultTextures.Add("adv_english_class", new TextureContainer("adv_english_floor", "adv_english_wall", "adv_english_ceiling"));
            __instance.defaultTextures.Add("adv_english_class_timer", new TextureContainer(__instance.defaultTextures["adv_english_class"]));
        }

    }
}
