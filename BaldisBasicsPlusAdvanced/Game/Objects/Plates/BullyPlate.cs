using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class BullyPlate : CooldownPlateBase
    {

        protected override void SetValues(ref PlateData plateData)
        {
            base.SetValues(ref plateData);
            plateData.targetPlayer = true;
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
