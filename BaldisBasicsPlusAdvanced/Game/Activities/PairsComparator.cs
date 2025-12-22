using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using Rewired;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Activities
{
    public struct PairsComparatorData
    {
        public PairBalloon balloon;

        public int totalValue;

        public float rotationAngle;

        public bool locked;
    }

    public class PairsComparator : Activity, IPrefab, IClickable<int>
    {
        [SerializeField]
        private SoundObject audWind;

        [SerializeField]
        private SoundObject audWindEnd;

        [SerializeField]
        private SoundObject audCorrect;

        [SerializeField]
        private SoundObject audIncorrect;

        [SerializeField]
        private SoundObject audPull;

        [SerializeField]
        private SphereCollider collider;

        [SerializeField]
        private MeshRenderer[] renderers;

        [SerializeField]
        private SpriteRenderer pulleyRenderer;

        [SerializeField]
        private PairBalloon[] balloonPre;

        [SerializeField]
        private AudioManager motorMan;

        [SerializeField]
        private float pulleySpriteOffset;

        [SerializeField]
        private float balloonPopRate;

        [SerializeField]
        private float balloonPopDelay;

        [SerializeField]
        private float rotationAnimationSpeed;

        [SerializeField]
        internal float spawnRadius;

        [SerializeField]
        internal int balloonAmmount;

        private List<PairsComparatorData> balloonData;

        private int chosenPair;

        private bool switching;

        private bool pulling;

        public void InitializePrefab(int variant)
        {
            audCorrect = AssetStorage.sounds["activity_correct"];
            audIncorrect = AssetStorage.sounds["activity_incorrect"];
            audPull = AssetHelper.LoadAsset<SoundObject>("BalloonBuster_Pulley");
            baldiPause = 5f;
            balloonAmmount = 8;
            spawnRadius = 20f;
            rotationAnimationSpeed = 2000f;
            balloonPopRate = 0.25f;
            balloonPopDelay = 3f;
            pulleySpriteOffset = 4f;
            renderers = new MeshRenderer[2];

            Sprite sprite = AssetHelper.SpriteFromFile("Textures/Objects/BluePulley.png", 22f);

            audMan = ObjectCreator.CreatePropagatedAudMan(gameObject);
            motorMan = ObjectCreator.CreatePropagatedAudMan(new GameObject("MotorMan"));
            motorMan.transform.SetParent(transform, false);
            pulleyRenderer = ObjectCreator.CreateSpriteRenderer(sprite);
            pulleyRenderer.transform.SetParent(transform, false);
            pulleyRenderer.name = "Pulley";
            pulleyRenderer.transform.localPosition = Vector3.up * pulleySpriteOffset;

            renderers[0] = ObjectCreator.CreateQuadRenderer();
            renderers[0].transform.localScale = new Vector3(20f, 20f, 1f);
            renderers[0].material.mainTexture = 
                AssetHelper.TextureFromFile("Textures/Activities/PairsComparator/PairsComparator_Base.png");
            renderers[0].transform.SetParent(transform, false);

            renderers[1] = ObjectCreator.CreateQuadRenderer();
            renderers[1].transform.localScale = renderers[0].transform.localScale;
            renderers[1].material.mainTexture =
                AssetHelper.TextureFromFile("Textures/Activities/PairsComparator/PairsComparator_Arrow.png");
            renderers[1].transform.SetParent(transform, false);

            renderers[0].transform.localPosition = Vector3.up * -5f;
            renderers[1].transform.localPosition = Vector3.up * -4.9f;
            renderers[0].transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            renderers[1].transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));

            audWind = 
                AssetHelper.SoundObjectFromFile(
                    "Audio/Sounds/Adv_Long_Wind.wav", "", SoundType.Effect, Color.white, sublength: 0f);
            audWindEnd =
                AssetHelper.SoundObjectFromFile(
                    "Audio/Sounds/Adv_Long_Wind_End.wav", "", SoundType.Effect, Color.white, sublength: 0f);

            collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 5f;

            balloonPre = new PairBalloon[10];

            if (variant == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    Sprite spr = AssetHelper.LoadAsset<Sprite>($"NumBall_{i}");
                    PairBalloon balloon = PrefabCreator.CreateBalloonPrefab<PairBalloon>($"PairBalloon_{i}", $"pair_balloon_{i}");
                    balloon.Renderer.sprite = spr;
                    balloon.value = i;
                }                
            }

            for (int i = 0; i < 10; i++)
            {
                balloonPre[i] = ObjectStorage.Objects[$"pair_balloon_{i}"].GetComponent<PairBalloon>();
            }
        }

        private void Start()
        {
            ReInit();
            room.ec.AddActivity(this);
        }

        public override void Completed(int player, bool correct)
        {
            base.Completed(player, correct);

            collider.enabled = false;
            StartCoroutine(BalloonPopper());
        }

        public override void ReInit()
        {
            base.ReInit();
            collider.enabled = true;
            SpawnBalloons();
            notebook.transform.position = transform.position;
            notebook.gameObject.SetActive(value: false);
        }

        public void SelectPair(PairBalloon balloon)
        {
            if (ClickableHidden()) return;
            for (int i = 0; i < balloonData.Count; i++)
            {
                if (balloonData[i].balloon == balloon || balloonData[i].balloon.ConnectedBalloon == balloon)
                {
                    if (balloonData[i].locked || chosenPair == i) break;

                    chosenPair = i;
                    switching = true;
                    StartCoroutine(Switcher());
                    break;
                }
            }
        }

        private IEnumerator Switcher()
        {
            motorMan.QueueAudio(audWind);

            Vector3 rotation = renderers[1].transform.rotation.eulerAngles;

            float timer = audWind.soundClip.length;

            while (timer > 0f)
            {
                timer -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                rotation.y += Time.deltaTime * room.ec.EnvironmentTimeScale * rotationAnimationSpeed;
                renderers[1].transform.rotation = Quaternion.Euler(rotation);
                yield return null;
            }

            motorMan.FlushQueue(true);
            motorMan.PlaySingle(audWindEnd);

            rotation.y = balloonData[chosenPair].rotationAngle;
            renderers[1].transform.rotation = Quaternion.Euler(rotation);

            balloonData[chosenPair].balloon.AnimateSelection();
            balloonData[chosenPair].balloon.ConnectedBalloon.AnimateSelection();

            switching = false;
        }

        private void SpawnBalloons()
        {
            balloonData = new List<PairsComparatorData>();

            float angle = 360f / balloonAmmount;
            int counter = balloonAmmount / 2;

            List<int> usedSums = new List<int>();

            float currentAngle = 0f;

            while (counter > 0)
            {
                List<PairBalloon> _balloons = balloonPre.ToList();

                PairBalloon pre1 = _balloons.GetRandomElementAndRemove();
                PairBalloon pre2 = _balloons.GetRandomElementAndRemove();

                while (_balloons.Count > 0 && usedSums.Contains(pre1.value + pre2.value))
                {
                    pre2 = _balloons.GetRandomElementAndRemove();
                }
                usedSums.Add(pre1.value + pre2.value);

                PairBalloon balloon1 = Instantiate(pre1, room.transform);
                PairBalloon balloon2 = Instantiate(pre2, room.transform);

                balloon1.Initialize(this);
                balloon2.Initialize(this);
                balloon2.Connect(balloon1);
                balloon1.transform.position = 
                    new Vector3(transform.position.x + spawnRadius * Mathf.Cos(Mathf.Deg2Rad * currentAngle), 5f,
                        transform.position.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * currentAngle));
                balloon2.transform.position =
                    new Vector3(transform.position.x - spawnRadius * Mathf.Cos(Mathf.Deg2Rad * currentAngle), 5f,
                        transform.position.z - spawnRadius * Mathf.Sin(Mathf.Deg2Rad * currentAngle));

                balloonData.Add(new PairsComparatorData()
                {
                    balloon = balloon1,
                    totalValue = balloon1.value + balloon2.value,
                    rotationAngle = Mathf.Abs(currentAngle)
                });

                currentAngle -= angle;
                counter--;
            }
        }

        public void Clicked(int player)
        {
            PairsComparatorData chosenPair = balloonData[this.chosenPair];
            if (!ClickableHidden() && chosenPair.balloon != null)
            {
                bool isUnusedDataFound = false;
                bool correct = true;
                foreach (PairsComparatorData data in balloonData)
                {
                    if (!data.locked && chosenPair.balloon != data.balloon)
                    {
                        isUnusedDataFound = true;
                    }

                    if (!data.locked && chosenPair.totalValue > data.totalValue && chosenPair.totalValue != data.totalValue)
                    {
                        correct = false;
                        break;
                    }
                }

                pulling = true;
                StartCoroutine(ActionCompleter(player, correct, !isUnusedDataFound, 1f));
                StartCoroutine(PulleyAnimator());
            }
        }

        private IEnumerator ActionCompleter(int player, bool correct, bool finalAction, float delay)
        {
            PairsComparatorData chosenPair = balloonData[this.chosenPair];

            while (delay > 0f)
            {
                delay -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                yield return null;
            }

            if (correct)
            {
                chosenPair.locked = true;
                balloonData[this.chosenPair] = chosenPair;

                audMan.PlaySingle(audCorrect);
                if (finalAction)
                {
                    Completed(player, true);
                    room.functions.OnActivityCompletion();
                }
                else
                {
                    room.functions.OnActivityProgress();
                }

                BaseGameManager.Instance.PleaseBaldi(baldiPause, rewardSticker: true);
            }
            else
            {
                chosenPair.locked = true;
                balloonData[this.chosenPair] = chosenPair;
                audMan.PlaySingle(audIncorrect);
                Completed(player, false);
                room.functions.OnActivityCompletion();
            }

            chosenPair.balloon.Reveal(correct);
            chosenPair.balloon.ConnectedBalloon.Reveal(correct);
        }

        //Hardcoded recreation
        private IEnumerator PulleyAnimator()
        {
            audMan.PlaySingle(audPull);

            Vector3 pos = pulleyRenderer.transform.localPosition;

            float initialTime = 0.5f;
            float time = initialTime;

            while (time > 0f)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                if (time < 0f) time = 0f;
                pos.y = pulleySpriteOffset - 1f * (1f - time / initialTime);
                pulleyRenderer.transform.localPosition = pos;
                yield return null;
            }

            time = 0.1f;

            while (time > 0f)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                yield return null;
            }

            initialTime = 0.25f;
            time = initialTime;

            while (time > 0f)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                if (time < 0f) time = 0f;
                pos.y = pulleySpriteOffset - 1f - 1f * (1f - time / initialTime);
                pulleyRenderer.transform.localPosition = pos;
                yield return null;
            }

            initialTime = 0.5f;
            time = initialTime;

            while (time > 0f)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                if (time < 0f) time = 0f;
                pos.y = pulleySpriteOffset - 2f * (time / initialTime);
                pulleyRenderer.transform.localPosition = pos;
                yield return null;
            }

            pulling = false;
        }

        private IEnumerator BalloonPopper()
        {
            List<PairBalloon> balloons = new List<PairBalloon>();
            while (balloonData.Count > 0)
            {
                balloons.Add(balloonData[0].balloon);
                balloons.Add(balloonData[0].balloon.ConnectedBalloon);
                balloonData.RemoveAt(0);
            }

            float initialDelay = balloonPopDelay;
            while (initialDelay > 0f)
            {
                initialDelay -= Time.deltaTime;
                yield return null;
            }

            float time = balloonPopRate;
            for (int count = 0; count < balloons.Count; count++)
            {
                while (time > 0f)
                {
                    time -= Time.deltaTime;
                    yield return null;
                }

                time = balloonPopRate;
                if (balloons[count] != null)
                {
                    balloons[count].Pop();
                }
            }
        }

        public bool ClickableHidden()
        {
            return completed || switching || pulling;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return true;
        }

        public void ClickableSighted(int player)
        {
            
        }

        public void ClickableUnsighted(int player)
        {
            
        }
    }

    public class PairBalloon : BaseBalloonBehaviour
    {
        [SerializeField]
        private SoundObject audSelected;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private Sprite correctSprite;

        [SerializeField]
        private Sprite incorrectSprite;

        private PairsComparator comparator;

        private PairBalloon connectedInstance;

        private bool revealed;

        public int value;

        public PairBalloon ConnectedBalloon => connectedInstance;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            audSelected = AssetHelper.LoadAsset<SoundObject>("Boink");
            correctSprite = AssetHelper.SpriteFromFile("Textures/Objects/Balloon_Correct.png", 30f);
            incorrectSprite = AssetHelper.SpriteFromFile("Textures/Objects/Balloon_Incorrect.png", 30f);

            audMan = ObjectCreator.CreatePropagatedAudMan(gameObject);
            offsetAnimationEnabled = false;
        }

        public void Initialize(PairsComparator comparator)
        {
            this.comparator = comparator;
            floater.Initialize(comparator.room);
        }

        public void Connect(PairBalloon balloon)
        {
            connectedInstance = balloon;
            balloon.connectedInstance = this;
        }

        public void Reveal(bool correct)
        {
            audMan.PlaySingle(audReveal);
            PlayRevealAnimation(correct ? correctSprite : incorrectSprite);
            revealed = true;
        }

        public void AnimateSelection()
        {
            audMan.PlaySingle(audSelected);
            StartCoroutine(SelectAnimator());
        }

        private IEnumerator SelectAnimator()
        {
            float value = 1.25f;

            while (value > 1f)
            {
                value -= Time.deltaTime * 0.5f;

                if (value < 1f) value = 1f;

                spriteRenderer.transform.localScale = Vector3.one * value;

                yield return null;
            }
        }

        protected override void VirtualStart()
        {
            MakeStaticBehaviour();
        }

        public override void Clicked(int player)
        {
            base.Clicked(player);
            comparator.SelectPair(this);
        }

        public override bool ClickableHidden()
        {
            return base.ClickableHidden() || revealed;
        }
    }
}
