using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.SpatialElevator;
using BaldisBasicsPlusAdvanced.Game.Components.Movement;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#warning refresh the code

namespace BaldisBasicsPlusAdvanced.Patches.UI.Elevator
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class ElevatorExpelHammerPatch
    {

        public enum Status
        {
            Available,
            ShouldBreak,
            Existing,
            NotAvailable
        }

        private static Image chalkboard;

        private static TMP_Text pagesText;

        private static Image arrowsImage;

        private static Image expelImage;

        private static List<List<NPC>> pages = new List<List<NPC>>();

        private static List<StandardMenuButton> textButtons = new List<StandardMenuButton>();

        private static int currentPageIndex;

        private static int maxCountOnPage = 4;

        private static int floorsToUnban = 2;

        private static float animSpeed = 1f;

        private static Color lockedColor = new Color(1f, 1f, 1f, 0.5f);

        private static ElevatorScreen elvScreen;

        private static bool state;

        private static bool shouldInitialize;

        public static int MaxCountOnPage => maxCountOnPage;

        public static int FloorsToUnbanValue => floorsToUnban;

        public static bool ShouldInitialize => shouldInitialize;

        public static bool Available => LevelDataManager.LevelData.hammerPurchased;

        public static List<NPC> CurrentPage => pages.Count == 0 ? new List<NPC>() : pages[currentPageIndex];

        public static Status GetStatus(BaseGameManager gameManager)
        {
            if (gameManager is PitstopGameManager || GetPotentialCharacters().Count == 0)
            {
                return Status.Existing;
            }
            else if (shouldInitialize && !Available)
            {
                return Status.ShouldBreak;
            }
            else if (shouldInitialize && Available)
            {
                return Status.Available;
            }
            else return Status.NotAvailable;
        }

        public static void OnGameManagerInit(BaseGameManager gameManager)
        {
            if (!shouldInitialize || elvScreen == null) return;

            switch (GetStatus(gameManager))
            {
                case Status.Available:
                    SetState(true, animate: true);
                    break;
                case Status.ShouldBreak:
                    elvScreen.GetComponent<AudioManager>().PlaySingle(AssetsStorage.sounds["bal_break"]);
                    expelImage.StartCoroutine(AlphaAnimation(false, animSpeed, new Color(1f, 1f, 1f, 0f), lockedColor, true));
                    break;
                default:
                    break;
            }
            
            if (arrowsImage != null) GameObject.Destroy(arrowsImage.gameObject);
        }

        [HarmonyPatch("AwakeFunction")]
        [HarmonyPostfix]
        private static void OnAwakeFunction(ElevatorScreen __instance)
        {
            shouldInitialize = false;
            if (!Available) return;
            shouldInitialize = true;

            if (IntegrationManager.IsActive<SpatialElevatorIntegration>()) return;

            elvScreen = __instance;

            expelImage = UIHelpers.CreateImage(AssetsStorage.sprites["adv_expel_hammer"], __instance.Canvas.transform,
                Vector3.zero, correctPosition: false);
            expelImage.name = "Expel Button";
            expelImage.ToCenter();
            expelImage.transform.localPosition = new Vector3(190f, -101f, 0f);
            expelImage.transform.localScale = Vector3.one * 2f;
            expelImage.tag = "Button";

            arrowsImage = UIHelpers.CreateImage(AssetsStorage.sprites["adv_arrows"], __instance.Canvas.transform,
                Vector3.zero, correctPosition: false);
            arrowsImage.ToCenter();
            arrowsImage.transform.localPosition = new Vector3(180f, -110f, 0f);
            arrowsImage.transform.localScale = Vector3.one * 1f;
            arrowsImage.tag = "Button";
            RotatableTransformBehaviour rotatedTransform = arrowsImage.gameObject.AddComponent<RotatableTransformBehaviour>();
            rotatedTransform.addend = Vector3.forward * -360f;
            rotatedTransform.unscaledTime = true;

            StandardMenuButton expelButton = expelImage.gameObject.AddComponent<StandardMenuButton>();
            expelButton.image = expelImage;
            expelButton.heldSprite = AssetsStorage.sprites["adv_expel_hammer"];
            expelButton.unhighlightedSprite = AssetsStorage.sprites["adv_expel_hammer"]; //on cursor enter
            expelButton.highlightedSprite = AssetsStorage.sprites["adv_expel_hammer"];
            expelButton.swapOnHold = true; //on press
            expelButton.swapOnHigh = true; //on high

            expelButton.eventOnHigh = true;

            expelButton.InitializeAllEvents();

            expelButton.OffHighlight = new UnityEvent();

            expelImage.transform.SetSiblingIndex(__instance.GetComponentInChildren<BigScreen>().transform.GetSiblingIndex());
            arrowsImage.transform.SetSiblingIndex(__instance.GetComponentInChildren<BigScreen>().transform.GetSiblingIndex());

            SetState(false, animate: false);
        }

        public static void CreatePages()
        {
            pages.Clear();
            textButtons.Clear();
            currentPageIndex = 0;

            List<NPC> potentialCharacters = GetPotentialCharacters();

            int counter = 0;
            foreach (NPC npc in potentialCharacters)
            {
                if (counter == 0) pages.Add(new List<NPC>());
                pages[pages.Count - 1].Add(npc);
                counter++;
                if (counter == maxCountOnPage) counter = 0;
            }
        }

        public static void SetNextPage()
        {
            //+ 1 because index
            //+ 1 because next
            if (currentPageIndex + 2 > pages.Count) currentPageIndex = 0;
            else currentPageIndex++;
            SetPage(currentPageIndex);
        }

        public static void SetBackPage()
        {
            if (currentPageIndex - 1 < 0) currentPageIndex = pages.Count - 1;
            else currentPageIndex--;
            SetPage(currentPageIndex);
        }

        public static void CreateMenu(Canvas canvas)
        {
            Singleton<GlobalCam>.Instance.Transition(UiTransition.Dither, 0.01666667f);

            chalkboard = UIHelpers.CreateImage(AssetsStorage.sprites["chalkboard_standard"], canvas.transform,
                Vector3.zero, correctPosition: false);
            chalkboard.ToCenter();

            string titleKey = "Adv_ExpelHammer_Character_Info";

            TMP_Text text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24,
                string.Format(
                    Singleton<LocalizationManager>.Instance.GetLocalizedText(titleKey), floorsToUnban),
                chalkboard.transform, new Vector3(0, 105, 0), false);
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Top;


            Image exitImage = UIHelpers.CreateImage(AssetsStorage.sprites["adv_exit_transparent"], chalkboard.transform,
                Vector3.zero, correctPosition: false);

            exitImage.ToCenter();

            exitImage.transform.localPosition = new Vector3(185, 140);//new Vector3(160, 112);
            //exitImage.transform.localScale = Vector3.one * 0.7f;
            exitImage.tag = "Button";
            //exitImage.rectTransform.sizeDelta = Vector2.one * 50f;

            StandardMenuButton exitButton = exitImage.gameObject.AddComponent<StandardMenuButton>();
            exitButton.image = exitImage;
            exitButton.heldSprite = AssetsStorage.sprites["adv_exit"];
            exitButton.unhighlightedSprite = AssetsStorage.sprites["adv_exit_transparent"];
            exitButton.highlightedSprite = AssetsStorage.sprites["adv_exit"];

            exitButton.swapOnHold = true; //on press
            exitButton.swapOnHigh = true; //on high
            exitButton.InitializeAllEvents();

            exitButton.OnPress.AddListener(
                delegate ()
                {
                    DestroyMenu();
                }
            );

            CreatePages();

            pagesText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans18, "",
                chalkboard.transform, new Vector3(0, -117, 0), false);
            pagesText.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);
            pagesText.color = Color.white;
            pagesText.alignment = TextAlignmentOptions.Top;

            AppendButtons();

            SetPage(0);

            if (pages.Count > 1)
            {
                Image leftArrowImage = UIHelpers.CreateImage(AssetsStorage.sprites["menuArrow2"], chalkboard.transform,
                Vector3.zero, correctPosition: false);
                leftArrowImage.ToCenter();
                leftArrowImage.transform.localPosition = new Vector3(-160, -108);
                leftArrowImage.rectTransform.sizeDelta = new Vector3(32, 32);
                leftArrowImage.tag = "Button";

                StandardMenuButton leftArrowButton = leftArrowImage.gameObject.AddComponent<StandardMenuButton>();
                leftArrowButton.image = leftArrowImage;
                leftArrowButton.heldSprite = AssetsStorage.sprites["menuArrow0"];
                leftArrowButton.unhighlightedSprite = AssetsStorage.sprites["menuArrow2"];
                leftArrowButton.highlightedSprite = AssetsStorage.sprites["menuArrow0"];

                leftArrowButton.swapOnHold = true; //on press
                leftArrowButton.swapOnHigh = true; //on high
                leftArrowButton.InitializeAllEvents();

                leftArrowButton.OnPress.AddListener(SetBackPage);

                Image rightArrowImage = UIHelpers.CreateImage(AssetsStorage.sprites["menuArrow3"], chalkboard.transform,
                Vector3.zero, correctPosition: false);
                rightArrowImage.ToCenter();
                rightArrowImage.transform.localPosition = new Vector3(156, -108);
                rightArrowImage.rectTransform.sizeDelta = new Vector3(32, 32);
                rightArrowImage.tag = "Button";

                StandardMenuButton rightArrowButton = rightArrowImage.gameObject.AddComponent<StandardMenuButton>();
                rightArrowButton.image = rightArrowImage;
                rightArrowButton.heldSprite = AssetsStorage.sprites["menuArrow1"];
                rightArrowButton.unhighlightedSprite = AssetsStorage.sprites["menuArrow3"];
                rightArrowButton.highlightedSprite = AssetsStorage.sprites["menuArrow1"];

                rightArrowButton.swapOnHold = true; //on press
                rightArrowButton.swapOnHigh = true; //on high
                rightArrowButton.InitializeAllEvents();

                rightArrowButton.OnPress.AddListener(SetNextPage);
            }

            chalkboard.transform.SetSiblingIndex(CursorController.Instance.transform.GetSiblingIndex()); //instead of cursor
        }

        public static void DestroyMenu()
        {
            if (elvScreen == null) return;

            Singleton<GlobalCam>.Instance.Transition(UiTransition.Dither, 0.01666667f);

            GameObject.Destroy(chalkboard.gameObject);
        }

        private static void SetState(bool available, bool animate, bool animateOnlyStateTransition = true)
        {
            bool stateChangedToNew = state != available;
            state = available;

            if (elvScreen == null) return;

            if (available)
            {
                if ((animate && stateChangedToNew) || !animateOnlyStateTransition) expelImage.StartCoroutine(AlphaAnimation(appearing: true, animSpeed, lockedColor, Color.white));
                else expelImage.color = Color.white;

                StandardMenuButton expelButton = expelImage.GetComponent<StandardMenuButton>();

                expelButton.OnPress.AddListener(
                    delegate ()
                    {
                        CreateMenu(elvScreen.Canvas);
                    }
                );

                expelButton.OnHighlight.AddListener(
                    delegate ()
                    {
                        ElevatorTipsPatch.SetOverride(true, "Adv_Elv_ExpelTip", false);
                    }
                );

                expelButton.OffHighlight.AddListener(
                    delegate ()
                    {
                        ElevatorTipsPatch.SetOverride(false, "", false);
                    }
                );
            }
            else
            {
                StandardMenuButton expelButton = expelImage.GetComponent<StandardMenuButton>();

                if ((animate && stateChangedToNew) || !animateOnlyStateTransition) expelImage.StartCoroutine(AlphaAnimation(appearing: false, animSpeed, lockedColor, Color.white));
                else expelImage.color = lockedColor;

                expelButton.OnPress.RemoveAllListeners();
                expelButton.OnHighlight.RemoveAllListeners();
                expelButton.OffHighlight.RemoveAllListeners();
            }
        }

        private static IEnumerator AlphaAnimation(bool appearing, float multiplier, Color lockedColor, Color disappearingColorOnBegin, bool disableOnDisappearing = false)
        {
            if (appearing)
            {
                Color color = lockedColor;
                while (color.a < Color.white.a)
                {
                    color.a += Time.unscaledDeltaTime * multiplier;
                    expelImage.color = color;
                    yield return null;
                }
                expelImage.color = Color.white;
            } else
            {
                Color color = disappearingColorOnBegin;
                while (color.a > lockedColor.a)
                {
                    color.a -= Time.unscaledDeltaTime * multiplier;
                    expelImage.color = color;
                    yield return null;
                }
                if (disableOnDisappearing) expelImage.gameObject.SetActive(false);
                expelImage.color = lockedColor;
            }
            yield break;
        }

        private static List<NPC> SetPage(int index)
        {
            for (int j = 0; j < textButtons.Count; j++)
            {
                textButtons[j].gameObject.SetActive(false);
            }

            if (pages.Count == 0)
            {
                if (pagesText != null)
                    pagesText.text = 
                        string.Format("Adv_Expel_Hammer_Pages".Localize(), 0, 0);
                return new List<NPC>();
            }

            List<NPC> characters = pages[index];
            int i = 0;
            foreach (NPC npc in characters)
            {
                if (i >= textButtons.Count) break;
                textButtons[i].text.text = npc.GetMeta().nameLocalizationKey.Localize();
                textButtons[i].OnPress = new UnityEvent();
                textButtons[i].OnPress.AddListener(
                    delegate()
                    {
                        HitCharacter(npc.Character);
                    }
                );
                i++;
            }

            for (int j = 0; j < i; j++)
            {
                if (j >= textButtons.Count) break;
                textButtons[j].gameObject.SetActive(true);
            }

            if (pagesText != null)
                pagesText.text = 
                    string.Format("Adv_Expel_Hammer_Pages".Localize(), 
                        index + 1, pages.Count);

            return pages[index];
        }

        public static void HitCharacter(Character character)
        {
            LevelDataManager.LevelData.BuyExpelHammer(0, cancelPurchase: true);
            LevelDataManager.LevelData.BanCharacter(character, floorsToUnban);

            if (elvScreen == null) return;

            DestroyMenu();

            SetState(false, animate: false);
            
            elvScreen.GetComponent<AudioManager>().PlaySingle(AssetsStorage.sounds["adv_boing"]);
            
        }

        private static void AppendButtons()
        {
            Vector3 pos = Vector3.up * 40f;
            Vector3 addend = Vector3.up * -40f;

            for (int i = 0; i < maxCountOnPage; i++)
            {
                TMP_Text npcText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "Text", chalkboard.transform, pos, false);
                pos += addend;

                Vector2 size = new Vector2(300, 50);
                npcText.GetComponent<RectTransform>().sizeDelta = size;
                npcText.alignment = TextAlignmentOptions.Top;

                StandardMenuButton menuButton = ObjectsCreator.AddButtonProperties(npcText, size, true);
                textButtons.Add(menuButton);
            }
        }

        public static List<NPC> GetPotentialCharacters()
        {
            List<NPC> potentialCharacters = new List<NPC>();

            if (BaseGameManager.Instance == null) return potentialCharacters;
            if (BaseGameManager.Instance.Ec == null) return potentialCharacters;

            List<NPC> npcs = BaseGameManager.Instance.Ec.npcsToSpawn;

            for (int i = 0; i < npcs.Count; i++)
            {
                NPC npc = npcs[i];
                NPCMetadata meta = npc.GetMeta();
                LevelGenerationParameters ld = BaseGameManager.Instance.levelObject;

                if (meta.tags.Contains(TagsStorage.expelHammerImmunity) || meta.tags.Contains(TagsStorage.faculty) ||
                    meta.tags.Contains(TagsStorage.teacher)) continue;

                //forced npcs and potential baldis
                if (!meta.tags.Contains(TagsStorage.expelHammerWeakness) &&
                    (ld.forcedNpcs.Contains(npc) || Array.Find(ld.potentialBaldis,
                        x => x.selection.Character == npc.Character) != null)) continue;

                if (LevelDataManager.LevelData.bannedCharacters.Contains(npc.Character)) continue;

                potentialCharacters.Add(npc);
            }

            return potentialCharacters;
        }

    }
}
