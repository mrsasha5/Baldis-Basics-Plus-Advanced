using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Spelling
{

    public class SymbolMachine : MonoBehaviour, IClickable<int>, IPrefab
    {
        [SerializeField]
        private SoundObject audCorrect;

        [SerializeField]
        private SoundObject audIncorrect;

        [SerializeField]
        private SoundObject audBeep;

        [SerializeField]
        private SoundObject audReinit;

        [SerializeField]
        private SoundObject audBell;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private List<TextMeshPro> texts;

        [SerializeField]
        private TextMeshPro screenText;

        [SerializeField]
        private int maxSpelloonsCount = 10;

        [SerializeField]
        private int symbolPointsPrice = 15;

        [SerializeField]
        private int wrongNoiseVal = 126;

        [SerializeField]
        private float symbolTime = 4f;

        public List<string> potentialSymbols = new List<string>();

        private Pickup reward;

        private RewardType rewardType = RewardType.None;

        private List<Spelloon> spelloons = new List<Spelloon>();

        private List<string> symbolsSpawned = new List<string>();

        private IEnumerator reinitializer; //The actual reason why it is not coroutine is
                                           //that corounties will be stopped when object is disabled
                                           //and that will happen on PIT Stop due of the 3D Trips which create a new scene and
                                           //disable all environment objects

        private bool playerIsHolding;

        private int playerHolding;

        private string answer = "";

        private string answerField = "";

        private bool completed = true;

        private bool reInitializing;

        private int rightSymbols;

        private RoomController room;

        private bool _roomPowered;

        private string _screenText;

        private string _faceName;

        private bool makeFailedOnUpdate;

        private bool isPitFloor;

        private float time;

        private bool timerActive;

        private bool playerRewarded;

        private bool timerMode;

        public string AnswerField => answerField;

        public bool TimerActive => timerActive;

        public bool IsCompleted => completed;

        public bool PlayerRewarded => playerRewarded;

        public float SymbolTime => symbolTime;

        public RoomController Room => room;

        public void InitializePrefab(int variant)
        {
            audCorrect = AssetStorage.sounds["activity_correct"];
            audIncorrect = AssetStorage.sounds["activity_incorrect"];
            audBell = AssetStorage.sounds["bell"];
            audBeep = AssetStorage.sounds["adv_beep"];
            audReinit = AssetStorage.sounds["adv_symbol_machine_reinit"];

            audMan = ObjectCreator.CreateAudMan(Vector3.zero);
            audMan.transform.SetParent(transform, false);

            texts = new List<TextMeshPro>();

            GameObject model = new GameObject("Model");
            model.transform.SetParent(transform, false);

            ClickableLink link = model.AddComponent<ClickableLink>();
            ReflectionHelper.SetValue<MonoBehaviour>(link, "link", this);

            model.AddComponent<MeshFilter>()
                .mesh = AssetHelper.LoadAsset<Mesh>("MathMachine_Final_Mesh");
            meshRenderer = model.AddComponent<MeshRenderer>();

            Material[] materials = new Material[3];
            materials[0] = new Material(AssetStorage.materials["math_front_normal"]);
            materials[1] = new Material(AssetStorage.materials["math_side"]);
            materials[2] = materials[1];

            materials[0].SetMainTexture(AssetStorage.textures["adv_symbol_machine_face"]);
            materials[1].SetMainTexture(AssetStorage.textures["adv_symbol_machine_side"]);

            meshRenderer.materials = materials;

            BoxCollider collider = model.AddComponent<BoxCollider>();
            collider.size = new Vector3(10f, 9f, 7f);
            collider.center = new Vector3(0f, 4.501f, 1.5f);

            NavMeshObstacle navObstacle = model.AddComponent<NavMeshObstacle>();
            navObstacle.shape = NavMeshObstacleShape.Box;
            navObstacle.size = collider.size;
            navObstacle.center = collider.center;

            Vector3[] symbolPositions = new Vector3[] 
                { new Vector3(-3.72f, 3.37f, -2f), new Vector3(-2.01f, 3.37f, -2f), new Vector3(-0.36f, 3.37f, -2f), 
                    new Vector3(1.35f, 3.37f, -2f), new Vector3(3f, 3.37f, -2f)};

            for (int i = 0; i < symbolPositions.Length; i++)
            {
                RectTransform rect = new GameObject($"Symbol{i + 1}").AddComponent<RectTransform>();
                rect.localPosition = symbolPositions[i];

                TextMeshPro text = rect.gameObject.AddComponent<TextMeshPro>();
                text.text = "";
                text.enableKerning = true;
                text.enableWordWrapping = true;
                text.vertexBufferAutoSizeReduction = true;
                text.alignment = TextAlignmentOptions.Center;
                text.font = BaldiFonts.ComicSans24.FontAsset(); //it always should be setted before it will be converted to prefab
                text.fontSize = BaldiFonts.ComicSans24.FontSize();
                text.color = Color.black;

                rect.SetParent(transform, false); //by this reason it's here!

                rect.localScale = Vector3.one * 0.7f;
                
                texts.Add(text);
            }

            RectTransform screenRect = new GameObject("ScreenText").AddComponent<RectTransform>();
            screenRect.localPosition = new Vector3(-1.65f, 7.2f, -2f);
            
            screenText = screenRect.gameObject.AddComponent<TextMeshPro>();
            screenText.text = "";
            screenText.alignment = TextAlignmentOptions.Center;
            screenText.font = BaldiFonts.ComicSans24.FontAsset();
            screenText.enableKerning = true;
            screenText.enableWordWrapping = true;
            screenText.vertexBufferAutoSizeReduction = true;
            screenText.fontSize = BaldiFonts.ComicSans24.FontSize();
            screenText.color = Color.red;

            screenRect.SetParent(transform, false);

            screenRect.localScale = Vector3.one * 0.23f;
            screenRect.sizeDelta = new Vector2(21f, 4f);
        }

        private void Start()
        {
            room = transform.parent.parent.GetComponent<RoomController>();
            _roomPowered = room.Powered;
            room.ec.OnEnvironmentBeginPlay += Initialize;
            isPitFloor = Singleton<BaseGameManager>.Instance is PitstopGameManager;
        }

        private void Update()
        {
            if (reinitializer != null && !reinitializer.MoveNext()) reinitializer = null;

            for (int i = 0; i < spelloons.Count; i++)
            {
                if (spelloons[i] == null)
                {
                    SpawnSpelloon(symbolsSpawned[i], i);
                }
            }

            if (room.Powered != _roomPowered)
            {
                _roomPowered = room.Powered;
                _screenText = screenText.text;
                string __faceName = _faceName; 

                SetScreenText(room.Powered ? _screenText : "");
                UpdateVisualAnswerField(clear: !room.Powered);
                SetFaceTex(room.Powered ? _faceName : "adv_symbol_machine_face");

                _faceName = __faceName;
            }

            if (!room.Powered || completed) return;

            if (makeFailedOnUpdate)
            {
                makeFailedOnUpdate = false;
                OnCompleted(false);
            }

            if (timerActive && time > 0f)
            {
                int time1 = (int)time;
                time -= Time.deltaTime;
                int time2 = (int)time;

                if (time <= 0f) OnCompleted(solved: false);
                else if (time1 != time2)
                {
                    UpdateTextProgress();
                    audMan.PlaySingle(audBeep);
                }
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < spelloons.Count; i++)
            {
                if (spelloons[i] != null && !spelloons[i].Popping && room.ec.CellFromPosition(spelloons[i].transform.position).room != room)
                {
                    spelloons[i].Pop();
                }
            }
        }

        private void Initialize()
        {
            if (isPitFloor && Random.value > 0.25f)
            {
                SetScreenText("");
                return;
            }

            GenerateProblem();            
        }

        public bool ReInit()
        {
            if (reInitializing || !room.Powered) return false;
            if (ApiManager.GetAllSymbolMachineWords().Count == 0)
            {
                SetScreenTextKey("Adv_Phrase_SM_No_Words");
                audMan.PlaySingle(audIncorrect);
                return false;
            }
            reinitializer = Reinitializer();
            return true;
        }

        private IEnumerator Reinitializer()
        {
            reInitializing = true;

            if (!completed) OnCompleted(solved: false);

            SetFaceTex("adv_symbol_machine_face");

            audMan.PlaySingle(audReinit);

            float time = 0f;

            while (time < audReinit.soundClip.length)
            {
                time += Time.deltaTime;
                SetScreenText(string.Format("Adv_Phrase_SM_ReInit".Localize(),
                    (int)(time / audReinit.soundClip.length * 100f)));
                yield return null;
            }

            while (spelloons.Count > 0) yield return null;

            SetScreenText(string.Format(
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Phrase_SM_ReInit"), 100));
            GenerateProblem();
            audMan.PlaySingle(audBell);
            reInitializing = false;
            yield break;
        }

        private void GenerateProblem()
        {
            if (ObjectStorage.SymbolMachineWords.Count > 0)
            {
                List<string> words = ApiManager.GetAllSymbolMachineWords();

                int wordNum = Random.Range(0, words.Count);

                answer = words[wordNum];

                ClearAnswerField();

                SpelloonsSpawner();

                UpdateTextProgress();

                SetFaceTex("adv_symbol_machine_face");

                if (!isPitFloor) SetRewardType(GetItemRewardTypeByAnswer(), 15);
                else SetRewardType(RewardType.Points, 30);

                completed = false;
                playerRewarded = false;
            }
            else
            {
                SetScreenTextKey("Adv_Phrase_SM_No_Words");
                audMan.PlaySingle(audIncorrect);
            }
        }

        public void Clicked(int player)
        {
            if (room.Powered && playerIsHolding && !completed && spelloons[playerHolding].trackingPlayer)
            {
                bool symbolIsRight = spelloons[playerHolding].Value == answer.ElementAt(answerField.Length).ToString().ToLower();

                texts[answerField.Length].text = spelloons[playerHolding].Value.ToUpper();
                answerField += spelloons[playerHolding].Value;

                if (symbolIsRight)
                {
                    rightSymbols++;
                    UpdateTextProgress();
                    if (answerField == answer.ToLower()) OnCompleted(solved: true);
                    else OnPutNextSymbol();
                }
                else
                {
                    OnCompleted(solved: false);
                }

                if (spelloons.Count - 1 >= playerHolding)
                {
                    spelloons[playerHolding].trackingPlayer = false;
                    spelloons[playerHolding].Use();
                }
                
                NumberDropped();
            }
        }

        private void OnPutNextSymbol()
        {
            audMan.PlaySingle(audCorrect);
            time = symbolTime;
        }

        public void OnCompleted(bool solved)
        {
            if (completed) return;

            completed = true;
            if (solved)
            {
                audMan.PlaySingle(audCorrect);
                SetScreenTextKey(GetPhrase(good: true));
                SetFaceTex("adv_symbol_machine_face_right");

                DropReward();
            }
            else
            {
                SetScreenTextKey(GetPhrase(good: false));
                audMan.PlaySingle(audIncorrect);
                SetFaceTex("adv_symbol_machine_face_wrong");

                DropReward(RewardType.Points);
                room.ec.MakeNoise(transform.position, wrongNoiseVal);

                answerField = answer;
                UpdateVisualAnswerField(clear: false);
            }

            timerActive = false;

            StartCoroutine(SpelloonPopper());
        }

        public void UpdateSymbolTimer(bool active, float symbolTime)
        {
            timerActive = active;
            if (symbolTime == float.NegativeInfinity)
            {
                makeFailedOnUpdate = true;
                return;
            }
            this.symbolTime = symbolTime;
            time = symbolTime;
        }

        public void NumberDropped()
        {
            playerIsHolding = false;
        }

        private void DropReward(RewardType rewardType = RewardType.None)
        {
            if (rewardType == RewardType.None) rewardType = this.rewardType;

            RewardType[] itemTypes = new RewardType[] { RewardType.PerfectItem, RewardType.GoodItem, RewardType.NormalItem, RewardType.CommonItem };
            if (itemTypes.Contains(rewardType))
            {
                string qualityTag = TagStorage.noneRate;
                qualityTag = rewardType.ConvertToTag();

                ItemMetaData[] metas = ItemMetaStorage.Instance.FindAll(x => x.tags.Contains(TagStorage.symbolMachinePotentialReward) 
                    && x.tags.Contains(qualityTag));
                if (metas.Length > 0)
                {
                    reward = Instantiate(AssetStorage.pickup, transform);
                    reward.transform.localPosition = new Vector3(0f, 5f, -5f);

                    ItemObject item = metas[Random.Range(0, metas.Length)].value;
                    reward.item = item;
                }
                else GivePointsReward();
                
            } else if (rewardType == RewardType.Points) GivePointsReward();

            if (rewardType != RewardType.None) playerRewarded = true;
        }

        public void NumberClicked(int index)
        {
            if (playerIsHolding)
            {
                spelloons[playerHolding].ReInit();
                spelloons[playerHolding].Floater.Entity.Teleport(spelloons[index].transform.position);
            }

            playerIsHolding = true;
            playerHolding = index;
            spelloons[index].TrackPlayer(Singleton<CoreGameManager>.Instance.GetPlayer(0), index);
        }

        private void SpelloonsSpawner()
        {
            potentialSymbols.Mix();

            for (int i = 0; i < answer.Length; i++)
            {
                string symbol = answer[i].ToString().ToLower();
                if (symbolsSpawned.Contains(symbol)) continue;
                SpawnSpelloon(symbol);
            }

            for (int i = 0; i < potentialSymbols.Count; i++)
            {
                if (spelloons.Count >= maxSpelloonsCount) break;
                string symbol = potentialSymbols[i].ToString();
                if (symbolsSpawned.Contains(symbol)) continue;
                SpawnSpelloon(symbol);
            }
        }

        private void UpdateTextProgress()
        {
            if (timerMode)
                screenText.SetText(string.Format(
                    LocalizationManager.Instance.GetLocalizedText("Adv_Phrase_SM_Timed"),
                        answer, (int)time + 1));
            else
                screenText.SetText(string.Format(
                    LocalizationManager.Instance.GetLocalizedText("Adv_Phrase_SM_Progress"),
                        answer, (float)rightSymbols / answer.Length * 100f));
        }

        private void SetScreenText(string text)
        {
            screenText.text = text;
        }

        private void SetScreenTextKey(string key)
        {
            screenText.text = LocalizationManager.Instance.GetLocalizedText(key);
        }

        private string GetPhrase(bool good)
        {
            if (good) return "Adv_Phrase_Good_" + Random.Range(1, 4);
            else return "Adv_Phrase_Bad_" + Random.Range(1, 4);
        }

        private void SetFaceTex(string assetKey)
        {
            _faceName = assetKey;
            Material[] materials = meshRenderer.materials;

            materials[0].SetMainTexture(AssetStorage.textures[assetKey]);
        }

        public void SetRewardType(RewardType rewardType, int symbolPrice)
        {
            this.rewardType = rewardType;
            this.symbolPointsPrice = symbolPrice;
        }

        private void GivePointsReward()
        {
            int points = rightSymbols * symbolPointsPrice;
            if (points != 0) Singleton<CoreGameManager>.Instance.AddPoints(points, 0, true);
        }

        private RewardType GetItemRewardTypeByAnswer()
        {
            if (answer.Length >= 5) return RewardType.PerfectItem;
            if (answer.Length >= 4) return RewardType.GoodItem;
            if (answer.Length >= 3) return RewardType.NormalItem;
            if (answer.Length <= 2) return RewardType.CommonItem;
            return RewardType.None;
        }

        public void SetTimerMode(bool timerMode, bool updadeProgress = true)
        {
            this.timerMode = timerMode;
            if (updadeProgress) UpdateTextProgress();
        }

        private void UpdateVisualAnswerField(bool clear)
        {
            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].text = "";
            }
            if (clear) return;
            for (int i = 0; i < answerField.Length; i++)
            {
                texts[i].text = answerField[i].ToString().ToUpper();
            }
        }

        private void ClearAnswerField()
        {
            answerField = "";

            rightSymbols = 0;

            UpdateVisualAnswerField(clear: true);
        }

        private Spelloon SpawnSpelloon(string symbol)
        {
            Spelloon spelloon = Instantiate(ObjectStorage.Spelloons["spelloon_" + symbol], transform.parent);
            spelloon.Initialize(spelloons.Count);
            spelloon.Floater.Initialize(room);
            spelloon.symbolMachine = this;
            spelloons.Add(spelloon);
            symbolsSpawned.Add(symbol);
            return spelloon;
        }

        private Spelloon SpawnSpelloon(string symbol, int listIndex)
        {
            Spelloon spelloon = Instantiate(ObjectStorage.Spelloons["spelloon_" + symbol], transform.parent);
            spelloon.Initialize(listIndex);
            spelloon.Floater.Initialize(room);
            spelloon.symbolMachine = this;
            spelloons[listIndex] = spelloon;
            symbolsSpawned[listIndex] = symbol;
            return spelloon;
        }

        private IEnumerator SpelloonPopper()
        {
            for (int i = 0; i < spelloons.Count; i++)
            {
                if (!spelloons[i].Floater.isActiveAndEnabled)
                {
                    GameObject.Destroy(spelloons[i].gameObject);
                    spelloons.RemoveAt(i);
                    symbolsSpawned.RemoveAt(i);
                    i--;
                }
                else
                {
                    spelloons[i].Disable();
                }
            }

            float minDelay = 0.1f;
            float maxDelay = 0.3f;
            while (spelloons.Count > 0)
            {
                int index = Random.Range(0, spelloons.Count);
                spelloons[index].Pop();
                spelloons.RemoveAt(index);
                symbolsSpawned.RemoveAt(index);
                float delay = Random.Range(minDelay, maxDelay);
                while (delay > 0f)
                {
                    delay -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                    yield return null;
                }
            }
        }

        public bool ClickableHidden()
        {
            return false;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return false;
        }

        public void ClickableSighted(int player)
        {

        }

        public void ClickableUnsighted(int player)
        {

        }

    }
}