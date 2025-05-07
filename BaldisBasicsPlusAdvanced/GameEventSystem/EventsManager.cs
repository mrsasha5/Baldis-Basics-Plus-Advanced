using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.GameEventSystem
{
    public class EventsManager
    {
        private static List<IEventable> listeners = new List<IEventable>();

        public static void add(IEventable eventAcceptor)
        {
            if (!listeners.Contains(eventAcceptor)) {
                listeners.Add(eventAcceptor);
            }
        }

        public static void remove(IEventable eventAcceptor)
        {
            if (listeners.Contains(eventAcceptor))
            {
                listeners.Remove(eventAcceptor);
            }
        }

        public static void onMathMachineLearn(bool answerIsRight)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].onMathMachineLearn(answerIsRight);
            }
        }

        public static void onNotebookClaim()
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].onNotebookClaim();
            }
        }

    }
}
