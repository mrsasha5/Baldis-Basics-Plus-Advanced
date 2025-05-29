namespace BaldisBasicsPlusAdvanced.Game.Spawning
{
    public class RoomGroupSpawningData : BaseSpawningData
    {
        private RoomGroup group;

        public RoomGroup Group => group;

        public RoomGroupSpawningData(string key, RoomGroup group) : base(key)
        {
            this.group = group;
        }
    }
}
