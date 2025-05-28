using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.SaveSystem.Data
{
    //[Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class ExtraSettingsData
    {
        internal float saveVersion;

        internal Dictionary<string, object> parameters = new Dictionary<string, object>();

        [NonSerialized]
        internal bool showNotif;

        //not needed now
        /*internal static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            //Gets or sets how reference loops (e.g. a class referencing itself) are handled. The default value is Error.
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
        };*/

        public T GetValue<T>(string key)
        {
            return (T)parameters[key];
        }

        public void SetValue(string key, object value)
        {
            if (value.GetType() != parameters[key].GetType()) throw new Exception("Different types detected! Cannot set the value " + value.ToString() + " for " + key);
            parameters[key] = value;
        }

        internal void CheckValues()
        {
            CheckValue("tips", true);
            CheckValue("tips_during_game", false);
            CheckValue("particles", true);
            CheckValue("first_prize_extensions", false);
        }

        private void CheckValue(string key, object value)
        {
            if (parameters == null) parameters = new Dictionary<string, object>();
            if (!parameters.ContainsKey(key)) parameters.Add(key, value);
            else if (value.GetType() != parameters[key].GetType())
            {
                AdvancedCore.Logging.LogWarning("Different types in the Extra Settings data have detected! Setting default value for " + key);
                parameters[key] = value;
            }
        }
    }
}
