using BaldisBasicsPlusAdvanced.Patches.Shop;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.GameEventsProvider
{
    [HarmonyPatch(typeof(BaseGameManager))]
    internal class BaseGameManagerEvents
    {
        private static List<IGameManagerEventsReceiver> receivers = new List<IGameManagerEventsReceiver>();

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void onStart()
        {
            StoreHammerPatch.selfUpdate = false;
        }

        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void onInit(BaseGameManager __instance)
        {
            if (!StoreHammerPatch.updateState())
            {
                StoreHammerPatch.selfUpdate = true;
            }

            List<IGameManagerEventsReceiver> _receivers = new List<IGameManagerEventsReceiver>(receivers);

            for (int i = 0; i < _receivers.Count; i++)
            {
                if (_receivers[i] != null)
                {
                    _receivers[i].onManagerInitPost();
                } else
                {
                    receivers.RemoveAt(i);
                }
            }
        }

        public static void register(IGameManagerEventsReceiver receiver)
        {
            receivers.Add(receiver);
        }

        public static void unregister(IGameManagerEventsReceiver receiver)
        {
            receivers.Remove(receiver);
        }

    }
}
