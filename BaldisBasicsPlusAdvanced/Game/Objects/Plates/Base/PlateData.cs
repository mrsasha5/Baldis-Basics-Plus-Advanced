using System;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{
    [Serializable]
    public class PlateData
    {
        public float timeToUnpress;

        public bool showsUses;

        public bool showsCooldown;

        public bool allowsToCopyTextures;

        //It means if is plate supposed to use cooldown usually
        public bool initiallyHasCooldown;

        //public bool hasLight;

        //public Color lightColor;

        //public int lightStrength;

        public int maxUses;

        public bool hasInfinityUses;

        public void SetUses(int uses)
        {
            if (uses > 0)
            {
                hasInfinityUses = false;
                this.maxUses = uses;
            } else
            {
                hasInfinityUses = true;
            }
        }

        public void MarkAsCooldownPlate(bool hideCounter = false)
        {
            if (!hideCounter) showsCooldown = true;
            initiallyHasCooldown = true;
        }

    }
}
