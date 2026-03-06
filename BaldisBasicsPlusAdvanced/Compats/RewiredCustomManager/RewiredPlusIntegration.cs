using BaldisBasicsPlusAdvanced.Extensions;
using BBPRewiredCompat;
using UnityEngine;
using static BBPRewiredCompat.RewiredPlusManager;

namespace BaldisBasicsPlusAdvanced.Compats.RewiredCustomManager
{
    internal class RewiredPlusIntegration : CompatibilityModule
    {
        private static InputMapCategory cat;

        public static RewiredPlusIntegration Instance { get; private set; }

        public RewiredPlusIntegration() : base()
        {   
            guid = RewiredPlusPlugin.GUID;
            versionInfo = new VersionInfo(this)
                .SetMinVersion("1.1.0.2", exceptCurrent: false);
            CreateConfigValue("Rewired Plus Manager",
                "API that helps adding custom key bindings through Rewired library which is used by the game. Very useful for people with controllers.");
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();
            if (IsIntegrable())
            {
                Instance = this;
                cat = CreateNewCategory($"{AdvancedCore.modId}", "Adv_RewiredCategory".Localize(), InputMapPage.Gameplay);
            }
        }

        public static void AddButtonBind(string id, string locName, KeyCode key)
        {
            CreateNewInput($"{AdvancedCore.modId}:{id}", locName, InputBehaviorID.Snap, cat, key);
        }
    }
}
