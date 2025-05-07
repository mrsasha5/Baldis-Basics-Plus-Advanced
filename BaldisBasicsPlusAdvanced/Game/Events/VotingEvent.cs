using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.NPCs.Behavior.States.Navigation;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting;
using BaldisBasicsPlusAdvanced.Game.Rooms.Functions;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class VotingEvent : RandomEvent, IPrefab
    {
        private VotingBallot ballot;

        private List<NavigationState_VotingEvent> navigationStates = new List<NavigationState_VotingEvent>();

        public void InitializePrefab()
        {
            string path = AssetsHelper.modPath + "Premades/Rooms/";

            int n = 1;
            while (File.Exists(path + "VotingRoom" + n + ".cbld"))
            {
                WeightedRoomAsset weightedRoom = new WeightedRoomAsset()
                {
                    selection = RoomHelper.CreateAssetFromPath(
                        path + "VotingRoom" + n + ".cbld",
                        isOffLimits: true,
                        keepTextures: false),
                    weight = 100
                };

                weightedRoom.selection.category = EnumExtensions.GetFromExtendedName<RoomCategory>("SchoolCouncil");
                weightedRoom.selection.maxItemValue = 30;
                weightedRoom.selection.lightPre = AssetsHelper.LoadAsset<Transform>("HangingLight");
                RoomHelper.SetupRoomFunction<CorruptedLightsFunction>(weightedRoom.selection.roomFunctionContainer);

                potentialRoomAssets = potentialRoomAssets.AddToArray(weightedRoom);
                n++;
            }
        }

        public override void AssignRoom(RoomController room)
        {
            base.AssignRoom(room);
            ballot = room.GetComponentInChildren<VotingBallot>();
            if (ballot == null) throw new Exception("The Voting Ballot is not found in the built room!");
            ballot.Initialize(room.ec);
        }

        public override void Begin()
        {
            base.Begin();
            ballot.OnVotingStarts(remainingTime);

            foreach (NPC npc in ec.Npcs)
            {
                if (npc.Navigator.enabled)
                {
                    NavigationState_VotingEvent navigationState_VotingEvent = new NavigationState_VotingEvent(npc, 31, room);
                    navigationStates.Add(navigationState_VotingEvent);
                    npc.navigationStateMachine.ChangeState(navigationState_VotingEvent);
                }
            }
        }

        public override void End()
        {
            base.End();
            ballot.OnVotingEnds();

            ec.GetAudMan().PlaySingle(AssetsStorage.sounds["adv_bell"]);
            Singleton<CoreGameManager>.Instance.GetHud(0).BaldiTv.AnnounceEvent(AssetsStorage.sounds["adv_bal_event_voting_ended"]);

            foreach (NavigationState_VotingEvent navigationState in navigationStates)
            {
                navigationState.End();
            }
            navigationStates.Clear();
        }
    }
}
