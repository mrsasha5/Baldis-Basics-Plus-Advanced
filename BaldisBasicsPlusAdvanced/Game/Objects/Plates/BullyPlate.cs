using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class BullyPlate : BasePlate
    {

        protected override void SetValues(PlateData data)
        {
            base.SetValues(data);
            data.MarkAsCooldownPlate();
            //plateData.hasLight = true;
            //plateData.lightColor = Color.yellow;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_bully_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            ItemManager itm = CoreGameManager.Instance.GetPlayer(0).itm;
            if (itm.CountItems() <= 0) return;
            itm.RemoveRandomItem();
            SetCooldown(150f);
            audMan.PlayRandomAudio(AssetStorage.bullyTakeouts);
        }

        protected override bool IsPressable(Entity target)
        {
            return base.IsPressable(target) && target.TryGetComponent(out PlayerManager pm) && !pm.Tagged; 
                                                    //Why did I miss Faculty Tag before???
        }

    }
}
