using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.SavedData
{
    [Serializable]
    public class ExtraSettingsData
    {
        public float saveVersion;

        public bool tipsEnabled;

        public bool firstPrizeFeaturesEnabled;

        public bool particlesEnabled;

        [NonSerialized]
        public bool showNotif;

        public void SetDefaults()
        {
            tipsEnabled = true;
            firstPrizeFeaturesEnabled = false;
            particlesEnabled = true;
        }
    }
}
