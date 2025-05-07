using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game;
using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.GameItems;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;
using UnityEngine;
using MTM101BaldAPI;
using BaldisBasicsPlusAdvanced.Game.Objects;
using MTM101BaldAPI.ObjectCreation;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Game.Builders;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using HarmonyLib;
using BaldisBasicsPlusAdvanced.SavedData;
using UnityCipher;
using BaldisBasicsPlusAdvanced.Json;
using BaldisBasicsPlusAdvanced.Game.Objects.Something;

namespace BaldisBasicsPlusAdvanced.Managers
{
    public class GameRegisterManager
    {

        public static void initializeGameItems()
        {
            PrefabsCreator.createItem<HammerItem>(
                nameKey: "ItemAdv_Hammer",
                descKey: "ItemAdv_Hammer_Desc",
                enumName: "Hammer",
                weight: 50,
                smallSpriteFileName: "adv_hammer_small.png",
                largeSpriteFileName: "adv_hammer_large.png",
                generatorCost: 75,
                price: 500,
                pixelsPerUnitLargeTexture: 100f,
                flags: ItemFlags.None,
                tags: new string[]
                {
                    "adv_repair_tool"
                }
                )
                .setSpawnsOnRooms(true)
                .setSpawnsOnFieldTrips(true)
                .setSpawnsOnParty(true)
                .setSpawnsOnMysteryRooms(true)
                .setEndless(true)
                .setBannedFloors(1);

            PrefabsCreator.createItem<WindBlowerItem>(
                nameKey: "ItemAdv_WindBlower",
                descKey: "ItemAdv_WindBlower_Desc",
                enumName: "WindBlower",
                weight: 75,
                smallSpriteFileName: "adv_wind_blower_small.png",
                largeSpriteFileName: "adv_wind_blower_large.png",
                generatorCost: 50,
                price: 300,
                pixelsPerUnitLargeTexture: 150f,
                flags: ItemFlags.None
                );
            
            PrefabsCreator.createItem<NPCTeleporterItem>(
                nameKey: "ItemAdv_MysteriousTeleporter",
                descKey: "ItemAdv_MysteriousTeleporter_Desc",
                enumName: "MysteriousTeleporter",
                weight: 20,
                smallSpriteFileName: "adv_mysterious_teleporter_small.png",
                largeSpriteFileName: "adv_mysterious_teleporter_large.png",
                generatorCost: 40,
                price: 500,
                pixelsPerUnitLargeTexture: 50f,
                flags: ItemFlags.None
                )
                .setSpawnsOnParty(true)
                .setSpawnsOnMysteryRooms(true)
                .setEndless(true)
                .setBannedFloors(1);

            PrefabsCreator.createItem<InvisibilityPotionItem>(
                nameKey: "ItemAdv_InvisibilityPotion",
                descKey: "ItemAdv_InvisibilityPotion_Desc",
                enumName: "InvisibilityPotion",
                weight: 50,
                smallSpriteFileName: "adv_invisibility_potion_small.png",
                largeSpriteFileName: "adv_invisibility_potion_large.png",
                generatorCost: 40,
                price: 500,
                pixelsPerUnitLargeTexture: 50f,
                flags: ItemFlags.None
                )
                .setSpawnsOnFieldTrips(true)
                .setSpawnsOnParty(true);

            PrefabsCreator.createItem<IceBootsItem>(
                nameKey: "ItemAdv_IceBoots",
                descKey: "ItemAdv_IceBoots_Desc",
                enumName: "IceBoots",
                weight: 75,
                smallSpriteFileName: "adv_ice_boots_small.png",
                largeSpriteFileName: "adv_ice_boots_large.png",
                generatorCost: 50,
                price: 300,
                pixelsPerUnitLargeTexture: 50f,
                flags: ItemFlags.None
                )
                .setSpawnsOnFieldTrips(true)
                .setEndless(true);

            /*PrefabsCreator.createItem<PlaceableFanItem>(
                nameKey: "ItemAdv_PlaceableFan",
                descKey: "ItemAdv_PlaceableFan_Desc",
                enumName: "PlaceableFan",
                weight: 50,
                flags: ItemFlags.None,
                smallSpriteFileName: "adv_placeable_fan_small.png",
                largeSpriteFileName: "adv_placeable_fan_large.png",
                generatorCost: 70,
                price: 750,
                pixelsPerUnitLargeTexture: 50f
                );*/
        }

        public static void initializeMultipleUsableItems()
        {

            ItemObject boxingGlove2 = PrefabsCreator.createItem<BoxingGloveItem>(
                nameKey: "ItemAdv_BoxingGlove",
                descKey: "ItemAdv_BoxingGlove_Desc",
                enumName: "BoxingGlove",
                weight: 15,
                smallSpriteFileName: "adv_boxing_glove_small.png",
                largeSpriteFileName: "adv_boxing_glove_large.png",
                generatorCost: 50,
                price: 500,
                pixelsPerUnitLargeTexture: 100f,
                flags: ItemFlags.MultipleUse
                )
                .setSpawnsOnFieldTrips(true)
                .ItemObject;

            ItemObject boxingGlove1 = PrefabsCreator.createItem<BoxingGloveItem>(
                nameKey: "ItemAdv_BoxingGlove1",
                descKey: "ItemAdv_BoxingGlove_Desc",
                enumName: "BoxingGlove1",
                weight: 15,
                smallSpriteFileName: "adv_boxing_glove_small.png",
                largeSpriteFileName: "adv_boxing_glove_large.png",
                generatorCost: 50,
                price: 500,
                pixelsPerUnitLargeTexture: 100f,
                flags: ItemFlags.MultipleUse,
                itemMeta: boxingGlove2.GetMeta()
                )
                .setFalseEverywhere()
                .convertTo<ItemSpawningData>()
                .ItemObject;

            boxingGlove2.item.convertTo<BaseMultipleUsableItem>().initialize(boxingGlove1);
            boxingGlove2.GetMeta().itemObjects = boxingGlove2.item.convertTo<BaseMultipleUsableItem>().AllVersions.AddItem(boxingGlove2).ToArray();

        }

        public static void initializeGameEvents()
        {
            PrefabsCreator.createEvent<DisappearingCharactersEvent>(
                name: "Disappearing Characters",
                enumName: "DisappearingCharacters",
                weight: 40,
                minTime: 60f,
                maxTime: 80f,
                flags: RandomEventFlags.None
                )
                .setEndless(true)
                .setBannedFloors(1, 3);

            PrefabsCreator.createEvent<ColdSchoolEvent>(
                name: "Cold School",
                enumName: "ColdSchool",
                weight: 50,
                minTime: 60f,
                maxTime: 90f,
                flags: RandomEventFlags.None
                )
                .setEndless(true)
                .setBannedFloors(1);

            PrefabsCreator.createEvent<PortalChaosEvent>(
                name: "Portal Chaos",
                enumName: "PortalChaos",
                weight: 40,
                minTime: 60f,
                maxTime: 90f,
                flags: RandomEventFlags.None
                )
                .setEndless(true)
                .setBannedFloors(1);
        }

        public static void initializeVendingMachines()
        {
            List<ItemObject> items = Singleton<PlayerFileManager>.Instance.itemObjects;

            PrefabsCreator.createMultipleRequiredVendingMachine("GoodMachine", items[12], 2,
                AssetsStorage.materials["adv_good_machine"], AssetsStorage.materials["adv_good_machine_out"], null, new WeightedItemObject[] {
                new WeightedItemObject()
                {
                    weight = 25,
                    selection = items[16] //teleporter
                },
                new WeightedItemObject()
                {
                    weight = 50,
                    selection = items[2] //apple
                },
                new WeightedItemObject()
                {
                    weight = 25,
                    selection = items[7] //grappling hook
                }
            });
        }

        public static void initializeObjectBuilders()
        {
            PrefabsCreator.createWeigthedHallObjectsBuilder<GenericHallBuilder>("GoodMachineHallsBuilder", 20, 1, 1, ObjectsStorage.SodaMachines["GoodMachine"].gameObject, false)
                .setEndless(true);
            PrefabsCreator.createWeigthedHallObjectsBuilder<GenericHallBuilder>("InvisibilityPlatesHallsBuilder", 40, 1, 2, ObjectsStorage.GameButtons["invisibility_plate"].gameObject, false)
                .setEndless(true)
                .setBannedFloors(1);
            PrefabsCreator.createWeigthedHallObjectsBuilder<GenericHallBuilder>("NoisyPlatesHallsBuilder", 30, 1, 4, ObjectsStorage.GameButtons["noisy_plate"].gameObject, false)
                .setEndless(true)
                .setBannedFloors(1, 3);
            PrefabsCreator.createWeigthedHallObjectsBuilder<GenericHallBuilder>("BullyPlatesHallsBuilder", 15, 1, 2, ObjectsStorage.GameButtons["bully_plate"].gameObject, false)
                .setEndless(true)
                .setBannedFloors(1);
            PrefabsCreator.createWeigthedHallObjectsBuilder<AccelerationPlatesBuilder>("AccelerationPlatesHallsBuilder", 50, 2, 5, ObjectsStorage.GameButtons["acceleration_plate"].gameObject, false)
                .setEndless(true)
                .setBannedFloors(1);
            PrefabsCreator.createWeigthedHallObjectsBuilder<PressurePlatesBuilder>("PlatesBuilder", 50, 1, 3, ObjectsStorage.GameButtons["plate"].gameObject, isForced: true)
                .setEndless(true)
                .setBannedFloors(1, 3);
            PrefabsCreator.createWeigthedHallObjectsBuilder<GenericHallBuilder>("PresentPlatesBuilder", 20, 1, 2, ObjectsStorage.GameButtons["present_plate"].gameObject, false)
                .setBannedFloors(1, 3);
            PrefabsCreator.createWeigthedHallObjectsBuilder<GenericHallBuilder>("SlowdownPlatesBuilder", 30, 1, 3, ObjectsStorage.GameButtons["slowdown_plate"].gameObject, false)
                .setEndless(true)
                .setBannedFloors(1);
            PrefabsCreator.createWeigthedHallObjectsBuilder<GenericHallBuilder>("SugarPlatesBuilder", 20, 1, 2, ObjectsStorage.GameButtons["sugar_addiction_plate"].gameObject, false)
                .setBannedFloors(1, 3);
        }

        public static void initializeOverlays()
        {
            PrefabsCreator.createOverlay("FrozenOverlay", AssetsStorage.sprites["adv_frozen_overlay"], false);
            PrefabsCreator.createOverlay("ElephantOverlay", AssetsStorage.sprites["adv_elephant_overlay"], true);
            PrefabsCreator.createOverlay("BullyOverlay", AssetsStorage.sprites["adv_bully_overlay"], true);
        }

        public static void initializeTipsAndWords()
        {
            
            LocalizationManager localization = Singleton<LocalizationManager>.Instance;

            string[] tipsBaseNames = new string[1];

            tipsBaseNames[0] = "Adv_Elv_Tip";

            //if (ModsIntegration.ExtraInstalled) tipsBaseNames[1] = "Adv_Elv_Tip_Extra";
            //if (ModsIntegration.CarnivalInstalled) tipsBaseNames[2] = "Adv_Elv_Tip_Carnival";
            //if (ModsIntegration.CarnellInstalled) tipsBaseNames[3] = "Adv_Elv_Tip_Carnell";

            foreach (string tipBaseName in tipsBaseNames)
            {
                if (tipBaseName == null) continue;
                int num = 1;
                string tipKey = tipBaseName + num;

                List<string> tips = new List<string>();

                while (localization.exists(tipKey))
                {
                    tips.Add(tipKey);
                    tipKey = tipBaseName + ++num;
                }

                ApiManager.AddNewTips(AdvancedCore.Instance.Info, tips.ToArray());
            }

            ApiManager.AddNewSymbolMachineWords(AdvancedCore.Instance.Info,
                "Beans",
                "Baldi",
                "boots",
                "learn",
                "brain",
                "ruler",
                "floor",
                "class",
                "flood",
                "party",
                "bsoda",
                "apple",
                "chalk",
                "shop",
                "math"
            );

        }

        public static void initializeObjects()
        {
            //Mysterious portal
            GameObject portalObj = new GameObject("Mysterious portal");
            BoxCollider portalCollider = portalObj.AddComponent<BoxCollider>();
            portalCollider.size = new Vector3(5f, 5f, 5f);
            portalCollider.isTrigger = true;
            GameObject rendererBase = ObjectsCreator.createSpriteRendererBillboard(AssetsStorage.sprites["adv_portal"]);
            rendererBase.transform.parent = portalObj.transform;
            rendererBase.transform.localPosition = Vector3.zero;
            SpriteRenderer renderer = rendererBase.GetComponentInChildren<SpriteRenderer>();
            MysteriousPortal portal = portalObj.AddComponent<MysteriousPortal>();
            portal.initialize(renderer);
            portalObj.ConvertToPrefab(true);
            
            ObjectsStorage.Objects.Add("mysterious_portal", portalObj);
            //portal end

            //plates

            PrefabsCreator.createPlate<PressurePlate>("plate");
            PrefabsCreator.createPlate<InvisibilityPlate>("invisibility_plate");
            PrefabsCreator.createPlate<AccelerationPlate>("acceleration_plate");
            PrefabsCreator.createPlate<NoisyPlate>("noisy_plate");
            PrefabsCreator.createPlate<StealingPlate>("stealing_plate");
            PrefabsCreator.createPlate<BullyPlate>("bully_plate");
            PrefabsCreator.createPlate<PresentPlate>("present_plate");
            PrefabsCreator.createPlate<SlowdownPlate>("slowdown_plate");
            PrefabsCreator.createPlate<SugarPlate>("sugar_addiction_plate");
            //plates end

            MathMachine mathMachineComp = GameObject.Instantiate(AssetsHelper.loadAsset<MathMachine>("MathMachine"));
            GameObject symbolMachineObj = mathMachineComp.gameObject;
            symbolMachineObj.name = "SymbolMachine";
            GameObject.Destroy(mathMachineComp);
            SymbolMachine symbolMachine = symbolMachineObj.AddComponent<SymbolMachine>();
            //symbolMachine.initializePrefabPre(ReflectionHelper.getValue<MeshRenderer>(mathMachineComp, "meshRenderer"));
            symbolMachine.initializePrefab();
            symbolMachineObj.ConvertToPrefab(true);
            ObjectsStorage.Objects.Add("symbol_machine", symbolMachineObj);

            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            
            for (int i = 0; i < alphabet.Length; i++)
            {
                string symbol = alphabet.ElementAt(i).ToString();
                MathMachineNumber mathNumComp = GameObject.Instantiate(AssetsHelper.loadAsset<MathMachineNumber>("MathNum_0"));
                GameObject.Destroy(mathNumComp);
                Spelloon spelloon = mathNumComp.gameObject.AddComponent<Spelloon>();
                spelloon.name = "Spelloon_" + symbol;
                spelloon.initializePrefab();
                spelloon.initializePrefabPost(symbol);
                alphabet.ElementAt(1).ToString();
                spelloon.gameObject.ConvertToPrefab(true);
                ObjectsStorage.Spelloons.Add("spelloon_" + symbol, spelloon);
            }
        }

        public static void initializeEntities()
        {
            /*Transform fanBaseRenderer = null;

            Entity fanEntity = new EntityBuilder()
                .SetName("Fan")
                .SetLayer("ClickableEntities")
                .AddTrigger(1f)
                .SetLayerCollisionMask((LayerMask)2113541)
                .AddRenderbaseFunction(delegate (Entity entity)
                {
                    fanBaseRenderer = ObjectsCreator.createSpriteRendererBillboard(AssetsStorage.sprites["adv_fan_face_1"])
                    .transform;
                    fanBaseRenderer.SetParent(entity.transform);
                    return fanBaseRenderer;
                })
                .Build();

            fanEntity.gameObject.AddComponent<Fan>().prefabInitialization(fanEntity, fanBaseRenderer.GetComponentInChildren<SpriteRenderer>());

            ObjectsStorage.Entities.Add("fan", fanEntity);*/
        }

        public static void setCharactersTags()
        {
            setTagTo("adv_exclusion_hammer_immunity",
                Character.Baldi, Character.Principal, Character.Pomp, Character.Sweep, Character.Cumulo, Character.DrReflex,
                Character.LookAt, Character.Null);

            setTagTo("adv_exclusion_hammer_weakness",
                Character.Beans, Character.Crafters, Character.Bully, Character.Chalkles, Character.Prize);

            setTagTo("adv_ev_cold_school_immunity",
                Character.Pomp, Character.Sweep, Character.Prize, Character.Chalkles);

            setTagTo("adv_ev_disappearing_characters_immunity",
                Character.Baldi, Character.Principal, Character.Null);
        }

        public static void initializePosters()
        {
            //Debug.Log(JsonUtility.ToJson(new PosterSpawningData() { weight = 99 }));

            int textureNum = 1;
            while (true)
            {
                string basePath = AssetsHelper.modPath + "Textures/Posters/adv_poster_" + textureNum;

                string imagePath = basePath + ".png";
                string jsonPath = basePath + ".json";

                if (!File.Exists(imagePath)) break;

                PosterSpawningData posterData = JsonUtility.FromJson<PosterSpawningData>(File.ReadAllText(jsonPath));

                ObjectsStorage.WeightedPosterObjects.Add(new WeightedPosterObject()
                {
                    selection = ObjectCreators.CreatePosterObject(AssetsHelper.textureFromFile("Textures/Posters/adv_poster_" + textureNum + ".png"),
                    new PosterTextData[0]),
                    weight = posterData.weight
                });

                textureNum++;
            }
        }

        public static void initializeRooms()
        {
            RoomAsset englishRoom = PrefabsCreator.createRoomAsset("EnglishRoom", 99999);
            englishRoom.category = RoomCategory.Null;
            englishRoom.type = RoomType.Room;
            englishRoom.setCellsRange(new IntVector2(0, 0), new IntVector2(3, 6));
            englishRoom.forcedDoorPositions.Add(new IntVector2(0, 1));
            englishRoom.basicObjects.Add(new BasicObjectData()
            {
                prefab = ObjectsStorage.Objects["symbol_machine"].transform,
                position = new Vector3(15f, 0f, 54f),
                rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f)),
                replaceable = false
            });
        }

        private static void setTagTo(string tag, params Character[] characters)
        {
            foreach (NPCMetadata metadata in Array.FindAll(NPCMetaStorage.Instance.All(), x => characters.Contains(x.character)))
            {
                metadata.tags.Add(tag);
            }
        }
    }
}
