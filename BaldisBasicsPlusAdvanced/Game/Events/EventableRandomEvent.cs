using BaldisBasicsPlusAdvanced.GameEventSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.Events
{
    public class EventableRandomEvent : RandomEvent, IEventable
    {

        public override void Begin()
        {
            base.Begin();
            EventsManager.Add(this);
        }

        public override void End()
        {
            base.End();
            EventsManager.Remove(this);
        }

        public override void ResetConditions()
        {
            base.ResetConditions();
            EventsManager.Remove(this);
        }

        public virtual void OnMathMachineLearn(bool answerIsRight)
        {
            
        }

        public virtual void OnNotebookClaim()
        {
        }
    }
}
