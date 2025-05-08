using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Triggers
{
    public class NoPlatesCooldownTrigger : BaseTrigger
    {

        protected override void OnEnvBeginPlay()
        {
            base.OnEnvBeginPlay();
            foreach (BaseCooldownPlate plate in FindObjectsOfType<BaseCooldownPlate>())
            {
                plate.SetIgnoreCooldown(true);
            }
        }

    }
}
