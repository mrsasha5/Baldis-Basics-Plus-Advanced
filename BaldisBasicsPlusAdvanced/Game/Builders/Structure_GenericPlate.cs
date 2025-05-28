using BaldisBasicsPlusAdvanced.Cache;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_GenericPlate : BaseStructure_Plate
    {

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);
            includeRooms = true;

            hallPrefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["safety_trapdoor"],
                    weight = 25,
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["invisibility_plate"],
                    weight = 75
                },
		        new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["present_plate"],
                    weight = 50
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["bully_plate"],
                    weight = 15
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["slowdown_plate"],
                    weight = 30
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["sugar_addiction_plate"],
                    weight = 40
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["protection_plate"],
                    weight = 50
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["teleportation_plate"],
                    weight = 50
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["fake_plate"],
                    weight = 40
                }
            };

            roomPrefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["invisibility_plate"],
                    weight = 75
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["sugar_addiction_plate"],
                    weight = 40
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["protection_plate"],
                    weight = 50
                },
                new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["teleportation_plate"],
                    weight = 50
                },
		        new WeightedGameObject()
                {
                    selection = ObjectsStorage.Objects["present_plate"],
                    weight = 50
                }
            };
        }

    }
}
