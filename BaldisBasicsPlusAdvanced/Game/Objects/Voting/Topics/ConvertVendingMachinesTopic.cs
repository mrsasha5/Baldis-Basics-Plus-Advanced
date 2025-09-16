using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics
{
    public class ConvertVendingMachinesTopic : BaseTopic
    {

        public override string Desc => "Adv_SC_Topic_VendingMachines".Localize();

        public override string BasicInfo => "Adv_SC_Topic_VendingMachines_BasicInfo".Localize();

        public override bool IsAvailable()
        {
            return base.IsAvailable() && GameObject.FindObjectOfType<SodaMachine>() != null;
        }

        public override void OnVotingEndedPre(bool isWin)
        {
            base.OnVotingEndedPre(isWin);
            List<WeightedItemObject> _items = new List<WeightedItemObject>();
            ItemMetaData[] metas = ItemMetaStorage.Instance.All();
            for (int i = 0; i < metas.Length; i++)
            {
                _items.Add(new WeightedItemObject() {
                    selection = metas[i].value,
                    weight = 100
                });
            }
            WeightedItemObject[] items = _items.ToArray();
            if (isWin)
            {
                foreach (SodaMachine vendingMachine in GameObject.FindObjectsOfType<SodaMachine>())
                {
                    ReflectionHelper.SetValue<WeightedItemObject[]>(vendingMachine, "potentialItems", items);
                    ReflectionHelper.SetValue<int>(vendingMachine, "usesLeft", 10);
                }
            }
        }

        public override BaseTopic Clone()
        {
            ConvertVendingMachinesTopic topic = new ConvertVendingMachinesTopic();
            CopyAllBaseValuesTo(topic);
            return topic;
        }
    }
}
