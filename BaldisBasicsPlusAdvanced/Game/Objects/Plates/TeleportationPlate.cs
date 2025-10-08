using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class TeleportationPlate : BasePlate
    {
        protected override void SetValues(PlateData data)
        {
            base.SetValues(data);
            data.MarkAsCooldownPlate();
            //plateData.hasLight = true;
            //plateData.lightColor = new Color(0.6f, 0.2f, 1f); //violet
        }

        protected override void SetTextures()
        {
            base.SetTextures();
            SetTexturesByBaseName("adv_teleportation_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            entities[0]?.DangerousTeleportation();
            SetCooldown(60f);
        }

    }
}
