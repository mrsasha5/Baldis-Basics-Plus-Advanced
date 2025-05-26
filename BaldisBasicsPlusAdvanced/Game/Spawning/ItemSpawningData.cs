namespace BaldisBasicsPlusAdvanced.Game.Spawning
{
    public class ItemSpawningData : BaseSpawningData
    {
        private bool spawnsOnRooms = true;

        private bool spawnsOnShop = true;

        private bool spawnsOnFieldTrips = false;

        private bool spawnsOnParty = false;

        private bool spawnsOnMysteryRooms = false;

        private ItemObject itemObject;

        public bool SpawnsOnRooms => spawnsOnRooms;

        public bool SpawnsOnShop => spawnsOnShop;

        public bool SpawnsOnFieldTrips => spawnsOnFieldTrips;

        public bool SpawnsOnParty => spawnsOnParty;

        public bool SpawnsOnMysteryRooms => spawnsOnMysteryRooms;

        public ItemObject ItemObject => itemObject;

        public ItemSpawningData(string key, ItemObject itemObject) : base(key)
        {
            this.itemObject = itemObject;
        }

        public override BaseSpawningData DoNotSpawn()
        {
            base.DoNotSpawn();
            spawnsOnRooms = false;
            spawnsOnShop = false;
            spawnsOnFieldTrips = false;
            spawnsOnParty = false;
            spawnsOnMysteryRooms = false;
            return this;
        }

        public ItemSpawningData SetSpawnsOnRooms(bool spawns)
        {
            spawnsOnRooms = spawns;
            return this;
        }

        public ItemSpawningData SetSpawnsOnShop(bool spawns)
        {
            spawnsOnShop = spawns;
            return this;
        }

        public ItemSpawningData SetSpawnsOnFieldTrips(bool spawns)
        {
            spawnsOnFieldTrips = spawns;
            return this;
        }

        public ItemSpawningData SetSpawnsOnParty(bool spawns)
        {
            spawnsOnParty = spawns;
            return this;
        }

        public ItemSpawningData SetSpawnsOnMysteryRooms(bool spawns)
        {
            spawnsOnMysteryRooms = spawns;
            return this;
        }
    }
}
