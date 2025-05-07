using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.GameEventSystem
{
    public class EventsManager
    {
        private static List<IEventable> listeners = new List<IEventable>();

        public static void Add(IEventable eventAcceptor)
        {
            if (!listeners.Contains(eventAcceptor)) {
                listeners.Add(eventAcceptor);
            }
        }

        public static void Remove(IEventable eventAcceptor)
        {
            if (listeners.Contains(eventAcceptor))
            {
                listeners.Remove(eventAcceptor);
            }
        }

        internal static void OnMathMachineLearn(bool answerIsRight)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnMathMachineLearn(answerIsRight);
            }
        }

        internal static void OnNotebookClaim()
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].OnNotebookClaim();
            }
        }

    }
}
