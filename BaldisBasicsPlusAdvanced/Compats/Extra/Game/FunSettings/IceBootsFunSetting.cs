using BaldisBasicsPlusAdvanced.Cache;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.PlusExtensions;
using MTM101BaldAPI.Registers;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings
{
    public class IceBootsFunSetting : BaseFunSetting
    {
        private static ValueModifier zeroValueModifier = new ValueModifier(0f);

        public override void Initialize()
        {
            base.Initialize();
            PlayerMovementStatModifier moveModifier = Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier();
            moveModifier.AddModifier("walkSpeed", zeroValueModifier);
            moveModifier.AddModifier("runSpeed", zeroValueModifier);

            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.SetItem(ObjectsStorage.ItemsObjects["IceBoots"], 0);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(0, val: true);
        }

        protected override void VirtualOnDestroy()
        {
            base.VirtualOnDestroy();
            PlayerManager pm = Singleton<CoreGameManager>.Instance.GetPlayer(0);
            if (pm == null) return; //to avoid exception when you leave from the level through Pause Menu
            PlayerMovementStatModifier moveModifier = pm.GetMovementStatModifier();
            moveModifier.RemoveModifier(zeroValueModifier);
            moveModifier.RemoveModifier(zeroValueModifier);

            pm.itm.SetItem(ItemMetaStorage.Instance.FindByEnum(Items.None).value, 0);
            pm.itm.LockSlot(0, val: false);
        }

    }
}
