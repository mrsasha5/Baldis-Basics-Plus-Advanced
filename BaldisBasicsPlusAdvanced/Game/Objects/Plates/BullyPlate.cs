using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class BullyPlate : BaseCooldownPlate
    {

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);
            plateData.targetsPlayer = true;
            //plateData.hasLight = true;
            //plateData.lightColor = Color.yellow;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_bully_plate");
            SetEditorSprite("adv_editor_bully_plate");
        }

        protected override void VirtualOnPress()
        {
            base.VirtualOnPress();
            ItemManager itm = Singleton<CoreGameManager>.Instance.GetPlayer(0).itm;
            if (itm.CountItems() <= 0) return;
            itm.RemoveRandomItem();
            SetCooldown(150f);
            audMan.PlaySingle(AssetsStorage.bullyTakeouts[UnityEngine.Random.Range(0, AssetsStorage.bullyTakeouts.Length)]);
        }

    }
}
