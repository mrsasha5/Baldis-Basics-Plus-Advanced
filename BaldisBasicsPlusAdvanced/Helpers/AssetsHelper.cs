using System;
using System.Collections.Generic;
using MTM101BaldAPI.AssetTools;
using UnityEngine;
using System.Linq;
using BepInEx.Bootstrap;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    class AssetsHelper
    {
        public static Sprite spriteFromFile(string path, float pixelsPerUnit = 1f)
        {
            Texture2D texture = AssetLoader.TextureFromFile(modPath + path);
            Sprite sprite = AssetLoader.SpriteFromTexture2D(texture, Vector2.one/2f, pixelsPerUnit);
            return sprite;
        }

        public static Texture2D textureFromFile(string path, float pixelsPerUnit = 1f)
        {
            Texture2D texture = AssetLoader.TextureFromFile(modPath + path);
            return texture;
        }

        public static bool modInstalled(string mod)
        {
            return Chainloader.PluginInfos.ContainsKey(mod);
        }

        public static T loadAsset<T>(string name) where T : UnityEngine.Object
        {
            return (from x in Resources.FindObjectsOfTypeAll<T>()
                    where x.name == name
                    select x).First();
        }

        public static T[] loadAssets<T>(string name) where T : UnityEngine.Object
        {
            return Array.FindAll(Resources.FindObjectsOfTypeAll<T>(), x => x.name == name);
        }

        public static AudioClip getAudioFromFile(string path)
        {
            AudioClip clip = AssetLoader.AudioClipFromMod(AdvancedCore.Instance, path);
            return clip;
        }

        public static string modPath = "BALDI_Data/StreamingAssets/Modded/" + AdvancedCore.modId + "/";
    }
}
