using BaldisBasicsPlusAdvanced.Game.Events;
using HarmonyLib;

namespace BaldisBasicsPlusAdvanced.Patches.GameEventsProvider
{
    [HarmonyPatch(typeof(Notebook))]
    internal class NotebookEvent
    {
        [HarmonyPatch("Clicked")]
        [HarmonyPostfix]
        private static void OnCLicked()
        {
            ColdSchoolEvent.InvokeOnNotebookClaim();
        }
    }
}
