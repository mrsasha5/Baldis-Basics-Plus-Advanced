using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SaveSystem.Data
{
    public class KeyBindingData : ICloneable
    {
        private string id;

        private string locNameKey;

        private string locDescKey;

        private KeyCode button;

        public string Id => id;

        public string LocNameKey => locNameKey;

        public string LocDescKey => locDescKey;

        public KeyCode Button => button;

        public KeyBindingData(string id, string nameKey, string descKey, KeyCode button)
        {
            this.id = id;
            locNameKey = nameKey;
            locDescKey = descKey;
            this.button = button;
        }

        public void OverrideButton(KeyCode keyBinding)
        {
            button = keyBinding;
        }

        public object Clone()
        {
            return new KeyBindingData(id, locNameKey, locDescKey, button);
        }

        /*public override bool Equals(object obj)
        {
            if (obj is KeyBindingData) return this == (KeyBindingData)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, locNameKey, locDescKey, button);
        }

        public static bool operator ==(KeyBindingData data1, KeyBindingData data2)
        {
            return (data1.id == data2.id) && (data1.locNameKey == data2.locNameKey) && (data1.locDescKey == data2.locDescKey) && (data1.button == data2.button);
        }

        public static bool operator !=(KeyBindingData data1, KeyBindingData data2)
        {
            return (data1.id != data2.id) && (data1.locNameKey != data2.locNameKey) && (data1.locDescKey != data2.locDescKey) && (data1.button != data2.button);
        }*/


    }
}
