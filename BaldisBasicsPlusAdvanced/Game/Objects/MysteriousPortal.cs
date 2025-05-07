using BaldisBasicsPlusAdvanced.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
{
    public class MysteriousPortal : MonoBehaviour
    {
        private EnvironmentController ec;

        //SerializeField to Unity also clones values

        [SerializeField]
        private AudioManager audMan;

        private MysteriousPortal connectedPortal;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private bool activated;

        private bool used;

        public AudioManager AudMan => audMan;

        private List<IEnumerator> animationsQueue = new List<IEnumerator>();

        private bool openPortal; //for animation

        private IEnumerator currentAnimation;

        public void initialize(SpriteRenderer spriteRenderer)
        {
            this.spriteRenderer = spriteRenderer;
            audMan = gameObject.AddComponent<PropagatedAudioManager>();
            Destroy(audMan.audioDevice.gameObject); //because don't need this in prefabs
        }

        public void postInitialize(EnvironmentController ec, bool hidden)
        {
            this.ec = ec;
            if (hidden)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
                transform.localScale = Vector3.zero;
            }
        }

        private void Update()
        {
            if (animationsQueue.Count > 0 && currentAnimation == null)
            {
                currentAnimation = animationsQueue[0];
                animationsQueue.RemoveAt(0);
            }
            if (currentAnimation != null && !currentAnimation.MoveNext())
            {
                currentAnimation = null;
            }

        }

        public void connectTo(MysteriousPortal portal)
        {
            connectedPortal = portal;
        }

        public void activate()
        {
            setAnim(true, 5f);
        }

        public void deactivate()
        {
            setAnim(false, 2.5f);
        }

        public void OnTriggerStay(Collider other)
        {
            if (activated && !used)
            {
                if (other.TryGetComponent(out MathMachineNumber balloon))
                {
                    balloon.trackPlayer = true; //to fix when balloons didn't respawn
                    balloon.Pop();
                    return;
                }

                if (other.TryGetComponent(out Entity entity))
                {
                    teleport(entity);
                    return;
                }
            }
        }

        private void teleport(Entity entity)
        {
            used = true;
            connectedPortal.used = true;

            entity.Teleport(connectedPortal.transform.position);

            audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
            connectedPortal.AudMan.PlaySingle(AssetsStorage.sounds["teleport"]);

            deactivate();
            connectedPortal.deactivate();
        }

        public void setAnim(bool appearing, float baseTime = 5f)
        {
            animationsQueue.Add(setAnimation(appearing, baseTime));
            openPortal = appearing;
        }

        private IEnumerator setAnimation(bool appearing, float baseTime = 5f)
        {
            float time = baseTime;
            Color color = Color.white;
            float percent = 1f;

            while (true)
            {
                if (appearing)
                {
                    if (time > 0)
                    {
                        time -= Time.deltaTime * ec.NpcTimeScale;
                        percent = (baseTime - time) / baseTime;

                        color.a = percent;

                        if (percent < 0) transform.localScale = Vector3.zero; //do not spam in log about negative scale isn't supported!!!
                        else transform.localScale = Vector3.one * percent;

                        spriteRenderer.color = color;
                        yield return null;
                    }
                    else
                    {
                        transform.localScale = Vector3.one;
                        spriteRenderer.color = Color.white;
                        if (openPortal)
                        {
                            spriteRenderer.sprite = AssetsStorage.sprites["adv_portal_opened"];
                            activated = true;
                            audMan.PlaySingle(AssetsStorage.sounds["teleport"]);
                        }
                        yield break;
                    }
                }
                else
                {
                    activated = false;
                    spriteRenderer.sprite = AssetsStorage.sprites["adv_portal"];
                    if (time > 0)
                    {
                        time -= Time.deltaTime * ec.NpcTimeScale;
                        percent = time / baseTime;

                        color.a = percent;

                        if (percent < 0) transform.localScale = Vector3.zero; //do not spam in log about negative scale isn't supported!!!
                        else transform.localScale = Vector3.one * percent;

                        spriteRenderer.color = color;
                        yield return null;
                    }
                    else
                    {
                        transform.localScale = Vector3.zero;
                        spriteRenderer.color = new Color(1, 1, 1, 0);
                        Destroy(gameObject);
                        yield break;
                    }
                }

            }

        }

    }
}
