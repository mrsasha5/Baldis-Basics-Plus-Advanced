using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Rooms.Functions;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.GameEventsProvider;
using BepInEx;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Spelling
{

    public class SymbolMachine : MonoBehaviour, IClickable<int>, IPrefab
    {
        [SerializeField]
        private SoundObject audBuzz;

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

        private Queue<IEnumerator> switcherAnimations = new Queue<IEnumerator>();

        private IEnumerator currentSwitcher;

        private IEnumerator reinitializer;

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

        private bool lightsActivated;

        private bool timerActive;

        private bool playerRewarded;

        private bool timerMode;

        //private Coroutine lightsSwitcher;

        public string AnswerField => answerField;

        public bool TimerActive => timerActive;

        public bool Completed => completed;

        public bool PlayerRewarded => playerRewarded;

        public float SymbolTime => symbolTime;

        public void InitializePrefab(int variant)
        {
            audBuzz = AssetsStorage.sounds["buzz_elv"];

            audMan = ObjectsCreator.CreateAudMan(Vector3.zero);
            audMan.transform.SetParent(transform, false);

            texts = new List<TextMeshPro>();

            GameObject model = new GameObject("Model");
            model.transform.SetParent(transform, false);

            ClickableLink link = model.AddComponent<ClickableLink>();
            ReflectionHelper.SetValue<MonoBehaviour>(link, "link", this);

            model.AddComponent<MeshFilter>()
                .mesh = AssetsHelper.LoadAsset<Mesh>("MathMachine_Final_Mesh");
            meshRenderer = model.AddComponent<MeshRenderer>();

            Material[] materials = new Material[3];
            materials[0] = new Material(AssetsStorage.materials["math_front_normal"]);
            materials[1] = new Material(AssetsStorage.materials["math_side"]);
            materials[2] = materials[1];

            materials[0].SetMainTexture(AssetsStorage.textures["adv_symbol_machine_face"]);
            materials[1].SetMainTexture(AssetsStorage.textures["adv_symbol_machine_side"]);

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

        public void OnGenerationFinishedInTimedRoom()
        {
            if (isPitFloor)
            {
                for (int i = 0; i < room.cells.Count; i++)
                {
                    if (room.cells[i].hasLight)
                    {
                        room.cells[i].SetLight(false);
                    }
                }
            }
        }

        private void Start()
        {
            room = transform.parent.parent.GetComponent<RoomController>();
            _roomPowered = room.Powered;
            if (room.Powered) room.ec.OnEnvironmentBeginPlay += Initialize;
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

            if (!room.Powered) return;

            if (switcherAnimations.Count > 0 && currentSwitcher == null)
            {
                currentSwitcher = switcherAnimations.Dequeue();
            }

            if (currentSwitcher != null && !currentSwitcher.MoveNext()) currentSwitcher = null;

            if (completed) return;

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

                if (time <= 0f) OnCompleted(decidedCorrectly: false);
                else if (time1 != time2)
                {
                    UpdateTextProgress();
                    audMan.PlaySingle(AssetsStorage.sounds["adv_beep"]);
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
                //SetScreenTextKey("Adv_Phrase_SM_Pit_NotActive");
                SetScreenText("");
                return;
            }

            if (isPitFloor) SwitchLight(true);
            GenerateProblem();
            
        }

        public bool ReInit()
        {
            if (reInitializing || !room.Powered) return false;
            if (ApiManager.GetAllSymbolMachineWords().Count == 0)
            {
                SetScreenTextKey("Adv_Phrase_SM_No_Words");
                audMan.PlaySingle(audBuzz);
                return false;
            }
            reinitializer = Reinitializer();
            return true;
        }

        private IEnumerator Reinitializer()
        {
            reInitializing = true;
            if (isPitFloor) SwitchLight(true);

            if (!completed) OnCompleted(decidedCorrectly: false);

            SetFaceTex("adv_symbol_machine_face");

            SoundObject reinitSound = AssetsStorage.sounds["adv_symbol_machine_reinit"];

            audMan.PlaySingle(reinitSound);

            float time = 0f;

            while (time < reinitSound.soundClip.length)
            {
                time += Time.deltaTime;
                SetScreenText(string.Format("Adv_Phrase_SM_ReInit".Localize(),
                    (int)(time / reinitSound.soundClip.length * 100f)));
                yield return null;
            }

            while (spelloons.Count > 0) yield return null;

            SetScreenText(string.Format(
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Phrase_SM_ReInit"), 100));
            GenerateProblem();
            audMan.PlaySingle(AssetsStorage.sounds["bell"]);
            reInitializing = false;
            yield break;
        }

        private void GenerateProblem()
        {
            if (ObjectsStorage.SymbolMachineWords.Count > 0)
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
                audMan.PlaySingle(audBuzz);
            }
        }

        private void SwitchLight(bool setOn, float? hideFaceIn = null)
        {
            switcherAnimations.Enqueue(LightsSwitcher(setOn, hideFaceIn));
        }

        private IEnumerator LightsSwitcher(bool setOn, float? hideFaceIn = null)
        {
            if (lightsActivated == setOn) yield break;

            List<Cell> lightSources = new List<Cell>();
            for (int i = 0; i < room.cells.Count; i++)
            {
                if (room.cells[i].hasLight) lightSources.Add(room.cells[i]);
            }

            float time = 2f;

            while (lightSources.Count > 0)
            {
                time -= Time.deltaTime * room.ec.EnvironmentTimeScale;

                if (time <= 0f)
                {
                    lightSources[0].SetLight(setOn);
                    lightSources.RemoveAt(0);
                    time = 2f;
                }

                yield return null;
            }

            lightsActivated = setOn;

            if (hideFaceIn != null)
            {
                time = (float)hideFaceIn;

                while (time > 0f && completed)
                {
                    time -= Time.deltaTime * room.ec.EnvironmentTimeScale;

                    if (time <= 0f && completed)
                    {
                        SetScreenText("");
                        SetFaceTex("adv_symbol_machine_face");
                    }

                    yield return null;
                }
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
                    if (answerField == answer.ToLower()) OnCompleted(decidedCorrectly: true);
                    else audMan.PlaySingle(AssetsStorage.sounds["bell"]);
                    time = symbolTime;
                }
                else
                {
                    OnCompleted(decidedCorrectly: false);
                }

                if (spelloons.Count - 1 >= playerHolding)
                {
                    spelloons[playerHolding].trackingPlayer = false;
                    //spelloons[playerHolding].gameObject.SetActive(false);
                    spelloons[playerHolding].Use();
                }
                
                NumberDropped();
            }
        }

        private void OnCompleted(bool decidedCorrectly)
        {
            completed = true;
            if (decidedCorrectly)
            {
                audMan.PlaySingle(AssetsStorage.sounds["activity_correct"]);
                SetScreenTextKey(GetPhrase(good: true));
                SetFaceTex("adv_symbol_machine_face_right");

                DropReward();
            }
            else
            {
                SetScreenTextKey(GetPhrase(good: false));
                audMan.PlaySingle(AssetsStorage.sounds["activity_incorrect"]);
                SetFaceTex("adv_symbol_machine_face_wrong");

                DropReward(RewardType.Points);
                room.ec.MakeNoise(transform.position, wrongNoiseVal);
            }

            timerActive = false;

            StartCoroutine(SpelloonPopper());

            if (isPitFloor) SwitchLight(false, hideFaceIn: 15f);
        }

        public void SetSymbolTimer(bool active, float symbolTime)
        {
            this.timerActive = active;
            if (symbolTime == float.NegativeInfinity)
            {
                makeFailedOnUpdate = true;
                return;
            }
            this.symbolTime = symbolTime;
            this.time = symbolTime;
            
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
                string qualityTag = TagsStorage.noneRate;
                qualityTag = rewardType.ConvertToTag();

                ItemMetaData[] metas = ItemMetaStorage.Instance.FindAll(x => x.tags.Contains(TagsStorage.symbolMachinePotentialReward) 
                    && x.tags.Contains(qualityTag));
                if (metas.Length > 0)
                {
                    reward = Instantiate(AssetsStorage.pickup, transform);
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
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Phrase_SM_Timed"),
                answer, (int)time + 1));
            else
            screenText.SetText(string.Format(
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Phrase_SM_Progress"),
                answer, (float)rightSymbols / (float)answer.Length * 100f));
        }

        private void SetScreenText(string text)
        {
            screenText.text = text;
        }

        private void SetScreenTextKey(string key)
        {
            screenText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
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

            materials[0].SetMainTexture(AssetsStorage.textures[assetKey]);
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

            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].text = "";
            }
        }

        private Spelloon SpawnSpelloon(string symbol)
        {
            Spelloon spelloon = Instantiate(ObjectsStorage.Spelloons["spelloon_" + symbol], transform.parent);
            spelloon.Initialize(spelloons.Count);
            spelloon.Floater.Initialize(room);
            spelloon.symbolMachine = this;
            spelloons.Add(spelloon);
            symbolsSpawned.Add(symbol);
            return spelloon;
        }

        private Spelloon SpawnSpelloon(string symbol, int listIndex)
        {
            Spelloon spelloon = Instantiate(ObjectsStorage.Spelloons["spelloon_" + symbol], transform.parent);
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