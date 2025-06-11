using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Menu;
using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.Objects.Pickups;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.SaveSystem;
using MTM101BaldAPI.UI;
using Rewired;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting
{
    public class VotingPickup : BasePickup
    {
        private VotingBallot ballot;

        private ChalkboardMenu chalkboardMenu;

        private Quaternion rotationOnCloseMenu;

        private static List<BaseTopic> activeTopics = new List<BaseTopic>();

        private BaseTopic chosenTopic;

        private VotingEvent votingEvent;

        private List<VoteData> votes = new List<VoteData>();

        private List<GameObject> objectsToDestroyOnChalkBoardUpdate = new List<GameObject>();

        private bool votingWas;

        private bool votingIsGoing;

        private MovementModifier zeroMod = new MovementModifier(Vector3.zero, 0f);

        public BaseTopic Topic => chosenTopic;

        public bool VotingIsGoing => votingIsGoing;

        internal static bool TopicIsActive<T>() where T : BaseTopic
        {
            for (int i = 0; i < activeTopics.Count; i++)
            {
                if (activeTopics[i] is T && activeTopics[i].Active)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDestroy()
        {
            if (chosenTopic != null)
            {
                activeTopics.Remove(chosenTopic);
                chosenTopic.Reset();
            }
        }

        private void ChooseTopic(System.Random rng)
        {
            List<BaseTopic> potentialTopics = new List<BaseTopic>();

            foreach (List<BaseTopic> topics in ObjectsStorage.Topics.Values)
            {
                foreach (BaseTopic topic in topics)
                {
                    BaseTopic _topic = topic.Clone();
                    _topic.Initialize(votingEvent, ballot.Ec);
                    if (_topic.IsAvailable())
                    {
                        potentialTopics.Add(_topic);
                    }
                }
            }

            chosenTopic = potentialTopics[rng.Next(0, potentialTopics.Count)];
            chosenTopic.OnBringUp(rng);
            ballot.Ec.OnEnvironmentBeginPlay += chosenTopic.OnEnvironmentBeginPlay;
        }

        protected override void OnCreationPost()
        {
            base.OnCreationPost();
            renderer.enabled = false;
        }

        public void Initialize(System.Random rng, VotingEvent votingEvent, VotingBallot ballot)
        {
            this.votingEvent = votingEvent;
            this.ballot = ballot;
            ChooseTopic(rng);
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
                chosenTopic.OnVotingEndedPre(true);
                activeTopics.Add(chosenTopic);
            } else chosenTopic.OnVotingEndedPre(false);
        }

        protected override void VirtualClicked(int player)
        {
            base.VirtualClicked(player);

            Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<Entity>().ExternalActivity.moveMods.Add(zeroMod);

            //lock camera
            Singleton<CoreGameManager>.Instance.GetCamera(player).matchTargetRotation = false;
            rotationOnCloseMenu = Singleton<CoreGameManager>.Instance.GetPlayer(player).transform.rotation;

            Singleton<CoreGameManager>.Instance.GetHud(player).CloseTooltip();

            InitializeMenu(player);
        }

        public override bool ClickableHidden()
        {
            return base.ClickableHidden() || chalkboardMenu != null;
        }

        private void InitializeMenu(int player)
        {
            chalkboardMenu = Instantiate(ObjectsStorage.Objects["chalkboard_menu"].GetComponent<ChalkboardMenu>());

            Destroy(CursorController.Instance.gameObject); //reinit cursor

            BaldiFonts bigFont = BaldiFonts.ComicSans24;
            BaldiFonts smallFont = BaldiFonts.ComicSans18;

            chalkboardMenu.GetButton("exit").OnPress.AddListener(
                delegate ()
                {
                    DestroyMenu(player);
                }
            );

            chalkboardMenu.GetText("title").text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Text_School_Council_Topic_Base");
            chalkboardMenu.GetText("title").font = bigFont.FontAsset();
            chalkboardMenu.GetText("title").fontSize = bigFont.FontSize();

            chalkboardMenu.GetText("info").text = chosenTopic.Desc;
            chalkboardMenu.GetText("info").font = smallFont.FontAsset();
            chalkboardMenu.GetText("info").fontSize = smallFont.FontSize();

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

            BaldiFonts bigFont = BaldiFonts.ComicSans24;

            if (votingIsGoing && votes.Find(x => x.pm == pm) == null)
            {
                TMP_Text supportText = UIHelpers.CreateText<TextMeshProUGUI>(bigFont,
                Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Voting_Plus"),
                chalkboardMenu.chalkboard.transform, new Vector3(-80f, -80f, 0f), false);
                supportText.GetComponent<RectTransform>().sizeDelta = buttonSize;
                supportText.alignment = TextAlignmentOptions.Top;

                TMP_Text rejectText = UIHelpers.CreateText<TextMeshProUGUI>(bigFont,
                    Singleton<LocalizationManager>.Instance.GetLocalizedText("Adv_Voting_Minus"),
                    chalkboardMenu.chalkboard.transform, new Vector3(80f, -80f, 0f), false);
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
                TMP_Text text = UIHelpers.CreateText<TextMeshProUGUI>(bigFont,
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
                        string.Format("Adv_Voting_Approved".Localize(), Math.Round(CountPosVotes() / (float)CountTotalVotes() * 100f, 2));
                else result = string.Format("Adv_Voting_Rejected".Localize(), Math.Round(CountNegVotes() / (float)CountTotalVotes() * 100f), 2);

                TMP_Text text = UIHelpers.CreateText<TextMeshProUGUI>(bigFont, result,
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
            Singleton<CoreGameManager>.Instance.GetPlayer(player).GetComponent<Entity>().ExternalActivity.moveMods.Remove(zeroMod);

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

        public int CountTotalVotes()
        {
            return votes.Count;
        }

        public bool VotingIsWon()
        {
            return CountPosVotes() > CountNegVotes();
        }

        public bool ContainsVoteFrom(NPC npc)
        {
            for (int i = 0; i < votes.Count; i++)
            {
                if (votes[i].npc == npc) return true;
            }
            return false;
        }

        public int CountPosVotes()
        {
            int counter = 0;
            for (int i = 0; i < votes.Count; i++)
            {
                if (votes[i].value) counter++;
            }
            return counter;
        }

        public int CountNegVotes()
        {
            int counter = 0;
            for (int i = 0; i < votes.Count; i++)
            {
                if (!votes[i].value) counter++;
            }
            return counter;
        }

    }
}
