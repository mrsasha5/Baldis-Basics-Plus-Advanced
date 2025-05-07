using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.GameEventSystem
{
    public interface IEventable
    {
        void onMathMachineLearn(bool answerIsRight);
        void onNotebookClaim();
    }
}
