using System;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using Newtonsoft.Json;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class RandomEventSpawnData : BaseSpawnData<RandomEvent>
    {
        [JsonProperty("reference")]
        private string Serialization_RandomEvent
        {
            set
            {
                instance = FindInstance(value).value;
            }
        }

        public RandomEventSpawnData(RandomEvent instance)
        {
            this.instance = instance;
        }

        public override void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            if (Instance == null)
                throw new Exception("Object reference is null!");

            int weight = GetWeight(floor, levelObject.type);
            if (weight != 0)
            {
                levelObject.randomEvents.Add(new WeightedRandomEvent()
                {
                    selection = Instance,
                    weight = weight
                });
            }
        }

        public static RandomEventMetadata FindInstance(string @enum)
        {
            RandomEventMetadata meta = 
                RandomEventMetaStorage.Instance.Find(x => x.type == EnumExtensions.GetFromExtendedName<RandomEventType>(@enum));
            if (meta == null) throw new Exception("Random event metadata was not found!");
            return meta;
        }

    }
}
