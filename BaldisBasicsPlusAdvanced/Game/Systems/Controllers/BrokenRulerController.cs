using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class BrokenRulerController : ControllerBase
    {
        public void Initialize(float time, bool destroyImmediately)
        {
            this.time = time;
            BreakRuler(destroyImmediately);
        }

        private void BreakRuler(bool destroyImmediately)
        {
            Baldi baldi = (Baldi)npc;
            baldi.BreakRuler();
            if (destroyImmediately)
            {
                baldi.Slap();
                baldi.SlapBreak();

                Baldi_Chase_Broken stateBroken = (Baldi_Chase_Broken)baldi.behaviorStateMachine.currentState;
                ReflectionHelper.SetValue<bool>(stateBroken, "broken", true);
            }
            
        }

        public override void OnPreDestroying()
        {
            Baldi baldi = (Baldi)npc;
            base.OnPreDestroying();
            baldi.RestoreRuler();
        }

    }
}
