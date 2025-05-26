using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Patches;
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
                    selection = ObjectsStorage.Objects["plate"],
                    weight = 100
                }
            };
        }

        public override void PostOpenCalcGenerate(LevelGenerator lg, Random rng)
        {
            
        }

        public override void OnGenerationFinished(LevelGenerator lg)
        {
            if (lg.controlledRNG.NextDouble() <= parameters.chance[0])
            {
                if (Array.Find(FindObjectsOfType<UnityEngine.Object>(), x => x is IButtonReceiver) != null)
                    Build(lg, lg.controlledRNG, isRoomCells: false);

                for (int i = 0; i < generatedPlates.Count; i++)
                {
                    ((PressurePlate)generatedPlates[i]).ConnectTo(CollectPotentialReceiversData(lg.controlledRNG));
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

            //potentialButtonReceivers.AddRange(FindObjectsOfType<BeltManager>());
            //potentialButtonReceivers.AddRange(FindObjectsOfType<RotoHall>());
            //potentialButtonReceivers.AddRange(FindObjectsOfType<LockdownDoor>());

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
