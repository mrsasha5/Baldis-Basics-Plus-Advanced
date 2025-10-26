using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_KitchenStove : BaseStructure_Plate
    {
        [SerializeField]
        private GameButtonBase buttonPre;

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
            buttonPre = AssetsStorage.gameButton;
        }

        public override void Load(List<StructureData> data)
        {
            base.Load(data);
            for (int i = 0; i < data.Count; i += 8)
            {
                KitchenStove stove = (KitchenStove)BuildPrefab(data[i].prefab.GetComponent<KitchenStove>(),
                    ec.cells[data[i].position.x, data[i].position.z], data[i].direction);

                GameButtonBase button = GameButton.Build(buttonPre, ec, data[i + 7].position, data[i + 7].direction);
                button.SetUp(stove);

                stove.Data.showsUses = data[i + 5].data.ToBool();
                stove.Data.showsCooldown = data[i + 6].data.ToBool();

                stove.SetMaxUses(data[i + 1].data);
                
                if (data[i + 2].data.ConvertToFloatNoRecast() >= 0f)
                {
                    stove.ForcefullyPatchCooldown(data[i + 2].data.ConvertToFloatNoRecast());
                }

                stove.CookingTime = data[i + 3].data.ConvertToFloatNoRecast();
                stove.CoolingTime = data[i + 4].data.ConvertToFloatNoRecast();
            }
        }

        public override BasePlate RandomlyBuildPrefab(Cell cell, System.Random rng, bool inRoom)
        {
            KitchenStove stove = (KitchenStove)base.RandomlyBuildPrefab(cell, rng, inRoom);

            GameButtonBase button = GameButton.BuildInArea(ec, cell.position, buttonRange, stove.gameObject, buttonPre, rng);

            if (button == null)
            {
                Destroy(stove.gameObject);
                AdvancedCore.Logging.LogWarning("Couldn't find a valid position for the button. Destroying Kitchen Stove!");
            }

            return stove;
        }

    }
}
