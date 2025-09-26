using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{
    [Serializable]
    public class PlateData
    {
        public float timeToUnpress;

        public bool targetsPlayer;

        public bool showsUses;

        public bool showsCooldown;

        public bool allowsToCopyTextures;

        //public bool hasLight;

        //public Color lightColor;

        //public int lightStrength;

        public int uses;

        public bool hasInfinityUses;

        public void SetUses(int uses)
        {
            if (uses >= 0)
            {
                hasInfinityUses = false;
                this.uses = uses;
            } else
            {
                hasInfinityUses = true;
            }
        }

    }
}
