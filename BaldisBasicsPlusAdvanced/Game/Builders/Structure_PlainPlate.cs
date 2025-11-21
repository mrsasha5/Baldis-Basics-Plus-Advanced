using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using System;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_PlainPlate : BaseStructure_Plate
    {

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);

            hallPrefabs = new WeightedGameObject[]
            {
                new WeightedGameObject()
                {
                    selection = ObjectStorage.Objects["plate"],
                    weight = 100
                }
            };
        }

        public override void PostOpenCalcGenerate(LevelGenerator lg, Random rng)
        {
            
        }

        public override void OnGenerationFinished(LevelBuilder lb)
        {
            if (lb.controlledRNG.NextDouble() <= parameters.chance[0])
            {
                if (Array.Find(FindObjectsOfType<UnityEngine.Object>(), x => x is IButtonReceiver) != null)
                    Build(lb, lb.controlledRNG, roomCells: false);

                for (int i = 0; i < generatedPlates.Count; i++)
                {
                    ((PressurePlate)generatedPlates[i]).ConnectTo(CollectPotentialReceiversData(lb.controlledRNG));
                }
                generatedPlates.Clear();
            }
        }

        private List<IButtonReceiver> CollectPotentialReceiversData(System.Random rng)
        {
            List<IButtonReceiver> potentialButtonReceivers = new List<IButtonReceiver>();

            UnityEngine.Object[] _potentialButtonReceivers = Array.FindAll(FindObjectsOfType<UnityEngine.Object>(), x => x is IButtonReceiver);
            for (int i = 0; i < _potentialButtonReceivers.Length; i++)
            {
                potentialButtonReceivers.Add((IButtonReceiver)_potentialButtonReceivers[i]);
            }

            List<IButtonReceiver> newReceiversList = new List<IButtonReceiver>(potentialButtonReceivers);
            newReceiversList.ControlledMix(rng);
            potentialButtonReceivers.Clear();

            int maxCount = rng.Next(1, 4);

            if (maxCount > newReceiversList.Count)
            {
                maxCount = newReceiversList.Count;
            }

            for (int i = 0; i < maxCount; i++)
            {
                potentialButtonReceivers.Add(newReceiversList[i]);
            }
            return potentialButtonReceivers;
        }
    }
}
