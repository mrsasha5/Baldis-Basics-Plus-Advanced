using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.GameManager;
using BBPlusCustomMusics.Plugin.Public;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Compats.CustomMusics
{
    internal class CustomMusicsIntegration : CompatibilityModule
    {

        public CustomMusicsIntegration() : base()
        {
            guid = "pixelguy.pixelmodding.baldiplus.custommusics";
            versionInfo = new VersionInfo(this)
                .SetMinVersion("1.1.2", exceptCurrent: false);

            CreateConfigValue("Custom Musics",
                "All required music will be synchronized with the internal system of this modification to provide better compatibility.");
        }

        protected override void OnAssetsPostLoad()
        {
            base.OnAssetsPostLoad();

            MIDIHolder[] holders = 
                MusicRegister.AddMIDIsFromDirectory(MidiDestiny.Schoolhouse, AssetsHelper.modPath + "Audio/Music/Floors");
            holders = holders.AddRangeToArray(
                MusicRegister.AddMIDIsFromDirectory(MidiDestiny.Schoolhouse, AssetsHelper.modPath + "Audio/Music/Floors/Compats"));

            for (int i = 0; i < holders.Length; i++)
            {
                holders[i].allowedFloors = null;
                if (holders[i].allowedLevelTypes != null)
                {
                    foreach (LevelType type in holders[i].allowedLevelTypes)
                    {
                        MusicPatch.Insert(holders[i].MidiName, type);
                    }
                }
                
            }
        }

    }
}
