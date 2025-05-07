using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.Overlays
{
    public class FadeInEffect : OverlayEffectBase
    {
        private float maxTime;

        private float percent = 0f;

        public override void onPlayStart()
        {
            base.onPlayStart();
            maxTime = time;
        }

        public override void virtualUpdate()
        {
            base.virtualUpdate();
            percent = (maxTime - time) / maxTime;
            overlay.color = new Color(1f, 1f, 1f, percent);
        }

    }
}
