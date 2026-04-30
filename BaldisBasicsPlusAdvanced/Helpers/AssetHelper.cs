using BepInEx.Bootstrap;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class AssetHelper
    {
        public static Texture2D LoadEmbeddedTexture(string name, TextureFormat format = TextureFormat.RGBA32)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            Texture2D tex = null;
            using (BinaryReader reader = new BinaryReader(stream))
            {
                tex = new Texture2D(2, 2, format, mipChain: false);
                tex.name = Path.GetFileNameWithoutExtension(name);
                tex.LoadImage(reader.ReadBytes((int)stream.Length));
                tex.filterMode = FilterMode.Point;
            }
            stream.Dispose();
            return tex;
        }

        public static Sprite SpriteFromFile(string path, float pixelsPerUnit = 1f, Vector2? center = null, bool overrideBasePath = false)
        {
            Texture2D texture = AssetLoader.TextureFromFile(overrideBasePath ? path : modPath + path);
            Sprite sprite = null;
            if (center == null)
                sprite = AssetLoader.SpriteFromTexture2D(texture, Vector2.one/2f, pixelsPerUnit);
            else sprite = AssetLoader.SpriteFromTexture2D(texture, (Vector2)center, pixelsPerUnit);
            return sprite;
        }

        public static Texture2D TextureFromFile(string path, bool overrideBasePath = false)
        {
            Texture2D texture = AssetLoader.TextureFromFile(overrideBasePath ? path : modPath + path);
            return texture;
        }

        public static SoundObject SoundObjectFromFile(
            string path, string subtitle, SoundType type, Color color, float sublength = -1f)
        {
            return ObjectCreators.CreateSoundObject(AudioFromFile(path), subtitle, type, color, sublength);
        }

        public static bool ModInstalled(string mod)
        {
            return Chainloader.PluginInfos.ContainsKey(mod);
        }

        public static T LoadAsset<T>() where T : UnityEngine.Object
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsHelper.LoadAsset<T>()\nLoading asset of type: " + typeof(T).FullName);
#endif
            return (from x in Resources.FindObjectsOfTypeAll<T>()
                    select x).First();
        }

        public static T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsHelper.LoadAsset<T>(string)\nLoading asset: " + name);
#endif
            return (from x in Resources.FindObjectsOfTypeAll<T>()
                    where x.name == name
                    select x).First();
        }

        public static T[] LoadAssets<T>(string name) where T : UnityEngine.Object
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsHelper.LoadAssets<T>(string)\nLoading assets: " + name);
#endif
            return Array.FindAll(Resources.FindObjectsOfTypeAll<T>(), x => x.name == name);
        }

        public static T[] LoadAssets<T>() where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>();
        }

        public static List<SoundObject> LoadSounds(string path, string baseName, SoundType type, Color? color = null, string extension = "wav")
        {
            List<SoundObject> sounds = new List<SoundObject>();
            int counter = 1;
            while (File.Exists(modPath + "Audio/" + path + "/Adv_" + baseName + counter + "." + extension))
            {
                AudioClip audClip = AudioFromFile("Audio/" + path + "/Adv_" + baseName + counter + "." + extension);
                sounds.Add(ObjectCreators.CreateSoundObject(
                    audClip, "Adv_Sub_" + baseName + counter, type, color != null ? (Color)color : Color.white));
                counter++;
            }
            return sounds;
        }

        public static AudioClip AudioFromFile(string path)
        {
            AudioClip clip = AssetLoader.AudioClipFromFile(modPath + path);
            return clip;
        }

        public static string modPath = AssetLoader.GetModPath(AdvancedCore.Instance) + "/";
    }
}
