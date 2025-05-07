using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{
    public struct PlateData
    {
        public float timeToUnpress;

        public bool targetPlayer;

        //public PlateTargets[] targets;

        public bool showUses;

        public bool showCooldown;

        public int uses;

        public bool infinityUses;

        /*public void setTargets(params PlateTargets[] targets)
        {
            this.targets = targets;
        }*/

        public void setUses(int uses)
        {
            if (uses >= 0)
            {
                infinityUses = false;
                this.uses = uses;
            } else
            {
                infinityUses = true;
            }
        }

    }
}
