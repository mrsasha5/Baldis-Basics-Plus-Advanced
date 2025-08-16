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
            }
        }

        public override void OnPlayerExit(PlayerManager player)
        {
            base.OnPlayerExit(player);
            if (symbolMachines.Length == 0) return;
            for (int i = 0; i < symbolMachines.Length; i++)
            {
                if (!symbolMachines[i].IsCompleted && symbolMachines[i].AnswerField.Length == 0) symbolMachines[i].UpdateSymbolTimer(false, symbolMachines[i].SymbolTime);
                else if (!symbolMachines[i].IsCompleted && !symbolMachines[i].PlayerRewarded) symbolMachines[i].UpdateSymbolTimer(true, float.NegativeInfinity);
            }
        }

        //Stay is required instead of Enter when player reloads Symbol Machine with timer and he doesn't leave the room
        public override void OnPlayerStay(PlayerManager player)
        {
            base.OnPlayerStay(player);
            if (symbolMachines.Length == 0) return;
            for (int i = 0; i < symbolMachines.Length; i++)
            {
                if (!symbolMachines[i].TimerActive && !symbolMachines[i].IsCompleted) symbolMachines[i].UpdateSymbolTimer(true, symbolMachines[i].SymbolTime);
            }
        }

    }
}
