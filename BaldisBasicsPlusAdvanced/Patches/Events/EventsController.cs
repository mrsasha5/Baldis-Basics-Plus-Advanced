using BaldisBasicsPlusAdvanced.Game.Events;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Patches.Events
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal class EventsController
    {

        [HarmonyPatch("RandomizeEvents")]
        [HarmonyPrefix]
        private static void OnRandomizeEvents(/*int numberOfEvents, float initialGap, float minGap, float maxGap,*/
            List<RandomEvent> ___events, List<float> ___eventTimes)
        {
            List<RandomEvent> mainEvents = ___events.FindAll(x => x is VotingEvent);

            bool votingEventGotPriority = false;

            for (int i = 0; i < mainEvents.Count; i++)
            {
                if (votingEventGotPriority && mainEvents[i] is VotingEvent) continue;
                int index = ___events.IndexOf(mainEvents[i]);
                if (index > 0)
                {
                    ___events.RemoveAt(index);
                    float time = ___eventTimes[index];
                    ___eventTimes.RemoveAt(index);
                    ___events.Insert(0, mainEvents[i]);
                    ___eventTimes.Insert(0, time);
                }
                if (mainEvents[i] is VotingEvent)
                {
                    votingEventGotPriority = true;
                }
            }
        }

        [HarmonyPatch("StartEventTimers")]
        [HarmonyPrefix]
        private static void OnStartEventTimers(ref List<RandomEvent> ___events, ref List<float> ___eventTimes)
        {
            for (int i = 0; i < ___events.Count; i++)
            {
                if (___events[i] is VotingEvent && !((VotingEvent)___events[i]).RoomAssigned)
                {
                    AdvancedCore.Logging.LogWarning("Generator room failure detected! Removing " + ___events[i].name +
                        " from the EnvironmentController events and event times lists before event timers will be started!");
                    ___events.RemoveAt(i);
                    ___eventTimes.RemoveAt(i);
                    i--;
                }
            }

            VotingEvent mainEvent = (VotingEvent)___events.Find(x => x is VotingEvent);
            if (mainEvent != null)
            {
                ___eventTimes[___events.IndexOf(mainEvent)] = 0f;
            }

        }

    }
}
