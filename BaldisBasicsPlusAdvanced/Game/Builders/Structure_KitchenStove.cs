using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_KitchenStove : BaseStructure_Plate
    {
        [SerializeField]
        private WeightedGameObject[] buttons;

        [SerializeField]
        private int buttonRange;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);
            buttonRange = 3;
            hallPrefabs = new WeightedGameObject[] {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["kitchen_stove"],
                    weight = 100
                }
            };
            buttons = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = AssetsStorage.gameButton.gameObject,
                    weight = 100
                }
            };
        }

        public override BasePlate BuildPrefab(Cell cell, System.Random rng, bool inRoom)
        {
            KitchenStove stove = (KitchenStove)base.BuildPrefab(cell, rng, inRoom);

            if (GameButton.BuildInArea(ec, cell.position, cell.position, buttonRange, stove.gameObject, 
                WeightedGameObject.ControlledRandomSelection(buttons, rng).GetComponent<GameButton>(), rng) == null)
            {
                Destroy(stove.gameObject);
                AdvancedCore.Logging.LogWarning("Couldn't find a valid position for the button. Destroying Kitchen Stove!");
            }

            return stove;
        }

    }
}
