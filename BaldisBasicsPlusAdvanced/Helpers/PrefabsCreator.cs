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
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Game.Objects.Triggers;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay;
using BaldisBasicsPlusAdvanced.Game.Objects;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class PrefabsCreator
    {
        private const string editorPath = "Textures/Compats/LevelEditor/";

        public static ItemSpawningData CreateItem<I>(string nameKey, string descKey, string enumName, int weight, 
            string smallSpriteFileName, string largeSpriteFileName, int generatorCost, int price,
            ItemFlags flags = ItemFlags.None, string[] tags = null, ItemMetaData itemMeta = null) where I : Item
        {
            Sprite smallSprite = AssetsHelper.SpriteFromFile("Textures/Items/SmallSprites/" + smallSpriteFileName);
            Sprite largeSprite = AssetsHelper.SpriteFromFile("Textures/Items/LargeSprites/" + largeSpriteFileName, 50f);

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
                ((IPrefab)item).InitializePrefab();
            }

            ObjectsStorage.ItemsObjects.Add(enumName, itemObject);
            ItemSpawningData spawnData = new ItemSpawningData(enumName, itemObject);
            ObjectsStorage.SpawningData.Add("item_" + enumName, spawnData);
            
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
                ObjectsStorage.EditorSprites.Add("item_" + enumName, AssetsHelper.SpriteFromFile(editorPath + "Items/adv_editor_" + enumName + ".png"));
            }

            return spawnData;
        }

        public static BaseSpawningData CreateEvent<T>(string name, string soundKey, string enumName, int weight, float minTime, float maxTime,
            RandomEventFlags flags) where T : RandomEvent
        {
            T randomEvent = new RandomEventBuilder<T>(AdvancedCore.Instance.Info)
                .SetName(name)
                .SetSound(AssetsStorage.sounds[soundKey])
                .SetEnum(enumName)
                .SetMinMaxTime(minTime, maxTime)
                .SetMeta(flags)
                .Build();

            if (randomEvent is IPrefab) ((IPrefab)randomEvent).InitializePrefab();

            WeightedRandomEvent weighted = new WeightedRandomEvent()
            {
                selection = randomEvent,
                weight = weight
            };
            ObjectsStorage.WeightedEvents.Add(enumName, weighted);
            ObjectsStorage.Events.Add(enumName, randomEvent);

            BaseSpawningData spawningData = new BaseSpawningData(enumName);
            ObjectsStorage.SpawningData.Add("random_event_" + enumName, spawningData);
            return spawningData;

        }

        public static SodaMachineBase CreateVendingMachine(string name, ItemObject requiredItem, Material face, Material faceOut, ItemObject item, WeightedItemObject[] potentialItems = null)
        {
            SodaMachine component = UnityEngine.Object.Instantiate(AssetsHelper.LoadAsset<SodaMachine>("ZestyMachine"));
            component.gameObject.name = name;
            SodaMachineBase sodaMachine = component.gameObject.AddComponent<SodaMachineBase>();
            GameObject.Destroy(component);

            MeshRenderer meshRenderer = sodaMachine.GetComponent<MeshRenderer>();
            Material[] goodMachineMaterials = meshRenderer.materials;
            goodMachineMaterials[1] = face;
            meshRenderer.materials = goodMachineMaterials;

            ReflectionHelper.SetValue<ItemObject>(sodaMachine, "requiredItem", requiredItem);
            ReflectionHelper.SetValue<ItemObject>(sodaMachine, "item", item);
            ReflectionHelper.SetValue<MeshRenderer>(sodaMachine, "meshRenderer", meshRenderer);
            ReflectionHelper.SetValue<Material>(sodaMachine, "outOfStockMat", faceOut);

            if (potentialItems != null) ReflectionHelper.SetValue<WeightedItemObject[]>(sodaMachine, "potentialItems", potentialItems);

            sodaMachine.gameObject.ConvertToPrefab(true);
            ObjectsStorage.SodaMachines.Add(name, sodaMachine);

            if (ModsIntegration.LevelEditorInstalled)
            {
                ObjectsStorage.EditorSprites.Add("vending_" + name, AssetsHelper.SpriteFromFile(editorPath + "Objects/adv_editor_" + name + ".png"));
            }

            return sodaMachine;
        }

        public static MultipleRequiredSodaMachine CreateMultipleRequiredVendingMachine(string name, ItemObject requiredItem, int requitedAmmount, Material face, Material faceOut, ItemObject item, WeightedItemObject[] potentialItems = null)
        {
            SodaMachine component = UnityEngine.Object.Instantiate(AssetsHelper.LoadAsset<SodaMachine>("ZestyMachine"));
            component.gameObject.name = name;
            MultipleRequiredSodaMachine sodaMachine = component.gameObject.AddComponent<MultipleRequiredSodaMachine>();
            GameObject.Destroy(component);

            MeshRenderer meshRenderer = sodaMachine.GetComponent<MeshRenderer>();
            Material[] goodMachineMaterials = meshRenderer.materials;
            goodMachineMaterials[1] = face;
            meshRenderer.materials = goodMachineMaterials;

            ReflectionHelper.SetValue<ItemObject>(sodaMachine, "requiredItem", requiredItem);
            sodaMachine.requiredAmmount = requitedAmmount;
            ReflectionHelper.SetValue<ItemObject>(sodaMachine, "item", item);
            ReflectionHelper.SetValue<MeshRenderer>(sodaMachine, "meshRenderer", meshRenderer);
            ReflectionHelper.SetValue<Material>(sodaMachine, "outOfStockMat", faceOut);

            if (potentialItems != null) ReflectionHelper.SetValue<WeightedItemObject[]>(sodaMachine, "potentialItems", potentialItems);

            sodaMachine.gameObject.ConvertToPrefab(true);
            ObjectsStorage.SodaMachines.Add(name, sodaMachine);

            if (ModsIntegration.LevelEditorInstalled)
            {
                ObjectsStorage.EditorSprites.Add("vending_" + name, AssetsHelper.SpriteFromFile(editorPath + "Objects/adv_editor_" + name + ".png"));
            }

            return sodaMachine;
        }

        public static BaseSpawningData CreateWeigthedHallObjectsBuilder<T>(string name, int weight, bool isForced = false) where T : ObjectBuilder
        {
            GenericHallBuilder baseInstance = AssetsHelper.LoadAsset<GenericHallBuilder>("ZestyHallBuilder");
            GenericHallBuilder genericHallBuilderToDestroy = UnityEngine.Object.Instantiate(baseInstance);
            genericHallBuilderToDestroy.name = name;
            T newBuilder = genericHallBuilderToDestroy.gameObject.AddComponent<T>();
            GameObject.Destroy(genericHallBuilderToDestroy);
            if (isForced)
            {
                ObjectsStorage.ForcedSpecialObjectBuilders.Add(name, newBuilder);
            }
            else
            {
                ObjectsStorage.WeightedObjectBuilders.Add(name, new WeightedObjectBuilder()
                {
                    selection = newBuilder,
                    weight = weight
                });
            }

            if (newBuilder is IPrefab)
            {
                ((IPrefab)newBuilder).InitializePrefab();
            }

            newBuilder.gameObject.ConvertToPrefab(true);

            BaseSpawningData spawningData = new BaseSpawningData(name);
            ObjectsStorage.SpawningData.Add("builder_" + name, spawningData);
            return spawningData;
        }

        public static BaseSpawningData CreateWeigthedHallObjectsBuilder<T>(string name, int weight, int min, int max, GameObject prefab, bool isForced = false) where T : GenericHallBuilder
        {
            GenericHallBuilder baseInstance = AssetsHelper.LoadAsset<GenericHallBuilder>("ZestyHallBuilder");
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

            ObjectPlacer objPlacer = ReflectionHelper.GetValue<ObjectPlacer>(genericHallBuilderToDestroy, "objectPlacer");

            ReflectionHelper.SetValue<int>(objPlacer, "min", min);
            ReflectionHelper.SetValue<int>(objPlacer, "max", max);
            ReflectionHelper.SetValue<GameObject>(objPlacer, "prefab", prefab);

            ReflectionHelper.SetValue<ObjectPlacer>(newGenericBuilder, "objectPlacer", objPlacer);

            if (newGenericBuilder is IPrefab)
            {
                ((IPrefab)newGenericBuilder).InitializePrefab();
            }

            newGenericBuilder.gameObject.ConvertToPrefab(true);

            BaseSpawningData spawningData = new BaseSpawningData(name);
            ObjectsStorage.SpawningData.Add("builder_" + name, spawningData);
            return spawningData;
        }

        public static void CreateOverlay(string name, Sprite sprite, bool setActive)
        {
            GameObject overlay = GameObject.Instantiate(AssetsHelper.LoadAsset<GameObject>("GumOverlay"));
            overlay.name = name;
            Image image = overlay.GetComponentInChildren<Image>();
            image.sprite = sprite;
            overlay.ConvertToPrefab(setActive);

            OverlayEffectsManager effectsManager = overlay.AddComponent<OverlayEffectsManager>();
            effectsManager.InitializePrefab(image);

            ObjectsStorage.Overlays.Add(name, overlay.GetComponent<Canvas>());
        }

        public static void CreatePlate<T>(string name) where T : PlateBase
        {
            GameObject plateObj = new GameObject(name);
            T plate = plateObj.AddComponent<T>();
            plate.InitializePrefab();

            plateObj.ConvertToPrefab(true);
            ObjectsStorage.GameButtons.Add(name, plate);
        }

        public static void CreateTrigger<T>(string name) where T : TriggerBase
        {
            GameObject triggerObj = new GameObject(name);
            T trigger = triggerObj.AddComponent<T>();
            trigger.InitializePrefab();

            triggerObj.ConvertToPrefab(true);
            ObjectsStorage.Triggers.Add(name, trigger);
        }

        public static void CreateObjectPrefab<T>(string name, string keyName) where T : MonoBehaviour, IPrefab
        {
            GameObject obj = new GameObject(name);
            T prefabComponent = obj.AddComponent<T>();
            prefabComponent.InitializePrefab();
            obj.ConvertToPrefab(true);
            ObjectsStorage.Objects.Add(keyName, obj);
        }

        public static void CreateFunctionContainerWithRoomFunction<T>(string name) where T : RoomFunction, new()
        {
            //RoomFunctionContainer container = GameObject.Instantiate(AssetsHelper.loadAsset<RoomFunctionContainer>("NoFunction"));
            RoomFunctionContainer container = new GameObject(name).AddComponent<RoomFunctionContainer>();
            RoomHelper.SetupRoomFunction<T>(container);
            container.gameObject.ConvertToPrefab(true);
            ObjectsStorage.RoomFunctionsContainers.Add(name, container);
        }

        public static RoomGroupSpawningData CreateRoomGroup(string name, int minRooms, int maxRooms)
        {
            EnumExtensions.ExtendEnum<RoomCategory>(name);
            ObjectsStorage.RoomGroups.Add(name,
                new RoomGroup()
                {
                    name = name,
                    minRooms = minRooms,
                    maxRooms = maxRooms,
                    ceilingTexture = new WeightedTexture2D[] {
                        new WeightedTexture2D()
                        {
                            selection = AssetsStorage.textures["regular_ceiling"],
                            weight = 100
                        }
                    },
                    wallTexture = new WeightedTexture2D[] {
                        new WeightedTexture2D()
                        {
                            selection = AssetsStorage.textures["regular_wall"],
                            weight = 100
                        }
                    },
                    floorTexture = new WeightedTexture2D[] {
                        new WeightedTexture2D()
                        {
                            selection = AssetsStorage.textures["carpet"],
                            weight = 100
                        }
                    },
                    light = new WeightedTransform[]
                    {
                        new WeightedTransform()
                        {
                            selection = AssetsHelper.LoadAsset<Transform>("HangingLight"),
                            weight = 100
                        }
                    }
                });

            RoomGroupSpawningData spawnData = new RoomGroupSpawningData(name, ObjectsStorage.RoomGroups.Values.Last());
            ObjectsStorage.SpawningData.Add("room_group_" + name, spawnData);

            return spawnData;
        }
    }
}
