using MTM101BaldAPI.SaveSystem;
using Newtonsoft.Json;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SaveSystem
{
    // I did it because the Unity JSON Serializer isn't good 
    public abstract class ModdedSaveGameIOJsonNet<T> : ModdedSaveGameIOText
    {

        protected JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };

        public abstract T GetObjectToSave();

        public abstract void OnObjectLoaded(T loadedObject);

        public override string SaveText()
        {
            return JsonConvert.SerializeObject(GetObjectToSave(), serializerSettings);
        }

        public override void LoadText(string toLoad)
        {
            OnObjectLoaded(JsonConvert.DeserializeObject<T>(toLoad));
        }
    }
}
