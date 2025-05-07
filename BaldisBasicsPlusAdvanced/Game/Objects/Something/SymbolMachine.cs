using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BepInEx;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Something
{
    public class SymbolMachine : MonoBehaviour, IClickable<int>, IPrefab
    {
        private Pickup reward;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private List<TextMeshPro> texts;

        [SerializeField]
        private TextMeshPro screenText;

        private List<Spelloon> spelloons = new List<Spelloon>();

        private List<string> symbolsSpawned = new List<string>();

        private bool playerIsHolding;

        private int playerHolding;

        private string answer = "";

        private string answerField = "";

        private bool completed = true;

        private RoomController room;

        public void initializePrefab()
        {
            this.meshRenderer = GetComponentInChildren<MeshRenderer>();
            audMan = GetComponent<AudioManager>();
            ReflectionHelper.setValue<MonoBehaviour>(GetComponentInChildren<ClickableLink>(), "link", this);
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
            pos.x = -1.5f;
            pos.y = 7.3f;
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

        private void Awake()
        {
            if (Singleton<BaseGameManager>.Instance != null)
            {
                Singleton<BaseGameManager>.Instance.Ec.OnEnvironmentBeginPlay += initialize;
            }
        }

        private void initialize()
        {
            room = transform.parent.parent.GetComponent<RoomController>();
            generareProblem();
        }

        public void generareProblem()
        {
            if (ObjectsStorage.SymbolMachineWords.Count > 0)
            {
                int modNum = Random.Range(0, ObjectsStorage.SymbolMachineWords.Count);

                PluginInfo key = ObjectsStorage.SymbolMachineWords.Keys.ToList()[modNum];

                int wordNum = Random.Range(0, ObjectsStorage.SymbolMachineWords[key].Count);

                answer = ObjectsStorage.SymbolMachineWords[key][wordNum];

                spelloonsSpawner();

                updateTextProgress();

                completed = false;
            }
        }

        private void Update()
        {
            if (completed) return;

            for (int i = 0; i < spelloons.Count; i++)
            {
                if (spelloons[i] == null)
                {
                    spawnSpelloon(symbolsSpawned[i], i);
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
                    spelloons[i].pop();
                }
            }
        }

        public void Clicked(int player)
        {
            if (playerIsHolding && !completed && spelloons[playerHolding].trackPlayer)
            {
                bool symbolIsRight = spelloons[playerHolding].Value == answer.ElementAt(answerField.Length).ToString().ToLower();

                texts[answerField.Length].text = spelloons[playerHolding].Value.ToUpper();
                answerField += spelloons[playerHolding].Value;

                if (symbolIsRight)
                {
                    audMan.PlaySingle(AssetsStorage.sounds["bell"]);
                    updateTextProgress();
                    if (answerField == answer) onCompleted(decidedCorrectly: true);
                }
                else
                {
                    audMan.PlaySingle(AssetsStorage.sounds["elv_buzz"]);;
                    onCompleted(decidedCorrectly: false);
                }

                spelloons[playerHolding].trackPlayer = false;
                //spelloons[playerHolding].gameObject.SetActive(false);
                spelloons[playerHolding].use();
                numberDropped();
            }
        }

        private void onCompleted(bool decidedCorrectly)
        {
            completed = true;
            if (decidedCorrectly)
            {
                setScreenText("Great job!");

                reward = Instantiate(AssetsStorage.pickup, transform);
                reward.transform.localPosition = new Vector3(0f, 5f, -5f);
                ItemMetaData[] metas = ItemMetaStorage.Instance.FindAll(x => x.id != Items.None);
                ItemObject item = metas[Random.Range(0, metas.Length)].value;
                reward.item = item;
            }
            else setScreenText("Be more careful next time!");
            StartCoroutine(BalloonPopper());
        }

        public void numberDropped()
        {
            playerIsHolding = false;
        }

        public void numberClicked(int index)
        {
            if (playerIsHolding)
            {
                spelloons[playerHolding].reInit();
                spelloons[playerHolding].Floater.Entity.Teleport(spelloons[index].transform.position);
            }

            playerIsHolding = true;
            playerHolding = index;
            spelloons[index].TrackPlayer(Singleton<CoreGameManager>.Instance.GetPlayer(0), index);
        }

        private void spelloonsSpawner()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";

            for (int i = 0; i < answer.Length; i++)
            {
                string symbol = answer[i].ToString().ToLower();
                if (symbolsSpawned.Contains(symbol)) continue;
                spawnSpelloon(symbol);
            }

            for (int i = 0; i < 13 - spelloons.Count; i++)
            {
                string symbol = alphabet[i].ToString().ToLower();
                if (symbolsSpawned.Contains(symbol)) continue;
                spawnSpelloon(symbol);
            }
        }

        private IEnumerator BalloonPopper()
        {
            for (int i = 0; i < spelloons.Count; i++)
            {
                if (!spelloons[i].Floater.isActiveAndEnabled)
                {
                    GameObject.Destroy(spelloons[i].gameObject);
                    spelloons.RemoveAt(i);
                    i--;
                }
                else
                {
                    spelloons[i].disable();
                }
            }

            float minDelay = 0.1f;
            float maxDelay = 0.3f;
            while (spelloons.Count > 0)
            {
                int index = Random.Range(0, spelloons.Count);
                spelloons[index].pop();
                spelloons.RemoveAt(index);
                float delay = Random.Range(minDelay, maxDelay);
                while (delay > 0f)
                {
                    delay -= Time.deltaTime * room.ec.EnvironmentTimeScale;
                    yield return null;
                }
            }
        }

        private Spelloon spawnSpelloon(string symbol)
        {
            Spelloon spelloon = Instantiate(ObjectsStorage.Spelloons["spelloon_" + symbol], transform.parent);
            spelloon.initialize(spelloons.Count);
            spelloon.Floater.Initialize(room);
            spelloon.transform.position = spelloon.transform.position + Vector3.up * 5f;
            spelloon.symbolMachine = this;
            spelloons.Add(spelloon);
            symbolsSpawned.Add(symbol);
            return spelloon;
        }

        private Spelloon spawnSpelloon(string symbol, int listIndex)
        {
            Spelloon spelloon = Instantiate(ObjectsStorage.Spelloons["spelloon_" + symbol], transform.parent);
            spelloon.initialize(listIndex);
            spelloon.Floater.Initialize(room);
            spelloon.symbolMachine = this;
            spelloons[listIndex] = spelloon;
            symbolsSpawned[listIndex] = symbol;
            return spelloon;
        }

        private void updateTextProgress()
        {
            screenText.SetText(string.Format("Write word: {0}\nProgress: {1}%", answer, (float)answerField.Length / (float)answer.Length * 100f));
        }

        private void setScreenText(string text)
        {
            screenText.text = text;
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
