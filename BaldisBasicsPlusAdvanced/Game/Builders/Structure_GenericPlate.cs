using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;

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

        public override void Load(List<StructureData> data)
        {
            base.Load(data);

            for (int i = 0; i < data.Count; i++)
            {
                BasePlate plate = BuildPrefab(data[i].prefab.GetComponent<BasePlate>(), 
                    ec.CellFromPosition(data[i].position), data[i].direction);

                plate.Data.showsUses = data[i + 5].data.ToBool();
                plate.Data.showsCooldown = data[i + 6].data.ToBool();

                plate.SetMaxUses(data[i + 1].data);

                if (data[i + 2].data.ToBool())
                    plate.ForcefullyPatchCooldown(BitConverter.ToSingle(BitConverter.GetBytes(data[i + 3].data), 0));

                plate.Data.timeToUnpress = BitConverter.ToSingle(BitConverter.GetBytes(data[i + 4].data), 0);

                i += 6;
            }

        }
    }
}
