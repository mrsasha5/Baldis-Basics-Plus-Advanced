using System;
using System.Collections.Generic;
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
        private bool forcedSpawn;
        
        private WeightData[] shopWeights;

        private WeightData[] partyWeights;

        private WeightData[] mysteryRoomWeights;

        private int[] bannedMysteryRoomFloors;

        private int[] bannedPartyFloors;

        //Enum, uses
        [JsonProperty("reference")]
        private KeyValuePair<string, int> Serialization_Item
        {
            set
            {
                ItemMetaData meta = FindInstance(value.Key);
                instance = meta.itemObjects[value.Value - 1];
            }
        }

        [JsonProperty("shopWeights")]
        private WeightData[] ShopWeights
        {
            get
            {
                return shopWeights == null ? _weightDataArr : shopWeights;
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
                return partyWeights == null ? _weightDataArr : partyWeights;
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
                return mysteryRoomWeights == null ? _weightDataArr : mysteryRoomWeights;
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
                return bannedMysteryRoomFloors == null ? _intArr : bannedMysteryRoomFloors;
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
                return bannedPartyFloors == null ? _intArr : bannedPartyFloors;
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

        public override int GetWeight(int floor, LevelType levelType)
        {
            return base.GetWeight(floor, levelType);
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

            return FindNearestWeightForFloor(ShopWeights, floor);
        }

        public int GetMysteryRoomWeight(int floor)
        {
            if (MysteryRoomWeights.Length == 0 || BannedMysteryRoomFloors.Contains(floor)) return 0;

            return FindNearestWeightForFloor(MysteryRoomWeights, floor);
        }

        public int GetPartyWeight(int floor)
        {
            if (PartyWeights.Length == 0 || BannedPartyFloors.Contains(floor)) return 0;

            return FindNearestWeightForFloor(PartyWeights, floor);
        }

        public ItemSpawnData ForceSpawn()
        {
            forcedSpawn = true;
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

        public static ItemMetaData FindInstance(string @enum)
        {
            ItemMetaData meta = 
                ItemMetaStorage.Instance.FindByEnumFromMod(EnumExtensions.GetFromExtendedName<Items>(@enum), AdvancedCore.Instance.Info);
            if (meta == null) throw new Exception("Item metadata was not found!");
            return meta;
                
        }

    }
}
