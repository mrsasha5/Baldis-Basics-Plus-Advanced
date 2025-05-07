using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Enums;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.GameData;
using BaldisBasicsPlusAdvanced.SavedData;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.UI;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace BaldisBasicsPlusAdvanced.Patches.Shop
{
    [HarmonyPatch(typeof(StoreScreen))]
    internal class StoreHammerPatch
    {
        private static int hammerCost = 300; 

        private static int maxCharactersOnPage = 4;

        private static StoreScreen store;

        private static AudioManager audMan;

        private static StandardMenuButton hammerButton;

        private static GameObject chalkboard;

        //private static Character[] cancellableNpcs = new Character[] { Character.Playtime, Character.Bully, Character.Beans, Character.Crafters, Character.LookAt, Character.Chalkles };

        //private static Character[] ignorableNPCs = new Character[] {Character.Baldi, Character.Principal, Character.Pomp, Character.Sweep, Character.Cumulo, Character.DrReflex, Character.LookAt, Character.Prize, Character.Null };

        private static bool bought;

        private static bool noCharacters;

        public static bool selfUpdate;

        private static List<HammerPage> pages;

        private static TMP_Text pagesText;

        private static int currentPage;

        private static List<StandardMenuButton> buttons;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void onStart(StoreScreen __instance)
        {
            pages = new List<HammerPage>();
            buttons = new List<StandardMenuButton>();

            store = __instance;
            TMP_Text banPriceText = ReflectionHelper.getValue<TMP_Text>(store, "banPriceText"); //store hammer
            banPriceText.text = "";
            audMan = ReflectionHelper.getValue<AudioManager>(store, "audMan");
            if (selfUpdate) updateState();
            selfUpdate = false;
        }

        public static bool updateState()
        {
            if (store == null) return false;

            bought = false;
            noCharacters = false;
            pagesText = null;

            TMP_Text banPriceText = ReflectionHelper.getValue<TMP_Text>(store, "banPriceText"); //store hammer

            setPriceByFloor();

            hammerButton = ReflectionHelper.getValue<GameObject>(store, "banHotSpot").GetComponent<StandardMenuButton>();

            List<NPC> npcs = getPotentialNPCs(Singleton<BaseGameManager>.Instance.Ec);

            if (!DataManager.LevelDataLoaded.hammerBoughtOnCurrentFloor && (npcs.Count > 0))
            {
                hammerButton.OnPress.AddListener(onPressHammer);
                banPriceText.text = hammerCost.ToString();
            }
            else if (npcs.Count == 0)
            {
                banPriceText.text = "NO";
                banPriceText.color = Color.red;
                noCharacters = true;
            }
            else
            {
                banPriceText.text = "OUT";
                banPriceText.color = Color.black;
                bought = true;
            }

            return true;
        }

        private static void onPressHammer()
        {
            if (canBuy(hammerCost))
            {
                Transform canvas = store.GetComponentInChildren<Canvas>().transform;
                createSelectMenu(canvas);
            }
            else if (!audMan.QueuedUp)
            {
                audMan.QueueRandomAudio(ReflectionHelper.getValue<SoundObject[]>(store, "audUnafforable"));
            }
        }

        private static bool buyHammer()
        {
            ReflectionHelper.getValue(store, "ytps", out int ytps);
            if (ytps >= hammerCost)
            {
                bought = true;
                hammerButton.enabled = false;

                TMP_Text banPriceText = ReflectionHelper.getValue<TMP_Text>(store, "banPriceText"); //store hammer
                banPriceText.text = "SOLD";
                banPriceText.color = Color.red;

                bool[] itemPurchased = ReflectionHelper.getValue<bool[]>(store, "itemPurchased");
                itemPurchased[7] = true;

                ytps -= hammerCost;
                ReflectionHelper.setValue<int>(store, "ytps", ytps);
                ReflectionHelper.getValue(store, "pointsSpent", out int pointsSpent);
                pointsSpent += hammerCost;
                ReflectionHelper.setValue<int>(store, "pointsSpent", pointsSpent);
                ReflectionHelper.getValue(store, "totalPoints", out TMP_Text totalPoints);
                totalPoints.text = ytps.ToString();

                ReflectionHelper.setValue<bool>(store, "purchaseMade", true);
                audMan.QueueRandomAudio(ReflectionHelper.getValue<SoundObject[]>(store, "audBuy"));

                store.StandardDescription();

                DataManager.LevelDataLoaded.hammerBoughtOnCurrentFloor = true;
		        
                hammerButton.OnPress.RemoveAllListeners();

                return true;
            }
            else if (!audMan.QueuedUp)
            {
                audMan.QueueRandomAudio(ReflectionHelper.getValue<SoundObject[]>(store, "audUnafforable"));
            }
            return false;
        }

        private static bool canBuy(int price)
        {
            ReflectionHelper.getValue(store, "ytps", out int ytps);
            return ytps >= price;
        }

        private static void createSelectMenu(Transform canvas)
        {
            chalkboard = GameObject.Instantiate(AssetsStorage.exitConfirmChalkboardStore);
            chalkboard.name = "SelectCharacterMenu";
            chalkboard.transform.SetParent(canvas, false);
            chalkboard.SetActive(true);
            chalkboard.transform.SetSiblingIndex(canvas.childCount - 3);

            foreach (Transform something in chalkboard.transform.GetComponentsInChildren<Transform>())
            {
                if (something.name != "ChalkBoard" && something.name != "SelectCharacterMenu") GameObject.Destroy(something.gameObject);
            }

            TMP_Text text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Expel_Character_Info"),
                chalkboard.transform, new Vector3(0, 105, 0), false);
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Top;

            RectTransform buttonTransform;

            StandardMenuButton cancelButton = ObjectsCreator.createSpriteButton(AssetsStorage.sprites["adv_exit"], AssetsStorage.sprites["adv_exit_transparent"], chalkboard.transform);

            buttonTransform = cancelButton.image.rectTransform;
            buttonTransform.localPosition = new Vector3(185, 140);
            buttonTransform.sizeDelta = new Vector3(50, 50);
            cancelButton.OnPress.AddListener(onCancelBanning);

            appendCharacterButtons(chalkboard.transform);

            pagesText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans18, string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Expel_Hammer_Pages"), currentPage + 1, pages.Count),
                chalkboard.transform, new Vector3(0, -117, 0), false);
            pagesText.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);
            pagesText.color = Color.white;
            pagesText.alignment = TextAlignmentOptions.Top;

            if (pages.Count > 1)
            {
                StandardMenuButton leftArrowButton = ObjectsCreator.createSpriteButton(AssetsStorage.sprites["menuArrow2"], AssetsStorage.sprites["menuArrow0"], chalkboard.transform);

                buttonTransform = leftArrowButton.image.rectTransform;
                buttonTransform.localPosition = new Vector3(-160, -108);
                buttonTransform.sizeDelta = new Vector3(32, 32);

                leftArrowButton.OnPress.AddListener(onPressLeftArrow);

                StandardMenuButton rightArrowButton = ObjectsCreator.createSpriteButton(AssetsStorage.sprites["menuArrow3"], AssetsStorage.sprites["menuArrow1"], chalkboard.transform);

                buttonTransform = rightArrowButton.image.rectTransform;
                buttonTransform.localPosition = new Vector3(156, -108);
                buttonTransform.sizeDelta = new Vector3(32, 32);

                rightArrowButton.OnPress.AddListener(onPressRightArrow);
            }
        }

        private static void appendCharacterButtons(Transform chalkboard)
        {
            List<NPC> npcs = new List<NPC>(getPotentialNPCs(Singleton<BaseGameManager>.Instance.Ec));

            int pageCount = npcs.Count / maxCharactersOnPage;
            if (npcs.Count % maxCharactersOnPage != 0) pageCount++;

            Vector3 pos = Vector3.up * 40f;
            Vector3 addend = Vector3.up * -40f;

            pages.Clear();
            buttons.Clear();
            for (int i = 0; i < maxCharactersOnPage; i++)
            {
                TMP_Text npcText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "", chalkboard, pos, false);
                pos += addend;

                Vector2 size = new Vector2(300, 50);
                npcText.GetComponent<RectTransform>().sizeDelta = size;
                npcText.alignment = TextAlignmentOptions.Top;

                StandardMenuButton menuButton = ObjectsCreator.addButtonProperties(npcText, size, true);
                buttons.Add(menuButton);
            }

            for (int i = 0; i < pageCount; i++)
            {
                HammerPage page = new HammerPage();
                page.index = i;
                for (int j = 0; j < maxCharactersOnPage; j++)
                {
                    if (npcs.Count == 0) break;

                    NPC npc = npcs[0];
                    npcs.Remove(npc);
                    if (!DataManager.LevelDataLoaded.bannedCharacters.Contains(npc.Character))
                    {
                        page.npcs.Add(npc);
                    }
                }
                pages.Add(page);
            }

            setPage(pages[0]);

        }

        private static void setPage(HammerPage page)
        {
            foreach (StandardMenuButton button in buttons)
            {
                button.gameObject.SetActive(false);
            }
            int i = 0;
            foreach (NPC npc in page.npcs)
            {
                //string name = npc.Character.GetName();
                string name = NPCMetaStorage.Instance.Get(npc).nameLocalizationKey;
                if (name == null) name = npc.name;

                if (name.Length > 22)
                {
                    name = name.Substring(0, 19);
                    name += "...";
                }

                buttons[i].gameObject.SetActive(true);
                buttons[i].text.text = name;
                buttons[i].OnPress = new UnityEvent();
                buttons[i].OnPress.AddListener(delegate { banCharacter(npc.Character); });
                i++;
            }
        }

        private static void setPriceByFloor()
        {
            hammerCost = 999;

            if (Singleton<BaseGameManager>.Instance != null) hammerCost = Singleton<BaseGameManager>.Instance.levelObject.mapPrice;
        }

        private static void destroyChalkboard()
        {
            GameObject.Destroy(chalkboard.gameObject);
        }

        private static void banCharacter(Character character)
        {
            DataManager.LevelDataLoaded.bannedCharacters = DataManager.LevelDataLoaded.bannedCharacters.AddItem(character).ToArray();
            audMan.QueueAudio(AssetsStorage.sounds["adv_boing"], true);
            buyHammer();
            destroyChalkboard();
        }

        private static void onCancelBanning()
        {
            destroyChalkboard();
            //audMan.PlaySingle(CachedAssets.elevatorBuzz);
        }

        private static void onPressRightArrow()
        {
            currentPage++;
            if (currentPage > pages.Count - 1) currentPage = 0;
            setPage(pages[currentPage]);
            onPageChange();
        }

        private static void onPressLeftArrow()
        {
            currentPage--;
            if (currentPage < 0)
            {
                currentPage = pages.Count - 1;
            }
            setPage(pages[currentPage]);
            onPageChange();
        }

        private static void onPageChange()
        {
            audMan.PlaySingle(AssetsStorage.sounds["scissors"]);
            pagesText.text = string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Expel_Hammer_Pages"), currentPage + 1, pages.Count);
        }

        private static List<NPC> getPotentialNPCs(EnvironmentController ec)
        {
            List<NPC> npcsToIgnore = new List<NPC>();
            npcsToIgnore.AddRange(Singleton<BaseGameManager>.Instance.levelObject.forcedNpcs);

            foreach (WeightedNPC weightedNPC in Singleton<BaseGameManager>.Instance.levelObject.potentialBaldis)
            {
                npcsToIgnore.Add(weightedNPC.selection);
            }

            return ec.npcsToSpawn.FindAll(x => x.GetMeta().tags.Contains("adv_exclusion_hammer_weakness") || !npcsToIgnore.Contains(x) && !x.GetMeta().tags.Contains("adv_exclusion_hammer_immunity"));
        }

        [HarmonyPatch("UpdateDescription")]
        [HarmonyPrefix]
        private static bool onUpdateDescription(int val)
        {
            if (val == 7 && !bought)
            {
                if (!audMan.QueuedAudioIsPlaying)
                {
                    audMan.QueueAudio(AssetsStorage.sounds["Jon_InfoExpel"]);
                }
            }

            return val != 7;
        }

        [HarmonyPatch("UpdateDescription")] //for endless floors fix.
        [HarmonyPostfix]
        private static void _onUpdateDescription(int val)
        {
            if (val == 7)
            {
                LocalizationManager localization = Singleton<LocalizationManager>.Instance;

                TMP_Text itemDescription = ReflectionHelper.getValue<TMP_Text>(store, "itemDescription");

                string desc = localization.GetLocalizedText("Adv_Store_Hammer_State_Desc_Base");

                if (bought) desc += localization.GetLocalizedText("Adv_Store_Hammer_State_Desc_Additional_Base_Bought");

                if (noCharacters)
                {
                    desc += localization.GetLocalizedText("Adv_Store_Hammer_State_Desc_No");
                } else
                {
                    desc += localization.GetLocalizedText("Adv_Store_Hammer_State_Desc_Available");
                }

                itemDescription.text = desc;
            }
        }
    }

    internal class HammerPage
    {
        public int index;

        public List<NPC> npcs = new List<NPC>();

    }

}
