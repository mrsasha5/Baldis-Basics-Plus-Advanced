using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI;
using UnityEngine;
using System.Linq;
using MTM101BaldAPI.ObjectCreation;
using UnityEngine.UI;
using BaldisBasicsPlusAdvanced.Game.Objects.SodaMachines;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Objects.Triggers;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay;
using BaldisBasicsPlusAdvanced.Game.Spawning;
namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class PrefabCreator
    {
        public static T CreateEntity<T>(EntityBuilder builder, int variant = 1) where T : MonoBehaviour, IEntityPrefab
        {
            Entity entity = builder.Build();

            T prefabComponent = entity.gameObject.AddComponent<T>();
            prefabComponent.InitializePrefab(entity, variant);

            ObjectStorage.Entities.Add(entity.name, entity);

            return prefabComponent;
        }

        public static T CreateNpc<T>(NPCBuilder<T> builder, int variant = 1) where T : NPC
        {
            T npc = builder.Build();

            if (npc is IPrefab)
            {
                ((IPrefab)npc).InitializePrefab(variant);
            }

            ObjectStorage.Npcs.Add(npc.Character.ToStringExtended(), npc);

            return npc;
        }

        public static ItemObject CreateItem<T>(string nameKey, string descKey, string enumName, 
            string smallSpriteFileName, string largeSpriteFileName, int generatorCost, int price,
            ItemFlags flags = ItemFlags.None, string[] tags = null, ItemMetaData itemMeta = null, int variant = 1) where T : Item
        {
            Sprite smallSprite = AssetHelper.SpriteFromFile("Textures/Items/SmallSprites/" + smallSpriteFileName);
            Sprite largeSprite = AssetHelper.SpriteFromFile("Textures/Items/LargeSprites/" + largeSpriteFileName, 50f);

            if (tags == null) tags = new string[0];

            bool isGenericItem = !flags.HasFlag(ItemFlags.MultipleUse) || (itemMeta == null && flags.HasFlag(ItemFlags.MultipleUse));

            ItemBuilder itemBuilder = new ItemBuilder(AdvancedCore.Instance.Info)
                .SetNameAndDescription(nameKey, descKey)
                .SetEnum(enumName)
                .SetSprites(smallSprite, largeSprite)
                .SetGeneratorCost(generatorCost)
                .SetShopPrice(price)
                .SetItemComponent<T>();

            if (itemMeta == null) itemBuilder.SetMeta(flags, tags);
            else itemBuilder.SetMeta(itemMeta);

            ItemObject itemObject = itemBuilder.Build();

            T item = (T)itemObject.item;

            if (item is IPrefab)
            {
                ((IPrefab)item).InitializePrefab(variant);
            }

            if (isGenericItem)
            {
                ObjectStorage.ItemObjects.Add(enumName, itemObject);
            }

            return itemObject;
        }

        public static T CreateEvent<T>(string name, string soundKey, string enumName, float minTime, float maxTime,
            RandomEventFlags flags, int variant = 1) where T : RandomEvent
        {
            T randomEvent = new RandomEventBuilder<T>(AdvancedCore.Instance.Info)
                .SetName(name)
                .SetSound(AssetStorage.sounds[soundKey])
                .SetEnum(enumName)
                .SetMinMaxTime(minTime, maxTime)
                .SetMeta(flags)
                .Build();

            if (randomEvent is IPrefab) ((IPrefab)randomEvent).InitializePrefab(variant);

            ObjectStorage.Events.Add(enumName, randomEvent);
            return randomEvent;

        }

        public static StructureBuilderExtensionsSpawningData CreateVendingMachine(
            string name, ItemObject requiredItem, Material face, Material faceOut, ItemObject item, int weight,
            WeightedItemObject[] potentialItems = null)
        {
            SodaMachine sodaMachine = UnityEngine.Object.Instantiate(AssetHelper.LoadAsset<SodaMachine>("ZestyMachine"));
            sodaMachine.gameObject.name = name;
            sodaMachine.gameObject.ConvertToPrefab(true);

            MeshRenderer meshRenderer = sodaMachine.GetComponent<MeshRenderer>();
            Material[] goodMachineMaterials = meshRenderer.materials;
            goodMachineMaterials[1] = face;
            meshRenderer.materials = goodMachineMaterials;

            ReflectionHelper.SetValue<ItemObject>(sodaMachine, "requiredItem", requiredItem);
            ReflectionHelper.SetValue<ItemObject>(sodaMachine, "item", item);
            ReflectionHelper.SetValue<MeshRenderer>(sodaMachine, "meshRenderer", meshRenderer);
            ReflectionHelper.SetValue<Material>(sodaMachine, "outOfStockMat", faceOut);

            if (potentialItems != null) ReflectionHelper.SetValue<WeightedItemObject[]>(sodaMachine, "potentialItems", potentialItems);

            ObjectStorage.SodaMachines.Add(name, sodaMachine);

            StructureBuilderExtensionsSpawningData spawningData = new StructureBuilderExtensionsSpawningData("structure_patch_"
                + AssetStorage.weightedPlacer.name + sodaMachine.name, AssetStorage.weightedPlacer);

            spawningData.SetStructureParameters(2, new StructureParameters()
            {
                prefab = new WeightedGameObject[]
                {
                    new WeightedGameObject()
                    {
                        selection = sodaMachine.gameObject,
                        weight = weight
                    }
                }
            });

            ObjectStorage.SpawningData.Add("structure_patch_"
                + AssetStorage.weightedPlacer.name + sodaMachine.name, spawningData);

            return spawningData;
        }

        public static StructureBuilderExtensionsSpawningData CreateMultipleRequiredVendingMachine(
            string name, ItemObject requiredItem, int requitedAmmount, Material face, Material faceOut, 
            ItemObject item, int weight, WeightedItemObject[] potentialItems = null)
        {
            MultipleRequiredItemsSodaMachine sodaMachine = UnityEngine.Object.Instantiate(AssetHelper.LoadAsset<SodaMachine>("ZestyMachine"))
                .gameObject.SwapComponent<SodaMachine, MultipleRequiredItemsSodaMachine>();
            sodaMachine.gameObject.name = name;
            sodaMachine.gameObject.ConvertToPrefab(true);

            MeshRenderer meshRenderer = sodaMachine.GetComponent<MeshRenderer>();
            Material[] goodMachineMaterials = meshRenderer.materials;
            goodMachineMaterials[1] = face;
            meshRenderer.materials = goodMachineMaterials;

            ReflectionHelper.SetValue<ItemObject>(sodaMachine, "requiredItem", requiredItem);
            sodaMachine.requiredItemsAmmount = requitedAmmount;
            ReflectionHelper.SetValue<ItemObject>(sodaMachine, "item", item);
            ReflectionHelper.SetValue<MeshRenderer>(sodaMachine, "meshRenderer", meshRenderer);
            ReflectionHelper.SetValue<Material>(sodaMachine, "outOfStockMat", faceOut);

            if (potentialItems != null) ReflectionHelper.SetValue<WeightedItemObject[]>(sodaMachine, "potentialItems", potentialItems);
            
            ObjectStorage.SodaMachines.Add(name, sodaMachine);

            StructureBuilderExtensionsSpawningData spawningData = new StructureBuilderExtensionsSpawningData("structure_patch_"
                + AssetStorage.weightedPlacer.name + sodaMachine.name, AssetStorage.weightedPlacer);

            spawningData.SetStructureParameters(2, new StructureParameters()
            {
                prefab = new WeightedGameObject[]
                {
                    new WeightedGameObject()
                    {
                        selection = sodaMachine.gameObject,
                        weight = weight
                    }
                }
            });

            ObjectStorage.SpawningData.Add("structure_patch_"
                + AssetStorage.weightedPlacer.name + sodaMachine.name, spawningData);

            return spawningData;
        }

        public static T CreateStructureBuilder<T>(string name, int variant = 1) where T : StructureBuilder
        {
            GameObject gm = new GameObject(name);
            gm.ConvertToPrefab(true);

            T builder = gm.AddComponent<T>();
            if (builder is IPrefab) ((IPrefab)builder).InitializePrefab(variant);

            ObjectStorage.StructureBuilders.Add(name, builder);

            return builder;
        }

        public static GameObject CreateOverlay(string name, Sprite sprite, bool setActive)
        {
            GameObject overlay = GameObject.Instantiate(AssetHelper.LoadAsset<GameObject>("GumOverlay"));
            overlay.ConvertToPrefab(setActive);

            overlay.name = name;
            Image image = overlay.GetComponentInChildren<Image>();
            image.sprite = sprite;

            OverlayEffectsManager effectsManager = overlay.AddComponent<OverlayEffectsManager>();
            effectsManager.InitializePrefab(image);

            ObjectStorage.Overlays.Add(name, overlay.GetComponent<Canvas>());

            return overlay;
        }

        public static T CreatePlate<T>(string name, int variant = 1, bool putInMemory = true) where T : BasePlate
        {
            GameObject plateObj = new GameObject(name);
            plateObj.ConvertToPrefab(true);

            T plate = plateObj.AddComponent<T>();
            plate.InitializePrefab(variant);

            if (putInMemory)
            {
                ObjectStorage.Objects.Add(name, plate.gameObject);
            }

            return plate;
        }

        public static T CreateTrigger<T>(string name, int variant = 1) where T : BaseTrigger
        {
            GameObject triggerObj = new GameObject(name);
            triggerObj.ConvertToPrefab(true);

            T trigger = triggerObj.AddComponent<T>();
            trigger.InitializePrefab(variant);
            
            ObjectStorage.Triggers.Add(name, trigger);

            return trigger;
        }

        public static T CreateObjectPrefab<T>(string name, string keyName, int variant = 1) where T : MonoBehaviour, IPrefab
        {
            GameObject obj = new GameObject(name);
            obj.ConvertToPrefab(true);

            T prefabComponent = obj.AddComponent<T>();
            prefabComponent.InitializePrefab(variant);
            ObjectStorage.Objects.Add(keyName, obj);
            return prefabComponent;
        }

        public static void CreateFunctionContainerWithRoomFunction<T>(string name, int variant = 1) where T : RoomFunction, new()
        {
            RoomFunctionContainer container = new GameObject(name).AddComponent<RoomFunctionContainer>();
            T func = RoomHelper.SetupRoomFunction<T>(container);

            if (func is IPrefab) ((IPrefab)func).InitializePrefab(variant);

            container.gameObject.ConvertToPrefab(true);
            ObjectStorage.RoomFunctionsContainers.Add(name, container);
        }

        public static RoomGroup CreateRoomGroup(string name, int minRooms, int maxRooms, string lightName = "FluorescentLight")
        {
            RoomGroup group = new RoomGroup()
            {
                name = name,
                minRooms = minRooms,
                maxRooms = maxRooms,
                ceilingTexture = new WeightedTexture2D[] {
                        new WeightedTexture2D()
                        {
                            selection = AssetStorage.textures["regular_ceiling"],
                            weight = 100
                        }
                    },
                wallTexture = new WeightedTexture2D[] {
                        new WeightedTexture2D()
                        {
                            selection = AssetStorage.textures["regular_wall"],
                            weight = 100
                        }
                    },
                floorTexture = new WeightedTexture2D[] {
                        new WeightedTexture2D()
                        {
                            selection = AssetStorage.textures["carpet"],
                            weight = 100
                        }
                    },
                light = new WeightedTransform[]
                    {
                        new WeightedTransform()
                        {
                            selection = AssetHelper.LoadAsset<Transform>(lightName),
                            weight = 100
                        }
                    }
            };

            EnumExtensions.ExtendEnum<RoomCategory>(name);
            ObjectStorage.RoomGroups.Add(name, group);

            return group;
        }

        public static void CreateDoorMatSet(string name, Material openMat, Material closedMat)
        {
            StandardDoorMats engDoorMats = ScriptableObject.CreateInstance<StandardDoorMats>();
            engDoorMats.name = name;
            engDoorMats.open = openMat;
            engDoorMats.shut = closedMat;
        }
    }
}
