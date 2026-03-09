using BaldisBasicsPlusAdvanced.Extensions;
using UnityEngine;
using static BBPRewiredCompat.RewiredPlusManager;

namespace BaldisBasicsPlusAdvanced.Compats.RewiredCustomManager
{
    internal class RewiredPlusIntegration : CompatibilityModule
    {
        private static int cat;

        public static RewiredPlusIntegration Instance { get; private set; }

        public RewiredPlusIntegration() : base()
        {   
            guid = "alexbw145.bbplus.rewiredcompat";
            versionInfo = new VersionInfo(this)
                .SetMinVersion("1.1.0.2", exceptCurrent: false);
            CreateConfigValue("Rewired Plus Manager",
                "API that helps adding custom key bindings through Rewired library which is used by the game. Very useful for people with controllers.");
        }

        protected override void OnModLoadingStarted()
        {
            base.OnModLoadingStarted();
            Instance = this;
            cat = (int)CreateNewCategory($"{AdvancedCore.modId}", "Adv_RewiredCategory".Localize(), InputMapPage.Gameplay);
        }

        public static void AddButtonBind(string id, string locName, KeyCode key)
        {
            // Platform-specific issue occurs when I use InputMapCategory as field type without soft dependency
            // So I am supposed to store it as int and cast here each time
            // I think it's related to Reflection I use actively for manage modules
            // And Reflection works different sometimes on different platforms from my experience with Mono
            CreateNewInput($"{AdvancedCore.modId}:{id}", locName, InputBehaviorID.Snap, (InputMapCategory)cat, key);
        }
    }
}
