using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay
{
    public class OverlayEffectBase
    {
        protected Image overlay;

        protected float time;

        private bool playing;

        public virtual bool DestroyOverlayOnEnd => false;

        public virtual bool Destroy => time < 0;

        public bool Playing => playing;

        public void Initialize(Image image, float time)
        {
            overlay = image;
            this.time = time;
        }

        public virtual void OnPlayStart()
        {
            playing = true;
        }

        public virtual void VirtualUpdate()
        {
            if (time > 0 && playing) time -= Time.deltaTime;
        }

        public virtual void OnDestroy()
        {

        }

    }
}
