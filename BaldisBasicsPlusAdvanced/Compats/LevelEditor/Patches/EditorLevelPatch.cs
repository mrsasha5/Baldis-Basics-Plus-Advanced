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
            __instance.defaultTextures.Add("adv_school_council_class", new TextureContainer("adv_basic_floor", "adv_school_council_wall", "Ceiling"));
            __instance.defaultTextures.Add("adv_advanced_class", new TextureContainer("adv_advanced_class_floor", "adv_advanced_class_wall", "adv_advanced_class_ceiling"));
            __instance.defaultTextures.Add("adv_corn_field", new TextureContainer("adv_corn_floor", "adv_corn_wall", "None"));
        }

    }
}
