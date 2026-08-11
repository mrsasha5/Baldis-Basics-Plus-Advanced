using System;
using System.Collections.Generic;
using System.IO;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using Newtonsoft.Json;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SerializableData
{
    [JsonObject]
    public class CellTextureSerializableData
    {
        public Dictionary<int, int> weights;

        public int[] bannedFloors;

        public string[] types;

        public string[] levelTypes;

        public bool endlessMode;

        public bool replacementWall;

        [NonSerialized]
        public Texture2D tex;

        public static CellTextureSerializableData LoadFrom(string path)
        {
            string jsonPath = path.Replace(".png", ".json");

            if (!File.Exists(jsonPath)) return null;

            CellTextureSerializableData instance = 
                JsonConvert.DeserializeObject<CellTextureSerializableData>(File.ReadAllText(jsonPath));
            instance.tex = AssetStorage.LoadModTexture(Path.GetFileNameWithoutExtension(path), path, overrideBasePath: true);
            return instance;
        }

    }
}
