using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.Overlays
{
    public class FadeOutEffect : OverlayEffectBase
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
            percent = time / maxTime;
            overlay.color = new Color(1f, 1f, 1f, percent);
        }

    }
}
