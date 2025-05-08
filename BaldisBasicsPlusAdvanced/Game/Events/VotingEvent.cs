using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.NPCs.Behavior.States.Navigation;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using BaldisBasicsPlusAdvanced.Game.Rooms;
using BaldisBasicsPlusAdvanced.Game.Rooms.Functions;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using HarmonyLib;
using MidiPlayerTK;
using MTM101BaldAPI;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class VotingEvent : RandomEvent, IPrefab
    {
        [SerializeField]
        private Balloon[] balloons;

        [SerializeField]
        private int balloonsOnChunk;

        [SerializeField]
        private int chunkSize;

        [SerializeField]
        private float minTimeUntilBellCanRing;

        [SerializeField]
        private BaldiFonts screenFont;

        [SerializeField]
        private IntVector2 minMaxScreens;

        [SerializeField]
        private TileShapeMask tileShapesForScreen;

        [SerializeField]
        private CellCoverage screenCoverage;

        [SerializeField]
        private bool includeOpenToGenScreens;

        //private PartyLightColors partyColors;

        //private LockedDoorsFunction lockedDoorsFunc;

        private SchoolCouncilFunction councilRoomFunc;

        private VotingState state;

        private bool lightTurnedOn;

        private float timeWhenBellRings;

        private List<TextMeshPro> screenTexts = new List<TextMeshPro>();

        private List<PrincipalController> principals = new List<PrincipalController>();

        private VotingBallot ballot;

        private List<NavigationState_VotingEvent> navigationStates = new List<NavigationState_VotingEvent>();

        private static TextMeshProUGUI[] texts = new TextMeshProUGUI[2];

        public bool RoomAssigned { get; private set; }

        public static bool TopicIsActive<T>() where T : BaseTopic
        {
            return VotingPickup.TopicIsActive<T>();
        }

        private void OnDestroy()
        {
            if (texts[0] != null) Destroy(texts[0].gameObject);
            if (texts[1] != null) Destroy(texts[1].gameObject);
        }

        private void BuildScreen(Cell cell, Direction direction)
        {
            float height = 9.5f;

            GameObject gm = new GameObject("VotingScreen");
            gm.transform.SetParent(cell.room.objectObject.transform, false);
            gm.transform.position = cell.FloorWorldPosition;

            GameObject cubeGameobject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeGameobject.name = "Model";
            cubeGameobject.transform.SetParent(gm.transform, false);
            cubeGameobject.transform.localPosition = Vector3.up * height;
            cubeGameobject.transform.localScale = Vector3.one + Vector3.right * 7f;

            MeshRenderer renderer = cubeGameobject.GetComponent<MeshRenderer>();
            renderer.material = new Material(AssetsStorage.materials["belt"]);
            renderer.material.SetMainTexture(AssetsStorage.textures["white"]);
            renderer.material.SetColor(Color.black);

            RectTransform rect1 = new GameObject().AddComponent<RectTransform>();
            rect1.name = "ForwardText";
            rect1.SetParent(gm.transform, false);
            TextMeshPro text1 = rect1.gameObject.AddComponent<TextMeshPro>();
            rect1.localPosition = Vector3.forward * 0.52f + Vector3.up * height;
            rect1.localScale = Vector3.one * 0.5f;
            rect1.rotation = Quaternion.Euler(0f, 180f, 0f);
            text1.text = "???";
            text1.alignment = TextAlignmentOptions.Center;
            text1.font = screenFont.FontAsset();
            text1.fontSize = screenFont.FontSize();
            text1.color = Color.green;

            RectTransform rect2 = new GameObject().AddComponent<RectTransform>();
            rect2.name = "BackText";
            rect2.SetParent(gm.transform, false);
            TextMeshPro text2 = rect2.gameObject.AddComponent<TextMeshPro>();
            rect2.localPosition = Vector3.back * 0.52f + Vector3.up * height;
            rect2.localScale = Vector3.one * 0.5f;
            text2.text = "???";
            text2.alignment = TextAlignmentOptions.Center;
            text2.font = screenFont.FontAsset();
            text2.fontSize = screenFont.FontSize();
            text2.color = Color.green;

            gm.transform.rotation = Quaternion.Euler(0f, direction.ToDegrees(), 0f);

            List<Direction> dirs = Directions.All().FindAll(x => x != direction && x != direction.GetOpposite());
            CellCoverage cover = CellCoverage.None;
            dirs.ForEach(x => cover |= x.ToCoverage());

            if (cover != CellCoverage.None)
            {
                cell.HardCover(screenCoverage | cover);
            }
            else cell.HardCover(screenCoverage);

            screenTexts.Add(text1);
            screenTexts.Add(text2);

            Destroy(cubeGameobject.GetComponent<Collider>());
        }

        public void InitializePrefab(int variant)
        {
            balloonsOnChunk = 2;
            chunkSize = 8;
            minTimeUntilBellCanRing = 15f;
            tileShapesForScreen = TileShapeMask.Straight;
            screenCoverage = CellCoverage.Up;
            includeOpenToGenScreens = true;
            minMaxScreens = new IntVector2(2, 5);
            screenFont = BaldiFonts.ComicSans12;

            balloons = new Balloon[]
            {
                AssetsHelper.LoadAsset<Balloon>("Balloon_Orange"),
                AssetsHelper.LoadAsset<Balloon>("Balloon_Purple"),
                AssetsHelper.LoadAsset<Balloon>("Balloon_Green"),
                AssetsHelper.LoadAsset<Balloon>("Balloon_Blue"),
            };

            string path = AssetsHelper.modPath + "Premades/Rooms/";

            int n = 1;
            while (File.Exists(path + "VotingRoom" + n + ".cbld"))
            {
                WeightedRoomAsset weightedRoom = new WeightedRoomAsset()
                {
                    selection = RoomHelper.CreateAssetFromPath(
                        path + "VotingRoom" + n + ".cbld",
                        isOffLimits: true,
                        autoAssignFunctionContainer: true,
                        keepTextures: true),
                    weight = 100
                };

                weightedRoom.selection.maxItemValue = 30;
                weightedRoom.selection.lightPre = AssetsHelper.LoadAsset<Transform>("HangingLight");

                potentialRoomAssets = potentialRoomAssets.AddToArray(weightedRoom);
                n++;
            }
        }

        public override void AssignRoom(RoomController room)
        {
            base.AssignRoom(room);
            RoomAssigned = true;
            councilRoomFunc = room.functions.GetComponent<SchoolCouncilFunction>();
            councilRoomFunc.Assign(this);
            ballot = room.GetComponentInChildren<VotingBallot>();
            if (ballot == null)
            {
                AdvancedCore.Logging.LogFatal("The Voting Ballot is not found in the built room!");
                throw new Exception("The Voting Ballot is not found in the built room!");
            }
            //lockedDoorsFunc = room.functions.GetComponent<LockedDoorsFunction>();
        }

        public override void AfterUpdateSetup(System.Random rng)
        {
            base.AfterUpdateSetup(rng);
            if (RoomAssigned)
            {
                BuildScreens(rng);
                UpdateTexts();
            }
            else gameObject.SetActive(false); //then don't update this
        }

#warning try to avoid nearest tiles
        private void BuildScreens(System.Random rng)
        {
            List<Cell> cells = ec.mainHall.GetTilesOfShape(tileShapesForScreen, screenCoverage, includeOpenToGenScreens);
            cells.FilterOutCellsThatDontFitCoverage(screenCoverage |
                CellCoverage.North | CellCoverage.East | CellCoverage.West | CellCoverage.South, 
                CellCoverageType.All, coverageMustHaveWalls: false);

            int counter = rng.Next(minMaxScreens.x, minMaxScreens.z + 1);
            while (counter > 0)
            {
                int index = rng.Next(0, cells.Count);

                Direction direction = Direction.North;

                if (cells[index].AllWallDirections.Count > 0)
                {
                    List<Direction> directions = Directions.All();
                    directions.FindAll(x => cells[index].AllWallDirections.Contains(x))
                        .ForEach(x => directions.Remove(x));
                    if (directions.Count > 0) direction = directions[0];
                }

                BuildScreen(cells[index], direction);
                cells.RemoveAt(index);
                counter--;
            }
        }

        private void UpdateTexts()
        {
            switch (state)
            {
                case VotingState.NotStarted:
                    for (int i = 0; i < screenTexts.Count; i++)
                    {
                        screenTexts[i].text = "Adv_Voting_Not_Started".Localize();
                    }
                    break;
                case VotingState.Active:
                    for (int i = 0; i < screenTexts.Count; i++)
                    {
                        screenTexts[i].text = string.Format("Adv_Voting_Active".Localize(),
                            (int)remainingTime / 60, (int)remainingTime - (int)remainingTime / 60 * 60);
                    }
                    break;
                case VotingState.WaitsResult:
                    for (int i = 0; i < screenTexts.Count; i++)
                    {
                        screenTexts[i].text = "Adv_Voting_Waits".Localize();
                    }
                    break;
                case VotingState.Finished:
                    for (int i = 0; i < screenTexts.Count; i++)
                    {
                        screenTexts[i].text = string.Format("Adv_Voting_Ended".Localize(),
                            $"{ballot.VotingPickup.CountPosVotes()} : <color=#ff0000>{ballot.VotingPickup.CountNegVotes()}</color>");
                    }
                    break;
            }
        }

        private void LateUpdate()
        {
            if (!RoomAssigned) return;
            ballot?.VotingPickup.Topic?.LateUpdate();
        }

        private void Update()
        {
            if (!RoomAssigned) return;
            if (!ballot.Initialized && councilRoomFunc.LevelBuilder != null
                && councilRoomFunc.LevelBuilder.levelCreated) ballot.Initialize(crng, this, room.ec);
            ballot?.VotingPickup.Topic?.Update();
            if (active)
            {
                ballot.Timer.UpdateTime(remainingTime);
                UpdateTexts();

                if (remainingTime <= timeWhenBellRings)
                {
                    AttractAllNonVoters();
                    ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_bell"]);
                    UpdateRingTime();
                }

                for (int i = 0; i < principals.Count; i++)
                {
                    if (!principals[i].updated)
                    {
                        if (principals[i].entity != null)
                        {
                            if (principals[i].entity.CurrentRoom == room)
                            {
                                CheckLight();
                                principals[i].SetCheckingRoomMode(true);
                                principals[i].updated = true;
                            }
                        } else
                        {
                            CheckLight();
                            principals.RemoveAt(i);
                            i--;
                        }
                        
                    }
                }
            }
        }

        public override void Begin()
        {
            base.Begin();
            //lockedDoorsFunc.SetLockedDoors(false);
            //lockedDoorsFunc.SetAutoUpdate(false);
            //lockedDoorsFunc.UpdateDoors();

            state = VotingState.Active;

            ballot.OnVotingStarts();

            ballot.Timer.SetTime(remainingTime);
            UpdateTexts();

            UpdateRingTime();

            StartCoroutine(AttentionUpdater());

            Principal principal = FindObjectOfType<Principal>();

            if (principal != null)
            {
                principal.WhistleReact(ballot.transform.position);

                NavigationState_TargetPosition navState = 
                    ReflectionHelper.GetValue<NavigationState_TargetPosition>(principal.navigationStateMachine, "currentState");
                if (navState != null) navState.priority = int.MaxValue;

                principals.Add(new PrincipalController()
                {
                    principal = principal,
                    entity = principal.GetComponent<Entity>(),
                    room = room
                });
            }
            else CheckLight();

            AttractAllNonVoters();
        }

        private IEnumerator AttentionUpdater()
        {
            float time = 15f;
            while (active)
            {
                if (time > 0f)
                {
                    time -= Time.deltaTime * ec.EnvironmentTimeScale;
                    yield return null;
                } else
                {
                    AttractAllNonVoters();
                    time = 15f;
                }
            }
        }

        private void AttractAllNonVoters()
        {
            foreach (NPC npc in ec.Npcs)
            {
                if (npc.Navigator.enabled && !ballot.VotingPickup.ContainsVoteFrom(npc))
                {
                    NavigationState_VotingEvent navigationState_VotingEvent = new NavigationState_VotingEvent(npc, 31, room);
                    npc.navigationStateMachine.ChangeState(navigationState_VotingEvent);
                    if (navigationState_VotingEvent.Entered)
                        navigationStates.Add(navigationState_VotingEvent);
                }
            }
        }

        public override void End()
        {
            base.End();
            state = VotingState.WaitsResult;

            ballot.OnVotingEnds();

            ballot.Timer.Stop();
            UpdateTexts();

            ShowVotes();

            foreach (NavigationState_VotingEvent navigationState in navigationStates)
            {
                if (navigationState.npc != null) navigationState.End();
            }

            navigationStates.Clear();
        }

        private void OnResultShowed(bool isWin)
        {
            state = VotingState.Finished;
            UpdateTexts();
            if (isWin)
            {
                int balloonsCount = room.cells.Count / chunkSize * balloonsOnChunk;
                for (int i = 0; i < balloonsCount; i++)
                {
                    Instantiate(balloons[UnityEngine.Random.Range(0, balloons.Length)]).Initialize(room);
                }
            }
        }

        private void ShowVotes()
        {
            ec.GetAudMan().PlaySingle(AssetsStorage.sounds["event_notification"]);

            InvokeTvEnumerator("Exclamation", 3f);
            InvokeTvEnumerator("Static", 1f);

            AddEnumeratorToTv(TvController());

            InvokeTvEnumerator("Static", 0.25f);

            //Singleton<CoreGameManager>.Instance.GetHud(0).BaldiTv.AnnounceEvent(AssetsStorage.sounds["adv_bal_event_voting_ended"]);
        }

        private IEnumerator TvController()
        {
            Canvas canvas = Singleton<CoreGameManager>.Instance.GetHud(0).Canvas();

            BaldiTV tvBase = canvas.GetComponentInChildren<BaldiTV>();

            ReflectionHelper.UseRequiredMethod(tvBase, "ResetScreen");

            AudioManager audMan = ec.GetAudMan();

            //Adv_Voting_Result

            if (texts[0] == null)
            {
                texts[0] = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans12, "Adv_Voting_Result".Localize(), tvBase.transform,
                new Vector3(70f, -65f, 0f));
                texts[0].alignment = TextAlignmentOptions.Center;
            }

            if (texts[1] == null)
            {
                texts[1] = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.BoldComicSans24,
                "<color=#00ff00>" + ballot.VotingPickup.CountPosVotes() + "</color> : <color=#ff0000>" + ballot.VotingPickup.CountNegVotes()
                + "</color>", tvBase.transform,
                new Vector3(70f, -90f, 0f));
                texts[1].alignment = TextAlignmentOptions.Center;
            } else
            {
                texts[1].text = "<color=#ff0000>" + ballot.VotingPickup.CountPosVotes() + "</color> : <color=#00ff00>" + ballot.VotingPickup.CountNegVotes()
                + "</color>";
            }

            //float wowSecond = 3.148f;
            //bool played = false;

            //if (ballot.VotingPickup.VotingIsWon())
            //{
                //0 - voices
                //1 - effects
                //2 - music
                //if (Singleton<PlayerFileManager>.Instance.volume[2] > 0f)
                //{
                //    audMan.PlaySingle(AssetsStorage.sounds["adv_mus_win"]);
                /*} else
                {
                    wowSecond = 0f;
                }
            } else
            {
                wowSecond = float.PositiveInfinity;*/
            //}

            OnResultShowed(ballot.VotingPickup.VotingIsWon());
            ballot.VotingPickup.Topic.OnVotingEndedPost(ballot.VotingPickup.VotingIsWon());

            float baseTime = 10f;
            float time = baseTime;

            while (time > 0f)
            {
                /*if (!played && baseTime - time > wowSecond)
                {
                    audMan.PlaySingle(AssetsStorage.sounds["bal_wow"]);
                    played = true;
                }*/

                time -= Time.deltaTime;
                yield return null;
            }

            foreach (PrincipalController controller in principals)
            {
                controller.SetCheckingRoomMode(false);
            }

            principals.Clear();

            Destroy(texts[0].gameObject);
            Destroy(texts[1].gameObject);

            ReflectionHelper.SetValue<bool>(tvBase, "busy", false);
            ReflectionHelper.UseRequiredMethod(tvBase, "QueueCheck");
        }

        private void InvokeTvEnumerator(string name, params object[] args)
        {
            ReflectionHelper.UseRequiredMethod(Singleton<CoreGameManager>.Instance.GetHud(0).BaldiTv, "QueueEnumerator",
                ReflectionHelper.UseRequiredMethod(Singleton<CoreGameManager>.Instance.GetHud(0).BaldiTv, name, args));
        }

        private void AddEnumeratorToTv(IEnumerator enumerator)
        {
            ReflectionHelper.UseRequiredMethod(Singleton<CoreGameManager>.Instance.GetHud(0).BaldiTv, "QueueEnumerator",
                enumerator);
        }

        private void UpdateRingTime()
        {
            int minutesLeft = (int)remainingTime / 60;

            timeWhenBellRings = minutesLeft * 60f;

            /*if (minutesLeft <= 0)
            {
                timeWhenBellRings = float.NegativeInfinity;
            }
            else */
            if (remainingTime - timeWhenBellRings < minTimeUntilBellCanRing)
            {
                timeWhenBellRings = (minutesLeft - 1) * 60f;
            }
        }

        private void CheckLight()
        {
            if (!lightTurnedOn)
            {
                room.SetPower(true);
                lightTurnedOn = true;
            }
        }

        public bool ContainsPrincipal(NPC npc)
        {
            for (int i = 0; i < principals.Count; i++)
            {
                if (principals[i].principal == npc) return true;
            }
            return false;
        }

        public enum VotingState
        {
            NotStarted,
            Active,
            WaitsResult,
            Finished
        }

        public class PrincipalController
        {
            public RoomController room;

            public Principal principal;

            public Entity entity;

            public NavigationState_PartyEvent state;

            public bool updated;

            public void SetCheckingRoomMode(bool value)
            {
                if (principal == null) return;
                if (value)
                {
                    principal.behaviorStateMachine.ChangeState(new Principal_Wandering(principal));
                    entity.SetBlinded(true);
                    state = new NavigationState_PartyEvent(principal, int.MaxValue, room);
                    principal.navigationStateMachine.ChangeState(state);
                }
                else
                {
                    if (state != null)
                    {
                        entity.SetBlinded(false);
                        state.End();
                        state = null;
                    }
                }
            }

        }

    }
}
