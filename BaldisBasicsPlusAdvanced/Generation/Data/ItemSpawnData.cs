using System.Linq;
using HarmonyLib;
using MTM101BaldAPI;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class ItemSpawnData : BaseSpawnData<ItemObject>
    {

        private bool mysteryRoomEvent;

        private bool partyEvent;

        private bool rooms;

        private bool forcedSpawn;

        private bool johnnyStore;

        private WeightData[] shopWeights = _weightDataArr;

        private WeightData[] partyWeights = _weightDataArr;

        private WeightData[] mysteryRoomWeights = _weightDataArr;

        private int[] bannedMysteryRoomFloors = _intArr;

        private int[] bannedPartyFloors = _intArr;

        public ItemSpawnData(ItemObject instance)
        {
            this.instance = instance;
        }

        public override int GetWeight(int floor, LevelType levelType)
        {
            if (!rooms) return 0;
            return base.GetWeight(floor, levelType);
        }

        public ItemSpawnData AddShopWeight(int floor, int weight)
        {
            shopWeights = shopWeights.AddToArray(new WeightData(floor, weight));
            return this;
        }

        public ItemSpawnData AddPartyWeight(int floor, int weight)
        {
            partyWeights = partyWeights.AddToArray(new WeightData(floor, weight));
            return this;
        }

        public ItemSpawnData AddMysteryRoomWeight(int floor, int weight)
        {
            mysteryRoomWeights = mysteryRoomWeights.AddToArray(new WeightData(floor, weight));
            return this;
        }

        public int GetJohnnyStoreWeight(int floor)
        {
            if (!johnnyStore || shopWeights.Length == 0) return 0;

            return FindNearestWeightForFloor(shopWeights, floor);
        }

        public int GetMysteryRoomWeight(int floor)
        {
            if (!mysteryRoomEvent || mysteryRoomWeights.Length == 0 || bannedMysteryRoomFloors.Contains(floor)) return 0;

            return FindNearestWeightForFloor(mysteryRoomWeights, floor);
        }

        public int GetPartyWeight(int floor)
        {
            if (!partyEvent || partyWeights.Length == 0 || bannedPartyFloors.Contains(floor)) return 0;

            return FindNearestWeightForFloor(partyWeights, floor);
        }

        public ItemSpawnData ForceSpawn()
        {
            forcedSpawn = true;
            return this;
        }

        public ItemSpawnData AddInRooms()
        {
            rooms = true;
            return this;
        }

        public ItemSpawnData AddToPartyEvent()
        {
            partyEvent = true;
            return this;
        }

        public ItemSpawnData AddToMysteryRoomEvent()
        {
            mysteryRoomEvent = true;
            return this;
        }

        public ItemSpawnData SetBannedMysteryRoomFloors(params int[] bannedFloors)
        {
            bannedMysteryRoomFloors = bannedFloors;
            return this;
        }

        public ItemSpawnData SetBannedPartyFloors(params int[] bannedFloors)
        {
            bannedPartyFloors = bannedFloors;
            return this;
        }

        public ItemSpawnData AddInJohnnyStore()
        {
            johnnyStore = true;
            return this;
        }

        public override void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            int weight = GetWeight(floor, levelObject.type);
            if (weight != 0)
            {
                if (forcedSpawn)
                {
                    levelObject.forcedItems.Add(Instance);
                }
                else
                {
                    levelObject.potentialItems = levelObject.potentialItems.AddToArray(new WeightedItemObject()
                    {
                        selection = Instance,
                        weight = weight
                    });
                }
            }
            int shopWeight = GetJohnnyStoreWeight(floor);
            if (shopWeight != 0)
            {
                sceneObject.shopItems = sceneObject.shopItems.AddToArray(new WeightedItemObject()
                {
                    selection = Instance,
                    weight = shopWeight
                });
            }
        }

    }
}
