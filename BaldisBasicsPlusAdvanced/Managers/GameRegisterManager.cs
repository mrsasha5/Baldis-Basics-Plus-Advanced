#region Used Namespaces
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.InventoryItems;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using MTM101BaldAPI;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Game.Builders;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using HarmonyLib;
using BaldisBasicsPlusAdvanced.SerializableData;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using BaldisBasicsPlusAdvanced.Game.Rooms.Functions;
using MTM101BaldAPI.UI;
using TMPro;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using MTM101BaldAPI.ObjectCreation;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using BaldisBasicsPlusAdvanced.Game.Objects.Triggers;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.FakePlate;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BaldisBasicsPlusAdvanced.Game.Spawning;
using BaldisBasicsPlusAdvanced.Game.Objects.Food;
using BaldisBasicsPlusAdvanced.Game.InventoryItems.Food;
using BaldisBasicsPlusAdvanced.Game.NPCs.CrissTheCrystal;
using PlusLevelFormat;
using PlusLevelLoader;
using BaldisBasicsPlusAdvanced.Compats;
using MTM101BaldAPI.AssetTools;
using BaldisBasicsPlusAdvanced.Patches.GameManager;
using BaldisBasicsPlusAdvanced.Patches.Player;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Objects.Portals;
using MTM101BaldAPI.PlusExtensions;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm.NPCs;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips.Farm.Objects;
using BaldisBasicsPlusAdvanced.Game.FieldTrips.SpecialTrips;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Menu;
using BaldisBasicsPlusAdvanced.SerializableData.Rooms;
using BaldisBasicsPlusAdvanced.AutoUpdate;
using BaldisBasicsPlusAdvanced.Compats.CustomMusics;
using BaldisBasicsPlusAdvanced.Game.Components.UI.MainMenu;
using Newtonsoft.Json;
#endregion

namespace BaldisBasicsPlusAdvanced.Managers
{
    internal class GameRegisterManager
    {

        #region Main MIDIs Initialization

        public static void InitializeMidis()
        {
            FarmFieldTripManager.farmTripMusicKey =
                AssetLoader.MidiFromFile(AssetsHelper.modPath + "Audio/Music/FieldTrips/Adv_BSideSkid_CornTime.mid",
                    "Adv_BSideSkid_CornTime");

            if (IntegrationManager.IsActive<CustomMusicsIntegration>()) return;

            void LoadFrom(string floorName, LevelType type)
            {
                string[] paths = Directory.GetFiles(AssetsHelper.modPath + "Audio/Music/Floors/" + floorName);
                foreach (string path in paths)
                {
                    MusicPatch.Insert(AssetLoader.MidiFromFile(path, Path.GetFileName(path)), type);
                }
            }

            LoadFrom("Schoolhouse", LevelType.Schoolhouse);
            LoadFrom("Laboratory", LevelType.Laboratory);
            LoadFrom("Maintenance", LevelType.Maintenance);
            LoadFrom("Factory", LevelType.Factory);
        }

        #endregion

        #region MIDIs Post Initialization (For mods)

        public static void InitializeMidisPost()
        {
            void LoadFrom(string floorName, string typeName)
            {
                LevelType type;
                try
                {
                    type = EnumExtensions.GetFromExtendedName<LevelType>(typeName);
                }
                catch
                {
                    return;
                }

                string[] paths = Directory.GetFiles(AssetsHelper.modPath + "Audio/Music/Floors/" + floorName);
                foreach (string path in paths)
                {
                    if (!MusicPatch.musicNames.ContainsKey(type)) MusicPatch.musicNames.Add(type, new List<string>());
                    MusicPatch.musicNames[type].Add(AssetLoader.MidiFromFile(path, Path.GetFileName(path)));
                }
            }

            LoadFrom("Compats/Castle", "Castle");
            LoadFrom("Compats/Prison", "Prison");

        }

        #endregion

        #region Don't Destroy On Scene Load Objects Initialization

        public static void InitializeDoNotDestroyOnLoadObjects()
        {
            NotificationManager notifMan = new GameObject("NotificationManager").AddComponent<NotificationManager>();
            notifMan.Initialize();
            notifMan.gameObject.SetActive(false);
            GameObject.DontDestroyOnLoad(notifMan);
        }

        #endregion

        #region Scene Objects Initialization

        public static void InitializeSceneObjects()
        {
            const string fieldTripsModeName = "Mode_SpecialFieldTrips";

            BinaryReader binaryReader = new BinaryReader(File.OpenRead(AssetsHelper.modPath + "Premades/Levels/Farm.cbld"));
            Level farmLevel = binaryReader.ReadLevel();

            SceneObject farmScene = CustomLevelLoader.LoadLevel(farmLevel);
            farmScene.name = "Farm";
            farmScene.levelTitle = "FRM";
            farmScene.usesMap = false;

            ObjectsStorage.SceneObjects.Add("Farm", farmScene);

            FarmFieldTripManager farmMan = new GameObject("FarmManager").AddComponent<FarmFieldTripManager>();
            farmMan.InitializePrefab(1);

            ReflectionHelper.SetValue<bool>(farmMan, "destroyOnLoad", true);
            ReflectionHelper.Static_SetValue<Singleton<BaseGameManager>>("m_Instance", null);

            ObjectsStorage.SceneObjects["Farm"].manager = farmMan;

            farmMan.beginPlayImmediately = true;
            farmMan.managerNameKey = fieldTripsModeName;

            ReflectionHelper.SetValue(farmMan, "elevatorScreenPre", Resources.FindObjectsOfTypeAll<ElevatorScreen>()[0]);

            farmMan.gameObject.ConvertToPrefab(true);

            farmScene.AddMeta(AdvancedCore.Instance, new string[] { "adv_special_field_trip" });

            binaryReader.Close();
        }

        #endregion

        #region Cells textures

        public static void InitializeCellTextures()
        {
            foreach (string path in 
                Directory.GetFiles(AssetsHelper.modPath + "Textures/Cells", "*.png", SearchOption.AllDirectories))
            {
                CellTextureSerializableData data = CellTextureSerializableData.LoadFrom(path);

                if (data != null)
                {
                    ObjectsStorage.CellTextureDatas.Add(data);
                }

            }
        }

        #endregion

        #region NPCs Initialization

        public static void InitializeNPCs()
        {
            PrefabsCreator.CreateNpc(
                new NPCBuilder<CrissTheCrystal>(AdvancedCore.Instance.Info)
                .SetName("Criss the Crystal")
                .SetMetaName("Adv_NPC_CrissTheCrystal")
                .SetEnum("CrissTheCrystal")
                .SetPoster(AssetsStorage.textures["adv_poster_criss_the_crystal"], 
                "Adv_NPC_CrissTheCrystal", "Adv_NPC_CrissTheCrystal_Desc")
                .AddLooker()
                .AddMetaFlag(NPCFlags.StandardNoCollide)
                .SetMetaTags(new string[] { TagsStorage.student })
            )
                .SetBannedFloors(1)
                .SetWeight(2, 50)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse);
        }

        #endregion

        #region Items Initialization

        public static void InitializeGameItems()
        {
            PrefabsCreator.CreateItem<HammerItem>(
                nameKey: "Adv_Item_Hammer",
                descKey: "Adv_Item_Hammer_Desc",
                enumName: "Hammer",
                smallSpriteFileName: "adv_hammer_small.png",
                largeSpriteFileName: "adv_hammer_large.png",
                generatorCost: 75,
                price: 500,
                flags: ItemFlags.None,
                tags: new string[]
                {
                    TagsStorage.repairTool,
                    TagsStorage.contraband
                }
                )
                .SetSpawnsOnRooms(true)
                .SetSpawnsOnFieldTrips(true)
                .SetSpawnsOnParty(true)
                .SetSpawnsOnMysteryRooms(true)
                .SetEndless(true)
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory);

            PrefabsCreator.CreateItem<WindBlowerItem>(
                nameKey: "Adv_Item_WindBlower",
                descKey: "Adv_Item_WindBlower_Desc",
                enumName: "WindBlower",
                smallSpriteFileName: "adv_wind_blower_small.png",
                largeSpriteFileName: "adv_wind_blower_large.png",
                generatorCost: 50,
                price: 300,
                flags: ItemFlags.None
                )
                .SetWeight(floor: 2, 75)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            PrefabsCreator.CreateItem<MysteriousTeleporterItem>(
                nameKey: "Adv_Item_MysteriousTeleporter",
                descKey: "Adv_Item_MysteriousTeleporter_Desc",
                enumName: "MysteriousTeleporter",
                smallSpriteFileName: "adv_mysterious_teleporter_small.png",
                largeSpriteFileName: "adv_mysterious_teleporter_large.png",
                generatorCost: 60,
                price: 500,
                tags: new string[]
                {
                    TagsStorage.contraband
                },
                flags: ItemFlags.CreatesEntity
                )
                .SetSpawnsOnParty(true)
                .SetSpawnsOnMysteryRooms(true)
                .SetEndless(true)
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            /*PrefabsCreator.CreateItem<InvisibilityPotionItem>(
                nameKey: "Adv_Item_InvisibilityPotion",
                descKey: "Adv_Item_InvisibilityPotion_Desc",
                enumName: "InvisibilityPotion",
                tags: new string[] { 
                    TagsStorage.drink
                },
                smallSpriteFileName: "adv_invisibility_potion_small.png",
                largeSpriteFileName: "adv_invisibility_potion_large.png",
                generatorCost: 40,
                price: 500,
                flags: ItemFlags.None
                )
                .SetSpawnsOnFieldTrips(true)
                .SetSpawnsOnParty(true)
                .SetWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);*/

            PrefabsCreator.CreateItem<IceBootsItem>(
                nameKey: "Adv_Item_IceBoots",
                descKey: "Adv_Item_IceBoots_Desc",
                enumName: "IceBoots",
                smallSpriteFileName: "adv_ice_boots_small.png",
                largeSpriteFileName: "adv_ice_boots_large.png",
                generatorCost: 50,
                price: 300,
                flags: ItemFlags.Persists
                )
                .SetSpawnsOnFieldTrips(true)
                .SetEndless(true)
                .SetWeight(floor: 2, 75)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            PrefabsCreator.CreateItem<PlaceableFanItem>(
                nameKey: "Adv_Item_PlaceableFan",
                descKey: "Adv_Item_PlaceableFan_Desc",
                enumName: "PlaceableFan",
                flags: ItemFlags.CreatesEntity,
                smallSpriteFileName: "adv_placeable_fan_small.png",
                largeSpriteFileName: "adv_placeable_fan_large.png",
                generatorCost: 75,
                price: 750
                )
                .SetWeight(floor: 2, 35)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory);

            PrefabsCreator.CreateItem<TeleportationBombItem>(
                nameKey: "Adv_Item_TeleportationBomb",
                descKey: "Adv_Item_TeleportationBomb_Desc",
                enumName: "TeleportationBomb",
                flags: ItemFlags.CreatesEntity | ItemFlags.Persists,
                smallSpriteFileName: "adv_teleportation_bomb_small.png",
                largeSpriteFileName: "adv_teleportation_bomb_large.png",
                generatorCost: 75,
                price: 500,
                tags: new string[]
                {
                    TagsStorage.contraband
                }
                )
                .SetSpawnsOnFieldTrips(true)
                .SetSpawnsOnParty(true)
                .SetBannedFloors(1)
                .SetEndless(true)
                .SetWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            PrefabsCreator.CreateItem<MagicClockItem>(
                nameKey: "Adv_Item_MagicClock",
                descKey: "Adv_Item_MagicClock_Desc",
                enumName: "MagicClock",
                flags: ItemFlags.Persists,
                smallSpriteFileName: "adv_magic_clock_small.png",
                largeSpriteFileName: "adv_magic_clock_large.png",
                generatorCost: 75,
                price: 750,
                tags: new string[]
                {
                    TagsStorage.contraband
                }
                )
                .SetSpawnsOnFieldTrips(true)
                .SetBannedFloors(1)
                .SetEndless(true)
                .SetWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory);

            PrefabsCreator.CreateItem<InflatableBalloonItem>(
                nameKey: "Adv_Item_InflatableBalloon",
                descKey: "Adv_Item_InflatableBalloon_Desc",
                enumName: "InflatableBalloon",
                flags: ItemFlags.None,
                smallSpriteFileName: "adv_inflatable_balloon_small.png",
                largeSpriteFileName: "adv_inflatable_balloon_large.png",
                generatorCost: 75,
                price: 300
                )
                .SetBannedFloors(1)
                .SetEndless(true)
                .SetWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory);

            PrefabsCreator.CreateItem<DoughItem>(
                nameKey: "Adv_Item_Dough",
                descKey: "Adv_Item_Dough_Desc",
                enumName: "Dough",
                flags: ItemFlags.CreatesEntity,
                smallSpriteFileName: "adv_dough_small.png",
                largeSpriteFileName: "adv_dough_large.png",
                generatorCost: 25,
                price: 300
                )
                .SetBannedFloors(1)
                .SetEndless(true)
                .SetWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse);

            PrefabsCreator.CreateItem<MysteriousBusPassItem>(
                nameKey: "Adv_Item_MysteriousBusPass",
                descKey: "Adv_Item_MysteriousBusPass_Desc",
                enumName: "MysteriousBusPass",
                flags: ItemFlags.NoUses,
                smallSpriteFileName: "adv_mysterious_bus_pass_small.png",
                largeSpriteFileName: "adv_mysterious_bus_pass_large.png",
                generatorCost: 75,
                price: 400,
                tags: new string[] { 
                    TagsStorage.forbiddenPresent 
                }
                )
                .SetSpawnsOnShop(false)
                .SetSpawnsOnMysteryRooms(true)
                .SetBannedFloors(1, 3)
                .SetWeight(floor: 2, 100)
                .SetLevelTypes(LevelType.Schoolhouse);
        }

        public static void InitializeMultipleUsableItems()
        {
            ItemObject bread2 = PrefabsCreator.CreateItem<BreadItem>(
                nameKey: "Adv_Item_Bread",
                descKey: "Adv_Item_Bread_Desc",
                enumName: "Bread",
                flags: ItemFlags.MultipleUse,
                tags: new string[] { TagsStorage.food },
                smallSpriteFileName: "adv_bread_small.png",
                largeSpriteFileName: "adv_bread_large.png",
                generatorCost: 50,
                price: 550
                )
                .SetBannedFloors(1)
                .SetEndless(true)
                .SetWeight(floor: 2, 50)
                .SetLevelTypes(LevelType.Schoolhouse)
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject bread1 = PrefabsCreator.CreateItem<BreadItem>(
                nameKey: "Adv_Item_BreadPiece",
                descKey: "Adv_Item_BreadPiece_Desc",
                enumName: "Bread",
                flags: ItemFlags.MultipleUse,
                tags: new string[] { TagsStorage.food },
                smallSpriteFileName: "adv_piece_of_bread_small.png",
                largeSpriteFileName: "adv_piece_of_bread_large.png",
                generatorCost: 50,
                price: 550,
                itemMeta: bread2.GetMeta()
                )
                .DoNotSpawn()
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject rawChicken2 = PrefabsCreator.CreateItem<ChickenItem>(
                nameKey: "Adv_Item_RawChickenLeg",
                descKey: "Adv_Item_RawChickenLeg_Desc",
                enumName: "RawChickenLeg",
                flags: ItemFlags.CreatesEntity | ItemFlags.MultipleUse,
                tags: new string[] { TagsStorage.food },
                smallSpriteFileName: "adv_raw_chicken_leg_small.png",
                largeSpriteFileName: "adv_raw_chicken_leg_large.png",
                generatorCost: 75,
                price: 500
                )
                .SetBannedFloors(1)
                .SetEndless(true)
                .SetWeight(floor: 2, 40)
                .SetLevelTypes(LevelType.Schoolhouse)
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject rawChicken1 = PrefabsCreator.CreateItem<ChickenItem>(
                nameKey: "Adv_Item_RawChickenLeg1",
                descKey: "Adv_Item_RawChickenLeg_Desc",
                enumName: "RawChickenLeg",
                flags: ItemFlags.CreatesEntity | ItemFlags.MultipleUse,
                tags: new string[] { TagsStorage.food },
                smallSpriteFileName: "adv_raw_chicken_leg_small.png",
                largeSpriteFileName: "adv_raw_chicken_leg_large.png",
                generatorCost: 75,
                price: 500,
                itemMeta: rawChicken2.GetMeta()
                )
                .DoNotSpawn()
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject cookedChicken2 = PrefabsCreator.CreateItem<ChickenItem>(
                nameKey: "Adv_Item_CookedChickenLeg",
                descKey: "Adv_Item_CookedChickenLeg_Desc",
                enumName: "CookedChickenLeg",
                flags: ItemFlags.CreatesEntity | ItemFlags.MultipleUse,
                tags: new string[] { TagsStorage.food },
                smallSpriteFileName: "adv_cooked_chicken_leg_small.png",
                largeSpriteFileName: "adv_cooked_chicken_leg_large.png",
                generatorCost: 75,
                price: 800,
                variant: 2
                )
                .SetSpawnsOnShop(false)
                .SetBannedFloors(1)
                .SetEndless(true)
                .SetWeight(floor: 2, 15)
                .SetLevelTypes(LevelType.Schoolhouse)
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject cookedChicken1 = PrefabsCreator.CreateItem<ChickenItem>(
                nameKey: "Adv_Item_CookedChickenLeg1",
                descKey: "Adv_Item_CookedChickenLeg_Desc",
                enumName: "CookedChickenLeg",
                flags: ItemFlags.CreatesEntity | ItemFlags.MultipleUse,
                tags: new string[] { TagsStorage.food },
                smallSpriteFileName: "adv_cooked_chicken_leg_small.png",
                largeSpriteFileName: "adv_cooked_chicken_leg_large.png",
                generatorCost: 75,
                price: 800,
                itemMeta: cookedChicken2.GetMeta(),
                variant: 2
                )
                .DoNotSpawn()
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject boxingGlove2 = PrefabsCreator.CreateItem<BoxingGloveItem>(
                nameKey: "Adv_Item_BoxingGlove",
                descKey: "Adv_Item_BoxingGlove_Desc",
                enumName: "BoxingGlove",
                smallSpriteFileName: "adv_boxing_glove_small.png",
                largeSpriteFileName: "adv_boxing_glove_large.png",
                generatorCost: 50,
                price: 500,
                flags: ItemFlags.MultipleUse
                )
                .SetWeight(floor: 2, 35)
                .SetLevelTypes(LevelType.Schoolhouse)
                .ConvertTo<ItemSpawningData>()
                .SetSpawnsOnFieldTrips(true)
                .ItemObject;

            ItemObject boxingGlove1 = PrefabsCreator.CreateItem<BoxingGloveItem>(
                nameKey: "Adv_Item_BoxingGlove1",
                descKey: "Adv_Item_BoxingGlove_Desc",
                enumName: "BoxingGlove",
                smallSpriteFileName: "adv_boxing_glove_small.png",
                largeSpriteFileName: "adv_boxing_glove_large.png",
                generatorCost: 50,
                price: 500,
                flags: ItemFlags.MultipleUse,
                itemMeta: boxingGlove2.GetMeta()
                )
                .DoNotSpawn()
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject portal2 = PrefabsCreator.CreateItem<PlaceablePortalItem>(
                nameKey: "Adv_Item_PlaceablePortal",
                descKey: "Adv_Item_PlaceablePortal_Desc",
                enumName: "PlaceablePortal",
                smallSpriteFileName: "adv_placeable_portal_small.png",
                largeSpriteFileName: "adv_placeable_portal_large.png",
                generatorCost: 60,
                price: 750,
                flags: ItemFlags.MultipleUse
                )
                .SetWeight(floor: 2, 30)
                .ConvertTo<ItemSpawningData>()
                .SetSpawnsOnFieldTrips(true)
                .SetEndless(true)
                .SetBannedFloors(1)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory)
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject portal1 = PrefabsCreator.CreateItem<PlaceablePortalItem>(
                nameKey: "Adv_Item_PlaceablePortal1",
                descKey: "Adv_Item_PlaceablePortal_Desc",
                enumName: "PlaceablePortal",
                smallSpriteFileName: "adv_placeable_portal_small.png",
                largeSpriteFileName: "adv_placeable_portal_large.png",
                generatorCost: 60,
                price: 750,
                flags: ItemFlags.MultipleUse,
                itemMeta: portal2.GetMeta()
                )
                .DoNotSpawn()
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            //metas
            ((BaseMultipleUsableItem)boxingGlove2.item).Initialize(boxingGlove1);
            boxingGlove2.GetMeta().itemObjects = ((BaseMultipleUsableItem)boxingGlove2.item).AllVersions.AddItem(boxingGlove2).ToArray();
            //boxingGlove1.AddMeta(boxingGlove2.GetMeta()); //for the 5.3.0.0 is not needed, because it is fixed.

            ((BaseMultipleUsableItem)portal2.item).Initialize(portal1);
            portal2.GetMeta().itemObjects = ((BaseMultipleUsableItem)portal2.item).AllVersions.AddItem(portal2).ToArray();

            ((BaseMultipleUsableItem)rawChicken2.item).Initialize(rawChicken1);
            rawChicken2.GetMeta().itemObjects = ((BaseMultipleUsableItem)rawChicken2.item).AllVersions.AddItem(rawChicken2).ToArray();

            ((BaseMultipleUsableItem)cookedChicken2.item).Initialize(cookedChicken1);
            cookedChicken2.GetMeta().itemObjects = ((BaseMultipleUsableItem)cookedChicken2.item).AllVersions.AddItem(cookedChicken2).ToArray();

            ((BaseMultipleUsableItem)bread2.item).Initialize(bread1);
            bread2.GetMeta().itemObjects = ((BaseMultipleUsableItem)bread2.item).AllVersions.AddItem(bread2).ToArray();
        }

        #endregion

        #region Random Events Initialization

        public static void InitializeRandomEvents()
        {
            PrefabsCreator.CreateEvent<DisappearingCharactersEvent>(
                name: "Disappearing Characters",
                soundKey: "adv_bal_event_disappearing_characters",
                enumName: "DisappearingCharacters",
                minTime: 60f,
                maxTime: 80f,
                flags: RandomEventFlags.None
                )
                .SetWeight(floor: 2, 100)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory)
                .SetBannedFloors(1, 3);

            PrefabsCreator.CreateEvent<ColdSchoolEvent>(
                name: "Cold School",
                soundKey: "adv_bal_event_cold_machine",
                enumName: "ColdSchool",
                minTime: 60f,
                maxTime: 90f,
                flags: RandomEventFlags.None
                )
                .SetWeight(floor: 2, 75)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse)
                .SetBannedFloors(1);

            PrefabsCreator.CreateEvent<PortalChaosEvent>(
                name: "Portal Chaos",
                enumName: "PortalChaos",
                soundKey: "adv_bal_event_portals",
                minTime: 75f,
                maxTime: 120f,
                flags: RandomEventFlags.None
                )
                .SetWeight(floor: 2, 50)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Laboratory)
                .SetBannedFloors(1);

            PrefabsCreator.CreateEvent<VotingEvent>(
                name: "Voting",
                enumName: "Voting",
                soundKey: "adv_bal_event_voting",
                minTime: 100f,
                maxTime: 100f,
                flags: RandomEventFlags.AffectsGenerator
                )
                .SetWeight(floor: 2, 125)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory, LevelType.Laboratory, LevelType.Maintenance)
                //.SetEndless(true) //no endless
                .SetBannedFloors(1);
        }

        #endregion

        #region Vending Machines Initialization

        public static void InitializeVendingMachines()
        {
            PrefabsCreator.CreateMultipleRequiredVendingMachine("GoodMachine",
                ItemMetaStorage.Instance.FindByEnum(Items.Quarter).itemObjects[0], 2,
                AssetsStorage.materials["adv_good_machine"], AssetsStorage.materials["adv_good_machine_out"], null,
                weight: 50, 
                new WeightedItemObject[] {
                new WeightedItemObject()
                {
                    weight = 25,
                    selection = ItemMetaStorage.Instance.FindByEnum(Items.Teleporter).itemObjects[0]
                },
                new WeightedItemObject()
                {
                    weight = 50,
                    selection = ItemMetaStorage.Instance.FindByEnum(Items.Apple).itemObjects[0]
                },
                new WeightedItemObject()
                {
                    weight = 25,
                    selection = ItemMetaStorage.Instance.FindByEnum(Items.GrapplingHook).itemObjects.Last()
                },
            }).SetForced(true);
        }

        #endregion

        #region Structure Builders Initialization

        public static void InitializeObjectBuilders()
        {
            //Rusty RotoHall Builder
            Structure_Rotohalls rotoHallBuilder = 
                GameObject.Instantiate(AssetsHelper.LoadAsset<Structure_Rotohalls>("Rotohall_Structure"));
            rotoHallBuilder.name = "Structure_RustyRotohall";
            rotoHallBuilder.gameObject.ConvertToPrefab(true);

            MeshRenderer rustyCornerCylinder = 
                GameObject.Instantiate(AssetsHelper.LoadAsset<MeshRenderer>("CornerCylinder_Model"));
            MeshRenderer rustyStraightCylinder =
                GameObject.Instantiate(AssetsHelper.LoadAsset<MeshRenderer>("StraightCylinder_Model"));
            rustyCornerCylinder.name = "RustyCornerCylinder_Model";
            rustyStraightCylinder.name = "RustyStraightCylinder_Model";
            rustyCornerCylinder.gameObject.ConvertToPrefab(true);
            rustyStraightCylinder.gameObject.ConvertToPrefab(true);

            Material[] materials = rustyCornerCylinder.materials;

            materials[0].SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall"]);
            materials[1].SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_sign_left"]);
            materials[2].SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_sign_right"]);
            rustyCornerCylinder.materials = materials;

            materials = rustyStraightCylinder.materials;

            materials[0].SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall"]);
            materials[1].SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_sign_straight"]);
            rustyStraightCylinder.materials = materials;

            Array.Find(rustyCornerCylinder.GetComponentsInChildren<MeshRenderer>(), x => x.name == "CylinderFloor_Model")
                .material.SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_floor"]);
            Array.Find(rustyStraightCylinder.GetComponentsInChildren<MeshRenderer>(), x => x.name == "CylinderFloor_Model")
                .material.SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_floor"]);

            ReflectionHelper.SetValue<RotoHall>(rotoHallBuilder, "rotoHallPre",
                ObjectsStorage.Objects["rusty_rotohall"].GetComponent<RotoHall>());
            ReflectionHelper.SetValue<MeshRenderer>(rotoHallBuilder, "cornerCylinderPre", rustyCornerCylinder);
            ReflectionHelper.SetValue<MeshRenderer>(rotoHallBuilder, "straightCylinderPre", rustyStraightCylinder);

            ObjectsStorage.StructureBuilders.Add("Structure_RustyRotohall", rotoHallBuilder);

            StructureBuilderSpawningData rotohallSpawningData = 
                new StructureBuilderSpawningData("Structure_RustyRotohall", rotoHallBuilder);

            rotohallSpawningData
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    minMax = new IntVector2[] { new IntVector2(1, 2), new IntVector2(0, 12) },
                })
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 75)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse);

            ObjectsStorage.SpawningData.Add("builder_Structure_RustyRotohall", rotohallSpawningData);
            //Builder ends

            PrefabsCreator.CreateStructureBuilder<Structure_Pulley>("Structure_Pulley")
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    minMax = new IntVector2[] { new IntVector2(1, 2)},
                })
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 75)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse);

            PrefabsCreator.CreateStructureBuilder<Structure_AccelerationPlate>("Structure_AccelerationPlate")
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    minMax = new IntVector2[] { new IntVector2(2, 5), new IntVector2(0, 0) },
                })
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 75)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory);

            PrefabsCreator.CreateStructureBuilder<Structure_KitchenStove>("Structure_KitchenStove")
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    minMax = new IntVector2[] { new IntVector2(1, 2), new IntVector2(0, 0) },
                })
                .SetBannedFloors(1)
                .SetWeight(2, 100)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory);

            PrefabsCreator.CreateStructureBuilder<Structure_GenericPlate>("Structure_GenericPlate")
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    minMax = new IntVector2[] { new IntVector2(2, 5), new IntVector2(0, 2) },
                })
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 75)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory, LevelType.Laboratory, LevelType.Maintenance);

            PrefabsCreator.CreateStructureBuilder<Structure_Zipline>("Structure_Zipline")
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    minMax = new IntVector2[] { new IntVector2(1, 2)},
                    prefab = new WeightedGameObject[]
                    {
                        new WeightedGameObject()
                        {
                            selection = ObjectsStorage.Objects["zipline_hanger"],
                            weight = 100
                        },
                        new WeightedGameObject()
                        {
                            selection = ObjectsStorage.Objects["zipline_black_hanger"],
                            weight = 75
                        }
                    }
                })
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 100)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse);

            PrefabsCreator.CreateStructureBuilder<Structure_NoisyPlate>("Structure_NoisyPlate")
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    minMax = new IntVector2[] { new IntVector2(1, 2), new IntVector2(0, 0) },
                })
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 30)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Maintenance);

            PrefabsCreator.CreateStructureBuilder<Structure_GumDispenser>("Structure_GumDispenser")
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    minMax = new IntVector2[] { new IntVector2(1, 2) },
                })
                .SetBannedFloors(1)
                .SetWeight(floor: 2, 75)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory);

            PrefabsCreator.CreateStructureBuilder<Structure_PlainPlate>("Structure_PlainPlate")
                .SetStructureParameters(floor: 2, new StructureParameters()
                {
                    chance = new float[] { 0.25f },
                    minMax = new IntVector2[] { new IntVector2(1, 2), new IntVector2(0, 0) },
                })
                .SetBannedFloors(1)
                .SetForced(true)
                .SetEndless(true)
                .SetLevelTypes(LevelType.Schoolhouse, LevelType.Factory);
        }

        #endregion

        #region UI Initialization

        public static void InitializeUi()
        {
            PrefabsCreator.CreateOverlay("FrozenOverlay", AssetsStorage.sprites["adv_frozen_overlay"], false);
            PrefabsCreator.CreateOverlay("ElephantOverlay", AssetsStorage.sprites["adv_elephant_overlay"], true);
            PrefabsCreator.CreateOverlay("ShieldOverlay", AssetsStorage.sprites["adv_protected_overlay"], true);

            //Initializing Chalkboard Menu
            Canvas canvas = ObjectsCreator.CreateCanvas(setGlobalCam: true);
            canvas.name = "Chalkboard Menu";

            ChalkboardMenu chalkboardMenu = canvas.gameObject.AddComponent<ChalkboardMenu>();

            chalkboardMenu.canvas = canvas;

            chalkboardMenu.chalkboard = UIHelpers.CreateImage(AssetsStorage.sprites["chalkboard_standard"], canvas.transform,
                Vector3.zero, correctPosition: false);
            chalkboardMenu.chalkboard.ToCenter();

            TextMeshProUGUI title = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24,
                "Title",
                chalkboardMenu.chalkboard.transform, new Vector3(0, 105, 0), false);
            title.name = "title";

            TextMeshProUGUI info = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24,
                "",
                chalkboardMenu.chalkboard.transform, Vector3.up * 50f, false);
            info.name = "info";

            chalkboardMenu.texts.Add(title);
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);
            title.color = Color.white;
            title.alignment = TextAlignmentOptions.Top;

            chalkboardMenu.texts.Add(info);
            info.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);
            info.color = Color.white;
            info.alignment = TextAlignmentOptions.Top;

            StandardMenuButton exit = ObjectsCreator.CreateSpriteButton(AssetsStorage.sprites["adv_exit_transparent"], 
                new Vector3(185, 140), chalkboardMenu.chalkboard.transform, AssetsStorage.sprites["adv_exit"]);
            exit.name = "exit";

            chalkboardMenu.buttons.Add(exit);
            exit.InitializeAllEvents();

            canvas.gameObject.ConvertToPrefab(true);

            canvas.SetCursorInitiator(setAutoInitiator: true);

            ObjectsStorage.Objects.Add("chalkboard_menu", canvas.gameObject);
            //Chalkboard Menu ends

            PrefabsCreator.CreateObjectPrefab<CreditsScreen>("Credits Screen", "credits_screen");
            //PrefabsCreator.CreateObjectPrefab<UpdatesCenterMenu>("Updates Center", "updates_center");
        }

        #endregion

        #region API Content Initialization 

        public static void InitializeApiThings()
        {
            
            LocalizationManager localization = Singleton<LocalizationManager>.Instance;

            string[] tipsBaseNames = new string[1];

            tipsBaseNames[0] = "Adv_Elv_Tip";

            foreach (string tipBaseName in tipsBaseNames)
            {
                if (tipBaseName == null) continue;
                int num = 1;
                string tipKey = tipBaseName + num;

                Dictionary<string, string> locText = ReflectionHelper.GetValue<Dictionary<string, string>>(localization, "localizedText");
                List<string> tipKeys = locText.Keys.ToList();

                for (int i = 0; i < tipKeys.Count; i++)
                {
                    if (!tipKeys[i].StartsWith(tipBaseName))
                    {
                        tipKeys.RemoveAt(i);
                        i--;
                    }
                }

                tipKeys.Remove("Adv_Elv_Tip_Base");

                ApiManager.AddNewTips(AdvancedCore.Instance.Info, tipKeys.ToArray());
            }

            ApiManager.AddNewSymbolMachineWords(AdvancedCore.Instance.Info,
                "BSODA", "Beans", "Baldi", "Bully", "Criss", "boots", "field", "learn", "expel",
                "spell", "blind", "laser", "brain", "ruler", "floor", "class", "flood", "party",
                "apple", "chalk", "erase", "cloud", "plate", "store", "clock", "farm", "math"
            );

            ApiManager.CreateSchoolCouncilTopic<NoPlatesCooldownTopic>(AdvancedCore.Instance.Info, 100);
            ApiManager.CreateSchoolCouncilTopic<PrincipalIgnoresSomeRulesTopic>(AdvancedCore.Instance.Info, 75);
            ApiManager.CreateSchoolCouncilTopic<LightsEconomyTopic>(AdvancedCore.Instance.Info, 50);
            ApiManager.CreateSchoolCouncilTopic<TurnOffFacultyNoisyPlatesTopic>(AdvancedCore.Instance.Info, 100);
            ApiManager.CreateSchoolCouncilTopic<GottaSweepTimeTopic>(AdvancedCore.Instance.Info, 50);
            ApiManager.CreateSchoolCouncilTopic<ConvertVendingMachinesTopic>(AdvancedCore.Instance.Info, 50);
            ApiManager.CreateSchoolCouncilTopic<OpenVentsTopic>(AdvancedCore.Instance.Info, 125);
            ApiManager.CreateSchoolCouncilTopic<BrokenZiplinesTopic>(AdvancedCore.Instance.Info, 125);
            ApiManager.CreateSchoolCouncilTopic<DisabledConveyorsTopic>(AdvancedCore.Instance.Info, 125);
            ApiManager.CreateSchoolCouncilTopic<DisabledFacultyLockdownDoorsTopic>(AdvancedCore.Instance.Info, 125);
            //ApiManager.CreateSchoolCouncilTopic<OpenVentsTopic>(AdvancedCore.Instance.Info, 100);
        }

        #endregion

        #region Special Field Trips Initialization

        public static void InitializeTrips()
        {
            new FieldTripData()
            {
                sceneName = "Farm",
                sceneObject = ObjectsStorage.SceneObjects["Farm"]
            }
            .SetDefaultSkybox()
            .Register();
        }

        #endregion

        #region Objects Initialization

        public static void InitializeObjects()
        {
            //Variables for everyone
            MathMachine mathMachineComp = GameObject.Instantiate(AssetsHelper.LoadAsset<MathMachine>("MathMachine"));

            //Cloning the Rotohall and making it as rusty

            RotoHall rotoHall = GameObject.Instantiate(AssetsHelper.LoadAsset<RotoHall>("RotoHall_Base"));
            rotoHall.gameObject.ConvertToPrefab(true);
            rotoHall.name = "RustyRotoHall_Base";

            RustyRotohall rustyRotoHall = rotoHall.gameObject.AddComponent<RustyRotohall>();
            rotoHall.CopyAllValuesTo(rustyRotoHall);
            GameObject.Destroy(rotoHall);

            ReflectionHelper.SetValue<EnvironmentObject>(rustyRotoHall.GetComponent<EntitySpinner>(), "environmentObject", rustyRotoHall);

            ReflectionHelper.SetValue<float>(rustyRotoHall, "speed", 25f);
            ReflectionHelper.SetValue<SoundObject>(rustyRotoHall, "audTurn", AssetsStorage.sounds["adv_turning_start"]);

            rotoHall.GetComponent<MeshRenderer>().material.SetMainTexture(AssetsStorage.textures["adv_rusty_rotohall_blank"]);

            ObjectsStorage.Objects.Add("rusty_rotohall", rustyRotoHall.gameObject);

            //Advanced Math Machine

            bool advMathMachineIsCorner = false;

            while (true)
            {
                if (advMathMachineIsCorner) mathMachineComp = GameObject.Instantiate(AssetsHelper.LoadAsset<MathMachine>("MathMachine_Corner"));

                GameObject advancedMathMachineObj = mathMachineComp.gameObject;
                advancedMathMachineObj.name = !advMathMachineIsCorner ? "AdvancedMathMachine" : "AdvancedMathMachineCorner";

                AdvancedMathMachine advancedMathMachine = advancedMathMachineObj.AddComponent<AdvancedMathMachine>();
                mathMachineComp.CopyAllValuesTo(advancedMathMachine);

                advancedMathMachine.InitializePrefab(1);

                GameObject.Destroy(mathMachineComp);

                advancedMathMachineObj.ConvertToPrefab(true);
                ObjectsStorage.Objects.Add(!advMathMachineIsCorner ? "advanced_math_machine" : "advanced_math_machine_corner", advancedMathMachineObj);

                if (advMathMachineIsCorner) break;
                advMathMachineIsCorner = true;
            }
            

            //Zipline Hangers
            PrefabsCreator.CreateObjectPrefab<ZiplineHanger>("Zipline Hanger", "zipline_hanger");
            PrefabsCreator.CreateObjectPrefab<ZiplineHanger>("Zipline Black Hanger", "zipline_black_hanger", variant: 2);

            //Hiding Plant
            //PrefabsCreator.CreateObjectPrefab<SuspiciousPlant>("Suspicious Plant", "suspicious_plant");

            //Voting Ballot
            PrefabsCreator.CreateObjectPrefab<VotingBallot>("Voting Ballot", "voting_ballot");

            //Pulley
            PrefabsCreator.CreateObjectPrefab<Pulley>("Pulley", "pulley");

            //Gum Dispenser
            PrefabsCreator.CreateObjectPrefab<GumDispenser>("Gum Dispenser", "gum_dispenser");

            //Mysterious portal
            PrefabsCreator.CreateObjectPrefab<MysteriousPortal>("Mysterious Portal", "mysterious_portal");
            PrefabsCreator.CreateObjectPrefab<CrazyMysteriousPortal>("Crazy Mysterious Portal", "crazy_mysterious_portal");

            //Infection Zone
            //PrefabsCreator.CreateObjectPrefab<InfectionZone>("Infection Zone", "infection_zone");

            //plates

            PrefabsCreator.CreatePlate<PressurePlate>("plate");
            PrefabsCreator.CreatePlate<InvisibilityPlate>("invisibility_plate");
            PrefabsCreator.CreatePlate<AccelerationPlate>("acceleration_plate");

            PrefabsCreator.CreatePlate<NoisyPlate>("noisy_plate");
            if (IntegrationManager.IsActive<LevelEditorIntegration>())
            {
                PrefabsCreator.CreatePlate<NoisyPlate>("noisy_faculty_plate");
                NoisyPlate facultyVersion = ObjectsStorage.Objects["noisy_faculty_plate"].GetComponent<NoisyPlate>();
                facultyVersion.SetEditorSprite("adv_editor_noisy_faculty_plate");
                facultyVersion.OverrideCooldown(60f);
                facultyVersion.SetCallsPrincipal(true);
                facultyVersion.SetLevelEditorMode(true);
            }

            PrefabsCreator.CreatePlate<StealingPlate>("stealing_plate");
            PrefabsCreator.CreatePlate<BullyPlate>("bully_plate");
            PrefabsCreator.CreatePlate<PresentPlate>("present_plate");
            PrefabsCreator.CreatePlate<SlowdownPlate>("slowdown_plate");
            PrefabsCreator.CreatePlate<SugarPlate>("sugar_addiction_plate");
            PrefabsCreator.CreatePlate<ProtectionPlate>("protection_plate");
            PrefabsCreator.CreatePlate<TeleportationPlate>("teleportation_plate");
            PrefabsCreator.CreatePlate<MysteriousPlate>("fake_plate");
            PrefabsCreator.CreatePlate<SafetyTrapdoor>("safety_trapdoor");
            PrefabsCreator.CreatePlate<KitchenStove>("kitchen_stove");
            PrefabsCreator.CreatePlate<JohnnyKitchenStove>("johnny_kitchen_stove");

            //plates end

            //triggers

            PrefabsCreator.CreateTrigger<NoPlatesCooldownTrigger>("no_plates_cooldown");
            PrefabsCreator.CreateTrigger<PitStopOverridesTrigger>("pit_stop_overrides");
            PrefabsCreator.CreateTrigger<NoCooldownPlateCurrentPositionTrigger>("no_plate_cooldown");
            PrefabsCreator.CreateTrigger<UnpressTimePlateCurrentPositionTrigger>("low_plate_unpress_time");

            //triggers end

            //spelling

            PrefabsCreator.CreateObjectPrefab<SymbolMachine>("SymbolMachine", "symbol_machine");

            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            
            for (int i = 0; i < alphabet.Length; i++)
            {
                ApiManager.CreateNewSpelloon(alphabet[i].ToString(),
                    AssetsStorage.sprites["adv_balloon_" + alphabet[i]]);
            }
            //spelling end

            PrefabsCreator.CreateObjectPrefab<TeleportationHole>("Teleportation Bomb", "teleportation_bomb");
            PrefabsCreator.CreateObjectPrefab<Reaper>("Farm Reaper", "farm_reaper");
            PrefabsCreator.CreateObjectPrefab<FinishFlag>("Farm Finish Flag", "farm_flag");

            GameObject cornSign = new GameObject("Corn Sign");
            ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_corn_sign1"])
                .transform.SetParent(cornSign.transform, false);
            cornSign.ConvertToPrefab(true);
            ObjectsStorage.Objects.Add("farm_sign1", cornSign);
        }

        #endregion

        #region Entities Initialization

        public static void InitializeEntities()
        {
            //Mysterious Teleporter
            PrefabsCreator.CreateObjectPrefab<MysteriousTeleporterProjectile>("Mysterious Teleporter", "mysterious_teleporter");

            //Anvil Projectile
            PrefabsCreator.CreateObjectPrefab<AnvilProjectile>("Anvil Projectile", "anvil_projectile");

            //Own gum
            Gum gumPre = AssetsHelper.LoadAsset<Gum>("Gum");

            Gum gumComp = GameObject.Instantiate(gumPre);

            GumProjectile gumProj = gumComp.gameObject.AddComponent<GumProjectile>();
            gumProj.gameObject.ConvertToPrefab(true);

            gumProj.Speed = ReflectionHelper.GetValue<float>(gumComp, "speed");
            gumProj.canvas = ReflectionHelper.GetValue<Canvas>(gumComp, "canvas");
            gumProj.moveMod = ReflectionHelper.GetValue<MovementModifier>(gumComp, "moveMod");
            gumProj.playerMod = ReflectionHelper.GetValue<MovementModifier>(gumComp, "playerMod");
            gumProj.groundedSprite = ReflectionHelper.GetValue<GameObject>(gumComp, "groundedSprite");
            gumProj.flyingSprite = ReflectionHelper.GetValue<GameObject>(gumComp, "flyingSprite");

            gumProj.InitializePrefab(1);

            GameObject.Destroy(gumComp);

            ObjectsStorage.Objects.Add("gum", gumProj.gameObject);

            //gum end

            PrefabsCreator.CreateEntity<Fan>(new EntityBuilder()
                .SetName("Fan")
                .SetLayer("ClickableEntities")
                .AddTrigger(1f)
                .SetLayerCollisionMask(LayersHelper.entityCollisionMask)
                .AddRenderbaseFunction(delegate (Entity entity)
                {
                    Transform fanBaseRenderer = ObjectsCreator.CreateSpriteRendererBase(null)
                    .transform.parent;
                    fanBaseRenderer.SetParent(entity.transform);
                    return fanBaseRenderer;
                }));

            PrefabsCreator.CreateEntity<PlateFoodTrap>(new EntityBuilder()
                .SetName("RawChichenGroundTrap")
                .SetLayer("ClickableEntities")
                .AddTrigger(1f)
                .SetLayerCollisionMask(LayersHelper.entityCollisionMask)
                .AddRenderbaseFunction(delegate (Entity entity)
                {
                    Transform fanBaseRenderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["food_plate"])
                    .transform.parent;
                    fanBaseRenderer.SetParent(entity.transform);
                    return fanBaseRenderer;
                }));

            PrefabsCreator.CreateEntity<PlateFoodTrap>(new EntityBuilder()
                .SetName("CookedChichenGroundTrap")
                .SetLayer("ClickableEntities")
                .AddTrigger(1f)
                .SetLayerCollisionMask(LayersHelper.entityCollisionMask)
                .AddRenderbaseFunction(delegate (Entity entity)
                {
                    Transform fanBaseRenderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["food_plate"])
                    .transform.parent;
                    fanBaseRenderer.SetParent(entity.transform);
                    return fanBaseRenderer;
                }),
                variant: 2);

            PrefabsCreator.CreateEntity<GroundDough>(new EntityBuilder()
                .SetName("Dough")
                .AddTrigger(1f)
                .SetLayerCollisionMask(LayersHelper.entityCollisionMask)
                .AddRenderbaseFunction(delegate (Entity entity)
                {
                    Transform fanBaseRenderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_dough"])
                    .transform.parent;
                    fanBaseRenderer.SetParent(entity.transform);
                    return fanBaseRenderer;
                }));
        }

        #endregion

        #region Internal Patches Corrections

        public static void CorrectPatches()
        {
            ItemManagerPatch.items.Add(EnumExtensions.GetFromExtendedName<Items>("Hammer"));
        }

        #endregion

        #region Door Materials Initialization

        public static void CreateDoorMats()
        {
            AssetsStorage.CreateDoorMats("adv_english_class", "adv_english_class_door");
            AssetsStorage.CreateDoorMats("adv_school_council", "adv_school_council_door");
            AssetsStorage.CreateDoorMats("adv_advanced_class", "adv_advanced_class_door");

            PrefabsCreator.CreateDoorMatSet("EnglishDoorSet", AssetsStorage.materials["adv_english_class_open"],
                AssetsStorage.materials["adv_english_class_closed"]);
            PrefabsCreator.CreateDoorMatSet("SchoolCouncilDoorSet", AssetsStorage.materials["adv_school_council_open"],
                AssetsStorage.materials["adv_school_council_closed"]);
            PrefabsCreator.CreateDoorMatSet("AdvancedClassDoorSet", AssetsStorage.materials["adv_advanced_class_open"],
                AssetsStorage.materials["adv_advanced_class_closed"]);
        }

        #endregion

        #region Rooms Initialization

        public static void InitializeRoomBasics()
        {
            InitializeRoomGroups();

            InitializeRoomFunctions();
        }

        private static void InitializeRoomFunctions()
        {
            PrefabsCreator.CreateFunctionContainerWithRoomFunction<EnglishClassTimerFunction>("EnglishClassTimerFunction");
            PrefabsCreator.CreateFunctionContainerWithRoomFunction<CorruptedLightsFunction>("CorruptedLightsFunction");
            PrefabsCreator.CreateFunctionContainerWithRoomFunction<SchoolCouncilFunction>("SchoolCouncilFunction");
            RoomHelper.SetupRoomFunction<DisabledPowerOnGenerationFinishFunction>(
                ObjectsStorage.RoomFunctionsContainers["SchoolCouncilFunction"]);
        }

        private static void InitializeRoomGroups()
        {
            PrefabsCreator.CreateRoomGroup("EnglishClass", minRooms: -6, maxRooms: 1)
                .SetBannedFloors(1)
                .ConvertTo<RoomGroupSpawningData>()
                .Group
                .SetCeilingTex(AssetsStorage.textures["adv_english_ceiling"], 100)
                .SetWallTex(AssetsStorage.textures["adv_english_wall"], 100)
                .SetFloorTex(AssetsStorage.textures["adv_english_floor"], 100);

            EnumExtensions.ExtendEnum<RoomCategory>("SchoolCouncil");
            EnumExtensions.ExtendEnum<RoomCategory>("CornField");
            /*PrefabsCreator.CreateRoomGroup("SchoolCouncil", minRooms: 0, maxRooms: 0, dontSpawn: true)
                .SetFalseEverywhere()
                .ConvertTo<RoomGroupSpawningData>()
                .Group
                .SetWallTex(AssetsStorage.textures["adv_school_council_wall"], 100);*/
        }

        public static void InitializeRoomAssets()
        {
            string[] filesPath = Directory.GetFiles(AssetsHelper.modPath + "Premades/Rooms/Objects", "*.cbld", SearchOption.AllDirectories);

            foreach (string path in filesPath)
            {
                string folderPath = path.Replace(Path.GetFileName(path), "");

                if (!File.Exists(folderPath + Path.GetFileNameWithoutExtension(path) + ".json")) 
                        continue;

                string jsonData = File.ReadAllText(folderPath + Path.GetFileNameWithoutExtension(path) + ".json");

                CustomRoomData roomData = JsonConvert.DeserializeObject<CustomRoomData>(jsonData);

                roomData.InheritProperties();

                RoomFunctionContainer funcContainer = null;

                if (!string.IsNullOrEmpty(roomData.functionContainerName))
                {
                    funcContainer = AssetsHelper.LoadAsset<RoomFunctionContainer>(roomData.functionContainerName);
                }

                RoomAsset roomAsset = RoomHelper.CreateAssetFromPath(path, 
                    roomData.offLimits == null ? false : (bool)roomData.offLimits,
                    roomData.autoAssignRoomFunctionContainer == null ? false : (bool)roomData.autoAssignRoomFunctionContainer,
                    funcContainer, isAHallway: roomData.isAHallway == null ? false : (bool)roomData.isAHallway,
                    keepTextures: roomData.keepTextures == null ? false : (bool)roomData.keepTextures);

                if (roomData.minItemValue != null) roomAsset.minItemValue = (int)roomData.minItemValue;
                if (roomData.maxItemValue != null) roomAsset.maxItemValue = (int)roomData.maxItemValue;

                if (!string.IsNullOrEmpty(roomData.doorMatsName))
                    roomAsset.doorMats = Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == roomData.doorMatsName);

                if (!string.IsNullOrEmpty(roomData.lightPre))
                {
                    roomAsset.lightPre = AssetsHelper.LoadAsset<Transform>(roomData.lightPre);
                }

                roomAsset.category = EnumExtensions.GetFromExtendedName<RoomCategory>(roomData.categoryName);

                roomData.roomAsset = roomAsset;

                ObjectsStorage.RoomDatas.Add(roomData);
            }
        }

        #endregion

        #region Kitchen Stove Recipe Posters Initialization
        public static void InitializeKitchenStovePosters()
        {
            int offsetX = -16;
            int offsetY = -250;

            IntVector2[] rawFoodPositions = new IntVector2[]
                { new IntVector2(39 + offsetX, 132 + offsetY), new IntVector2(77 + offsetX, 132 + offsetY), 
                    new IntVector2(39 + offsetX, 95 + offsetY), new IntVector2(77 + offsetX, 95 + offsetY) };
            IntVector2[] cookedFoodPositions = new IntVector2[]
                { new IntVector2(179 + offsetX, 132 + offsetY), new IntVector2(218 + offsetX, 132 + offsetY), 
                    new IntVector2(179 + offsetX, 95 + offsetY), new IntVector2(218 + offsetX, 95 + offsetY) };

            Texture2D tex = AssetsHelper.TextureFromFile("Textures/Posters/adv_poster_recipe_example.png");

            ExtendedPosterObject CreateRecipePoster(FoodRecipeData data, string name)
            {
                ExtendedPosterObject poster = ScriptableObject.CreateInstance<ExtendedPosterObject>();
                poster.name = name;
                poster.baseTexture = tex;
                poster.overlayData = new PosterImageData[data.RawFood.Length + data.CookedFood.Length];
                poster.textData = new PosterTextData[]
                {
                    new PosterTextData()
                    {
                        textKey = "Adv_PST_Recipe_1",
                        position = new IntVector2(32, 165),
                        color = Color.black,
                        alignment = TextAlignmentOptions.Center,
                        style = FontStyles.Bold,
                        font = BaldiFonts.ComicSans24.FontAsset(),
                        fontSize = (int)BaldiFonts.ComicSans24.FontSize(),
                        size = new IntVector2(200, 50)
                    },
                    new PosterTextData()
                    {
                        textKey = "Adv_PST_Recipe_2",
                        position = new IntVector2(32, 140),
                        color = Color.black,
                        alignment = TextAlignmentOptions.Center,
                        style = FontStyles.Normal,
                        font = BaldiFonts.ComicSans12.FontAsset(),
                        fontSize = (int)BaldiFonts.ComicSans12.FontSize(),
                        size = new IntVector2(200, 50)
                    },
                    new PosterTextData()
                    {
                        textKey = "Adv_PST_Recipe_3",
                        position = new IntVector2(32, 128),
                        color = Color.black,
                        alignment = TextAlignmentOptions.Center,
                        style = FontStyles.Normal,
                        font = BaldiFonts.ComicSans12.FontAsset(),
                        fontSize = (int)BaldiFonts.ComicSans12.FontSize(),
                        size = new IntVector2(200, 50)
                    }
                };

                for (int i = 0; i < data.RawFood.Length; i++)
                {
                    poster.overlayData[i] = 
                        new PosterImageData(data.RawFood[i].itemSpriteSmall.texture, rawFoodPositions[i], 
                        new IntVector2(32, 32));
                }

                for (int i = 0; i < data.CookedFood.Length; i++)
                {
                    poster.overlayData[data.RawFood.Length + i] = 
                        new PosterImageData(data.CookedFood[i].itemSpriteSmall.texture, cookedFoodPositions[i], 
                        new IntVector2(32, 32));
                }

                return poster;
            }

            List<FoodRecipeData> datas = ApiManager.GetAllKitchenStoveRecipes();
            List<PosterObject> posters = new List<PosterObject>();

            for (int i = 0; i < datas.Count; i++)
            {
                posters.Add(CreateRecipePoster(datas[i], "recipe_" + i));
            }

            datas.Clear();

            int weight = 100 / posters.Count;

            for (int i = 0; i < posters.Count; i++)
            {
                foreach (LevelObject level in AssetsHelper.LoadAssets<LevelObject>())
                {
                    level.posters = level.posters.AddToArray(new WeightedPosterObject()
                    {
                        selection = posters[i],
                        weight = weight
                    });
                }
                /*ObjectsStorage.WeightedPosterObjects.Add(
                    new WeightedPosterObject()
                    {
                        selection = posters[i],
                        weight = weight
                    }
                );*/
            }

            posters.Clear();

        }

        #endregion

        #region Generic Posters Initialization
        public static void InitializePosters()
        {
            PosterTextData[] emptyTextArray = new PosterTextData[0];

            foreach (string path in Directory.GetFiles(
                AssetsHelper.modPath + "Textures/Posters/GenericPosters/", "*.png", SearchOption.AllDirectories))
            {
                string jsonPath = path.Replace(".png", ".json");

                if (!File.Exists(jsonPath)) throw new Exception("Json for generic poster is missing!");

                PosterSerializableData posterData = PosterSerializableData.GetFromFile(jsonPath);

                ObjectsStorage.WeightedPosterObjects.Add(new WeightedPosterObject()
                {
                    selection = ObjectCreators.CreatePosterObject(
                        AssetsHelper.TextureFromFile(path, overrideBasePath: true),
                            posterData.Texts != null ? posterData.Texts : emptyTextArray),
                    weight = posterData.weight
                });

                ObjectsStorage.WeightedPosterObjects[ObjectsStorage.WeightedPosterObjects.Count - 1]
                    .selection.name = Path.GetFileNameWithoutExtension(path);
            }
        }

        #endregion

        #region Tags Management

        public static void SetTags()
        {
            SetTagsTo(new string[] { TagsStorage.firstPrizeImmunity },
                Character.Bully, Character.Sweep, Character.Prize);

            SetTagsTo(new string[] { TagsStorage.coldSchoolEventImmunity },
                Character.Pomp, Character.Sweep, Character.Prize, Character.Chalkles);

            SetTagsTo(new string[] { TagsStorage.disappearingCharactersEventImmunity },
                Character.Baldi, Character.Principal);

            SetTagsTo(new string[] { TagsStorage.narrowlyFunctional },
                Items.BusPass, Items.lostItem0, Items.lostItem1, Items.lostItem2, Items.lostItem3,
                Items.lostItem4, Items.lostItem5, Items.lostItem6, Items.lostItem7, Items.lostItem8, Items.lostItem9,
                Items.CircleKey, Items.TriangleKey, Items.SquareKey, Items.PentagonKey, Items.HexagonKey, Items.WeirdKey);
            SetTagsTo(new string[] { TagsStorage.perfectRate, TagsStorage.symbolMachinePotentialReward },
                Items.GrapplingHook, Items.Apple, Items.Bsoda, Items.Teleporter);
            SetTagsTo(new string[] { TagsStorage.goodRate, TagsStorage.symbolMachinePotentialReward },
                Items.PortalPoster, Items.NanaPeel, Items.Quarter, Items.ZestyBar, Items.DietBsoda);
            SetTagsTo(new string[] { TagsStorage.normalRate, TagsStorage.symbolMachinePotentialReward },
                Items.Nametag, Items.ChalkEraser, Items.DetentionKey);
            SetTagsTo(new string[] { TagsStorage.commonRate, TagsStorage.symbolMachinePotentialReward },
                Items.Scissors, Items.Tape, Items.PrincipalWhistle, Items.Wd40);
        }

        private static void SetTagsTo(string[] tags, params Character[] characters)
        {
            foreach (NPCMetadata metadata in Array.FindAll(NPCMetaStorage.Instance.All(), x => characters.Contains(x.character)))
            {
                foreach (string tag in tags)
                {
                    if (!metadata.tags.Contains(tag)) metadata.tags.Add(tag);
                }
            }
        }

        private static void SetTagsTo(string[] tags, params Items[] items)
        {
            foreach (ItemMetaData metadata in Array.FindAll(ItemMetaStorage.Instance.All(), x => items.Contains(x.id)))
            {
                foreach (string tag in tags)
                {
                    if (!metadata.tags.Contains(tag)) metadata.tags.Add(tag);
                }
            }
        }

        #endregion

    }
}