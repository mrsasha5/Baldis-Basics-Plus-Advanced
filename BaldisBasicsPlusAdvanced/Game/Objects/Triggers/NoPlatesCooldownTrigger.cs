using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Triggers
{
    public class NoPlatesCooldownTrigger : TriggerBase
    {

        protected override void OnEnvBeginPlay()
        {
            base.OnEnvBeginPlay();
            foreach (CooldownPlateBase plate in FindObjectsOfType<CooldownPlateBase>())
            {
                plate.SetIgnoreCooldown(true);
            }
        }

    }
}
