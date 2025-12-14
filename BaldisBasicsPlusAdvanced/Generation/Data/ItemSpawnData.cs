using System;
using System.Linq;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using Newtonsoft.Json;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class ItemSpawnData : BaseSpawnData<ItemObject>
    {
        [JsonProperty]
        private bool forced;

        [JsonProperty]
        private int uses = 1;
        
        private WeightData[] shopWeights;

        private WeightData[] partyWeights;

        private WeightData[] mysteryRoomWeights;

        private int[] bannedMysteryRoomFloors;

        private int[] bannedPartyFloors;

        //Enum, uses
        [JsonProperty("reference")]
        private string Serialization_Item
        {
            set
            {
                ItemMetaData meta = FindInstance(value);
                instance = meta.itemObjects[uses - 1];
            }
        }

        [JsonProperty("shopWeights")]
        private WeightData[] ShopWeights
        {
            get
            {
                return shopWeights == null ? StandardGenerationData._weightDataArr : shopWeights;
            }
            set
            {
                shopWeights = value;
            }
        }

        [JsonProperty("partyWeights")]
        private WeightData[] PartyWeights
        {
            get
            {
                return partyWeights == null ? StandardGenerationData._weightDataArr : partyWeights;
            }
            set
            {
                partyWeights = value;
            }
        }

        [JsonProperty("mysteryRoomWeights")]
        private WeightData[] MysteryRoomWeights
        {
            get
            {
                return mysteryRoomWeights == null ? StandardGenerationData._weightDataArr : mysteryRoomWeights;
            }
            set
            {
                mysteryRoomWeights = value;
            }
        }
        
        [JsonProperty("bannedMysteryRoomFloors")]
        private int[] BannedMysteryRoomFloors
        {
            get
            {
                return bannedMysteryRoomFloors == null ? StandardGenerationData._intArr : bannedMysteryRoomFloors;
            }
            set
            {
                bannedMysteryRoomFloors = value;
            }
        }

        [JsonProperty("bannedPartyFloors")]
        private int[] BannedPartyFloors
        {
            get
            {
                return bannedPartyFloors == null ? StandardGenerationData._intArr : bannedPartyFloors;
            }
            set
            {
                bannedPartyFloors = value;
            }
        }

        public ItemSpawnData(ItemObject instance)
        {
            this.instance = instance;
        }

        public ItemSpawnData AddShopWeight(int floor, int weight)
        {
            ShopWeights = ShopWeights.AddToArray(new WeightData(floor, weight));
            return this;
        }

        public ItemSpawnData AddPartyWeight(int floor, int weight)
        {
            PartyWeights = PartyWeights.AddToArray(new WeightData(floor, weight));
            return this;
        }

        public ItemSpawnData AddMysteryRoomWeight(int floor, int weight)
        {
            MysteryRoomWeights = MysteryRoomWeights.AddToArray(new WeightData(floor, weight));
            return this;
        }

        public int GetJohnnyStoreWeight(int floor)
        {
            if (ShopWeights.Length == 0) return 0;

            return WeightData.FindNearestWeightForFloor(ShopWeights, floor);
        }

        public int GetMysteryRoomWeight(int floor)
        {
            if (MysteryRoomWeights.Length == 0 || BannedMysteryRoomFloors.Contains(floor)) return 0;

            return WeightData.FindNearestWeightForFloor(MysteryRoomWeights, floor);
        }

        public int GetPartyWeight(int floor)
        {
            if (PartyWeights.Length == 0 || BannedPartyFloors.Contains(floor)) return 0;

            return WeightData.FindNearestWeightForFloor(PartyWeights, floor);
        }

        public ItemSpawnData ForceSpawn()
        {
            forced = true;
            return this;
        }

        public ItemSpawnData SetBannedMysteryRoomFloors(params int[] bannedFloors)
        {
            BannedMysteryRoomFloors = bannedFloors;
            return this;
        }

        public ItemSpawnData SetBannedPartyFloors(params int[] bannedFloors)
        {
            BannedPartyFloors = bannedFloors;
            return this;
        }

        public override void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            if (Instance == null)
                throw new Exception("Object reference is null!");

            int weight = GetWeight(floor, levelObject.type);
            if (forced)
            {
                levelObject.forcedItems.Add(Instance);
            }
            else if (weight != 0)
            {
                levelObject.potentialItems = levelObject.potentialItems.AddToArray(new WeightedItemObject()
                {
                    selection = Instance,
                    weight = weight
                });
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

        public static ItemMetaData FindInstance(string @enum)
        {
            ItemMetaData meta = 
                ItemMetaStorage.Instance.FindByEnumFromMod(EnumExtensions.GetFromExtendedName<Items>(@enum), AdvancedCore.Instance.Info);
            if (meta == null) throw new Exception("Item metadata was not found!");
            return meta;
                
        }

    }
}
