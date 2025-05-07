using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using BaldisBasicsPlusAdvanced.Game.Objects.Pickups;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.SavedData;
using MTM101BaldAPI.UI;
using Rewired;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting
{
    public class VotingPickup : PickupBase
    {
        private VotingBallot ballot;

        private ChalkboardMenu chalkboardMenu;

        private Quaternion rotationOnCloseMenu;

        private TopicBase chosenTopic;

        private List<TopicBase> topics = new List<TopicBase>();

        private List<VoteData> votes = new List<VoteData>();

        private List<GameObject> objectsToDestroyOnChalkBoardUpdate = new List<GameObject>();

        private bool votingWas;

        private bool votingIsGoing;

        public bool VotingIsGoing => votingIsGoing;

        protected override void OnCreationPost()
        {
            base.OnCreationPost();
            renderer.enabled = false;
        }

        public void Initialize(VotingBallot ballot)
        {
            this.ballot = ballot;

            AddTopic<StudentExpelingTopic>();
            AddTopic<NoPlatesCooldownTopic>();
            AddTopic<PrincipalIgnoresRules>();

            List<TopicBase> potentialTopics = new List<TopicBase>(topics);

            for (int i = 0; i < potentialTopics.Count; i++)
            {
                for (int j = 0; j < LevelDataManager.LevelData.topics.Count; j++)
                {
                    if (LevelDataManager.LevelData.topics[j].GetType() == potentialTopics[i].GetType())
                    {
                        potentialTopics.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            chosenTopic = topics[ControlledRNG.Object.Next(0, potentialTopics.Count)];
            chosenTopic.Initialize();

            LevelDataManager.LevelData.topics.Add(chosenTopic);
        }

        public void OnVotingStarts()
        {
            votingIsGoing = true;
            votingWas = true;
            UpdateChalkBoard(0);
        }

        public void OnVotingEnds()
        {
            votingIsGoing = false;
            UpdateChalkBoard(0);
            if (VotingIsWon())
            {
                if (chosenTopic.SaveToNextFloors) LevelDataManager.LevelData.addTopicsOnTheNextFloor.Add(chosenTopic);
                chosenTopic.OnVotingEndedSuccessfully();
            }
        }

        protected override void VirtualClicked(int player)
        {
            base.VirtualClicked(player);

            Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<Entity>().SetInteractionState(false);

            //lock camera
            Singleton<CoreGameManager>.Instance.GetCamera(player).matchTargetRotation = false;
            rotationOnCloseMenu = Singleton<CoreGameManager>.Instance.GetPlayer(player).transform.rotation;

            Singleton<CoreGameManager>.Instance.GetHud(player).CloseTooltip();

            InitializeMenu(player);
        }

        private void InitializeMenu(int player)
        {
            chalkboardMenu = Instantiate(ObjectsStorage.Objects["chalkboard_menu"].GetComponent<ChalkboardMenu>());

            Destroy(CursorController.Instance.gameObject); //reinit cursor

            chalkboardMenu.GetText("title").text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Text_School_Council_Topic_Base");
            chalkboardMenu.GetText("info").text = chosenTopic.Desc;

            chalkboardMenu.GetButton("exit").OnPress.AddListener(
                delegate ()
                {
                    DestroyMenu(player);
                }
            );

            UpdateChalkBoard(player);            

            Singleton<GlobalCam>.Instance.FadeIn(UiTransition.Dither, 0.01666667f);
        }

        private bool UpdateChalkBoard(int player)
        {
            if (chalkboardMenu == null) return false;

            foreach (GameObject obj in objectsToDestroyOnChalkBoardUpdate)
            {
                Destroy(obj);
            }

            objectsToDestroyOnChalkBoardUpdate.Clear();

            PlayerManager pm = Singleton<CoreGameManager>.Instance.GetPlayer(player);

            Vector2 buttonSize = new Vector2(100, 50);

            Vector2 textSize = new Vector2(300, 50);

            if (votingIsGoing && votes.Find(x => x.pm == pm) == null)
            {
                TMP_Text supportText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24,
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Voting_Plus"),
                chalkboardMenu.chalkboard.transform, new Vector3(-80f, -70f, 0f), false);
                supportText.GetComponent<RectTransform>().sizeDelta = buttonSize;
                supportText.alignment = TextAlignmentOptions.Top;

                TMP_Text rejectText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24,
                    Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Voting_Minus"),
                    chalkboardMenu.chalkboard.transform, new Vector3(80f, -70f, 0f), false);
                rejectText.GetComponent<RectTransform>().sizeDelta = buttonSize;
                rejectText.alignment = TextAlignmentOptions.Top;

                StandardMenuButton supportButton = ObjectsCreator.AddButtonProperties(supportText, buttonSize, true);
                supportButton.InitializeAllEvents();

                supportButton.OnPress.AddListener(delegate ()
                {
                    InsertVote(new VoteData()
                    {
                        value = true,
                        isPlayer = true,
                        pm = pm
                    });
                    DestroyMenu(player);
                });

                StandardMenuButton rejectButton = ObjectsCreator.AddButtonProperties(rejectText, buttonSize, true);
                rejectButton.InitializeAllEvents();

                rejectButton.OnPress.AddListener(delegate ()
                {
                    InsertVote(new VoteData()
                    {
                        value = false,
                        isPlayer = true,
                        pm = pm
                    });
                    DestroyMenu(player);
                });

                objectsToDestroyOnChalkBoardUpdate.Add(supportText.gameObject);
                objectsToDestroyOnChalkBoardUpdate.Add(rejectText.gameObject);
            }
            else if (votingWas && votingIsGoing)
            {
                TMP_Text text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24,
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Voting_Player_Voted"),
                chalkboardMenu.chalkboard.transform, new Vector3(0f, -50f, 0f), false);
                text.GetComponent<RectTransform>().sizeDelta = textSize;
                text.alignment = TextAlignmentOptions.Top;

                objectsToDestroyOnChalkBoardUpdate.Add(text.gameObject);
            }
            else if (votingWas)
            {
                string result = "";

                if (CountPosVotes() == CountNegVotes()) result = "Adv_Voting_EqualVotes".Localize();
                else if (CountPosVotes() > CountNegVotes()) result =
                        string.Format("Adv_Voting_Approved".Localize(), CountPosVotes() / (float)CountTotalVotes() * 100f);
                else result = string.Format("Adv_Voting_Rejected".Localize(), CountNegVotes() / (float)CountTotalVotes() * 100f);

                TMP_Text text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, result,
                chalkboardMenu.chalkboard.transform, new Vector3(0f, -50f, 0f), false);
                text.GetComponent<RectTransform>().sizeDelta = textSize;
                text.alignment = TextAlignmentOptions.Top;

                objectsToDestroyOnChalkBoardUpdate.Add(text.gameObject);
            }

            return true;
        }

        private void DestroyMenu(int player)
        {
            Singleton<GlobalCam>.Instance.Transition(UiTransition.Dither, 0.01666667f);
            Destroy(chalkboardMenu.canvas.gameObject);
            Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<Entity>().SetInteractionState(true);

            //unlock camera
            Singleton<CoreGameManager>.Instance.GetCamera(player).matchTargetRotation = true;
            Singleton<CoreGameManager>.Instance.GetPlayer(player).transform.rotation = rotationOnCloseMenu;

            //openedMenu = false;
            nonClickableTime = 1f;
        }

        public bool InsertVote(VoteData vote)
        {
            if ((vote.isNPC && votes.Find(x => x.npc == vote.npc) != null) || (vote.isPlayer && votes.Find(x => x.pm == vote.pm) != null))
            {
                return false;
            }

            votes.Add(vote);
            ballot.AudMan.PlaySingle(AssetsStorage.sounds["adv_throwing_vote"]);

            return true;
        }

        private int CountTotalVotes()
        {
            return votes.Count;
        }

        private bool VotingIsWon()
        {
            return CountPosVotes() > CountNegVotes();
        }

        private int CountPosVotes()
        {
            int counter = 0;
            for (int i = 0; i < votes.Count; i++)
            {
                if (votes[i].value) counter++;
            }
            return counter;
        }

        private int CountNegVotes()
        {
            int counter = 0;
            for (int i = 0; i < votes.Count; i++)
            {
                if (!votes[i].value) counter++;
            }
            return counter;
        }

        private void OnDestroy()
        {
            if (chosenTopic != null && !chosenTopic.SaveToNextFloors) chosenTopic.OnDestroying();
        }

        private void AddTopic<T>() where T : TopicBase, new()
        {
            topics.Add(new T());
        }

    }
}
