using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        private Canvas canvas;

        private AudioManager audMan;

        private Queue<Notification> notifsQueue = new Queue<Notification>();

        private Notification currentNotif;

        public Notification Message => currentNotif;

        public bool GenericNotificationsHidden => !AdvancedCore.notificationsEnabled;

        public void Initialize()
        {
            canvas = ObjectsCreator.CreateCanvas(setGlobalCam: false);
            canvas.transform.SetParent(transform);

            audMan = canvas.gameObject.AddComponent<AudioManager>();
            AudioSource source = canvas.gameObject.AddComponent<AudioSource>();
            audMan.audioDevice = source;
        }

        private void Update()
        {
            if (notifsQueue.Count > 0 && currentNotif == null)
            {
                currentNotif = notifsQueue.Dequeue();
                currentNotif.active = true;
                currentNotif.gameObject = CreateNotification(currentNotif.key);
                currentNotif.tmpText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                StartCoroutine(NotificationAnimator(true));
            }

            if (currentNotif != null) UpdateNotification();
        }

        private void UpdateNotification()
        {
            if (currentNotif.time > 0f)
            {
                currentNotif.time -= Time.unscaledDeltaTime;

                if (currentNotif.time <= 0f)
                {
                    StartCoroutine(NotificationAnimator(false));
                }
            }

            if (currentNotif.sound != null)
            {
                audMan.PlaySingle(currentNotif.sound);
                currentNotif.sound = null;
            }
        }

        public Notification Queue(string key, SoundObject sound = null, float time = 10f, bool isForced = false)
        {
            if (GenericNotificationsHidden && !isForced) return null;
            gameObject.SetActive(true);

            Notification notif = new Notification()
            {
                key = key,
                sound = sound,
                time = time
            };

            notifsQueue.Enqueue(notif);

            return notif;
        }

        private GameObject CreateNotification(string key)
        {
            GameObject notifBase = new GameObject("NotifBase", typeof(RectTransform));
            notifBase.transform.SetParent(canvas.transform, false);
            RectTransform rectTransform = notifBase.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            notifBase.transform.localPosition = Vector3.right * 245f + Vector3.up * -230f;

            Image imageBg = UIHelpers.CreateImage(AssetsStorage.sprites["tooltip_bg"], notifBase.transform, Vector3.zero, correctPosition: false);
            imageBg.type = Image.Type.Sliced;

            TextMeshProUGUI tmpText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "", notifBase.transform, Vector3.zero);
            tmpText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.black;
            tmpText.rectTransform.sizeDelta = new Vector2(140f, 90f);

            //tmpText.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            imageBg.rectTransform.sizeDelta = new Vector2(150f, 100f);

            return notifBase;
        }

        private IEnumerator NotificationAnimator(bool appearing)
        {
            float maxHeight = -130f;
            float minHeight = -230f;

            Vector3 pos = currentNotif.gameObject.transform.localPosition;

            if (appearing)
            {
                currentNotif.state = AnimationState.Appearing;
                while (pos.y < maxHeight)
                {
                    if (currentNotif == null) yield break;

                    pos.y += Time.unscaledDeltaTime * currentNotif.speed;

                    if (pos.y > maxHeight) pos.y = maxHeight;

                    currentNotif.gameObject.transform.localPosition = pos;
                    yield return null;
                }
                currentNotif.state = AnimationState.None;
            } else
            {
                currentNotif.state = AnimationState.Disappearing;
                while (pos.y > minHeight)
                {
                    if (currentNotif == null) yield break;

                    pos.y -= Time.unscaledDeltaTime * currentNotif.speed;
                    currentNotif.gameObject.transform.localPosition = pos;
                    yield return null;
                }

                currentNotif.state = AnimationState.None;
                currentNotif.active = false;
                Destroy(currentNotif.gameObject);
                currentNotif = null;

                if (notifsQueue.Count == 0) gameObject.SetActive(false);

                //pos.y = minHeight;
                //if (currentNotif != null) currentNotif.gameObject.transform.localPosition = pos;
            }
        }

        public class Notification
        {
            public bool active;

            public AnimationState state;

            public GameObject gameObject;

            public TextMeshProUGUI tmpText;

            public string key;

            public float time;

            public float speed = 200f;

            public SoundObject sound;

        }

        public enum AnimationState
        {
            None,
            Appearing,
            Disappearing
        }

    }
}
