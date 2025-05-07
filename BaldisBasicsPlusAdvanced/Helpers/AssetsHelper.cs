using System;
using MTM101BaldAPI.AssetTools;
using UnityEngine;
using System.Linq;
using BepInEx.Bootstrap;
using UnityEngine.Networking;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    class AssetsHelper
    {

        public static Sprite SpriteFromFile(string path, float pixelsPerUnit = 1f)
        {
            Texture2D texture = AssetLoader.TextureFromFile(modPath + path);
            Sprite sprite = AssetLoader.SpriteFromTexture2D(texture, Vector2.one/2f, pixelsPerUnit);
            return sprite;
        }

        public static Texture2D TextureFromFile(string path, float pixelsPerUnit = 1f)
        {
            Texture2D texture = AssetLoader.TextureFromFile(modPath + path);
            return texture;
        }

        public static bool ModInstalled(string mod)
        {
            return Chainloader.PluginInfos.ContainsKey(mod);
        }

        public static T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            return (from x in Resources.FindObjectsOfTypeAll<T>()
                    where x.name == name
                    select x).First();
        }

        public static T[] LoadAssets<T>(string name) where T : UnityEngine.Object
        {
            return Array.FindAll(Resources.FindObjectsOfTypeAll<T>(), x => x.name == name);
        }

        public static T[] LoadAssets<T>() where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>();
        }

        public static AudioClip GetAudioFromFile(string path)
        {
            AudioClip clip = AssetLoader.AudioClipFromMod(AdvancedCore.Instance, path);
            return clip;
        }

        public static string modPath = "BALDI_Data/StreamingAssets/Modded/" + AdvancedCore.modId + "/";
    }
}
