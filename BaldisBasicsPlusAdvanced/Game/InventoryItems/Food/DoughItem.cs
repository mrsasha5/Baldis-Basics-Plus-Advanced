using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Food;
using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems.Food
{
    public class DoughItem : Item, IPrefab
    {
        [SerializeField]
        private GroundDough dough;

        public void InitializePrefab(int variant)
        {
            dough = ObjectStorage.Entities["Dough"].GetComponent<GroundDough>();
        }

        public override bool Use(PlayerManager pm)
        {
            GroundDough dough = Instantiate(this.dough);
            dough.Initialize(pm.ec, pm.transform.position,
                Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward);

            Destroy(gameObject);
            return true;
        }
    }
}
