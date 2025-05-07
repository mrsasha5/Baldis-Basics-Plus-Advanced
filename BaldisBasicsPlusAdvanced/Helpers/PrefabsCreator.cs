using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.GameItems;
using BaldisBasicsPlusAdvanced.Game;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI;
using UnityEngine;
using System.Linq;
using MTM101BaldAPI.ObjectCreation;
using UnityEngine.UI;
using BaldisBasicsPlusAdvanced.Game.Objects.SodaMachines;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Game.Components.Overlays;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class PrefabsCreator
    {
        private const string editorPath = "Textures/Compats/LevelEditor/";

        public static ItemSpawningData createItem<I>(string nameKey, string descKey, string enumName, int weight, 
            string smallSpriteFileName, string largeSpriteFileName, int generatorCost, int price,
            float pixelsPerUnitLargeTexture = 100f, ItemFlags flags = ItemFlags.None,
            string[] tags = null, ItemMetaData itemMeta = null) where I : Item
        {
            Sprite smallSprite = AssetsHelper.spriteFromFile("Textures/Items/SmallSprites/" + smallSpriteFileName);
            Sprite largeSprite = AssetsHelper.spriteFromFile("Textures/Items/LargeSprites/" + largeSpriteFileName, pixelsPerUnitLargeTexture);

            if (tags == null) tags = new string[0];

            ItemBuilder itemBuilder = new ItemBuilder(AdvancedCore.Instance.Info)
                .SetNameAndDescription(nameKey, descKey).SetSprites(smallSprite, largeSprite)
                .SetEnum(enumName)
                .SetGeneratorCost(generatorCost)
                .SetShopPrice(price)
                .SetItemComponent<I>();

            if (itemMeta == null) itemBuilder.SetMeta(flags, tags);
            else itemBuilder.SetMeta(itemMeta);

            ItemObject itemObject = itemBuilder.Build();

            I item = (I)itemObject.item;

            if (item is IPrefab)
            {
                IPrefab _item = (IPrefab)item;
                _item.initializePrefab();
            }

            ObjectsStorage.ItemsObjects.Add(enumName, itemObject);
            ItemSpawningData spawnData = new ItemSpawningData(enumName, itemObject);
            ObjectsStorage.SpawningData.Add(enumName, spawnData);
            
            Singleton<PlayerFileManager>.Instance.itemObjects.Add(itemObject); //нужно потом для сериализации предметов в файл сохранения 
            //но похоже из-за использования системы сохранений АПИ, которое переопределяет сохранения в игре, оно стало не нужно

            WeightedItemObject ItemResult = new WeightedItemObject()
            {
                selection = itemObject,
                weight = weight
            };

            ObjectsStorage.WeightedItemObjects.Add(enumName, ItemResult);

            WeightedItem weightedItem;

            weightedItem = new WeightedItem()
            {
                selection = itemObject,
                weight = weight
            };

            ObjectsStorage.WeightedItems.Add(enumName, weightedItem);

            if (ModsIntegration.LevelEditorInstalled) {
                ObjectsStorage.EditorSprites.Add("item_" + enumName, AssetsHelper.spriteFromFile(editorPath + "Items/adv_editor_" + enumName + ".png"));
            }

            return spawnData;
        }

        public static BaseSpawningData createEvent<T>(string name, string enumName, int weight, float minTime, float maxTime,
            RandomEventFlags flags) where T : BaseRandomEvent
        {
            T randomEvent = new RandomEventBuilder<T>(AdvancedCore.Instance.Info)
                .SetName(name)
                .SetEnum(enumName)
                .SetMinMaxTime(minTime, maxTime)
                .SetMeta(flags)
                .Build();

            WeightedRandomEvent weighted = new WeightedRandomEvent()
            {
                selection = randomEvent,
                weight = weight
            };
            ObjectsStorage.WeightedEvents.Add(enumName, weighted);
            ObjectsStorage.Events.Add(enumName, randomEvent);

            BaseSpawningData spawningData = new BaseSpawningData(enumName);
            ObjectsStorage.SpawningData.Add(enumName, spawningData);
            return spawningData;

        }

        public static BaseSodaMachine createVendingMachine(string name, ItemObject requiredItem, Material face, Material faceOut, ItemObject item, WeightedItemObject[] potentialItems = null)
        {
            SodaMachine component = UnityEngine.Object.Instantiate(AssetsHelper.loadAsset<SodaMachine>("ZestyMachine"));
            component.gameObject.name = name;
            BaseSodaMachine sodaMachine = component.gameObject.AddComponent<BaseSodaMachine>();
            GameObject.Destroy(component);

            MeshRenderer meshRenderer = sodaMachine.GetComponent<MeshRenderer>();
            Material[] goodMachineMaterials = meshRenderer.materials;
            goodMachineMaterials[1] = face;
            meshRenderer.materials = goodMachineMaterials;

            ReflectionHelper.setValue<ItemObject>(sodaMachine, "requiredItem", requiredItem);
            ReflectionHelper.setValue<ItemObject>(sodaMachine, "item", item);
            ReflectionHelper.setValue<MeshRenderer>(sodaMachine, "meshRenderer", meshRenderer);
            ReflectionHelper.setValue<Material>(sodaMachine, "outOfStockMat", faceOut);

            if (potentialItems != null) ReflectionHelper.setValue<WeightedItemObject>(sodaMachine, "potentialItems", potentialItems);

            sodaMachine.gameObject.ConvertToPrefab(true);
            ObjectsStorage.SodaMachines.Add(name, sodaMachine);

            if (ModsIntegration.LevelEditorInstalled)
            {
                ObjectsStorage.EditorSprites.Add("vending_" + name, AssetsHelper.spriteFromFile(editorPath + "Objects/adv_editor_" + name + ".png"));
            }

            return sodaMachine;
        }

        public static MultipleRequiredSodaMachine createMultipleRequiredVendingMachine(string name, ItemObject requiredItem, int requitedAmmount, Material face, Material faceOut, ItemObject item, WeightedItemObject[] potentialItems = null)
        {
            SodaMachine component = UnityEngine.Object.Instantiate(AssetsHelper.loadAsset<SodaMachine>("ZestyMachine"));
            component.gameObject.name = name;
            MultipleRequiredSodaMachine sodaMachine = component.gameObject.AddComponent<MultipleRequiredSodaMachine>();
            GameObject.Destroy(component);

            MeshRenderer meshRenderer = sodaMachine.GetComponent<MeshRenderer>();
            Material[] goodMachineMaterials = meshRenderer.materials;
            goodMachineMaterials[1] = face;
            meshRenderer.materials = goodMachineMaterials;

            ReflectionHelper.setValue<ItemObject>(sodaMachine, "requiredItem", requiredItem);
            sodaMachine.requiredAmmount = requitedAmmount;
            ReflectionHelper.setValue<ItemObject>(sodaMachine, "item", item);
            ReflectionHelper.setValue<MeshRenderer>(sodaMachine, "meshRenderer", meshRenderer);
            ReflectionHelper.setValue<Material>(sodaMachine, "outOfStockMat", faceOut);

            if (potentialItems != null) ReflectionHelper.setValue<WeightedItemObject>(sodaMachine, "potentialItems", potentialItems);

            sodaMachine.gameObject.ConvertToPrefab(true);
            ObjectsStorage.SodaMachines.Add(name, sodaMachine);

            if (ModsIntegration.LevelEditorInstalled)
            {
                ObjectsStorage.EditorSprites.Add("vending_" + name, AssetsHelper.spriteFromFile(editorPath + "Objects/adv_editor_" + name + ".png"));
            }

            return sodaMachine;
        }

        public static BaseSpawningData createWeigthedHallObjectsBuilder<T>(string name, int weight, int min, int max, GameObject prefab, bool isForced = false) where T : GenericHallBuilder
        {
            GenericHallBuilder baseInstance = AssetsHelper.loadAsset<GenericHallBuilder>("ZestyHallBuilder");
            GenericHallBuilder genericHallBuilderToDestroy = UnityEngine.Object.Instantiate(baseInstance);
            genericHallBuilderToDestroy.name = name;
            T newGenericBuilder = genericHallBuilderToDestroy.gameObject.AddComponent<T>();
            GameObject.Destroy(genericHallBuilderToDestroy);
            if (isForced)
            {
                ObjectsStorage.ForcedSpecialObjectBuilders.Add(name, newGenericBuilder);
            } else
            {
                ObjectsStorage.WeightedObjectBuilders.Add(name, new WeightedObjectBuilder()
                {
                    selection = newGenericBuilder,
                    weight = weight
                });
            }
            
            ObjectPlacer objPlacer = ReflectionHelper.getValue<ObjectPlacer>(genericHallBuilderToDestroy, "objectPlacer");
            ReflectionHelper.setValue<int>(objPlacer, "min", min);
            ReflectionHelper.setValue<int>(objPlacer, "max", max);
            ReflectionHelper.setValue<GameObject>(objPlacer, "prefab", prefab);
            ReflectionHelper.setValue<ObjectPlacer>(newGenericBuilder, "objectPlacer", objPlacer);
            newGenericBuilder.gameObject.ConvertToPrefab(true);

            BaseSpawningData spawningData = new BaseSpawningData(name);
            ObjectsStorage.SpawningData.Add(name, spawningData);
            return spawningData;
        }

        public static void createOverlay(string name, Sprite sprite, bool setActive)
        {
            GameObject overlay = GameObject.Instantiate(AssetsHelper.loadAsset<GameObject>("GumOverlay"));
            overlay.name = name;
            Image image = overlay.GetComponentInChildren<Image>();
            image.sprite = sprite;
            overlay.ConvertToPrefab(setActive);

            OverlayEffectsManager effectsManager = overlay.AddComponent<OverlayEffectsManager>();
            effectsManager.initializePrefab(image);

            ObjectsStorage.Overlays.Add(name, overlay.GetComponent<Canvas>());
        }

        public static void createPlate<T>(string name) where T : BasePlate
        {
            GameObject plateObj = new GameObject(name);
            T plate = plateObj.AddComponent<T>();
            plate.initializePrefab();

            plateObj.ConvertToPrefab(true);
            ObjectsStorage.GameButtons.Add(name, plate);
        }

        public static RoomAsset createRoomAsset(string name, int weight)
        {
            foreach (StandardDoorMats doorMats in ScriptableObject.FindObjectsOfType<StandardDoorMats>())
            {
                Debug.Log("Door mats name: " + doorMats.name);
            }

            RoomAsset roomAsset = ScriptableObject.CreateInstance<RoomAsset>();
            ScriptableObject _room = roomAsset;
            _room.name = name;
            roomAsset.name = name;
            roomAsset.category = RoomCategory.Class;
            roomAsset.type = RoomType.Room;
            roomAsset.mapMaterial = AssetsHelper.loadAsset<Material>("MapTile_Classroom");//MapTile_Classroom
            roomAsset.florTex = AssetsHelper.loadAsset<Texture2D>("Placeholder_Floor");
            roomAsset.wallTex = AssetsHelper.loadAsset<Texture2D>("Placeholder_Wall_W");
            roomAsset.ceilTex = AssetsHelper.loadAsset<Texture2D>("Placeholder_Celing");
            //ClassStandard_Open
            //ClassStandard_Closed
            //roomAsset.doorMats = Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(), x => x.name == "ClassDoorSet");
            roomAsset.doorMats = ScriptableObject.CreateInstance<StandardDoorMats>();
            roomAsset.doorMats.open = AssetsHelper.loadAsset<Material>("ClassStandard_Open");
            roomAsset.doorMats.shut = AssetsHelper.loadAsset<Material>("ClassStandard_Closed");
            roomAsset.roomFunctionContainer = null;
            ObjectsStorage.WeightedRoomAssets.Add(new WeightedRoomAsset()
            {
                selection = roomAsset,
                weight = weight
            });
            return roomAsset;
        }
    }
}
