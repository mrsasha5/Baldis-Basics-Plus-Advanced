using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using HarmonyLib;
using MTM101BaldAPI;
using Newtonsoft.Json;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class StructureBuilderSpawnData : BaseSpawnData<StructureBuilder>
    {
        [JsonProperty]
        private StructureParametersData[] parameters;

        [JsonProperty]
        private bool forced;

        [JsonProperty("reference")]
        private string Serialization_Reference
        {
            set
            {
                instance = ObjectStorage.StructureBuilders[value];
            }
        }

        public override void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            int weight = GetWeight(floor, levelObject.type);

            if (forced)
            {
                levelObject.forcedStructures = levelObject.forcedStructures.AddToArray(new StructureWithParameters()
                {
                    prefab = Instance,
                    parameters = LoadParametersFromSerializedData(floor)
                });
            }
            else if (weight != 0)
            {
                levelObject.potentialStructures = levelObject.potentialStructures.AddToArray(new WeightedStructureWithParameters()
                {
                    selection = new StructureWithParameters()
                    {
                        prefab = Instance,
                        parameters = LoadParametersFromSerializedData(floor)
                    },
                    weight = weight
                });
            }
        }

        private StructureParameters LoadParametersFromSerializedData(int floor)
        {
            if (parameters == null || parameters.Length == 0) return null;
            StructureParametersData nearestParams = default;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (Mathf.Abs(parameters[i].floor - floor) < Mathf.Abs(nearestParams.floor - floor))
                {
                    nearestParams = parameters[i];
                }
            }

            StructureParameters _parameters = new StructureParameters()
            {
                minMax = nearestParams.minMax,
                chance = nearestParams.chance
            };

            List<WeightedGameObject> weightedPrefabs = new List<WeightedGameObject>();

            if (nearestParams.prefab != null)
            {
                foreach (WeightedPrefabData prefabData in nearestParams.prefab)
                {
                    weightedPrefabs.Add(new WeightedGameObject
                    {
                        selection = ObjectStorage.Objects[prefabData.selection],
                        weight = prefabData.weight
                    });
                }
            }

            _parameters.prefab = weightedPrefabs.ToArray();

            return _parameters;
        }
    }

    internal struct StructureParametersData
    {
        public int floor;

        public WeightedPrefabData[] prefab;

        public IntVector2[] minMax;

        public float[] chance;
    }

    internal struct WeightedPrefabData
    {
        public string selection;

        public int weight;
    }
}
