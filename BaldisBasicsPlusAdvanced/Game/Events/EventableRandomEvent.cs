using BaldisBasicsPlusAdvanced.GameEventSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class EventableRandomEvent : BaseRandomEvent, IEventable
    {

        public override void Begin()
        {
            base.Begin();
            EventsManager.add(this);
        }

        public override void End()
        {
            base.End();
            EventsManager.remove(this);
        }

        public override void ResetConditions()
        {
            base.ResetConditions();
            EventsManager.remove(this);
        }

        public virtual void onMathMachineLearn(bool answerIsRight)
        {
            
        }

        public virtual void onNotebookClaim()
        {
        }
    }
}
