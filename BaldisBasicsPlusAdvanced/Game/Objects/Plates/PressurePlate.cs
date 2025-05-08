using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class PressurePlate : BasePlate
    {
        private List<IButtonReceiver> buttonReceivers = new List<IButtonReceiver>();

        /*protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.hasLight = true;
            plateData.lightColor = Color.green;
        }*/

        public virtual void ConnectTo(List<IButtonReceiver> receivers)
        {
            buttonReceivers = receivers;
        }

        protected virtual void ActivateReceivers()
        {
            for (int i = 0; i < buttonReceivers.Count; i++)
            {
                buttonReceivers[i]?.ButtonPressed(true);
            }
        }

        /*protected override void VirtualStart()
        {
            //don't set light until builder do it
            //base.VirtualStart();
        }*/

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_pressure_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            ActivateReceivers();
        }

    }
}
