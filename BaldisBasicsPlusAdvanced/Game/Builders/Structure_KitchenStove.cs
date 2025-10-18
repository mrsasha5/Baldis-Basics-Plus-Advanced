using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
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
            for (int i = 0; i < data.Count; i += 9)
            {
                KitchenStove stove = (KitchenStove)BuildPrefab(data[i].prefab.GetComponent<KitchenStove>(), 
                    ec.CellFromPosition(data[i].position), data[i].direction);

                GameButtonBase button = GameButton.Build(buttonPre, ec, data[i + 8].position, data[i + 8].direction);
                button.SetUp(stove);

                stove.Data.showsUses = data[i + 6].data.ToBool();
                stove.Data.showsCooldown = data[i + 7].data.ToBool();

                stove.SetMaxUses(data[i + 1].data);
                
                if (data[i + 2].data.ToBool())
                {
                    stove.ForcefullyPatchCooldown(BitConverter.ToSingle(BitConverter.GetBytes(data[i + 3].data), 0));
                }

                stove.CookingTime = BitConverter.ToSingle(BitConverter.GetBytes(data[i + 4].data), 0);
                stove.CoolingTime = BitConverter.ToSingle(BitConverter.GetBytes(data[i + 5].data), 0);
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
