using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI;
using Newtonsoft.Json;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class StructureBuilderSpawnData : BaseSpawnData
    {
        [JsonProperty]
        private StructureParametersData[] parameters;

        [JsonProperty]
        private bool forced;

        private StructureBuilder instance;

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
                    prefab = instance,
                    parameters = StructureParametersData.Deserialize(parameters, floor)
                });
            }
            else if (weight != 0)
            {
                levelObject.potentialStructures = levelObject.potentialStructures.AddToArray(new WeightedStructureWithParameters()
                {
                    selection = new StructureWithParameters()
                    {
                        prefab = instance,
                        parameters = StructureParametersData.Deserialize(parameters, floor)
                    },
                    weight = weight
                });
            }
        }

    }

    internal class StructureBuilderExtensionSpawnData : BaseSpawnData
    {
        private StructureBuilder builder;

        [JsonProperty("parameters")]
        private StructureParametersData[] instance;

        [JsonProperty]
        private bool isStructureInForcedList;

        [JsonProperty("structure")]
        private string Serialization_Builder
        {
            set
            {
                if (value == "cached_weighted")
                {
                    builder = AssetStorage.weightedPlacer;
                    return;
                }
                else if (value == "cached_individual")
                {
                    builder = AssetStorage.individualPlacer;
                    return;
                }
                builder = AssetHelper.LoadAsset<StructureBuilder>(value);
            }
        }

        public override void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            StructureParameters structureParametersData = StructureParametersData.Deserialize(instance, floor);
            if (structureParametersData == null) return;
            StructureWithParameters structure = null;

            // WHO THE FUCK ADDS NULL REFERENCE IN THE FORCED STRUCTURES
            if (isStructureInForcedList)
                structure = Array.Find(levelObject.forcedStructures, x => x?.prefab == builder);
            else
                structure = Array.Find(levelObject.potentialStructures, x => x?.selection?.prefab == builder)?.selection;

            if (structure != null && structure.parameters != null)
            {
                if (structureParametersData.chance != null)
                {
                    if (structure.parameters.chance != null)
                        structure.parameters.chance = structure.parameters.chance.AddRangeToArray(structureParametersData.chance);
                    else
                        structure.parameters.chance = structureParametersData.chance;
                } 
                if (structureParametersData.prefab != null)
                {
                    if (structure.parameters.prefab != null)
                        structure.parameters.prefab = structure.parameters.prefab.AddRangeToArray(structureParametersData.prefab);
                    else
                        structure.parameters.prefab = structureParametersData.prefab;
                }  
                if (structureParametersData.minMax != null)
                {
                    if (structure.parameters.minMax != null)
                        structure.parameters.minMax = structure.parameters.minMax.AddRangeToArray(structureParametersData.minMax);
                    else
                        structure.parameters.minMax = structureParametersData.minMax;
                }
            }
        }
    }

    internal struct StructureParametersData
    {
        private static IntVector2[] _minMax = new IntVector2[0];
        private static WeightedGameObject[] _prefabData = new WeightedGameObject[0];
        private static float[] _chance = new float[0];

        public int floor;

        public WeightedPrefabData[] prefab;

        public IntVector2[] minMax;

        public float[] chance;

        public static StructureParameters Deserialize(StructureParametersData[] parameters, int floor)
        {
            if (parameters == null || parameters.Length == 0) return null;
            StructureParametersData nearestParams = parameters[0];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (Mathf.Abs(parameters[i].floor - floor) < Mathf.Abs(nearestParams.floor - floor))
                {
                    nearestParams = parameters[i];
                }
            }

            StructureParameters _parameters = new StructureParameters()
            {
                minMax = nearestParams.minMax != null ? nearestParams.minMax : _minMax,
                chance = nearestParams.chance != null ? nearestParams.chance : _chance
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
            if (weightedPrefabs.Count > 0)
                _parameters.prefab = weightedPrefabs.ToArray();
            else
                _parameters.prefab = _prefabData;
            
            return _parameters;
        }
    }

    internal struct WeightedPrefabData
    {
        public string selection;

        public int weight;
    }
}
