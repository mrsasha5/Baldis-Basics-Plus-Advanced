using BaldisBasicsPlusAdvanced.Cache;
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
using BaldisBasicsPlusAdvanced.Game.Objects.Triggers;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Overlay;
using BaldisBasicsPlusAdvanced.Game.Spawning;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class PrefabsCreator
    {
        private const string editorPath = "Textures/Compats/LevelEditor/";

        public static T CreateEntity<T>(EntityBuilder builder, int variant = 1) where T : MonoBehaviour, IEntityPrefab
        {
            Entity entity = builder.Build();

            T prefabComponent = entity.gameObject.AddComponent<T>();
            prefabComponent.InitializePrefab(entity, variant);

            ObjectsStorage.Entities.Add(entity.name, entity);

            return prefabComponent;
        }

        public static NPCSpawningData CreateNpc<T>(NPCBuilder<T> builder, int variant = 1) where T : NPC
        {
            T npc = builder.Build();

            if (npc is IPrefab)
            {
                ((IPrefab)npc).InitializePrefab(variant);
            }

            ObjectsStorage.Npcs.Add(npc.Character.ToStringExtended(), npc);
            ObjectsStorage.SpawningData.Add("npc_" + npc.Character.ToStringExtended(), new NPCSpawningData(npc.Character.ToStringExtended(), npc));

            return (NPCSpawningData)ObjectsStorage.SpawningData["npc_" + npc.Character.ToStringExtended()];
        }

        public static ItemSpawningData CreateItem<I>(string nameKey, string descKey, string enumName, 
            string smallSpriteFileName, string largeSpriteFileName, int generatorCost, int price,
            ItemFlags flags = ItemFlags.None, string[] tags = null, ItemMetaData itemMeta = null, int variant = 1) where I : Item
        {
            Sprite smallSprite = AssetsHelper.SpriteFromFile("Textures/Items/SmallSprites/" + smallSpriteFileName);
            Sprite largeSprite = AssetsHelper.SpriteFromFile("Textures/Items/LargeSprites/" + largeSpriteFileName, 50f);

            if (tags == null) tags = new string[0];

            bool isGenericItem = !flags.HasFlag(ItemFlags.MultipleUse) || (itemMeta == null && flags.HasFlag(ItemFlags.MultipleUse));

            ItemBuilder itemBuilder = new ItemBuilder(AdvancedCore.Instance.Info)
                .SetNameAndDescription(nameKey, descKey)
                .SetSprites(smallSprite, largeSprite)
                .SetGeneratorCost(generatorCost)
                .SetShopPrice(price)
                .SetItemComponent<I>();

            Items @enum = Items.None;
            
            try //for what? Just to avoid logs about enum duplicates.
            {
                @enum = EnumExtensions.GetFromExtendedName<Items>(enumName);
            } catch
            {
                @enum = EnumExtensions.ExtendEnum<Items>(enumName);
            }
            
            itemBuilder.SetEnum(@enum);

            if (itemMeta == null) itemBuilder.SetMeta(flags, tags);
            else itemBuilder.SetMeta(itemMeta);

            ItemObject itemObject = itemBuilder.Build();

            I item = (I)itemObject.item;

            if (item is IPrefab)
            {
                ((IPrefab)item).InitializePrefab(variant);
            }

            ItemSpawningData spawnData = new ItemSpawningData(enumName, itemObject);

            if (isGenericItem)
            {
                ObjectsStorage.ItemObjects.Add(enumName, itemObject);
                ObjectsStorage.SpawningData.Add("item_" + enumName, spawnData);

                if (IntegrationManager.IsActive<LevelEditorIntegration>())
                {
                    ObjectsStorage.EditorSprites.Add("item_" + enumName, AssetsHelper.SpriteFromFile(editorPath + "Items/adv_editor_" + enumName + ".png"));
                }
            }
            
            //Singleton<PlayerFileManager>.Instance.itemObjects.Add(itemObject); //needed for serialization without api saves
            //But with API it isn't needed 

            return spawnData;
        }

        public static EventSpawningData CreateEvent<T>(string name, string soundKey, string enumName, float minTime, float maxTime,
            RandomEventFlags flags, int variant = 1) where T : RandomEvent
        {
            T randomEvent = new RandomEventBuilder<T>(AdvancedCore.Instance.Info)
                .SetName(name)
                .SetSound(AssetsStorage.sounds[soundKey])
                .SetEnum(enumName)
                .SetMinMaxTime(minTime, maxTime)
                .SetMeta(flags)
                .Build();

            if (randomEvent is IPrefab) ((IPrefab)randomEvent).InitializePrefab(variant);

            ObjectsStorage.Events.Add(enumName, randomEvent);

            EventSpawningData spawningData = new EventSpawningData(enumName, randomEvent);
            ObjectsStorage.SpawningData.Add("random_event_" + enumName, spawningData);
            return spawningData;

        }

        public static StructureBuilderExtensionsSpawningData CreateVendingMachine(
            string name, ItemObject requiredItem, Material face, Material faceOut, ItemObject item, int weight,
            WeightedItemObject[] potentialItems = null)
        {
            SodaMachine component = UnityEngine.Object.Instantiate(AssetsHelper.LoadAsset<SodaMachine>("ZestyMachine"));
            component.gameObject.name = name;
            component.gameObject.ConvertToPrefab(true);
            BaseSodaMachine sodaMachine = component.gameObject.AddComponent<BaseSodaMachine>();
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

            ObjectsStorage.SodaMachines.Add(name, sodaMachine);

            if (IntegrationManager.IsActive<LevelEditorIntegration>())
            {
                ObjectsStorage.EditorSprites.Add("vending_" + name, AssetsHelper.SpriteFromFile(editorPath + "Objects/adv_editor_" + name + ".png"));
            }

            StructureBuilderExtensionsSpawningData spawningData = new StructureBuilderExtensionsSpawningData("structure_patch_"
                + AssetsStorage.weightedPlacer.name + sodaMachine.name, AssetsStorage.weightedPlacer);

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

            ObjectsStorage.SpawningData.Add("structure_patch_"
                + AssetsStorage.weightedPlacer.name + sodaMachine.name, spawningData);

            return spawningData;
        }

        public static StructureBuilderExtensionsSpawningData CreateMultipleRequiredVendingMachine(
            string name, ItemObject requiredItem, int requitedAmmount, Material face, Material faceOut, 
            ItemObject item, int weight, WeightedItemObject[] potentialItems = null)
        {
            SodaMachine component = UnityEngine.Object.Instantiate(AssetsHelper.LoadAsset<SodaMachine>("ZestyMachine"));
            component.gameObject.name = name;
            component.gameObject.ConvertToPrefab(true);
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

            
            ObjectsStorage.SodaMachines.Add(name, sodaMachine);

            if (IntegrationManager.IsActive<LevelEditorIntegration>())
            {
                ObjectsStorage.EditorSprites.Add("vending_" + name, AssetsHelper.SpriteFromFile(editorPath + "Objects/adv_editor_" + name + ".png"));
            }

            StructureBuilderExtensionsSpawningData spawningData = new StructureBuilderExtensionsSpawningData("structure_patch_"
                + AssetsStorage.weightedPlacer.name + sodaMachine.name, AssetsStorage.weightedPlacer);

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

            ObjectsStorage.SpawningData.Add("structure_patch_"
                + AssetsStorage.weightedPlacer.name + sodaMachine.name, spawningData);

            return spawningData;
        }

        public static StructureBuilderSpawningData CreateStructureBuilder<T>(string name, int variant = 1) where T : StructureBuilder
        {
            GameObject gm = new GameObject(name);
            gm.ConvertToPrefab(true);

            T builder = gm.AddComponent<T>();
            if (builder is IPrefab) ((IPrefab)builder).InitializePrefab(variant);

            ObjectsStorage.StructureBuilders.Add(name, builder);

            StructureBuilderSpawningData spawningData = new StructureBuilderSpawningData(name, builder);

            ObjectsStorage.SpawningData.Add("builder_" + name, spawningData);

            return spawningData;
        }

        public static GameObject CreateOverlay(string name, Sprite sprite, bool setActive)
        {
            GameObject overlay = GameObject.Instantiate(AssetsHelper.LoadAsset<GameObject>("GumOverlay"));
            overlay.ConvertToPrefab(setActive);

            overlay.name = name;
            Image image = overlay.GetComponentInChildren<Image>();
            image.sprite = sprite;

            OverlayEffectsManager effectsManager = overlay.AddComponent<OverlayEffectsManager>();
            effectsManager.InitializePrefab(image);

            ObjectsStorage.Overlays.Add(name, overlay.GetComponent<Canvas>());

            return overlay;
        }

        public static T CreatePlate<T>(string name, int variant = 1, bool putInMemory = true) where T : BasePlate
        {
            GameObject plateObj = new GameObject(name);
            plateObj.ConvertToPrefab(true);

            T plate = plateObj.AddComponent<T>();
            plate.InitializePrefab(variant);

            if (putInMemory) ObjectsStorage.Objects.Add(name, plate.gameObject);

            return plate;
        }

        public static T CreateTrigger<T>(string name, int variant = 1) where T : BaseTrigger
        {
            GameObject triggerObj = new GameObject(name);
            triggerObj.ConvertToPrefab(true);

            T trigger = triggerObj.AddComponent<T>();
            trigger.InitializePrefab(variant);
            
            ObjectsStorage.Triggers.Add(name, trigger);

            return trigger;
        }

        public static T CreateObjectPrefab<T>(string name, string keyName, int variant = 1) where T : MonoBehaviour, IPrefab
        {
            GameObject obj = new GameObject(name);
            obj.ConvertToPrefab(true);

            T prefabComponent = obj.AddComponent<T>();
            prefabComponent.InitializePrefab(variant);
            ObjectsStorage.Objects.Add(keyName, obj);
            return prefabComponent;
        }

        public static void CreateFunctionContainerWithRoomFunction<T>(string name, int variant = 1) where T : RoomFunction, new()
        {
            //RoomFunctionContainer container = GameObject.Instantiate(AssetsHelper.loadAsset<RoomFunctionContainer>("NoFunction"));
            RoomFunctionContainer container = new GameObject(name).AddComponent<RoomFunctionContainer>();
            T func = RoomHelper.SetupRoomFunction<T>(container);

            if (func is IPrefab) ((IPrefab)func).InitializePrefab(variant);

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

        public static void CreateDoorMatSet(string name, Material openMat, Material closedMat)
        {
            StandardDoorMats engDoorMats = ScriptableObject.CreateInstance<StandardDoorMats>();
            engDoorMats.name = name;
            engDoorMats.open = openMat;
            engDoorMats.shut = closedMat;
        }
    }
}
