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
using UnityEngine.UI;
using MTM101BaldAPI.UI;
using TMPro;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using MTM101BaldAPI.ObjectCreation;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using BaldisBasicsPlusAdvanced.Game.Objects.Triggers;
using BaldisBasicsPlusAdvanced.Game.Objects.Voting;

namespace BaldisBasicsPlusAdvanced.Managers
{
    public class GameRegisterManager
    {

        public static void InitializeGameItems()
        {
            PrefabsCreator.CreateItem<HammerItem>(
                nameKey: "ItemAdv_Hammer",
                descKey: "ItemAdv_Hammer_Desc",
                enumName: "Hammer",
                weight: 50,
                smallSpriteFileName: "adv_hammer_small.png",
                largeSpriteFileName: "adv_hammer_large.png",
                generatorCost: 75,
                price: 500,
                flags: ItemFlags.None,
                tags: new string[]
                {
                    "adv_repair_tool"
                }
                )
                .SetSpawnsOnRooms(true)
                .SetSpawnsOnFieldTrips(true)
                .SetSpawnsOnParty(true)
                .SetSpawnsOnMysteryRooms(true)
                .SetEndless(true)
                .SetBannedFloors(1);

            PrefabsCreator.CreateItem<WindBlowerItem>(
                nameKey: "ItemAdv_WindBlower",
                descKey: "ItemAdv_WindBlower_Desc",
                enumName: "WindBlower",
                weight: 75,
                smallSpriteFileName: "adv_wind_blower_small.png",
                largeSpriteFileName: "adv_wind_blower_large.png",
                generatorCost: 50,
                price: 300,
                flags: ItemFlags.None
                );

            PrefabsCreator.CreateItem<MysteriousTeleporterItem>(
                nameKey: "ItemAdv_MysteriousTeleporter",
                descKey: "ItemAdv_MysteriousTeleporter_Desc",
                enumName: "MysteriousTeleporter",
                weight: 35,
                smallSpriteFileName: "adv_mysterious_teleporter_small.png",
                largeSpriteFileName: "adv_mysterious_teleporter_large.png",
                generatorCost: 40,
                price: 500,
                flags: ItemFlags.CreatesEntity
                )
                .SetSpawnsOnParty(true)
                .SetSpawnsOnMysteryRooms(true)
                .SetEndless(true)
                .SetBannedFloors(1);

            PrefabsCreator.CreateItem<InvisibilityPotionItem>(
                nameKey: "ItemAdv_InvisibilityPotion",
                descKey: "ItemAdv_InvisibilityPotion_Desc",
                enumName: "InvisibilityPotion",
                weight: 50,
                smallSpriteFileName: "adv_invisibility_potion_small.png",
                largeSpriteFileName: "adv_invisibility_potion_large.png",
                generatorCost: 40,
                price: 500,
                flags: ItemFlags.None
                )
                .SetSpawnsOnFieldTrips(true)
                .SetSpawnsOnParty(true);

            PrefabsCreator.CreateItem<IceBootsItem>(
                nameKey: "ItemAdv_IceBoots",
                descKey: "ItemAdv_IceBoots_Desc",
                enumName: "IceBoots",
                weight: 75,
                smallSpriteFileName: "adv_ice_boots_small.png",
                largeSpriteFileName: "adv_ice_boots_large.png",
                generatorCost: 50,
                price: 300,
                flags: ItemFlags.Persists
                )
                .SetSpawnsOnFieldTrips(true)
                .SetEndless(true);

            PrefabsCreator.CreateItem<PlaceableFanItem>(
                nameKey: "ItemAdv_PlaceableFan",
                descKey: "ItemAdv_PlaceableFan_Desc",
                enumName: "PlaceableFan",
                weight: 25,
                flags: ItemFlags.CreatesEntity,
                smallSpriteFileName: "adv_placeable_fan_small.png",
                largeSpriteFileName: "adv_placeable_fan_large.png",
                generatorCost: 75,
                price: 750
                );

            PrefabsCreator.CreateItem<TeleportationBombItem>(
                nameKey: "ItemAdv_TeleportationBomb",
                descKey: "ItemAdv_TeleportationBomb_Desc",
                enumName: "TeleportationBomb",
                weight: 50,
                flags: ItemFlags.CreatesEntity | ItemFlags.Persists,
                smallSpriteFileName: "adv_teleportation_bomb_small.png",
                largeSpriteFileName: "adv_teleportation_bomb_large.png",
                generatorCost: 75,
                price: 500
                )
                .SetSpawnsOnFieldTrips(true)
                .SetSpawnsOnParty(true)
                .SetBannedFloors(1)
                .SetEndless(true);

        }

        public static void InitializeMultipleUsableItems()
        {

            ItemObject boxingGlove2 = PrefabsCreator.CreateItem<BoxingGloveItem>(
                nameKey: "ItemAdv_BoxingGlove",
                descKey: "ItemAdv_BoxingGlove_Desc",
                enumName: "BoxingGlove",
                weight: 15,
                smallSpriteFileName: "adv_boxing_glove_small.png",
                largeSpriteFileName: "adv_boxing_glove_large.png",
                generatorCost: 50,
                price: 500,
                flags: ItemFlags.MultipleUse
                )
                .SetSpawnsOnFieldTrips(true)
                .ItemObject;

            ItemObject boxingGlove1 = PrefabsCreator.CreateItem<BoxingGloveItem>(
                nameKey: "ItemAdv_BoxingGlove1",
                descKey: "ItemAdv_BoxingGlove_Desc",
                enumName: "BoxingGlove1",
                weight: 15,
                smallSpriteFileName: "adv_boxing_glove_small.png",
                largeSpriteFileName: "adv_boxing_glove_large.png",
                generatorCost: 50,
                price: 500,
                flags: ItemFlags.MultipleUse,
                itemMeta: boxingGlove2.GetMeta()
                )
                .SetFalseEverywhere()
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject portal2 = PrefabsCreator.CreateItem<PlaceablePortalItem>(
                nameKey: "ItemAdv_PlaceablePortal",
                descKey: "ItemAdv_PlaceablePortal_Desc",
                enumName: "PlaceablePortal",
                weight: 30,
                smallSpriteFileName: "adv_placeable_portal_small.png",
                largeSpriteFileName: "adv_placeable_portal_large.png",
                generatorCost: 60,
                price: 750,
                flags: ItemFlags.MultipleUse
                )
                .SetSpawnsOnFieldTrips(true)
                .SetEndless(true)
                .SetBannedFloors(1)
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            ItemObject portal1 = PrefabsCreator.CreateItem<PlaceablePortalItem>(
                nameKey: "ItemAdv_PlaceablePortal1",
                descKey: "ItemAdv_PlaceablePortal_Desc",
                enumName: "PlaceablePortal1",
                weight: 30,
                smallSpriteFileName: "adv_placeable_portal_small.png",
                largeSpriteFileName: "adv_placeable_portal_large.png",
                generatorCost: 60,
                price: 750,
                flags: ItemFlags.MultipleUse,
                itemMeta: portal2.GetMeta()
                )
                .SetFalseEverywhere()
                .ConvertTo<ItemSpawningData>()
                .ItemObject;

            //metas
            boxingGlove2.item.ConvertTo<BaseMultipleUsableItem>().Initialize(boxingGlove1);
            boxingGlove2.GetMeta().itemObjects = boxingGlove2.item.ConvertTo<BaseMultipleUsableItem>().AllVersions.AddItem(boxingGlove2).ToArray();
            boxingGlove1.AddMeta(boxingGlove2.GetMeta());

            portal2.item.ConvertTo<BaseMultipleUsableItem>().Initialize(portal1);
            portal2.GetMeta().itemObjects = portal2.item.ConvertTo<BaseMultipleUsableItem>().AllVersions.AddItem(portal2).ToArray();
            portal1.AddMeta(portal2.GetMeta());
        }

        public static void InitializeGameEvents()
        {
            PrefabsCreator.CreateEvent<DisappearingCharactersEvent>(
                name: "Disappearing Characters",
                soundKey: "adv_bal_event_disappearing_characters",
                enumName: "DisappearingCharacters",
                weight: 40,
                minTime: 60f,
                maxTime: 80f,
                flags: RandomEventFlags.None
                )
                .SetEndless(true)
                .SetBannedFloors(1, 3);

            PrefabsCreator.CreateEvent<ColdSchoolEvent>(
                name: "Cold School",
                soundKey: "adv_bal_event_cold_machine",
                enumName: "ColdSchool",
                weight: 50,
                minTime: 60f,
                maxTime: 90f,
                flags: RandomEventFlags.None
                )
                .SetEndless(true)
                .SetBannedFloors(1);

            PrefabsCreator.CreateEvent<PortalChaosEvent>(
                name: "Portal Chaos",
                enumName: "PortalChaos",
                soundKey: "adv_bal_event_portals",
                weight: 40,
                minTime: 60f,
                maxTime: 90f,
                flags: RandomEventFlags.None
                )
                .SetEndless(true)
                .SetBannedFloors(1);

            PrefabsCreator.CreateEvent<VotingEvent>(
                name: "Voting",
                enumName: "Voting",
                soundKey: "adv_bal_event_voting",
                weight: 40000000,
                minTime: 100f,
                maxTime: 140f,
                flags: RandomEventFlags.AffectsGenerator
                )
                .SetEndless(true)
                .SetBannedFloors(1);
        }

        public static void InitializeVendingMachines()
        {
            List<ItemObject> items = Singleton<PlayerFileManager>.Instance.itemObjects;

            PrefabsCreator.CreateMultipleRequiredVendingMachine("GoodMachine", items[12], 2,
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

        public static void InitializeObjectBuilders()
        {
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GenericHallBuilder>(
                "GoodMachineHallsBuilder", 20, 1, 1, ObjectsStorage.SodaMachines["GoodMachine"].gameObject, false)
                .SetEndless(true);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GenericHallBuilder>(
                "InvisibilityPlatesHallsBuilder", 40, 1, 2, ObjectsStorage.GameButtons["invisibility_plate"].gameObject, false)
                .SetEndless(true)
                .SetBannedFloors(1);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<NoisyPlatesBuilder>(
                "NoisyPlatesHallsBuilder", 20, 1, 2, ObjectsStorage.GameButtons["noisy_plate"].gameObject, false) 
                .SetEndless(true)
                .SetBannedFloors(1, 3);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GenericHallBuilder>(
                "BullyPlatesHallsBuilder", 15, 1, 2, ObjectsStorage.GameButtons["bully_plate"].gameObject, false)
                .SetEndless(true)
                .SetBannedFloors(1);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<AccelerationPlatesBuilder>(
                "AccelerationPlatesHallsBuilder", 50, 2, 5, ObjectsStorage.GameButtons["acceleration_plate"].gameObject, false)
                .SetEndless(true)
                .SetBannedFloors(1);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<PressurePlatesBuilder>(
                "PlatesBuilder", 50, 1, 3, ObjectsStorage.GameButtons["plate"].gameObject, isForced: true)
                .SetEndless(true)
                .SetBannedFloors(1, 3);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GenericHallBuilder>(
                "PresentPlatesBuilder", 20, 1, 2, ObjectsStorage.GameButtons["present_plate"].gameObject, false)
                .SetBannedFloors(1, 3);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GenericHallBuilder>(
                "SlowdownPlatesBuilder", 30, 1, 3, ObjectsStorage.GameButtons["slowdown_plate"].gameObject, false)
                .SetEndless(true)
                .SetBannedFloors(1);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GenericHallBuilder>(
                "SugarPlatesBuilder", 20, 1, 2, ObjectsStorage.GameButtons["sugar_addiction_plate"].gameObject, false)
                .SetBannedFloors(1, 3);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GenericHallBuilder>(
                "ProtectionPlatesBuilder", 30, 1, 2, ObjectsStorage.GameButtons["protection_plate"].gameObject, false)
                .SetBannedFloors(1);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GenericHallBuilder>(
                "TeleportationPlatesBuilder", 20, 2, 3, ObjectsStorage.GameButtons["teleportation_plate"].gameObject, false)
                .SetBannedFloors(1);
            PrefabsCreator.CreateWeigthedHallObjectsBuilder<GumDispenserBuilder>(
                "GumDispenserBuilder", 40, false)
                .SetBannedFloors(1);
        }

        public static void InitializeUI()
        {
            PrefabsCreator.CreateOverlay("FrozenOverlay", AssetsStorage.sprites["adv_frozen_overlay"], false);
            PrefabsCreator.CreateOverlay("ElephantOverlay", AssetsStorage.sprites["adv_elephant_overlay"], true);
            PrefabsCreator.CreateOverlay("BullyOverlay", AssetsStorage.sprites["adv_bully_overlay"], true);
            PrefabsCreator.CreateOverlay("ShieldOverlay", AssetsStorage.sprites["adv_protected_overlay"], true);

            //initializing Chalkboard Menu
            Canvas canvas = ObjectsCreator.CreateCanvas(setGlobalCam: true);
            canvas.name = "Chalkboard Menu";
            GraphicRaycaster graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();

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

            Image exitImage = UIHelpers.CreateImage(AssetsStorage.sprites["adv_exit_transparent"], chalkboardMenu.chalkboard.transform,
                Vector3.zero, correctPosition: false);
            exitImage.name = "exit";

            exitImage.ToCenter();

            exitImage.transform.localPosition = new Vector3(185, 140);
            exitImage.tag = "Button";

            StandardMenuButton exit = exitImage.gameObject.AddComponent<StandardMenuButton>();

            chalkboardMenu.buttons.Add(exit);
            exit.image = exitImage;
            exit.heldSprite = AssetsStorage.sprites["adv_exit"];
            exit.unhighlightedSprite = AssetsStorage.sprites["adv_exit_transparent"];
            exit.highlightedSprite = AssetsStorage.sprites["adv_exit"];

            exit.swapOnHold = true; //on press
            exit.swapOnHigh = true; //on high
            exit.InitializeAllEvents();

            canvas.gameObject.ConvertToPrefab(true);

            CursorInitiator cursorInitiator = canvas.gameObject.AddComponent<CursorInitiator>();
            cursorInitiator.cursorPre = AssetsStorage.cursor;
            cursorInitiator.graphicRaycaster = graphicRaycaster;
            canvas.gameObject.AddComponent<CursorAutoInitiator>().initiator = cursorInitiator;

            ObjectsStorage.Objects.Add("chalkboard_menu", canvas.gameObject);
        }

        public static void InitializeTipsAndWords()
        {
            
            LocalizationManager localization = Singleton<LocalizationManager>.Instance;

            string[] tipsBaseNames = new string[1];

            tipsBaseNames[0] = "Adv_Elv_Tip";

            foreach (string tipBaseName in tipsBaseNames)
            {
                if (tipBaseName == null) continue;
                int num = 1;
                string tipKey = tipBaseName + num;

                List<string> tips = new List<string>();

                while (localization.Exists(tipKey))
                {
                    tips.Add(tipKey);
                    tipKey = tipBaseName + ++num;
                }

                ApiManager.AddNewTips(AdvancedCore.Instance.Info, tips.ToArray());
            }

            ApiManager.AddNewSymbolMachineWords(AdvancedCore.Instance.Info,
                "BSODA",
                "Beans",
                "Baldi",
                "Bully",
                "boots",
                "learn",
                "brain",
                "ruler",
                "floor",
                "class",
                "flood",
                "party",
                "apple",
                "chalk",
                "erase",
                "plate",
                "store",
                "plate",
                "math"
            );

        }

        public static void InitializeObjects()
        {
            //Independent sound source

            /*AudioManager audMan = ObjectsCreator.createAudMan(Vector3.zero);
            audMan.name = "Independent Audio Manager";
            audMan.ignoreListenerPause = true;
            audMan.positional = false;
            audMan.audioDevice.dopplerLevel = 0; //no 3D sound. No matter the distance
            audMan.audioDevice.ignoreListenerPause = true;

            audMan.audioDevice.pitch = 1f;
            GameObject.DontDestroyOnLoad(audMan);
            ObjectsStorage.IndependentAudMan = audMan;*/

            //Advanced Math Machine
            MathMachine mathMachineComp1 = GameObject.Instantiate(AssetsHelper.LoadAsset<MathMachine>("MathMachine"));

            GameObject advancedMathMachineObj = mathMachineComp1.gameObject;
            advancedMathMachineObj.name = "AdvancedMathMachine";

            AdvancedMathMachine advancedMathMachine = advancedMathMachineObj.AddComponent<AdvancedMathMachine>();
            mathMachineComp1.CopyAllValuesTo(advancedMathMachine);
            advancedMathMachine.InitializePrefab();

            GameObject.Destroy(mathMachineComp1);

            advancedMathMachineObj.ConvertToPrefab(true);
            ObjectsStorage.Objects.Add("advanced_math_machine", advancedMathMachineObj);

            //Own gum
            Gum gumPre = AssetsHelper.LoadAsset<Gum>("Gum");

            Gum gumComp = GameObject.Instantiate(gumPre);

            GumProjectile gumProj = gumComp.gameObject.AddComponent<GumProjectile>();

            gumProj.Speed = ReflectionHelper.GetValue<float>(gumComp, "speed");
            gumProj.canvas = ReflectionHelper.GetValue<Canvas>(gumComp, "canvas");
            gumProj.moveMod = ReflectionHelper.GetValue<MovementModifier>(gumComp, "moveMod");
            gumProj.playerMod = ReflectionHelper.GetValue<MovementModifier>(gumComp, "playerMod");
            gumProj.groundedSprite = ReflectionHelper.GetValue<GameObject>(gumComp, "groundedSprite");
            gumProj.flyingSprite = ReflectionHelper.GetValue<GameObject>(gumComp, "flyingSprite");

            gumProj.InitializePrefab();

            GameObject.Destroy(gumComp);

            ObjectsStorage.Objects.Add("gum", gumProj.gameObject);
            gumProj.gameObject.ConvertToPrefab(true);

            //gum end

            //Voting Ballot
            PrefabsCreator.CreateObjectPrefab<VotingBallot>("Voting Ballot", "voting_ballot");

            //Gum Dispenser
            PrefabsCreator.CreateObjectPrefab<GumDispenser>("Gum Dispenser", "gum_dispenser");

            //Mysterious Teleporter
            PrefabsCreator.CreateObjectPrefab<MysteriousTeleporterProjectile>("Mysterious Teleporter", "mysterious_teleporter");

            //Mysterious portal
            PrefabsCreator.CreateObjectPrefab<MysteriousPortal>("Mysterious portal", "mysterious_portal");

            //plates

            PrefabsCreator.CreatePlate<PressurePlate>("plate");
            PrefabsCreator.CreatePlate<InvisibilityPlate>("invisibility_plate");
            PrefabsCreator.CreatePlate<AccelerationPlate>("acceleration_plate");
            PrefabsCreator.CreatePlate<NoisyPlate>("noisy_plate");
            PrefabsCreator.CreatePlate<StealingPlate>("stealing_plate");
            PrefabsCreator.CreatePlate<BullyPlate>("bully_plate");
            PrefabsCreator.CreatePlate<PresentPlate>("present_plate");
            PrefabsCreator.CreatePlate<SlowdownPlate>("slowdown_plate");
            PrefabsCreator.CreatePlate<SugarPlate>("sugar_addiction_plate");
            PrefabsCreator.CreatePlate<ProtectionPlate>("protection_plate");
            PrefabsCreator.CreatePlate<TeleportationPlate>("teleportation_plate");

            //plates end

            //triggers

            PrefabsCreator.CreateTrigger<NoPlatesCooldownTrigger>("no_plates_cooldown");
            PrefabsCreator.CreateTrigger<PitStopOverridesTrigger>("pit_stop_overrides");

            //triggers end

            //spelling
            MathMachine mathMachineComp2 = GameObject.Instantiate(AssetsHelper.LoadAsset<MathMachine>("MathMachine"));
            GameObject symbolMachineObj = mathMachineComp2.gameObject;
            symbolMachineObj.name = "SymbolMachine";
            GameObject.Destroy(mathMachineComp2);
            SymbolMachine symbolMachine = symbolMachineObj.AddComponent<SymbolMachine>();
            //symbolMachine.initializePrefabPre(ReflectionHelper.getValue<MeshRenderer>(mathMachineComp, "meshRenderer"));
            symbolMachine.InitializePrefab();
            symbolMachineObj.ConvertToPrefab(true);
            ObjectsStorage.Objects.Add("symbol_machine", symbolMachineObj);

            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            
            for (int i = 0; i < alphabet.Length; i++)
            {
                string symbol = alphabet.ElementAt(i).ToString();
                MathMachineNumber mathNumComp = GameObject.Instantiate(AssetsHelper.LoadAsset<MathMachineNumber>("MathNum_0"));
                GameObject.Destroy(mathNumComp);
                Spelloon spelloon = mathNumComp.gameObject.AddComponent<Spelloon>();
                spelloon.name = "Spelloon_" + symbol;
                spelloon.InitializePrefab();
                spelloon.InitializePrefabPost(symbol);
                alphabet.ElementAt(1).ToString();
                spelloon.gameObject.ConvertToPrefab(true);
                ObjectsStorage.Spelloons.Add("spelloon_" + symbol, spelloon);
            }
            //spelling end

            //teleportation bomb
            PrefabsCreator.CreateObjectPrefab<TeleportationBomb>("Teleportation Bomb", "teleportation_bomb");
        }

        public static void InitializeEntities()
        {
            //fan
            Transform fanBaseRenderer = null;

            Entity fanEntity = new EntityBuilder()
                .SetName("Fan")
                .SetLayer("ClickableEntities")
                .AddTrigger(1f)
                .SetLayerCollisionMask((LayerMask)2113541)
                .AddRenderbaseFunction(delegate (Entity entity)
                {
                    fanBaseRenderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["adv_fan_face_1"])
                    .transform.parent;
                    fanBaseRenderer.SetParent(entity.transform);
                    return fanBaseRenderer;
                })
                .Build();

            fanEntity.gameObject.AddComponent<Fan>().PrefabInitialization(fanEntity, fanBaseRenderer.GetComponentInChildren<SpriteRenderer>());

            ObjectsStorage.Entities.Add("fan", fanEntity);
        }

        public static void SetTags()
        {
            SetTagsTo(new string[] { "adv_exclusion_hammer_immunity" },
                Character.Baldi, Character.Principal, Character.Pomp, Character.Sweep, Character.Cumulo, Character.DrReflex,
                Character.LookAt, Character.Null);

            SetTagsTo(new string[] { "adv_exclusion_hammer_weakness" },
                Character.Beans, Character.Crafters, Character.Bully, Character.Chalkles, Character.Prize);

            SetTagsTo(new string[] { "adv_ev_cold_school_immunity" },
                Character.Pomp, Character.Sweep, Character.Prize, Character.Chalkles);

            SetTagsTo(new string[] { "adv_ev_disappearing_characters_immunity" },
                Character.Baldi, Character.Principal, Character.Null);

            SetTagsTo(new string[] { "adv_perfect", "adv_sm_potential_reward" },
                Items.Apple, Items.GrapplingHook);
            SetTagsTo(new string[] { "adv_good", "adv_sm_potential_reward" },
                Items.PortalPoster, Items.Bsoda, Items.Teleporter);
            SetTagsTo(new string[] { "adv_normal", "adv_sm_potential_reward" },
                Items.NanaPeel, Items.Nametag, Items.ChalkEraser, Items.Quarter, Items.ZestyBar, Items.DietBsoda);
            SetTagsTo(new string[] { "adv_common", "adv_sm_potential_reward" },
                Items.Scissors, Items.DetentionKey, Items.Tape, Items.PrincipalWhistle, Items.Wd40);
        }

        public static void InitializeRoomBasics()
        {
            InitializeRoomGroups();

            InitializeRoomFunctions();
        }

        private static void InitializeRoomFunctions()
        {
            PrefabsCreator.CreateFunctionContainerWithRoomFunction<EnglishClassTimerFunction>("EnglishClassTimerFunction");
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

            PrefabsCreator.CreateRoomGroup("SchoolCouncil", minRooms: 0, maxRooms: 0);
                //.SetCeilingTex(AssetsStorage.textures["adv_english_ceiling"], 100)
                //.SetWallTex(AssetsStorage.textures["adv_english_wall"], 100)
                //.SetFloorTex(AssetsStorage.textures["adv_english_floor"], 100);
        }

        public static void InitializeRoomAssets()
        {
            string[] filesPath = Directory.GetFiles(AssetsHelper.modPath + "Premades/Rooms/");

            foreach (string path in filesPath)
            {
                string extensionName = Path.GetExtension(path);

                if (extensionName != ".cbld") continue;

                if (!File.Exists(AssetsHelper.modPath + "Premades/Rooms/" + Path.GetFileNameWithoutExtension(path) + ".json")) continue;

                string jsonData = File.ReadAllText(AssetsHelper.modPath + "Premades/Rooms/" + Path.GetFileNameWithoutExtension(path) + ".json");

                CustomRoomData roomData = JsonUtility.FromJson<CustomRoomData>(jsonData);

                RoomAsset roomAsset = RoomHelper.CreateAssetFromPath(path, roomData.isOffLimits, null, isAHallway: roomData.isAHallway,
                    keepTextures: roomData.keepTextures);

                roomAsset.minItemValue = roomData.minItemValue;
                roomAsset.maxItemValue = roomData.maxItemValue;

                if (!string.IsNullOrEmpty(roomData.doorMatsName))
                    roomAsset.doorMats = Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == roomData.doorMatsName);

                roomAsset.category = EnumExtensions.GetFromExtendedName<RoomCategory>(roomData.categoryName);

                roomData.weightedRoomAsset = new WeightedRoomAsset()
                {
                    selection = roomAsset,
                    weight = roomData.weight
                };

                ObjectsStorage.RoomDatas.Add(roomData);
                RoomAssetMetaStorage.Instance.Add(new RoomAssetMeta(AdvancedCore.Instance.Info, roomAsset));
            }
        }

        private static void SetTagsTo(string[] tags, params Character[] characters)
        {
            foreach (NPCMetadata metadata in Array.FindAll(NPCMetaStorage.Instance.All(), x => characters.Contains(x.character)))
            {
                metadata.tags.AddRange(tags);
            }
        }

        private static void SetTagsTo(string[] tags, params Items[] characters)
        {
            foreach (ItemMetaData metadata in Array.FindAll(ItemMetaStorage.Instance.All(), x => characters.Contains(x.id)))
            {
                metadata.tags.AddRange(tags);
            }
        }

        public static void InitializePosters()
        {
            int textureNum = 1;
            while (true)
            {
                string basePath = AssetsHelper.modPath + "Textures/Posters/adv_poster_" + textureNum;

                string imagePath = basePath + ".png";
                string jsonPath = basePath + ".json";

                if (!File.Exists(imagePath)) break;
                if (!File.Exists(jsonPath)) continue;

                PosterSpawningData posterData = JsonUtility.FromJson<PosterSpawningData>(File.ReadAllText(jsonPath));

                for (int i = 0; i < posterData.fonts.Length; i++)
                {
                    Enum.TryParse(posterData.fonts[i], out BaldiFonts font);
                    Enum.TryParse(posterData.alignments[i], out TextAlignmentOptions alignment);
                    posterData.posterTextDatas[i].font = font.FontAsset();//Enum.Parse<BaldiFonts>(posterData.fonts[i]).FontAsset();
                    posterData.posterTextDatas[i].alignment = alignment;
                }

                //alignment

                ObjectsStorage.WeightedPosterObjects.Add(new WeightedPosterObject()
                {
                    selection = ObjectCreators.CreatePosterObject(AssetsHelper.TextureFromFile("Textures/Posters/adv_poster_" + textureNum + ".png"),
                    posterData.posterTextDatas),
                    weight = posterData.weight
                });

                textureNum++;
            }
        }
    }
}
