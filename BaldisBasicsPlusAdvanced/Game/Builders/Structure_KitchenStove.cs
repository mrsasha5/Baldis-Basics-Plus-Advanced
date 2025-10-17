using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
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
            for (int i = 0; i < data.Count; i += 4)
            {
                KitchenStove stove = (KitchenStove)BuildPrefab(data[i].prefab.GetComponent<KitchenStove>(), 
                    ec.CellFromPosition(data[i].position), data[i].direction);

                GameButtonBase button = GameButton.Build(buttonPre, ec, data[i + 3].position, data[i + 3].direction);

                stove.CookingTime = BitConverter.ToSingle(BitConverter.GetBytes(data[i + 1].data), 0);
                stove.CoolingTime = BitConverter.ToSingle(BitConverter.GetBytes(data[i + 2].data), 0);
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
