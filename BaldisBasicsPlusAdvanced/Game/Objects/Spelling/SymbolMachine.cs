using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Rooms.Functions;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.GameEventsProvider;
using BepInEx;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Spelling
{
#warning TODO: rebalancing?
    public class SymbolMachine : MonoBehaviour, IClickable<int>, IPrefab
    {

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

        private bool playerIsHolding;

        private int playerHolding;

        private string answer = "";

        private string answerField = "";

        private bool completed = true;

        private bool reInitializing;

        private int rightSymbols;

        private RoomController room;

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
            this.meshRenderer = GetComponentInChildren<MeshRenderer>();
            audMan = GetComponent<AudioManager>();

            ReflectionHelper.SetValue<MonoBehaviour>(GetComponentInChildren<ClickableLink>(), "link", this);

            texts = GetComponentsInChildren<TextMeshPro>().ToList();

            //296,4
            Vector3 pos = texts[3].rectTransform.localPosition;

            pos.x = 1.45f;
            texts[3].rectTransform.localPosition = pos;
            texts[3].color = Color.black;

            texts.Add(GameObject.Instantiate(texts[3], transform));
            pos.x = 3.1f;
            texts[4].rectTransform.localPosition = pos;

            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].name = "Symbol" + (i + 1);
                texts[i].transform.localScale = Vector3.one * 0.7f;
                Vector3 symbolPos = texts[i].rectTransform.localPosition;
                symbolPos.x -= 0.1f;
                texts[i].rectTransform.localPosition = symbolPos;
                texts[i].text = "";
            }

            screenText = Instantiate(texts[4], transform);
            screenText.name = "ScreenText";
            pos.x = -1.65f;
            pos.y = 7.2f;
            screenText.rectTransform.localPosition = pos;
            screenText.transform.localScale = Vector3.one * 0.23f;
            screenText.color = Color.red;
            screenText.rectTransform.sizeDelta = new Vector2(21f, 4f);

            Material[] materials = meshRenderer.materials;
            materials[0] = new Material(AssetsStorage.materials["math_front_normal"]);
            materials[1] = new Material(AssetsStorage.materials["math_side"]);
            materials[2] = materials[1];

            materials[0].SetMainTexture(AssetsStorage.textures["adv_symbol_machine_face"]);
            materials[1].SetMainTexture(AssetsStorage.textures["adv_symbol_machine_side"]);

            meshRenderer.materials = materials;
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
                        //Debug.Log("Turning off light...");
                    }
                }
            }
        }

        private void Start()
        {
            room = transform.parent.parent.GetComponent<RoomController>();
            room.ec.OnEnvironmentBeginPlay += Initialize;
            isPitFloor = Singleton<BaseGameManager>.Instance is PitstopGameManager;
        }

        private void Update()
        {
            if (completed) return;

            if (timerActive && time > 0)
            {
                int time1 = (int)time;
                time -= Time.deltaTime;
                int time2 = (int)time;

                if (time < 0) OnCompleted(decidedCorrectly: false);
                else if (time1 != time2)
                {
                    UpdateTextProgress();
                    audMan.PlaySingle(AssetsStorage.sounds["adv_beep"]);
                }
            }

            for (int i = 0; i < spelloons.Count; i++)
            {
                if (spelloons[i] == null)
                {
                    SpawnSpelloon(symbolsSpawned[i], i);
                }
            }
        }

        private void LateUpdate()
        {
            if (completed) return;

            for (int i = 0; i < spelloons.Count; i++)
            {
                if (spelloons[i] != null && !spelloons[i].Popping && room.ec.CellFromPosition(spelloons[i].transform.position).room != room)
                {
                    spelloons[i].Pop();
                }

                if (spelloons[i].Popping && !spelloons[i].AudMan.AnyAudioIsPlaying)
                {
                    Destroy(spelloons[i].gameObject);
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
            if (reInitializing) return false;
            StartCoroutine(ReInitializer());
            return true;
        }

        private IEnumerator ReInitializer()
        {
            reInitializing = true;
            if (isPitFloor) SwitchLight(true);

            if (!completed) OnCompleted(decidedCorrectly: false);

            SetFaceTex("adv_symbol_machine_face");

            SoundObject reInitSound = AssetsStorage.sounds["adv_symbol_machine_reinit"];

            audMan.PlaySingle(reInitSound);

            float time = 0f;

            while (audMan.AnyAudioIsPlaying)
            {
                time += Time.deltaTime;
                SetScreenText(string.Format(
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Phrase_SM_ReInit"),
                (int)(time / reInitSound.soundClip.length * 100)));
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
        }

        private void SwitchLight(bool setOn, float? hideFaceIn = null)
        {
            StartCoroutine(LightsSwitcher(setOn, hideFaceIn));
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

                while (time > 0f)
                {
                    time -= Time.deltaTime * room.ec.EnvironmentTimeScale;

                    if (time <= 0f)
                    {
                        SetScreenText("");
                        SetFaceTex("adv_symbol_machine_face");
                    }

                    yield return null;
                }
            }

            yield break;
        }

        public void Clicked(int player)
        {
            if (playerIsHolding && !completed && spelloons[playerHolding].trackingPlayer)
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
            if (symbolTime == -1f)
            {
                OnCompleted(false);
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
                string forcedTag = "adv_sm_potential_reward";
                string qualityTag = "adv_none";
                qualityTag = rewardType.ConvertToTag();

                ItemMetaData[] metas = ItemMetaStorage.Instance.FindAll(x => x.tags.Contains(forcedTag) && x.tags.Contains(qualityTag));
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
            //spelloon.transform.position = spelloon.transform.position + Vector3.up * 5f; //useless??
            spelloon.symbolMachine = this;
            //spelloon.AudMan.PlaySingle(AssetsStorage.sounds["adv_balloon_inflation"]); //bye bye
            spelloons.Add(spelloon);
            symbolsSpawned.Add(symbol);
            return spelloon;
        }

        private Spelloon SpawnSpelloon(string symbol, int listIndex)
        {
            Spelloon spelloon = Instantiate(ObjectsStorage.Spelloons["spelloon_" + symbol], transform.parent);
            spelloon.Initialize(listIndex);
            spelloon.Floater.Initialize(room);
            //spelloon.transform.position = spelloon.transform.position + Vector3.up * 5f; //useless??
            spelloon.symbolMachine = this;
            //spelloon.AudMan.PlaySingle(AssetsStorage.sounds["adv_balloon_inflation"]);
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