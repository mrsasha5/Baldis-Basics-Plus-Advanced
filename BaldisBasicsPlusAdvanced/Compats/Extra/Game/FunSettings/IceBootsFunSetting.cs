using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.GameItems;
using BaldisBasicsPlusAdvanced.Helpers;
using BBE.CustomClasses;
using MTM101BaldAPI;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.PlusExtensions;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Compats.Extra.Game.FunSettings
{
    public class IceBootsFunSetting : FunSetting
    {
        private static ValueModifier zeroValueModifier = new ValueModifier(0f);

        //public override bool UnlockConditional => true;

        public override void OnBaseGameManagerInitialize(BaseGameManager baseGameManager)
        {
            base.OnBaseGameManagerInitialize(baseGameManager);
            PlayerMovementStatModifier moveModifier = Singleton<CoreGameManager>.Instance.GetPlayer(0).GetMovementStatModifier();
            moveModifier.AddModifier("walkSpeed", zeroValueModifier);
            moveModifier.AddModifier("runSpeed", zeroValueModifier);

            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.SetItem(ObjectsStorage.ItemObjects["IceBoots"], 0);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(0, val: true);
        }

        /*private void Update()
        {
            ObjectsStorage.ItemObjects["IceBoots"].item
        }*/

        public override void OnNPCSpawn(NPC npc)
        {
            base.OnNPCSpawn(npc);
            if (npc is Bully)
            {
                ReflectionHelper.GetValue<List<Items>>(npc, "itemsToReject").Add(
                    EnumExtensions.GetFromExtendedName<Items>("IceBoots"));
            }
        }

    }
}
