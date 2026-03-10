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
        public CompassBalloon balloon;

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
        private SpriteRenderer pulleyRenderer;

        [SerializeField]
        private CompassBalloon balloonPre;

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
        private int bonusPointsPerPair;

        [SerializeField]
        private int bonusFinalPoints;

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
            bonusPointsPerPair = 20;
            bonusFinalPoints = 40;

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

            Renderer[] renderers = new Renderer[2];
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i] = ObjectCreator.CreateQuadRenderer();
                renderers[i].transform.localScale = new Vector3(20f, 20f, 1f);
                renderers[i].transform.SetParent(transform, false);
                renderers[i].transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            }

            renderers[0].material.mainTexture = AssetStorage.textures["PairsComparator_Base"];
            renderers[1].material.mainTexture = AssetStorage.textures["PairsComparator_Arrow"];
            renderers[0].transform.localPosition = Vector3.up * -5f;
            renderers[1].transform.localPosition = Vector3.up * -4.95f;
            arrow = renderers[1].transform;

            // Own hanging sign
            Animator sign = Instantiate(AssetHelper.LoadAsset<Animator>("ActivityExteriorSign"));
            sign.transform.SetParent(transform, false);
            sign.transform.localPosition = Vector3.up * -4f;
            sign.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            this.ReflectionSetValue("exteriorSigns", new List<Animator>() { sign });

            // Outside sign
            this.ReflectionSetValue("exteriorSignPrefab", PrefabCreator.CreateActivityWallSign("ActivityExteriorSign_PairsComparator", 
                AssetStorage.sprites["PairsComparator_WallSign_Right"], AssetStorage.sprites["PairsComparator_WallSign_Left"]));

            collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 5f;

            balloonPre = PrefabCreator.CreateBalloonPrefab<CompassBalloon>($"PairBalloon_0", $"pair_balloon_0");

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
            StartCoroutine(BalloonPopper());
        }

        public override void ReInit()
        {
            base.ReInit();
            List<int> usedSums = new List<int>();
            int counter = balloonAmmount / 2;
            while (counter > 0)
            {
                List<PotentialPairBalloonData> _values = new List<PotentialPairBalloonData>(values);
                PotentialPairBalloonData val1 = _values.GetRandomElementAndRemove();
                PotentialPairBalloonData val2 = default;

                while (_values.Count > 0)
                {
                    val2 = _values.GetRandomElementAndRemove();
                    if (!usedSums.Contains(val1.value + val2.value))
                    {
                        usedSums.Add(val1.value + val2.value);
                        break;
                    }
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

            collider.enabled = true;
            notebook.transform.position = transform.position;
            notebook.gameObject.SetActive(value: false);

            balloonData[chosenPair].balloon.HideClick(true);
            balloonData[chosenPair].balloon.ConnectedBalloon.HideClick(true);
        }

        public void SelectPair(CompassBalloon balloon)
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
                CompassBalloon balloon1 = Instantiate(balloonPre, transform);
                CompassBalloon balloon2 = Instantiate(balloonPre, transform);

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
            PairsComparatorData _chosenPair = balloonData[chosenPair];
            if (!ClickableHidden())
            {
                int nonLockedPairsLeft = 0;
                int lastPairIndex = 0;
                bool correct = true;
                for (int i = 0; i < balloonData.Count; i++)
                {
                    if (!balloonData[i].locked && balloonData[i].balloon != _chosenPair.balloon)
                    {
                        nonLockedPairsLeft++;
                        lastPairIndex = i;
                    }

                    if (!balloonData[i].locked && _chosenPair.totalValue > balloonData[i].totalValue && 
                        balloonData[i].totalValue != _chosenPair.totalValue)
                    {
                        correct = false;
                    }
                }

                pulling = true;
                // Player does not need anymore click last pair to complete activity
                // This change is made since Match Activity follows the same rule beginning from 0.14
                if (nonLockedPairsLeft == 1 && correct)
                {
                    StartCoroutine(ActionCompleter(lastPairIndex, player, correct, false, 1f));
                    StartCoroutine(ActionCompleter(chosenPair, player, correct, true, 1f));
                }
                else
                {
                    StartCoroutine(ActionCompleter(chosenPair, player, correct, false, 1f));
                }
                StartCoroutine(PulleyAnimator());
            }
        }

        private IEnumerator ActionCompleter(int index, int player, bool correct, bool finalAction, float delay)
        {
            PairsComparatorData chosenPair = balloonData[index];
            chosenPair.locked = true;
            balloonData[index] = chosenPair;

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
                    CoreGameManager.Instance.AddPoints(bonusMode ? bonusFinalPoints : finalPoints, 0, true);
                }
                else
                {
                    room.functions.OnActivityProgress();
                    CoreGameManager.Instance.AddPoints(bonusMode ? bonusPointsPerPair : pointsPerPair, 0, true);
                }
                CoreGameManager.Instance.GetPlayer(0).plm.AddStamina(CoreGameManager.Instance.GetPlayer(0).plm.StaminaMax, limited: true);
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

        // Hardcoded recreation
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
            List<CompassBalloon> balloons = new List<CompassBalloon>();
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

    public class CompassBalloon : BaseBalloonBehaviour
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

        private CompassBalloon connectedInstance;

        private bool revealed;

        private bool clickHidden;

        public int value;

        public CompassBalloon ConnectedBalloon => connectedInstance;

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
            Renderer.color = Color.white;
            revealed = false;
            clickHidden = false;
        }

        public void Initialize(PairsComparator comparator)
        {
            this.comparator = comparator;
            floater.Initialize(comparator.room);
        }

        public void HideClick(bool state) => clickHidden = state;

        public void Connect(CompassBalloon balloon)
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
