using System;
using System.Collections.Generic;
using System.IO;
using BaldisBasicsPlusAdvanced.Generation.Data;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using Newtonsoft.Json;

namespace BaldisBasicsPlusAdvanced.Generation
{
    //This class is going fully replace GeneratorPatchingManager someday
    internal class GenerationManager
    {

        private static List<IStandardSpawnData> data = new List<IStandardSpawnData>();

        public static List<IStandardSpawnData> Data => data;

        public static void LoadGenerationDataFromFiles()
        {
            string basePath = AssetHelper.modPath + "Data/Generation/";
            foreach (string path in Directory.GetFiles(basePath + "Items"))
            {
                data.Add(JsonConvert.DeserializeObject<ItemSpawnData>(File.ReadAllText(path)));
            }
            foreach (string path in Directory.GetFiles(basePath + "RandomEvents"))
            {
                data.Add(JsonConvert.DeserializeObject<RandomEventSpawnData>(File.ReadAllText(path)));
            }
            foreach (string path in Directory.GetFiles(basePath + "Npcs"))
            {
                data.Add(JsonConvert.DeserializeObject<NpcSpawnData>(File.ReadAllText(path)));
            }
            foreach (string path in Directory.GetFiles(basePath + "RoomGroups"))
            {
                data.Add(JsonConvert.DeserializeObject<RoomGroupSpawnData>(File.ReadAllText(path)));
            }
            foreach (string path in Directory.GetFiles(basePath + "Structures"))
            {
                data.Add(JsonConvert.DeserializeObject<StructureBuilderSpawnData>(File.ReadAllText(path)));
            }
        }

        public static void LoadHardcodedGenerationData()
        {
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
#warning Back it when GeneratorPatchingManager will be finally destroyed
                //else levelObject.MarkAsModifiedByMod(AdvancedCore.Instance.Info);

                RegisterLevelObjectData(name, floor, sceneObject, levelObject);
            }
        }

        private static ItemSpawnData RegisterItem(string @enum)
        {
            ItemMetaData meta = ItemSpawnData.FindInstance(@enum);

            ItemSpawnData data = new ItemSpawnData(meta.value);

            GenerationManager.data.Add(data);

            return data;
        }

        private static RandomEventSpawnData RegisterRandomEvent(string @enum)
        {
            RandomEventMetadata meta = RandomEventSpawnData.FindInstance(@enum);

            RandomEventSpawnData data = new RandomEventSpawnData(meta.value);

            GenerationManager.data.Add(data);

            return data;
        }

        private static NpcSpawnData RegisterNpcSpawnData(string @enum)
        {
            NPCMetadata meta = NpcSpawnData.FindInstance(@enum);

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
