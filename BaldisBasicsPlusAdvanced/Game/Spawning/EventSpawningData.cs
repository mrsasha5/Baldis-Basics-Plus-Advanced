namespace BaldisBasicsPlusAdvanced.Game.Spawning
{
    public class EventSpawningData : BaseSpawningData
    {
        private RandomEvent randomEvent;

        public RandomEvent RandomEvent => randomEvent;

        public EventSpawningData(string key, RandomEvent @event) : base(key)
        {
            randomEvent = @event;
        }
    }
}
