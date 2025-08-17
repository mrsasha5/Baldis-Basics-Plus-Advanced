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

    }
}
