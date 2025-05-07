using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay
{
    public class OverlayEffectsManager : MonoBehaviour
    {
        [SerializeField]
        private Image overlayImage;

        private List<OverlayEffectBase> quededEffects = new List<OverlayEffectBase>();

        public void InitializePrefab(Image image)
        {
            overlayImage = image;
        }

        private void Update()
        {
            if (quededEffects.Count > 0)
            {
                if (!quededEffects[0].Playing) quededEffects[0].OnPlayStart();
                quededEffects[0].VirtualUpdate();
                if (quededEffects[0].Destroy)
                {
                    OverlayEffectBase effectToDestroy = quededEffects[0];
                    effectToDestroy.OnDestroy();
                    quededEffects.RemoveAt(0);

                    if (effectToDestroy.DestroyOverlayOnEnd)
                    {
                        ClearQueue(true);
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }

        public void SetCanvasCam()
        {
            overlayImage.transform.parent.GetComponent<Canvas>().worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
        }

        public void SetColor(Color color)
        {
            overlayImage.color = color;
        }

        public void SetAlpha(float a)
        {
            overlayImage.color = new Color(1f, 1f, 1f, a);
        }

        public void QueueEffect<T>(float time) where T : OverlayEffectBase, new()
        {
            T effect = new T();
            effect.Initialize(overlayImage, time);
            quededEffects.Add(effect);
        }

        public void ClearQueue(bool stopEffectImmediately)
        {
            if (quededEffects.Count > 0)
            {
                List<OverlayEffectBase> trashedEffects = new List<OverlayEffectBase>();

                for (int i = 0; i < quededEffects.Count; i++)
                {
                    if (!stopEffectImmediately && i == 0) continue;

                    quededEffects[i].OnDestroy();
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
