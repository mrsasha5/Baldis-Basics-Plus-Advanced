using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class PressurePlate : PlateBase
    {
        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_pressure_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            ActivateReceivers();
        }

        protected override void VirtualOnUnpress()
        {
            base.VirtualOnUnpress();
        }

    }
}
