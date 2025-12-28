using System;
using System.Collections;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
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

    [Serializable]
    public struct PotentialPairBalloonData
    {
        public Sprite sprite;

        public int value;
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
        private Transform arrow;

        [SerializeField]
        private MeshRenderer hologram;

        [SerializeField]
        private SpriteRenderer pulleyRenderer;

        [SerializeField]
        private PairBalloon balloonPre;

        [SerializeField]
        private PotentialPairBalloonData[] values;

        [SerializeField]
        private AudioManager motorMan;

        [SerializeField]
        private int wrongNoiseVal;

        [SerializeField]
        private float pulleySpriteOffset;

        [SerializeField]
        private float balloonPopRate;

        [SerializeField]
        private float balloonPopDelay;

        [SerializeField]
        private float rotationAnimationSpeed;

        [SerializeField]
        private int pointsPerPair;

        [SerializeField]
        private int finalPoints;

        [SerializeField]
        internal float spawnRadius;

        [SerializeField]
        internal int balloonAmmount;

        private List<PairsComparatorData> balloonData;

        private int chosenPair;

        private bool switching;

        private bool pulling;

        public bool TrulyCompleted => completed;

        public bool IsPlayingAnimations => switching || pulling;

        public bool Powered => powered;

        public void InitializePrefab(int variant)
        {
            audWind =
                AssetHelper.SoundObjectFromFile(
                    "Audio/Sounds/Adv_Long_Wind.wav", "", SoundType.Effect, Color.white, sublength: 0f);
            audWindEnd =
                AssetHelper.SoundObjectFromFile(
                    "Audio/Sounds/Adv_Long_Wind_End.wav", "", SoundType.Effect, Color.white, sublength: 0f);
            audCorrect = AssetStorage.sounds["activity_correct"];
            audIncorrect = AssetStorage.sounds["activity_incorrect"];
            audPull = AssetHelper.LoadAsset<SoundObject>("BalloonBuster_Pulley");
            audRespawn = AssetHelper.LoadAsset<SoundObject>("NoteRespawn");
            baldiPause = 5f;
            balloonAmmount = 8;
            spawnRadius = 20f;
            rotationAnimationSpeed = 2000f;
            wrongNoiseVal = 126;
            balloonPopRate = 0.15f;
            balloonPopDelay = 3f;
            pulleySpriteOffset = 4f;
            pointsPerPair = 10;
            finalPoints = 20;

            SphereCollider endlessCollider = new GameObject("Trigger").AddComponent<SphereCollider>();
            endlessCollider.transform.SetParent(transform, false);
            endlessCollider.isTrigger = true;
            endlessCollider.radius = 100f;
            endlessCollider.gameObject.layer = LayerHelper.ignoreRaycast;
            trigger = endlessCollider.gameObject.AddComponent<ColliderGroup>();

            Sprite sprite = AssetHelper.SpriteFromFile("Textures/Objects/BluePulley.png", 22f);

            audMan = ObjectCreator.CreatePropagatedAudMan(gameObject);
            motorMan = ObjectCreator.CreatePropagatedAudMan(new GameObject("MotorMan"));
            motorMan.transform.SetParent(transform, false);
            pulleyRenderer = ObjectCreator.CreateSpriteRenderer(sprite);
            pulleyRenderer.transform.SetParent(transform, false);
            pulleyRenderer.name = "Pulley";
            pulleyRenderer.transform.localPosition = Vector3.up * pulleySpriteOffset;

            Transform billboardBase = new GameObject("Billboard").AddComponent<BillboardUpdater>().transform;
            billboardBase.transform.SetParent(transform, false);
            hologram = ObjectCreator.CreateQuadRenderer();
            hologram.transform.localScale = new Vector3(15f, 15f, 1f);
            hologram.name = "Hologram";
            hologram.transform.SetParent(billboardBase, false);
            hologram.transform.localPosition = Vector3.up * -4.65f + Vector3.forward * 0.2f;
            hologram.material.mainTexture = AssetHelper.LoadAsset<Texture2D>("BalloonBuster_Hologram");

            Renderer[] renderers = new Renderer[2];

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i] = ObjectCreator.CreateQuadRenderer();
                renderers[i].transform.localScale = new Vector3(20f, 20f, 1f);
                renderers[i].transform.SetParent(transform, false);
                renderers[i].transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            }

            renderers[0].material.mainTexture = 
                AssetHelper.TextureFromFile("Textures/Activities/PairsComparator/PairsComparator_Base.png");
            renderers[1].material.mainTexture =
                AssetHelper.TextureFromFile("Textures/Activities/PairsComparator/PairsComparator_Arrow.png");

            renderers[0].transform.localPosition = Vector3.up * -5f;
            renderers[1].transform.localPosition = Vector3.up * -4.95f;

            arrow = renderers[1].transform;

            bonusQSignSpriteRenderer = Instantiate(AssetHelper.LoadAsset<SpriteRenderer>("BonusQSign"));
            bonusQSignSpriteRenderer.transform.SetParent(transform, false);
            bonusQSignSpriteRenderer.transform.localPosition = Vector3.up * -4.9f;
            bonusQSignSpriteRenderer.transform.localScale = new Vector3(2f, 2f, 1f);
            bonusQSignSpriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            bonusQSign = bonusQSignSpriteRenderer.GetComponent<Animator>();

            collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 5f;

            balloonPre = PrefabCreator.CreateBalloonPrefab<PairBalloon>($"PairBalloon_0", $"pair_balloon_0");

            Sprite[] sprites = AssetHelper.LoadAssets<Sprite>();

            values = new PotentialPairBalloonData[10];

            for (int i = 0; i < 10; i++)
            {
                values[i].sprite = Array.Find(sprites, x => x.name == $"NumBall_{i}");
                values[i].value = i;
                if (values[i].sprite == null)
                    throw new Exception("Numballoon sprite is null!");
            }
        }

        private void Start()
        {
            SpawnBalloons();
            ReInit();
            room.ec.AddActivity(this);
        }

        public override void SetPower(bool val)
        {
            base.SetPower(val);
            hologram.gameObject.SetActive(val);
        }

        public override void SetBonusMode(bool val)
        {
            base.SetBonusMode(val);
            if (val)
                ReInit();
        }

        public override void Completed(int player, bool correct)
        {
            base.Completed(player, correct);

            collider.enabled = false;
            if (!correct)
            {
                hologram.material.SetColor(Color.red);
            }

            StartCoroutine(BalloonPopper());
        }

        public override void ReInit()
        {
            base.ReInit();

            List<PotentialPairBalloonData> _values = new List<PotentialPairBalloonData>(values);
            List<int> usedSums = new List<int>();

            int counter = balloonAmmount / 2;

            while (counter > 0)
            {
                PotentialPairBalloonData val1 = _values.GetRandomElementAndRemove();
                PotentialPairBalloonData val2 = _values.GetRandomElementAndRemove();

                while (_values.Count > 0)
                {
                    if (!usedSums.Contains(val1.value + val2.value))
                    {
                        usedSums.Add(val1.value + val2.value);
                        break;
                    }
                    val2 = _values.GetRandomElementAndRemove();
                }

                PairsComparatorData data = balloonData[counter - 1];
                data.locked = false;
                data.totalValue = val1.value + val2.value;
                data.balloon.Renderer.sprite = val1.sprite;
                data.balloon.ConnectedBalloon.Renderer.sprite = val2.sprite;
                data.balloon.value = val1.value;
                data.balloon.ConnectedBalloon.value = val2.value;
                data.balloon.Reset();
                data.balloon.ConnectedBalloon.Reset();
                data.balloon.ReInit();
                data.balloon.ConnectedBalloon.ReInit();

                balloonData[counter - 1] = data;
                counter--;
            }

            hologram.material.SetColor(Color.white);

            collider.enabled = true;
            notebook.transform.position = transform.position;
            notebook.gameObject.SetActive(value: false);

            balloonData[chosenPair].balloon.HideClick(true);
            balloonData[chosenPair].balloon.ConnectedBalloon.HideClick(true);
        }

        public void SelectPair(PairBalloon balloon)
        {
            if (balloonData[chosenPair].balloon != null)
            {
                balloonData[chosenPair].balloon.HideClick(false);
                balloonData[chosenPair].balloon.ConnectedBalloon.HideClick(false);
            }

            for (int i = 0; i < balloonData.Count; i++)
            {
                if (balloonData[i].balloon == balloon || balloonData[i].balloon.ConnectedBalloon == balloon)
                {
                    if (balloonData[i].locked || chosenPair == i) break;

                    chosenPair = i;
                    balloonData[chosenPair].balloon.HideClick(true);
                    balloonData[chosenPair].balloon.ConnectedBalloon.HideClick(true);
                    switching = true;
                    StartCoroutine(Switcher());
                    break;
                }
            }
        }

        private void SpawnBalloons()
        {
            balloonData = new List<PairsComparatorData>();
            float angle = 360f / balloonAmmount;
            int counter = balloonAmmount / 2;
            float currentAngle = 0f;

            while (counter > 0)
            {
                PairBalloon balloon1 = Instantiate(balloonPre, transform);
                PairBalloon balloon2 = Instantiate(balloonPre, transform);

                balloon1.Initialize(this);
                balloon2.Initialize(this);
                balloon2.Connect(balloon1);
                balloon1.transform.localPosition =
                    new Vector3(spawnRadius * Mathf.Cos(Mathf.Deg2Rad * currentAngle), 0f, 
                        spawnRadius * Mathf.Sin(Mathf.Deg2Rad * currentAngle));
                balloon2.transform.localPosition =
                    new Vector3(-spawnRadius * Mathf.Cos(Mathf.Deg2Rad * currentAngle), 0f, 
                        -spawnRadius * Mathf.Sin(Mathf.Deg2Rad * currentAngle));

                balloonData.Add(new PairsComparatorData()
                {
                    balloon = balloon1,
                    rotationAngle = Mathf.Abs(currentAngle)
                });

                currentAngle -= angle;
                counter--;
            }
        }

        public void Clicked(int player)
        {
            PairsComparatorData chosenPair = balloonData[this.chosenPair];
            if (!ClickableHidden())
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
            chosenPair.locked = true;
            balloonData[this.chosenPair] = chosenPair;

            while (delay > 0f)
            {
                delay -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                yield return null;
            }

            chosenPair.balloon.Reveal(correct);
            chosenPair.balloon.ConnectedBalloon.Reveal(correct);

            while (chosenPair.balloon.Revealing)
            {
                yield return null;
            }

            if (correct)
            {
                audMan.PlaySingle(audCorrect);
                if (finalAction)
                {
                    Completed(player, true);
                    room.functions.OnActivityCompletion();
                    if (finalPoints != 0)
                        CoreGameManager.Instance.AddPoints(finalPoints, 0, true);
                }
                else
                {
                    room.functions.OnActivityProgress();
                    if (pointsPerPair != 0)
                        CoreGameManager.Instance.AddPoints(pointsPerPair, 0, true);
                }

                BaseGameManager.Instance.PleaseBaldi(baldiPause, rewardSticker: true);
            }
            else
            {
                BaseGameManager.Instance.AngerBaldi(1f);
                room.ec.MakeNoise(transform.position, wrongNoiseVal);
                audMan.PlaySingle(audIncorrect);
                Completed(player, false);
                room.functions.OnActivityCompletion();
            }
        }

        private IEnumerator Switcher()
        {
            motorMan.QueueAudio(audWind);

            Vector3 rotation = arrow.rotation.eulerAngles;

            float timer = audWind.soundClip.length;

            while (timer > 0f)
            {
                timer -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                rotation.y += Time.deltaTime * room.ec.EnvironmentTimeScale * rotationAnimationSpeed;
                arrow.rotation = Quaternion.Euler(rotation);
                yield return null;
            }

            motorMan.FlushQueue(true);
            motorMan.PlaySingle(audWindEnd);

            rotation.y = balloonData[chosenPair].rotationAngle + transform.rotation.eulerAngles.y;
            arrow.rotation = Quaternion.Euler(rotation);

            balloonData[chosenPair].balloon.AnimateSelection();
            balloonData[chosenPair].balloon.ConnectedBalloon.AnimateSelection();

            switching = false;
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

            time = 0.05f;

            while (time > 0f)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                yield return null;
            }

            initialTime = 0.25f;
            time = initialTime;

            float multiplier = 1f;

            while (time > 0f)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale * multiplier;
                multiplier += Time.deltaTime * 5f;
                if (time < 0f) time = 0f;
                pos.y = pulleySpriteOffset - 1f - 1f * (1f - time / initialTime);
                pulleyRenderer.transform.localPosition = pos;
                yield return null;
            }

            initialTime = 0.75f;
            time = initialTime;

            multiplier = 1f;

            while (time > 0f)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale * multiplier;
                multiplier += Time.deltaTime * room.ec.EnvironmentTimeScale * 10f;
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
            for (int i = 0; i < balloonData.Count; i++) 
            {
                balloons.Add(balloonData[i].balloon);
                balloons.Add(balloonData[i].balloon.ConnectedBalloon);
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
            PairsComparatorData chosenPair = balloonData[this.chosenPair];
            return completed || !powered || IsPlayingAnimations || chosenPair.balloon == null || chosenPair.locked;
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

        private bool clickHidden;

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
            destroyOnPop = false;

            entity.ReflectionSetVariable("persistent", true);
        }

        public void Reset()
        {
            revealed = false;
            clickHidden = false;
        }

        public void Initialize(PairsComparator comparator)
        {
            this.comparator = comparator;
            floater.Initialize(comparator.room);
        }

        public void HideClick(bool state) => clickHidden = state;

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
            if (!ClickableHidden())
                comparator.SelectPair(this);
        }

        public override bool ClickableHidden()
        {
            return base.ClickableHidden() || clickHidden || revealed || !comparator.Powered || 
                comparator.IsPlayingAnimations || comparator.TrulyCompleted;
        }
    }
}
