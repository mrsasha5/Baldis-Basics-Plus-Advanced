using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.GameEventSystem
{
    public interface IEventable
    {
        void OnMathMachineLearn(bool answerIsRight);
        void OnNotebookClaim();
    }
}
