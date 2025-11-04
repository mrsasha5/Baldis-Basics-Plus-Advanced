using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using PlusStudioLevelLoader;

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

            for (int i = 0; i < data.Count; i += 6)
            {
                BasePlate plate = BuildPrefab(data[i].prefab.GetComponent<BasePlate>(),
                    ec.cells[data[i].position.x, data[i].position.z], data[i].direction);

                plate.Data.showsUses = data[i + 4].data.ToBool();
                plate.Data.showsCooldown = data[i + 5].data.ToBool();

                plate.SetMaxUses(data[i + 1].data);

                if (data[i + 2].data.ConvertToFloatNoRecast() >= 0f)
                    plate.ForcefullyPatchCooldown(data[i + 2].data.ConvertToFloatNoRecast());

                plate.Data.timeToUnpress = data[i + 3].data.ConvertToFloatNoRecast();
            }

        }
    }
}
