using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Rooms.Functions
{
    public class EnglishClassTimerFunction : RoomFunction
    {

        private SymbolMachine[] symbolMachines;

        public override void OnGenerationFinished()
        {
            base.OnGenerationFinished();
            symbolMachines = transform.parent.GetComponentsInChildren<SymbolMachine>();
            if (symbolMachines.Length == 0) AdvancedCore.Logging.LogWarning("English Class: the Symbol Machine(s) not founded. Machine timer(s) is not controlled!");
            for (int i = 0; i < symbolMachines.Length; i++)
            {
                symbolMachines[i].SetTimerMode(true);
                symbolMachines[i].OnGenerationFinishedInTimedRoom();
            }
        }

        public override void OnPlayerExit(PlayerManager player)
        {
            base.OnPlayerExit(player);
            if (symbolMachines.Length == 0) return;
            for (int i = 0; i < symbolMachines.Length; i++)
            {
                if (!symbolMachines[i].Completed && symbolMachines[i].AnswerField.Length == 0) symbolMachines[i].SetSymbolTimer(false, symbolMachines[i].SymbolTime);
                else if (!symbolMachines[i].Completed && !symbolMachines[i].PlayerRewarded) symbolMachines[i].SetSymbolTimer(true, -1f);
            }
            
        }

        public override void OnPlayerStay(PlayerManager player)
        {
            base.OnPlayerStay(player);
            if (symbolMachines.Length == 0) return;
            for (int i = 0; i < symbolMachines.Length; i++)
            {
                if (!symbolMachines[i].TimerActive && !symbolMachines[i].Completed) symbolMachines[i].SetSymbolTimer(true, symbolMachines[i].SymbolTime);
            }
        }

    }
}
