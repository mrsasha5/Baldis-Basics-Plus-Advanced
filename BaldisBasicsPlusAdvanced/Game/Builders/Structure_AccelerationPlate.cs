using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_AccelerationPlate : BaseStructure_Plate
    {
        [SerializeField]
        private int buttonRange;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);
            buttonRange = 6;

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
                GameButton.BuildInArea(ec, ec.CellFromPosition(plate.transform.position).position,
                ec.CellFromPosition(plate.transform.position).position, buttonRange, plate.gameObject, AssetsStorage.gameButton, rng);
            }
            return plate;
        }

    }
}
