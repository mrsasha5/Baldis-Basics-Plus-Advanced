using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class PowerPlateButtonComponent : GameButtonBase, IClickable<int>
    {
        private bool on;

        public override void Set(bool val)
        {
            base.Set(val);
            this.on = val;
            foreach (IButtonReceiver buttonReceiver in buttonReceivers)
            {
                buttonReceiver.ButtonPressed(on);
            }
        }

        protected override void Pressed(int playerNumber)
        {
            base.Pressed(playerNumber);
            Set(!on);
        }

        bool IClickable<int>.ClickableHidden()
        {
            return true;
        }
    }

    public class PowerPlate : BasePlate
    {
        [SerializeField]
        private PowerPlateButtonComponent button;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(variant);
            button = gameObject.AddComponent<PowerPlateButtonComponent>();
        }

        /*protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.hasLight = true;
            plateData.lightColor = Color.green;
        }*/

        public virtual void ConnectTo(List<IButtonReceiver> receivers)
        {
            button.SetUp(receivers.ToArray());
        }

        protected virtual void ActivateReceivers()
        {
            button.Clicked(0);
        }

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
