using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Food;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.InventoryItems.Food
{
    public class ChickenItem : BaseMultipleUsableItem, IPrefab
    {
        [SerializeField]
        private PlateFoodTrap chickenGround;

        public void InitializePrefab(int variant)
        {
            switch (variant)
            {
                case 1:
                    chickenGround = ObjectsStorage.Entities["RawChichenGroundTrap"].GetComponent<PlateFoodTrap>();
                    break;
                case 2:
                    chickenGround = ObjectsStorage.Entities["CookedChichenGroundTrap"].GetComponent<PlateFoodTrap>();
                    break;
            }

        }

        public override bool Use(PlayerManager pm)
        {
            this.pm = pm;
            PlateFoodTrap groundedChicken = Instantiate(chickenGround);
            groundedChicken.Initialize(pm.ec, pm.transform.position,
                Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward);
            groundedChicken.AssignVisualWith(pm.itm.items[pm.itm.selectedItem]);

            Destroy(gameObject);
            return ReturnOnUse();
        }

    }
}
