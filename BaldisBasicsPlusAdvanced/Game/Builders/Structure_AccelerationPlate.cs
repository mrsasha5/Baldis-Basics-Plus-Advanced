using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using MTM101BaldAPI.Registers.Buttons;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_AccelerationPlate : BaseStructure_Plate
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
            buttonRange = 6;
            buttonsPre = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = AssetsStorage.gameButton.gameObject,
                    weight = 100
                }
            };
            buttonMaterialsName = "adv_generic_button_orange";

            hallPrefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["acceleration_plate"],
                    weight = 100
                }
            };
        }

        public override BasePlate BuildPrefab(Cell cell, System.Random rng, bool inRoom)
        {
            AccelerationPlate plate = (AccelerationPlate)base.BuildPrefab(cell, rng, inRoom);
            plate.InitializePotentialDirections();
            plate.ChooseBestRotation();
            if (rng.Next(0, 101) <= 50 && plate.IsRotatable)
            {
                GameButton.BuildInArea(ec, cell.position, cell.position, buttonRange, plate.gameObject,
                    WeightedGameObject.ControlledRandomSelection(buttonsPre, rng).GetComponent<GameButton>(), rng)
                    .ChangeColor(buttonMaterialsName);
            }
            return plate;
        }

    }
}
