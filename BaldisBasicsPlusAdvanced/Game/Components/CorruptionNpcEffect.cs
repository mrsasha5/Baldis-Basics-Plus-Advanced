using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class CorruptionNpcEffect : MonoBehaviour
    {
        public delegate void OnHitEvent();

        public delegate void OnEffectEndEvent();

        public delegate void OnEffectPreEndEvent();

        private MaterialPropertyBlock spriteProperties;

        private SpriteRenderer spriteRenderer;

        public OnHitEvent onHit;

        public OnEffectPreEndEvent onEffectPreEnd;

        public OnEffectEndEvent onEffectEnd;

        private bool destroyOnEffectEnd;

        public void initialize(SpriteRenderer spriteRenderer, bool destroyOnEffectEnd = true)
        {
            this.spriteRenderer = spriteRenderer;
            this.destroyOnEffectEnd = destroyOnEffectEnd;
        }

        public void hit()
        {
            this.spriteProperties = new MaterialPropertyBlock();
            this.spriteRenderer.GetPropertyBlock(this.spriteProperties);
            this.spriteProperties.SetInt("_SpriteColorGlitching", 1);
            this.spriteProperties.SetFloat("_SpriteColorGlitchPercent", 0.9f);
            this.spriteProperties.SetFloat("_SpriteColorGlitchVal", UnityEngine.Random.Range(0f, 4096f));
            this.spriteRenderer.SetPropertyBlock(this.spriteProperties);
            StartCoroutine(Flash(1f));
            if (onHit != null) onHit.Invoke();
        }

        private IEnumerator Flash(float time)
        {
            while (time > 0f)
            {
                time -= Time.unscaledDeltaTime;
                this.spriteRenderer.GetPropertyBlock(this.spriteProperties);
                this.spriteProperties.SetFloat("_SpriteColorGlitchVal", UnityEngine.Random.Range(0f, 4096f));
                this.spriteRenderer.SetPropertyBlock(this.spriteProperties);
                if (onEffectPreEnd != null && time < 0f) onEffectPreEnd.Invoke();
                yield return null;
            }
            this.spriteRenderer.GetPropertyBlock(this.spriteProperties);
            this.spriteProperties.SetInt("_SpriteColorGlitching", 0);
            this.spriteRenderer.SetPropertyBlock(this.spriteProperties);
            if (onEffectEnd != null) onEffectEnd.Invoke();
            if (destroyOnEffectEnd) Destroy(this);
            yield break;
        }

        private void OnDestroy()
        {
            if (spriteRenderer != null)
            {
                this.spriteRenderer.GetPropertyBlock(this.spriteProperties);
                this.spriteProperties.SetInt("_SpriteColorGlitching", 0);
                this.spriteRenderer.SetPropertyBlock(this.spriteProperties);
            }
        }

    }
}
