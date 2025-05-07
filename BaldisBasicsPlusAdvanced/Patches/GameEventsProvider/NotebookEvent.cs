using BaldisBasicsPlusAdvanced.GameEventSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Patches.GameEventsProvider
{
    [HarmonyPatch(typeof(Notebook))]
    internal class NotebookEvent
    {
        [HarmonyPatch("Clicked")]
        [HarmonyPostfix]
        private static void onCLicked()
        {
            EventsManager.onNotebookClaim();
        }
    }
}
