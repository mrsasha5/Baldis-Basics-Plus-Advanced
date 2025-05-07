using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay
{
    public class FadeOutEffect : OverlayEffectBase
    {
        private float maxTime;

        private float percent = 0f;

        public override void OnPlayStart()
        {
            base.OnPlayStart();
            maxTime = time;
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            percent = time / maxTime;
            overlay.color = new Color(1f, 1f, 1f, percent);
        }

    }
}
