using BaldisBasicsPlusAdvanced.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Game
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

        public override BaseSpawningData setFalseEverywhere()
        {
            base.setFalseEverywhere();
            spawnsOnRooms = false;
            spawnsOnShop = false;
            spawnsOnFieldTrips = false;
            spawnsOnParty = false;
            spawnsOnMysteryRooms = false;
            return this;
        }

        public ItemSpawningData setSpawnsOnRooms(bool spawns)
        {
            this.spawnsOnRooms = spawns;
            return this;
        }

        public ItemSpawningData setSpawnsOnShop(bool spawns)
        {
            this.spawnsOnShop = spawns;
            return this;
        }

        public ItemSpawningData setSpawnsOnFieldTrips(bool spawns)
        {
            this.spawnsOnFieldTrips = spawns;
            return this;
        }

        public ItemSpawningData setSpawnsOnParty(bool spawns)
        {
            this.spawnsOnParty = spawns;
            return this;
        }

        public ItemSpawningData setSpawnsOnMysteryRooms(bool spawns)
        {
            this.spawnsOnMysteryRooms = spawns;
            return this;
        }
    }
}
