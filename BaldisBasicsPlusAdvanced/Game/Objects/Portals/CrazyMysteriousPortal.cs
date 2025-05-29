using System.Collections;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Helpers;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Portals
{
    public class CrazyMysteriousPortal : MysteriousPortal
    {
        [SerializeField]
        private SoundObject audPortalStartsTeleporting;

        [SerializeField]
        private SoundObject audPortalTeleports;

        [SerializeField]
        private SoundObject audBuzz;

        [SerializeField]
        private AudioManager selfTeleportAudMan;

        [SerializeField]
        private Vector2 minMaxTime;

        private float time;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);

            selfTeleportAudMan = ObjectsCreator.CreatePropagatedAudMan(Vector3.zero);
            selfTeleportAudMan.transform.SetParent(transform, false);

            minMaxTime = new Vector2(10f, 30f);

            audPortalStartsTeleporting = AssetsStorage.sounds["adv_portal_starts_teleport"];
            audPortalTeleports = AssetsStorage.sounds["adv_portal_teleports"];
            audBuzz = AssetsStorage.sounds["buzz_lose"];
        }

        public void StartTimer()
        {
            time = Random.Range(minMaxTime.x, minMaxTime.y);
        }

        protected override void VirtualUpdate()
        {
            if (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;

                if (time <= 0f)
                {
                    animationsQueue.Add(Teleporting());
                }
            }
            base.VirtualUpdate();
        }

        private IEnumerator Teleporting()
        {
            selfTeleportAudMan.FlushQueue(true);

            selfTeleportAudMan.QueueAudio(audPortalStartsTeleporting);
            selfTeleportAudMan.QueueAudio(audPortalTeleports);

            while (selfTeleportAudMan.filesQueued > 0)
            {
                yield return null;
            }

            MaterialPropertyBlock spriteProperties = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(spriteProperties);
            spriteProperties.SetInt("_SpriteColorGlitching", 1);
            spriteRenderer.SetPropertyBlock(spriteProperties);

            collider.enabled = false;

            while (selfTeleportAudMan.QueuedAudioIsPlaying)
            {
                spriteRenderer.GetPropertyBlock(spriteProperties);

                spriteProperties.SetFloat("_SpriteColorGlitchPercent",
                Mathf.Clamp01(selfTeleportAudMan.audioDevice.time / selfTeleportAudMan.audioDevice.clip.length));
                spriteProperties.SetFloat("_SpriteColorGlitchVal", UnityEngine.Random.Range(0f, 4096f));

                spriteRenderer.SetPropertyBlock(spriteProperties);
                yield return null;
            }

            AudioManager teleportAudMan = ObjectsCreator.CreatePropagatedAudMan(transform.position, true);
                teleportAudMan.PlaySingle(audTeleport);

            transform.position = ec.RandomCell(includeOffLimits: false, includeWithObjects: false, useEntitySafeCell: true)
                .FloorWorldPosition + Vector3.up * 5f;

            selfTeleportAudMan.FlushQueue(true);
            selfTeleportAudMan.PlaySingle(audTeleport);
            selfTeleportAudMan.PlaySingle(audBuzz);

            spriteRenderer.GetPropertyBlock(spriteProperties);

            spriteProperties.SetFloat("_SpriteColorGlitchPercent", 0.9f);

            spriteRenderer.SetPropertyBlock(spriteProperties);

            float time = 1f;

            while (time > 0f)
            {
                time -= Time.deltaTime * ec.EnvironmentTimeScale;
                spriteRenderer.GetPropertyBlock(spriteProperties);
                spriteProperties.SetFloat("_SpriteColorGlitchVal", UnityEngine.Random.Range(0f, 4096f));
                spriteRenderer.SetPropertyBlock(spriteProperties);
                yield return null;
            }

            if (activated && Connected)
                collider.enabled = true;

            spriteRenderer.GetPropertyBlock(spriteProperties);
            spriteProperties.SetInt("_SpriteColorGlitching", 0);
            spriteRenderer.SetPropertyBlock(spriteProperties);

            StartTimer();
        }

    }
}
