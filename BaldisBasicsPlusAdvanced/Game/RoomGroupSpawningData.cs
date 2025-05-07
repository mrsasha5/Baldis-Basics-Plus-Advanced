namespace BaldisBasicsPlusAdvanced.Game
{
    public class RoomGroupSpawningData : BaseSpawningData
    {
        private readonly RoomGroup group;

        public RoomGroup Group => group;

        public RoomGroupSpawningData(string key, RoomGroup group) : base(key)
        {
            this.group = group;
        }
    }
}
