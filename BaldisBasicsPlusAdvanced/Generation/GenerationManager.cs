using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Generation.Data;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;

namespace BaldisBasicsPlusAdvanced.Generation
{
    internal class GenerationManager
    {

        private static List<IStandardSpawnData> data = new List<IStandardSpawnData>();

        public static List<IStandardSpawnData> Data => data;

        public static void DefineGeneration()
        {
#warning Add weights for mystery room
            RegisterItem("Hammer")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 50)
                .AddWeight(floor: 2, 50)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory);

            RegisterItem("WindBlower")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 75)
                .AddWeight(floor: 2, 75)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            RegisterItem("MysteriousTeleporter")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddToMysteryRoomEvent()
                .AddShopWeight(floor: 2, 50)
                .AddMysteryRoomWeight(floor: 2, 50)
                .AddPartyWeight(floor: 2, 50)
                .AddWeight(floor: 2, 50)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            RegisterItem("IceBoots")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 75)
                .AddWeight(floor: 2, 75)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            RegisterItem("PlaceableFan")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 35)
                .AddWeight(floor: 2, 35)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory, LevelType.Maintenance);

            RegisterItem("TeleportationBomb")
                .AddInRooms()
                .AddToMysteryRoomEvent()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 50)
                .AddMysteryRoomWeight(floor: 2, 50)
                .AddWeight(floor: 2, 50)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            RegisterItem("MagicClock")
                .AddInRooms()
                .AddToMysteryRoomEvent()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 50)
                .AddMysteryRoomWeight(floor: 2, 50)
                .AddWeight(floor: 2, 50)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            RegisterItem("InflatableBalloon")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 50)
                .AddWeight(floor: 2, 50)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory, LevelType.Maintenance);

            RegisterItem("Dough")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 50)
                .AddWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse);

            RegisterItem("Bread")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 50)
                .AddWeight(floor: 2, 50)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse);

            RegisterItem("RawChickenLeg")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 40)
                .AddWeight(floor: 2, 40)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse);

            RegisterItem("CookedChickenLeg")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 15)
                .AddWeight(floor: 2, 15)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse);

            RegisterItem("BoxingGlove")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 35)
                .AddWeight(floor: 2, 35)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory, LevelType.Maintenance, LevelType.Factory);

            RegisterItem("PlaceablePortal")
                .AddInRooms()
                .AddInJohnnyStore()
                .AddShopWeight(floor: 2, 30)
                .AddWeight(floor: 2, 30)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory)
                .SetBannedFloors(1);

#warning TODO: reimplement its appearing
            RegisterItem("MysteriousBusPass")
                .AddInRooms()
                .AddWeight(floor: 2, 100)
                .SetBannedFloors(1, 3)
                .SetLevelTypes(LevelType.Schoolhouse);
        }

        public static void RegisterLevelObjectData(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            foreach (IStandardSpawnData spawnData in data)
            {
                spawnData.Register(name, floor, sceneObject, levelObject);
            }
        }

        public static void RegisterSceneObjectData(string name, int floor, SceneObject sceneObject)
        {
            floor++; //It starts from zero, but I prefer 1 in this case
            foreach (CustomLevelObject levelObject in sceneObject.GetCustomLevelObjects())
            {
                if (levelObject.IsModifiedByMod(AdvancedCore.Instance.Info)) continue;
                //else levelObject.MarkAsModifiedByMod(AdvancedCore.Instance.Info); //Back it when GeneratorPatchingManager
                                                                                    //will be finally destroyed

                RegisterLevelObjectData(name, floor, sceneObject, levelObject);
            }
        }

        private static ItemSpawnData RegisterItem(string @enum)
        {
            ItemMetaData meta = 
                ItemMetaStorage.Instance.FindByEnumFromMod(
                    EnumExtensions.GetFromExtendedName<Items>(@enum), AdvancedCore.Instance.Info);

            if (meta == null) throw new Exception("Item metadata was not found!");

            ItemSpawnData data = new ItemSpawnData(meta.value);

            GenerationManager.data.Add(data);

            return data;
        }

        private static RandomEventSpawnData RegisterRandomEvent(string @enum)
        {
            RandomEventMetadata meta =
                RandomEventMetaStorage.Instance.Find(x => x.type == EnumExtensions.GetFromExtendedName<RandomEventType>(@enum));

            if (meta == null) throw new Exception("Random event metadata was not found!");

            RandomEventSpawnData data = new RandomEventSpawnData(meta.value);

            GenerationManager.data.Add(data);

            return data;
        }

        private static NpcSpawnData RegisterNpcSpawnData(string @enum)
        {
            NPCMetadata meta =
                NPCMetaStorage.Instance.Find(x => x.character == EnumExtensions.GetFromExtendedName<Character>(@enum));

            if (meta == null) throw new Exception("NPC metadata was not found!");

            NpcSpawnData data = new NpcSpawnData(meta.value);

            GenerationManager.data.Add(data);

            return data;
        }

        public static List<WeightedItemObject> GetMysteryRoomItems(int floor)
        {
            List<WeightedItemObject> items = new List<WeightedItemObject>();
            foreach (IStandardSpawnData data in data)
            {
                if (data is ItemSpawnData itemData)
                {
                    int weight = itemData.GetMysteryRoomWeight(floor);
                    if (weight != 0)
                    {
                        items.Add(new WeightedItemObject
                        {
                            selection = itemData.Instance,
                            weight = weight
                        });
                    }
                }
            }
            return items;
        }

        public static List<WeightedItemObject> GetPartyItems(int floor)
        {
            List<WeightedItemObject> items = new List<WeightedItemObject>();
            foreach (IStandardSpawnData data in data)
            {
                if (data is ItemSpawnData itemData)
                {
                    int weight = itemData.GetPartyWeight(floor);
                    if (weight != 0)
                    {
                        items.Add(new WeightedItemObject
                        {
                            selection = itemData.Instance,
                            weight = weight
                        });
                    }
                }
            }
            return items;
        }

    }
}
