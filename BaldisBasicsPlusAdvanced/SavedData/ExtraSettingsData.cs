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

        [NonSerialized]
        public bool authenticMode;

        public void setDefaults()
        {
            tipsEnabled = true;
            firstPrizeFeaturesEnabled = false;
        }
    }
}
