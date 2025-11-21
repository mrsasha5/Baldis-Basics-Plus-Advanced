using MTM101BaldAPI;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class RandomEventSpawnData : BaseSpawnData<RandomEvent>
    {

        public RandomEventSpawnData(RandomEvent instance)
        {
            this.instance = instance;
        }

        public override void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
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

    }
}
