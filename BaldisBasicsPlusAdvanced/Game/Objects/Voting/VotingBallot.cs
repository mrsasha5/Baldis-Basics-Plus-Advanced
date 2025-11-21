using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Menu;
using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.NPCs.Behavior.States.Navigation;
using BaldisBasicsPlusAdvanced.Game.Objects.Texts;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using BaldisBasicsPlusAdvanced.Game.WeightedSelections;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.GameManager;
using MTM101BaldAPI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting
{
    public class VotingBallot : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private MeshRenderer[] renderers;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private TimerText timer;

        [SerializeField]
        private InteractionObject interactionObject;

        [SerializeField]
        private Material sideMat;

        [SerializeField]
        private Material signSideMat;

        [SerializeField]
        private Material topMat;

        [SerializeField]
        private Material topFulledMat;

        private Quaternion rotationOnCloseMenu;

        private BaseTopic chosenTopic;

        private ChalkboardMenu chalkboardMenu;

        private EnvironmentController ec;

        private bool initialized;

        private List<GameObject> objectsToDestroyOnChalkBoardUpdate = new List<GameObject>();

        private bool votingWas;

        private bool votingIsGoing;

        private VotingEvent votingEvent;

        private List<VoteData> votes = new List<VoteData>();

        private MovementModifier zeroMod = new MovementModifier(Vector3.zero, 0f);

        private static List<BaseTopic> activeTopics = new List<BaseTopic>();

        public bool Initialized => initialized;

        public TimerText Timer => timer;

        public BaseTopic Topic => chosenTopic;

        internal static bool IsTopicActive<T>() where T : BaseTopic
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

        public void InitializePrefab(int variant)
        {
            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            renderers = new MeshRenderer[6];

            sideMat = new Material(AssetStorage.materials["belt"]);
            signSideMat = new Material(AssetStorage.materials["belt"]);
            topMat = new Material(AssetStorage.materials["belt"]);
            topFulledMat = new Material(AssetStorage.materials["belt"]);

            sideMat.mainTexture = AssetStorage.textures["adv_ballot_front"];
            signSideMat.mainTexture = AssetStorage.textures["adv_ballot_front_voting"];
            topMat.mainTexture = AssetStorage.textures["adv_ballot_empty_top"];
            topFulledMat.mainTexture = AssetStorage.textures["adv_ballot_top"];

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(15f, 10f, 15f);
            collider.center = new Vector3(0f, 5f, 0f);

            NavMeshObstacle obstacle = collider.gameObject.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(12f, 10f, 12f);
            obstacle.carving = true; //let's cut out nav mesh!

            InitializeRenderers();

            transform.localScale = Vector3.one * 0.4f;

            timer = ObjectCreator.CreateBobTimerText(null);
            timer.transform.parent.transform.position = Vector3.up * 5.6f;
            timer.transform.parent.transform.parent = transform;

            interactionObject = new GameObject("InteractionObject").AddComponent<InteractionObject>();
            interactionObject.InitializePrefab(1);
            interactionObject.InitializePrefabPost(this);
            interactionObject.transform.localPosition = Vector3.up * 5f;

            interactionObject.transform.parent = transform;
        }

        private void ChooseTopic(System.Random rng)
        {
            List<WeightedCouncilTopic> potentialTopics = new List<WeightedCouncilTopic>();

            foreach (List<WeightedCouncilTopic> topics in ObjectStorage.Topics.Values)
            {
                foreach (WeightedCouncilTopic topic in topics)
                {
                    BaseTopic _topic = topic.selection.Clone();
                    _topic.Initialize(votingEvent, ec);
                    if (_topic.IsAvailable())
                    {
                        potentialTopics.Add(new WeightedCouncilTopic()
                        {
                            selection = _topic,
                            weight = topic.weight
                        });
                    }
                }
            }

            chosenTopic = WeightedCouncilTopic.ControlledRandomSelection(potentialTopics.ToArray(), rng);
            chosenTopic.OnBringUp(rng);
            ec.OnEnvironmentBeginPlay += chosenTopic.OnEnvironmentBeginPlay;
        }

        private void OnTriggerStay(Collider collider)
        {
            if (collider.CompareTag("NPC"))
            {
                NPC npc = collider.GetComponent<NPC>();

                NavigationState navState = ReflectionHelper.GetValue<NavigationState>(npc.navigationStateMachine, "currentState");
                if (navState is NavigationState_VotingEvent) {
                    ((NavigationState_VotingEvent)navState).End();

                    if (votingIsGoing)
                        InsertVote(new VoteData()
                        {
                            isNPC = true,
                            npc = npc,
                            value = UnityEngine.Random.Range(0, 2) == 1 ? true : false
                        });
                }
                
            }
        }

        public void Initialize(System.Random rng, VotingEvent votingEvent, EnvironmentController ec)
        {
            initialized = true;
            this.votingEvent = votingEvent;
            this.ec = ec;
            timer.Initialize(ec);
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
            if (IsVotingWon())
            {
                chosenTopic.OnVotingEndedPre(true);
                activeTopics.Add(chosenTopic);
            }
            else chosenTopic.OnVotingEndedPre(false);
        }

        private void OnPause()
        {
            chalkboardMenu.chalkboard.gameObject.SetActive(false);
        }

        private void OnUnpause()
        {
            chalkboardMenu.chalkboard.gameObject.SetActive(true);
        }

        private void InitializeMenu(int player)
        {
            chalkboardMenu = Instantiate(ObjectStorage.Objects["chalkboard_menu"].GetComponent<ChalkboardMenu>());
            CoreManagerPausePatch.onPause += OnPause;
            CoreManagerPausePatch.onUnpause += OnUnpause;

            Destroy(CursorController.Instance.gameObject); //reinit cursor

            BaldiFonts bigFont = BaldiFonts.ComicSans24;
            BaldiFonts smallFont = BaldiFonts.ComicSans18;
            chalkboardMenu.GetButton("exit").OnPress.AddListener(() => DestroyMenu(player));

            chalkboardMenu.GetText("title").text = "Adv_SC_Topic_Base".Localize();
            chalkboardMenu.GetText("title").font = bigFont.FontAsset();
            chalkboardMenu.GetText("title").fontSize = bigFont.FontSize();

            if (chosenTopic != null)
            {
                chalkboardMenu.GetText("info").text = chosenTopic.Desc;
                chalkboardMenu.GetText("info").font = smallFont.FontAsset();
                chalkboardMenu.GetText("info").fontSize = smallFont.FontSize();
            }

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
                    "Adv_Voting_Plus".Localize(),
                    chalkboardMenu.chalkboard.transform, new Vector3(-80f, -80f, 0f), false);
                supportText.GetComponent<RectTransform>().sizeDelta = buttonSize;
                supportText.alignment = TextAlignmentOptions.Top;

                TMP_Text rejectText = UIHelpers.CreateText<TextMeshProUGUI>(bigFont,
                    "Adv_Voting_Minus".Localize(),
                    chalkboardMenu.chalkboard.transform, new Vector3(80f, -80f, 0f), false);
                rejectText.GetComponent<RectTransform>().sizeDelta = buttonSize;
                rejectText.alignment = TextAlignmentOptions.Top;

                StandardMenuButton supportButton = ObjectCreator.AddButtonProperties(supportText, buttonSize, true);

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

                StandardMenuButton rejectButton = ObjectCreator.AddButtonProperties(rejectText, buttonSize, true);

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
                    "Adv_Voting_Player_Voted".Localize(),
                    chalkboardMenu.chalkboard.transform, new Vector3(0f, -50f, 0f), false);
                text.GetComponent<RectTransform>().sizeDelta = textSize;
                text.alignment = TextAlignmentOptions.Top;

                objectsToDestroyOnChalkBoardUpdate.Add(text.gameObject);
            }
            else if (votingWas)
            {
                string result = "";

                if (CountPosVotes() == CountNegVotes())
                    result = "Adv_Voting_EqualVotes".Localize();
                else if (CountPosVotes() > CountNegVotes())
                    result =
                        string.Format("Adv_Voting_Approved".Localize(), 
                        Math.Round(CountPosVotes() / (float)CountTotalVotes() * 100f, 2));
                else
                    result = string.Format("Adv_Voting_Rejected".Localize(), 
                    Math.Round(CountNegVotes() / (float)CountTotalVotes() * 100f), 2);

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
            CoreManagerPausePatch.onPause -= OnPause;
            CoreManagerPausePatch.onUnpause -= OnUnpause;

            GlobalCam.Instance.Transition(UiTransition.Dither, 0.01666667f);
            Destroy(chalkboardMenu.canvas.gameObject);
            CoreGameManager.Instance.GetPlayer(player).plm.Entity.ExternalActivity.moveMods.Remove(zeroMod);

            //Unlocking camera
            CoreGameManager.Instance.GetCamera(player).matchTargetRotation = true;
            CoreGameManager.Instance.GetPlayer(player).transform.rotation = rotationOnCloseMenu;
        }

        private void OnDestroy()
        {
            if (chosenTopic != null)
            {
                activeTopics.Remove(chosenTopic);
                chosenTopic.Reset();
            }
        }

        public bool InsertVote(VoteData vote)
        {
            if ((vote.isNPC && votes.Find(x => x.npc == vote.npc) != null) || (vote.isPlayer && votes.Find(x => x.pm == vote.pm) != null))
            {
                return false;
            }

            votes.Add(vote);
            audMan.PlaySingle(AssetStorage.sounds["adv_throwing_vote"]);

            return true;
        }

        public bool ShouldVotingBeEnded()
        {
            if (votingEvent.Ec.Npcs.Count == 0 && !IsPlayerVoted()) return false;

            int npcsLeft = votingEvent.Ec.Npcs.Count - CountTotalVotes();

            if (npcsLeft <= 0) return true;

            return CountPosVotes() > npcsLeft || CountNegVotes() > npcsLeft;
        }

        public int CountTotalVotes()
        {
            return votes.Count;
        }

        public bool IsVotingWon()
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

        public bool IsPlayerVoted()
        {
            for (int i = 0; i < votes.Count; i++)
            {
                if (votes[i].value && votes[i].isPlayer) return true;
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

        private void InitializeRenderers()
        {
            //CreateRenderer(Vector3.zero, Vector3.right * 90f, sideMat, 0);
            CreateRenderer(new Vector3(-5f, 5f, 0f), new Vector3(0f, 90f, 0f), signSideMat, 1); //side mat
            CreateRenderer(new Vector3(5f, 5f, 0f), new Vector3(0f, 270f, 0f), signSideMat, 2); //side mat
            CreateRenderer(new Vector3(0f, 5f, 5f), new Vector3(0f, 180f, 0f), signSideMat, 3);
            CreateRenderer(new Vector3(0f, 5f, -5f), new Vector3(0f, 0f, 0f), signSideMat, 4); //side mat
            CreateRenderer(new Vector3(0f, 10f, 0f), new Vector3(90f, 0f, 0f), topMat, 5);
        }

        private void CreateRenderer(Vector3 pos, Vector3 angles, Material mat, int index)
        {
            GameObject childObj = new GameObject("Renderer" + index);
            childObj.transform.parent = transform;
            childObj.transform.localPosition = pos;
            childObj.transform.localScale = new Vector3(10f, 10f, 1f);

            Quaternion rotation = childObj.transform.rotation;
            rotation.eulerAngles = angles;
            childObj.transform.rotation = rotation;

            renderers[index] = childObj.AddComponent<MeshRenderer>();
            renderers[index].material = mat;

            MeshFilter meshFilter = childObj.AddComponent<MeshFilter>();
            meshFilter.mesh = AssetStorage.meshes["quad"];
        }

        private class InteractionObject : MonoBehaviour, IClickable<int>, IPrefab
        {

            [SerializeField]
            private VotingBallot ballot;

            public void InitializePrefab(int variant)
            {
                SphereCollider collider = gameObject.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.radius = 3f;
            }

            public void InitializePrefabPost(VotingBallot ballot) => this.ballot = ballot;

            public void Clicked(int player)
            {
                if (!ClickableHidden())
                {
                    CoreGameManager.Instance.GetPlayer(player)
                    .plm.Entity.ExternalActivity.moveMods.Add(ballot.zeroMod);

                    //Lock camera
                    CoreGameManager.Instance.GetCamera(player).matchTargetRotation = false;
                    ballot.rotationOnCloseMenu = CoreGameManager.Instance.GetPlayer(player).transform.rotation;

                    CoreGameManager.Instance.GetHud(player).CloseTooltip();

                    ballot.InitializeMenu(player);
                }
                
            }

            public bool ClickableHidden()
            {
                return ballot.chalkboardMenu != null;// || time > 0f;
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

    }
}
