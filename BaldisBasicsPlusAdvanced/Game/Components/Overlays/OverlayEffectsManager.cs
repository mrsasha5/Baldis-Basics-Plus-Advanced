using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.Overlays
{
    public class OverlayEffectsManager : MonoBehaviour
    {
        [SerializeField]
        private Image overlayImage;

        private List<OverlayEffectBase> quededEffects = new List<OverlayEffectBase>();

        public void initializePrefab(Image image)
        {
            overlayImage = image;
        }

        private void Update()
        {
            if (quededEffects.Count > 0) {
                if (!quededEffects[0].Playing) quededEffects[0].onPlayStart();
                quededEffects[0].virtualUpdate();
                if (quededEffects[0].Destroy)
                {
                    OverlayEffectBase effectToDestroy = quededEffects[0];
                    effectToDestroy.onDestroy();
                    quededEffects.RemoveAt(0);

                    if (effectToDestroy.DestroyOverlayOnEnd)
                    {
                        clearQueue(true);
                        Destroy(this.gameObject);
                        return;
                    }
                }
            }
        }

        public void queueEffect<T>(float time) where T : OverlayEffectBase, new()
        {
            T effect = new T();
            effect.initialize(overlayImage, time);
            quededEffects.Add(effect);
        }

        public void clearQueue(bool stopEffectImmediately)
        {
            if (quededEffects.Count > 0)
            {
                List<OverlayEffectBase> trashedEffects = new List<OverlayEffectBase>();

                for (int i = 0; i < quededEffects.Count; i++)
                {
                    if (!stopEffectImmediately && i == 0) continue;

                    quededEffects[i].onDestroy();
                    trashedEffects.Add(quededEffects[i]);
                }

                foreach (OverlayEffectBase effect in trashedEffects)
                {
                    quededEffects.Remove(effect);
                }
            }
        }
    }
}
