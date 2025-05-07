using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Patches;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class BullyPlate : BaseCooldownPlate
    {

        protected override void setValues(ref PlateData plateData)
        {
            base.setValues(ref plateData);
            plateData.targetPlayer = true;
        }

        protected override void setTextures()
        {
            setTexturesByBaseName("adv_bully_plate");
            setEditorSprite("adv_editor_bully_plate");
        }

        protected override void virtualOnPress()
        {
            base.virtualOnPress();
            ItemManager itm = Singleton<CoreGameManager>.Instance.GetPlayer(0).itm;
            if (itm.countItems() <= 0) return;
            itm.RemoveRandomItem();
            setCooldown(150f);
            audMan.PlaySingle(AssetsStorage.bullyTakeouts[UnityEngine.Random.Range(0, AssetsStorage.bullyTakeouts.Length)]);
        }

    }
}
