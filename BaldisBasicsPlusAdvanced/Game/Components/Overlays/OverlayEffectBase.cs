using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.Overlays
{
    public class OverlayEffectBase
    {
        protected Image overlay;

        protected float time;

        private bool playing;

        public virtual bool DestroyOverlayOnEnd => false;

        public virtual bool Destroy => time < 0;

        public bool Playing => playing;

        public void initialize(Image image, float time)
        {
            overlay = image;
            this.time = time;
        }

        public virtual void onPlayStart()
        {
            playing = true;
        }

        public virtual void virtualUpdate()
        {
            if (time > 0 && playing) time -= Time.deltaTime; 
        }

        public virtual void onDestroy()
        {

        }

    }
}
