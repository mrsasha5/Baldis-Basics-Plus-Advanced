using System.Collections;
using BaldisBasicsPlusAdvanced.Game;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
using The3DElevator.MonoBehaviours.ElevatorObjects;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects
{
    internal class SpatialTipsMonitor : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private SoundObject audMove;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private Vector2 minMaxHeight;

        [SerializeField]
        private TextMeshPro tmp;

        [SerializeField]
        private float appearingTime;

        [SerializeField]
        private float movingSpeed;

        public ElevatorAppearanceHole hole;

        private float time;

        public void InitializePrefab(int variant)
        {
            audMan = ObjectsCreator.CreateAudMan(gameObject);
            ReflectionHelper.SetValue(audMan, "disableSubtitles", true);
            audMan.ignoreListenerPause = true;
            audMan.useUnscaledPitch = true;

            audMove = AssetsHelper.LoadAsset<SoundObject>("ShrinkMachine_Door");
            minMaxHeight = new Vector2(8f, 18.4f);
            appearingTime = 6f;
            movingSpeed = 20f;

            tmp = GetComponentInChildren<TextMeshPro>();
        }

        private void Start()
        {
            time = appearingTime;
            Vector3 pos = transform.localPosition;
            pos.y = minMaxHeight.y;
            transform.localPosition = pos;
        }

        private void Update()
        {
            if (time > 0f)
            {
                time -= Time.unscaledDeltaTime;
                if (time <= 0f)
                {
                    StartCoroutine(AppearAnimation());
                }
            }
        }

        private IEnumerator AppearAnimation()
        {
            Vector3 vec = transform.localPosition;
            vec.y = 27.92f;
            vec += Vector3.forward * 9f;
            hole.Appear(vec, new Vector3(90f, 0f, 0f), 6f);

            while (hole.IsTransitioning) yield return null;

            audMan.PlaySingle(audMove, 2f);

            Vector3 pos = transform.localPosition;
            pos.y = minMaxHeight.y;
            transform.localPosition = pos;

            float multiplier = 0f;

            while (pos.y > minMaxHeight.x)
            {
                if (multiplier < 1f)
                {
                    multiplier += Time.unscaledDeltaTime * 0.5f;
                    if (multiplier > 1f) multiplier = 1f;
                }

                pos.y -= Time.unscaledDeltaTime * movingSpeed * multiplier;
                if (pos.y <= minMaxHeight.x) pos.y = minMaxHeight.x;

                transform.localPosition = pos;
                yield return null;
            }

            StartCoroutine(TextLoader());
        }

        private IEnumerator TextLoader()
        {
            const float symbolCooldown = 0.01f;

            float time = 4f;

            tmp.text = "";

            string text = ElevatorTipsPatch.GetRawTip();

            int index = 0;

            time = symbolCooldown;

            while (index < text.Length)
            {

                while (time > 0f)
                {
                    time -= Time.unscaledDeltaTime;
                    yield return null;
                }

                time = symbolCooldown;

                tmp.text += text[index];
                index++;
            }
        }


    }
}
