using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using MTM101BaldAPI.Registers.Buttons;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_KitchenStove : BaseStructure_Plate
    {
        [SerializeField]
        private WeightedGameObject[] buttonsPre;

        [SerializeField]
        private string buttonMaterialsName;

        [SerializeField]
        private int buttonRange;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);
            buttonMaterialsName = "adv_generic_button_gray";
            buttonRange = 3;
            hallPrefabs = new WeightedGameObject[] {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["kitchen_stove"],
                    weight = 100
                }
            };
            buttonsPre = new WeightedGameObject[]
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

            GameButton button = GameButton.BuildInArea(ec, cell.position, cell.position, buttonRange, stove.gameObject,
                WeightedGameObject.ControlledRandomSelection(buttonsPre, rng).GetComponent<GameButton>(), rng);

            if (button == null)
            {
                Destroy(stove.gameObject);
                AdvancedCore.Logging.LogWarning("Couldn't find a valid position for the button. Destroying Kitchen Stove!");
            }
            else button.ChangeColor(buttonMaterialsName);

            return stove;
        }

    }
}
