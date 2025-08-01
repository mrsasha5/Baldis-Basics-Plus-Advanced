﻿using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.NPCs.Behavior.States.Navigation;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using BaldisBasicsPlusAdvanced.Game.Rooms;
using BaldisBasicsPlusAdvanced.Game.Rooms.Functions;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class VotingEvent : RandomEvent, IPrefab
    {
        public enum VotingState
        {
            NotStarted,
            Active,
            WaitingTvToShowResult,
            Finished
        }

        public class PrincipalController
        {
            public RoomController room;

            public Principal principal;

            public Entity entity;

            public NavigationState_PartyEvent state;

            public bool inRoom;

            public void Update()
            {
                if (inRoom)
                    principal.WhistleReached(); //set default speed always
            }

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

        [SerializeField]
        private Balloon[] balloons;

        [SerializeField]
        private int balloonsOnChunk;

        [SerializeField]
        private int chunkSize;

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

        [SerializeField]
        private float minScreensDistance;

        //private LockedDoorsFunction lockedDoorsFunc;

        private static List<LevelType> highCeilingLevelTypes = new List<LevelType>()
            { LevelType.Factory };

        private SchoolCouncilFunction councilRoomFunc;

        private VotingState state;

        private bool lightTurnedOn;

        private List<TextMeshPro> screenTexts = new List<TextMeshPro>();

        private List<PrincipalController> principals = new List<PrincipalController>();

        private VotingBallot ballot;

        private List<NavigationState_VotingEvent> navigationStates = new List<NavigationState_VotingEvent>();

        private static TextMeshProUGUI[] texts = new TextMeshProUGUI[2];

        private static List<Cell> usedCells = new List<Cell>();

        public bool RoomAssigned { get; private set; }

        public EnvironmentController Ec => ec;

        public static bool TopicIsActive<T>() where T : BaseTopic
        {
            return VotingPickup.TopicIsActive<T>();
        }

        private bool IsEnoughDistance(Cell cell1, Cell cell2)
        {
            if (Vector3.Distance(cell1.CenterWorldPosition, cell2.CenterWorldPosition) > minScreensDistance)
            {
                return true;
            }
            return false;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] != null) Destroy(texts[i].gameObject);
            }

            usedCells.Clear();
        }

        public void InitializePrefab(int variant)
        {
            minScreensDistance = 50f;
            balloonsOnChunk = 2;
            chunkSize = 8;
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

            List<WeightedRoomAsset> assets = new List<WeightedRoomAsset>();

            foreach (string path in Directory.GetFiles(AssetsHelper.modPath + "Premades/Rooms/Objects/Voting/",
                "*.cbld", SearchOption.AllDirectories))
            {

                WeightedRoomAsset weightedRoom = new WeightedRoomAsset()
                {
                    selection = RoomHelper.CreateAssetFromPath(
                        path,
                        isOffLimits: true,
                        autoAssignFunctionContainer: true,
                        keepTextures: true),
                    weight = 100
                };

                weightedRoom.selection.maxItemValue = 30;
                weightedRoom.selection.lightPre = AssetsHelper.LoadAsset<Transform>("HangingLight");

                assets.Add(weightedRoom);
            }

            potentialRoomAssets = assets.ToArray();
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
                if (!highCeilingLevelTypes.Contains(councilRoomFunc.LevelBuilder.ld.type))
                {
                    BuildScreens(rng);
                    UpdateTexts();
                }
                
            }
            else gameObject.SetActive(false); //then don't update this
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
                case VotingState.WaitingTvToShowResult:
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
                if (state == VotingState.Active)
                {
                    ballot.Timer.UpdateTime(remainingTime);

                    if (ballot.VotingPickup.ShouldVotingBeEnded())
                        EndVoting();
                    
                    UpdateTexts();
                }

                for (int i = 0; i < principals.Count; i++)
                {
                    principals[i].Update();
                    if (!principals[i].inRoom)
                    {
                        if (principals[i].entity != null)
                        {
                            if (principals[i].entity.CurrentRoom == room)
                            {
                                CheckLight();
                                principals[i].SetCheckingRoomMode(true);
                                principals[i].inRoom = true;
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

        private Principal GetPrincipal()
        {
            Principal fallback = null;

            foreach (NPC npc in ec.Npcs)
            {
                if (!(npc is Principal)) continue;
        
                if (npc.Character == Character.Principal)
                    return (Principal)npc;
        
                fallback = (Principal)npc;
            }

            return fallback;
        }

        public override void Begin()
        {
            base.Begin();
            //lockedDoorsFunc.SetLockedDoors(false);
            //lockedDoorsFunc.SetAutoUpdate(false);
            //lockedDoorsFunc.UpdateDoors();

            for (int i = 0; i < room.TileCount; i++)
            {
                ec.map.Find(room.TileAtIndex(i).position.x, room.TileAtIndex(i).position.z, room.TileAtIndex(i).ConstBin, room);
            }

            state = VotingState.Active;

            ballot.OnVotingStarts();

            ballot.Timer.UpdateTime(remainingTime, false);
            UpdateTexts();

            StartCoroutine(AttentionUpdater());

            Principal principal = GetPrincipal();

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
            while (state == VotingState.Active)
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
            EndVoting();
        }

        private void EndVoting() => OnVotingEnds(ballot.VotingPickup.VotingIsWon());

        private void OnVotingEnds(bool isWin)
        {
            if (state == VotingState.WaitingTvToShowResult || state == VotingState.Finished) return;

            state = VotingState.WaitingTvToShowResult;

            ballot.OnVotingEnds();

            ballot.Timer.Stop();
            UpdateTexts();

            ShowVotes();

            foreach (NavigationState_VotingEvent navigationState in navigationStates)
            {
                if (navigationState.npc != null) navigationState.End();
            }

            navigationStates.Clear();

            foreach (PrincipalController controller in principals)
            {
                controller.SetCheckingRoomMode(false);
            }

            principals.Clear();

            if (isWin)
                StartCoroutine(BalloonSpawner());
        }

        private IEnumerator BalloonSpawner()
        {
            int balloonsCount = room.cells.Count / chunkSize * balloonsOnChunk;
            float time = UnityEngine.Random.Range(1f, 2f);
            for (int i = 0; i < balloonsCount; i++)
            {
                Instantiate(balloons[UnityEngine.Random.Range(0, balloons.Length)]).Initialize(room);
                while (time > 0f)
                {
                    time -= Time.deltaTime * ec.EnvironmentTimeScale;
                    yield return null;
                }
                time = UnityEngine.Random.Range(1f, 2f);
            }
        }

        private void ShowVotes()
        {
            AudioManager audMan = ec.GetAudMan();
            audMan.PlaySingle(AssetsStorage.sounds["event_notification"]);
            audMan.PlaySingle(AssetsStorage.sounds["adv_bell"]);

            InvokeTvEnumerator("Exclamation", 3f);
            InvokeTvEnumerator("Static", 0.5f);

            AddEnumeratorToTv(TvController());

            InvokeTvEnumerator("Static", 0.25f);

            if (ballot.VotingPickup.VotingIsWon())
            {
                AddEnumeratorToTv(ShowBasicInfo());

                InvokeTvEnumerator("Static", 0.25f);
            }
        }

        private IEnumerator TvController()
        {
            Canvas canvas = Singleton<CoreGameManager>.Instance.GetHud(0).Canvas();

            BaldiTV tvBase = canvas.GetComponentInChildren<BaldiTV>();

            ReflectionHelper.UseRequiredMethod(tvBase, "ResetScreen");

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

            ballot.VotingPickup.Topic.OnVotingEndedPost(ballot.VotingPickup.VotingIsWon());

            float time = 7f;

            while (time > 0f)
            {
                time -= Time.deltaTime;
                yield return null;
            }

            state = VotingState.Finished;

            Destroy(texts[0].gameObject);
            Destroy(texts[1].gameObject);

            ReflectionHelper.SetValue<bool>(tvBase, "busy", false);
            ReflectionHelper.UseRequiredMethod(tvBase, "QueueCheck");
        }

        private IEnumerator ShowBasicInfo()
        {
            Canvas canvas = Singleton<CoreGameManager>.Instance.GetHud(0).Canvas();

            BaldiTV tvBase = canvas.GetComponentInChildren<BaldiTV>();

            ReflectionHelper.UseRequiredMethod(tvBase, "ResetScreen");

            //AudioManager audMan = ec.GetAudMan();
            //audMan.PlaySingle(AssetsStorage.sounds["adv_bal_super_wow"]);

            float time = 7f;

            texts[0] = UIHelpers.CreateText<TextMeshProUGUI>(
                BaldiFonts.ComicSans12,
                ballot.VotingPickup.Topic.BasicInfo,
                tvBase.transform,
                new Vector3(68f, -80f, 0f));
            texts[0].rectTransform.sizeDelta = new Vector2(100f, 50f);
            texts[0].alignment = TextAlignmentOptions.Center;

            while (time > 0f)
            {
                time -= Time.deltaTime;
                yield return null;
            }

            Destroy(texts[0].gameObject);

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

        private void BuildScreens(System.Random rng)
        {
            List<Cell> cells = ec.mainHall.GetTilesOfShape(tileShapesForScreen, screenCoverage, includeOpenToGenScreens);
            cells.FilterOutCellsThatDontFitCoverage(screenCoverage |
                CellCoverage.North | CellCoverage.East | CellCoverage.West | CellCoverage.South,
                CellCoverageType.All, coverageMustHaveWalls: false);

            int counter = rng.Next(minMaxScreens.x, minMaxScreens.z + 1);
            while (counter > 0)
            {
                if (cells.Count == 0) break;

                int index = rng.Next(0, cells.Count);

                bool ignored = false;

                Direction direction = Direction.North;

                if (cells[index].AllWallDirections.Count > 0)
                {
                    List<Direction> directions = Directions.All();
                    directions.FindAll(x => cells[index].AllWallDirections.Contains(x))
                        .ForEach(x => directions.Remove(x));
                    if (directions.Count > 0) direction = directions[0];
                }

                for (int i = 0; i < usedCells.Count; i++)
                {
                    if (!IsEnoughDistance(cells[index], usedCells[i]))
                    {
                        ignored = true;
                        break;
                    }
                }

                if (!ignored)
                {
                    BuildScreen(cells[index], direction);
                    counter--;
                }

                cells.RemoveAt(index);

            }
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
    }
}
