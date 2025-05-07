using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class PressurePlate : BasePlate
    {
        protected override void setTextures()
        {
            setTexturesByBaseName("adv_pressure_plate");
        }

        protected override void virtualOnPress()
        {
            base.virtualOnPress();
            activateReceivers();
        }

        protected override void virtualOnUnpress()
        {
            base.virtualOnUnpress();
        }

    }
}
