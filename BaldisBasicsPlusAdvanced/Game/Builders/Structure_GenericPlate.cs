using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
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

            List<int> extraData = new List<int>();

            for (int i = 0; i < data.Count; i++)
            {
                BasePlate plate = BuildPrefab(data[i].prefab.GetComponent<BasePlate>(), 
                    ec.CellFromPosition(data[i].position), data[i].direction);

                for (int i2 = i + 1; i2 < data.Count; i2++)
                {
                    if (data[i2].prefab == null)
                    {
                        extraData.Add(data[i2].data);
                        i = i2;
                    }
                    else
                    {
                        i = i2 - 1;
                        break;
                    }
                }

                if (extraData.Count > 0)
                    plate.SetMaxUses(extraData[0]);

                if (extraData.Count > 1)
                    plate.ForcefullyPatchCooldown(extraData[1]);

                if (extraData.Count > 2)
                    plate.Data.timeToUnpress = BitConverter.ToSingle(BitConverter.GetBytes(extraData[2]), 0);

                extraData.Clear();
            }

        }
    }
}
