﻿using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor;
using BaldisBasicsPlusAdvanced.Game.Components.UI.Elevator;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.SerializableData;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Cache.AssetsManagement
{

    public class AssetsStorage
    {

        public class AssetDictionary<T>
        {
            private Dictionary<string, T> instances = new Dictionary<string, T>();

            private List<string> ignoreInstances = new List<string>();

            public void Add(string key, T instance, bool doNotUnload = false)
            {
                instances.Add(key, instance);
                if (doNotUnload) ignoreInstances.Add(key);
            }

            public void Clear()
            {
                for (int i = 0; i < instances.Count; i++)
                {
                    if (!ignoreInstances.Contains(instances.Keys.ElementAt(i)))
                    {
                        instances.Remove(instances.Keys.ElementAt(i));
                    }
                }
            }

            public T this[string i]
            {
                get
                {
                    return instances[i];
                }
                set
                {
                    instances[i] = value;
                }
            }

        }

        private static bool cached = false;

        private static bool overridden = false;

        public static Exception exception;

        public static bool Debugging => false;

        public static bool Cached => cached;

        public static bool Overridden => overridden;

        public static readonly AssetDictionary<SoundObject> sounds = new AssetDictionary<SoundObject>();

        public static readonly AssetDictionary<Sprite> sprites = new AssetDictionary<Sprite>();

        public static readonly AssetDictionary<Sprite[]> spriteSheets = new AssetDictionary<Sprite[]>();

        public static readonly AssetDictionary<Texture2D> textures = new AssetDictionary<Texture2D>();

        public static readonly AssetDictionary<ItemObject> itemObjects = new AssetDictionary<ItemObject>();

        public static readonly AssetDictionary<GameObject> gameObjects = new AssetDictionary<GameObject>();

        public static readonly AssetDictionary<Material> materials = new AssetDictionary<Material>();

        public static readonly AssetDictionary<TMP_Text> texts = new AssetDictionary<TMP_Text>();

        public static readonly AssetDictionary<Mesh> meshes = new AssetDictionary<Mesh>();


        //game assets

        public static Shader graphsStandardShader;

        public static readonly Baldi genericBaldi;

        public static readonly Structure_EnvironmentObjectPlacer weightedPlacer;

        public static readonly Structure_EnvironmentObjectPlacer individualPlacer;

        public static readonly BeltManager windManager;

        public static readonly Pickup pickup;

        public static readonly Transform windGraphicsParent;

        public static readonly AudioClip weirdErrorSound;

        public static readonly TextAsset campingMidi;

        //public static readonly GameObject exitConfirmChalkboardStore;

        public static readonly GameButton gameButton;

        public static readonly CoverCloud coverCloud;

        public static readonly SoundObject[] bullyTakeouts;

        public static readonly StandardDoor classDoor;

        public static readonly CursorController cursor;

        //mod assets

        #region Main

        static AssetsStorage()
        {
            //priority game assets
            try
            {
                Stopwatch sw = null;

                if (Debugging)
                {
                    sw = new Stopwatch();
                    sw.Start();
                }

                weirdErrorSound = AssetsHelper.LoadAsset<AudioClip>("WeirdError");
                LoadSound("buzz_elv", "Elv_Buzz");

                graphsStandardShader = Shader.Find("Shader Graphs/Standard");
                genericBaldi = AssetsHelper.LoadAsset<Baldi>("Baldi_Main1");

                campingMidi = AssetsHelper.LoadAsset<TextAsset>("Minigame_Campfire");

                weightedPlacer = AssetsHelper.LoadAsset<Structure_EnvironmentObjectPlacer>("Structure_EnvironmentObjectBuilder_Weighted");
                individualPlacer = AssetsHelper.LoadAsset<Structure_EnvironmentObjectPlacer>("Structure_EnvironmentObjectBuilder_Individual");
                windManager = AssetsHelper.LoadAsset<BeltManager>("BeltManager");
                windGraphicsParent = AssetsHelper.LoadAsset<Transform>("WindGraphicsParent");
                coverCloud = AssetsHelper.LoadAsset<CoverCloud>("ChalkCloudStartsOff");
                pickup = Array.Find(AssetsHelper.LoadAssets<Pickup>("Pickup"), x => x.gameObject.activeSelf);
                classDoor = AssetsHelper.LoadAsset<StandardDoor>("ClassDoor_Standard");
                cursor = AssetsHelper.LoadAsset<CursorController>("CursorOrigin");
                gameButton = Array.Find(AssetsHelper.LoadAssets<GameButton>("GameButton"), x => x.gameObject.activeSelf);

                texts.Add("total_display", AssetsHelper.LoadAsset<TextMeshPro>("TotalDisplay"));

                bullyTakeouts = ReflectionHelper.GetValue<SoundObject[]>(AssetsHelper.LoadAsset<Bully>("Bully"), "takeouts");

                LoadSound("blowing", "Cmlo_Blowing");
                LoadSound("buzz_lose", "Lose_Buzz");
                LoadSound("teleport", "Teleport");
                LoadSound("scissors", "Scissors");
                LoadSound("hammer", "DrR_Hammer");
                LoadSound("water_slurp", "WaterSlurp");
                LoadSound("bang", "Bang");
                LoadSound("button_press", "Sfx_Button_Press");
                LoadSound("button_unpress", "Sfx_Button_Unpress");
                LoadSound("bal_break", "BAL_Break");
                LoadSound("bal_wow", "BAL_Wow");
                LoadSound("bal_game_over", "BAL_GameOver");
                LoadSound("grapple_launch", "GrappleLaunch");
                LoadSound("grapple_clang", "GrappleClang");
                LoadSound("banana_slip", "Nana_Slip");
                LoadSound("banana_sput", "Nana_Sput");
                //LoadSound("grapple_loop", "GrappleLoop");

                sounds.Add(
                    "grapple_loop",
                    ObjectCreators.CreateSoundObject(AssetsHelper.LoadAsset<AudioClip>("GrappleLoop"), "",
                        SoundType.Effect, Color.white, sublength: 0f)
                );

                LoadSound("nana_sput", "Nana_Sput");
                LoadSound("lock_door_stop", "LockDoorStop");
                LoadSound("lockdown_door", "LockdownDoor_Move");
                LoadSound("clock_wind", "ClockWind");
                LoadSound("bell", "CashBell");
                LoadSound("pop", "Gen_Pop");
                LoadSound("elv_buzz", "Elv_Buzz");
                LoadSound("activity_correct", "Activity_Correct");
                LoadSound("activity_incorrect", "Activity_Incorrect");
                LoadSound("ytp_pickup_0", "YTPPickup_0");
                LoadSound("ytp_pickup_1", "YTPPickup_1");
                LoadSound("ytp_pickup_2", "YTPPickup_2");
                LoadSound("error_maybe", "ErrorMaybe");
                LoadSound("spit", "Ben_Spit");
                LoadSound("whoosh", "Ben_Gum_Whoosh");
                LoadSound("splat", "Ben_Splat");
                LoadSound("slap", "Slap");
                LoadSound("event_notification", "mus_EventNotification_Low");
                LoadSound("creepy_old_computer", "MusicTest");
                LoadSound("food_plate_drop", "PlateDrop");
                LoadSound("food_plate_lift", "PlateLift");
                LoadSound("chip_crunch", "ChipCrunch");
                LoadSound("vent_vacuum", "Vent_Vacuum"); //Store here
                //LoadSound("vent_travel", "Vent_Travel"); //BRUH, Mystman uses AudioClip
                sounds.Add(
                    "vent_travel",
                    ObjectCreators.CreateSoundObject(AssetsHelper.LoadAsset<AudioClip>("Vent_Travel"), "Adv_Sub_Vent_Air",
                        SoundType.Effect, Color.white)
                );
                sounds.Add(
                    "static",
                    ObjectCreators.CreateSoundObject(AssetsHelper.LoadAsset<AudioClip>("Static"), "",
                        SoundType.Effect, Color.white)
                );

                LoadSprite("elv_button_up", "Button_Up");
                LoadSprite("elv_button_down", "Button_Down");
                LoadSprite("transparent", "Transparent");
                LoadSprite("menuArrow0", "MenuArrowSheet_0");
                LoadSprite("menuArrow1", "MenuArrowSheet_1");
                LoadSprite("menuArrow2", "MenuArrowSheet_2");
                LoadSprite("menuArrow3", "MenuArrowSheet_3");
                LoadSprite("grappling_hook", "GrapplingHookSprite");
                LoadSprite("chalkboard_standard", "ChalkBoardStandard");
                LoadSprite("about_notif", "AboutNotif");
                LoadSprite("balloon_orange", "Orange");
                LoadSprite("plant", "Plant");
                LoadSprite("food_plate_cover", "Cover_Sprite");
                LoadSprite("food_plate", "Plate_Sprite");
                LoadSprite("tooltip_bg", "TooltipBG");

                //QMarkSheet_0

                LoadSprite("exclamation_point_sheet0", "ExclamationPoint_Sheet_0");
                /*for (int i = 0; i <= 5; i++)
                {
                    LoadSprite("exclamation_point_sheet" + i, "ExclamationPoint_Sheet_" + i);
                }*/

                LoadTexture("white", "WhiteTexture"); //huh?
                //LoadTexture("white", "UnityWhite"); 
                LoadTexture("qmark_sheet", "QMarkSheet");
                LoadTexture("regular_wall", "Wall");
                LoadTexture("regular_ceiling", "CeilingNoLight");
                LoadTexture("carpet", "Carpet");
                LoadTexture("grass", "Grass");

                sprites.Add("adv_white", AssetLoader.SpriteFromTexture2D(textures["white"], Vector2.one / 2f, 1f));

                //Quad Instance
                LoadMesh("quad", "Quad");

                // SpriteStandard_Billboard (Instance)
                // SpriteStandard_NoBillboard
                LoadMaterial("wind", "Wind");
                LoadMaterial("sprite_standard_billboard", "SpriteStandard_Billboard");
                LoadMaterial("sprite_standard_no_billboard", "SpriteStandard_NoBillboard");
                LoadMaterial("zesty_machine", "ZestyMachine");
                LoadMaterial("belt", "Belt");
                LoadMaterial("math_front_normal", "math_front_normal");
                LoadMaterial("math_side", "math_side");
                LoadMaterial("class_standard_open", "ClassStandard_Open");
                LoadMaterial("class_standard_closed", "ClassStandard_Closed");
                LoadMaterial("black_behind", "BlackBehind");

                //LoadGameObject("line_renderer_hook", "LineRenderer");
                //LoadGameObject("cracks_hook", "Cracks");

                //mod assets

                if (IntegrationManager.IsActive<LevelEditorIntegration>()) LevelEditorIntegration.LoadEditorAssets();

                LoadModTexture("adv_criss_the_crystal", "Npcs/CrissTheCrystal/adv_criss_the_crystal.png");
                LoadModTexture("adv_criss_the_crystal_crazy", "Npcs/CrissTheCrystal/adv_criss_the_crystal_crazy.png");
                LoadModTexture("adv_poster_criss_the_crystal", "Npcs/CrissTheCrystal/adv_poster_criss_the_crystal.png");

                //ROOMS AND POSTERS
                LoadModTexture("adv_poster_symbol_machine", "Posters/adv_poster_symbol_machine.png");
                LoadModTexture("adv_poster_extra_points", "Posters/adv_poster_extra_points.png");
                // LoadModTexture("adv_poster_extra_ad", "Posters/adv_poster_extra_ad.png");
                LoadModTexture("adv_poster_recipe_example", "Posters/adv_poster_recipe_example.png");

                LoadModTexture("adv_english_ceiling", "Rooms/EnglishClass/adv_english_ceiling.png");
                LoadModTexture("adv_english_floor", "Rooms/EnglishClass/adv_english_floor.png");
                LoadModTexture("adv_english_wall", "Rooms/EnglishClass/adv_english_wall.png");
                LoadModTexture("adv_english_class_bg", "Rooms/EnglishClass/adv_english_class_bg.png");
                LoadModTexture("adv_english_class_door_closed", "Rooms/EnglishClass/adv_english_door_closed.png");
                LoadModTexture("adv_english_class_door_open", "Rooms/EnglishClass/adv_english_door_open.png");

                LoadModTexture("adv_advanced_class_ceiling", "Rooms/AdvancedClass/adv_advanced_class_ceiling.png");
                LoadModTexture("adv_advanced_class_floor", "Rooms/AdvancedClass/adv_advanced_class_floor.png");
                LoadModTexture("adv_advanced_class_wall", "Rooms/AdvancedClass/adv_advanced_class_wall.png");
                LoadModTexture("adv_advanced_class_door_closed", "Rooms/AdvancedClass/adv_advanced_class_door_closed.png");
                LoadModTexture("adv_advanced_class_door_open", "Rooms/AdvancedClass/adv_advanced_class_door_open.png");
                LoadModTexture("adv_advanced_class_bg", "Rooms/AdvancedClass/adv_advanced_class_bg.png");
                LoadModTexture("adv_advanced_class_lamp", "Rooms/AdvancedClass/adv_advanced_class_lamp.png");

                LoadModTexture("adv_school_council_door_open", "Rooms/SchoolCouncil/adv_school_council_door_open.png");
                LoadModTexture("adv_school_council_door_closed", "Rooms/SchoolCouncil/adv_school_council_door_closed.png");
                LoadModTexture("adv_school_council_wall", "Rooms/SchoolCouncil/adv_school_council_wall.png");
                LoadModTexture("adv_school_council_bg", "Rooms/SchoolCouncil/adv_school_council_bg.png");
                //Rooms & posters END

                LoadModTexture("adv_ballot_front", "Objects/VotingBallot/adv_ballot_front.png");
                LoadModTexture("adv_ballot_front_voting", "Objects/VotingBallot/adv_ballot_front_voting.png");
                LoadModTexture("adv_ballot_empty_top", "Objects/VotingBallot/adv_ballot_empty_top.png");
                LoadModTexture("adv_ballot_top", "Objects/VotingBallot/adv_ballot_top.png");

                LoadModTexture("adv_rusty_rotohall", "Objects/RustyRotoHall/adv_rusty_rotohall.png");
                LoadModTexture("adv_rusty_rotohall_blank", "Objects/RustyRotoHall/adv_rusty_rotohall_blank.png");
                LoadModTexture("adv_rusty_rotohall_floor", "Objects/RustyRotoHall/adv_rusty_rotohall_floor.png");
                LoadModTexture("adv_rusty_rotohall_sign_left", "Objects/RustyRotoHall/adv_rusty_rotohall_sign_left.png");
                LoadModTexture("adv_rusty_rotohall_sign_right", "Objects/RustyRotoHall/adv_rusty_rotohall_sign_right.png");
                LoadModTexture("adv_rusty_rotohall_sign_straight", "Objects/RustyRotoHall/adv_rusty_rotohall_sign_straight.png");

                LoadModTexture("adv_advanced_math_machine_face", "Objects/AdvancedMathMachine/adv_advanced_math_machine_front.png");
                LoadModTexture("adv_advanced_math_machine_face_right", "Objects/AdvancedMathMachine/adv_advanced_math_machine_front_right.png");
                LoadModTexture("adv_advanced_math_machine_face_wrong", "Objects/AdvancedMathMachine/adv_advanced_math_machine_front_wrong.png");
                LoadModTexture("adv_advanced_math_machine_side", "Objects/AdvancedMathMachine/adv_advanced_math_machine_side.png");

                LoadModTexture("adv_symbol_machine_face", "Objects/SymbolMachine/adv_symbol_machine_front.png");
                LoadModTexture("adv_symbol_machine_face_right", "Objects/SymbolMachine/adv_symbol_machine_front_right.png");
                LoadModTexture("adv_symbol_machine_face_wrong", "Objects/SymbolMachine/adv_symbol_machine_front_wrong.png");
                LoadModTexture("adv_symbol_machine_side", "Objects/SymbolMachine/adv_symbol_machine_side.png");

                LoadModTexture("adv_pressure_plate_activated", "Objects/Plates/adv_pressure_plate_activated.png");
                LoadModTexture("adv_pressure_plate_deactivated", "Objects/Plates/adv_pressure_plate_deactivated.png");

                LoadModTexture("adv_invisibility_plate_activated", "Objects/Plates/adv_invisibility_plate_activated.png");
                LoadModTexture("adv_invisibility_plate_deactivated", "Objects/Plates/adv_invisibility_plate_deactivated.png");

                LoadModTexture("adv_acceleration_plate_activated", "Objects/Plates/adv_acceleration_plate_activated.png");
                LoadModTexture("adv_acceleration_plate_deactivated", "Objects/Plates/adv_acceleration_plate_deactivated.png");
                LoadModTexture("adv_acceleration_plate_activated_arrow", "Objects/Plates/adv_acceleration_plate_activated_arrow.png");
                LoadModTexture("adv_acceleration_plate_deactivated_arrow", "Objects/Plates/adv_acceleration_plate_deactivated_arrow.png");

                LoadModTexture("adv_noisy_plate_activated", "Objects/Plates/adv_noisy_plate_activated.png");
                LoadModTexture("adv_noisy_plate_deactivated", "Objects/Plates/adv_noisy_plate_deactivated.png");

                LoadModTexture("adv_stealing_plate_activated", "Objects/Plates/adv_stealing_plate_activated.png");
                LoadModTexture("adv_stealing_plate_deactivated", "Objects/Plates/adv_stealing_plate_deactivated.png");

                LoadModTexture("adv_bully_plate_activated", "Objects/Plates/adv_bully_plate_activated.png");
                LoadModTexture("adv_bully_plate_deactivated", "Objects/Plates/adv_bully_plate_deactivated.png");

                LoadModTexture("adv_present_plate_activated", "Objects/Plates/adv_present_plate_activated.png");
                LoadModTexture("adv_present_plate_deactivated", "Objects/Plates/adv_present_plate_deactivated.png");

                LoadModTexture("adv_sugar_addiction_plate_activated", "Objects/Plates/adv_sugar_addiction_plate_activated.png");
                LoadModTexture("adv_sugar_addiction_plate_deactivated", "Objects/Plates/adv_sugar_addiction_plate_deactivated.png");

                LoadModTexture("adv_slowdown_plate_activated", "Objects/Plates/adv_slowdown_plate_activated.png");
                LoadModTexture("adv_slowdown_plate_deactivated", "Objects/Plates/adv_slowdown_plate_deactivated.png");

                LoadModTexture("adv_protection_plate_activated", "Objects/Plates/adv_protection_plate_activated.png");
                LoadModTexture("adv_protection_plate_deactivated", "Objects/Plates/adv_protection_plate_deactivated.png");

                LoadModTexture("adv_teleportation_plate_activated", "Objects/Plates/adv_teleportation_plate_activated.png");
                LoadModTexture("adv_teleportation_plate_deactivated", "Objects/Plates/adv_teleportation_plate_deactivated.png");

                LoadModTexture("adv_fake_plate_deactivated", "Objects/Plates/FakePlate/adv_fake_plate_deactivated.png");
                LoadModTexture("adv_fake_plate_activated", "Objects/Plates/FakePlate/adv_fake_plate_activated.png");
                LoadModTexture("adv_fake_plate_surprize", "Objects/Plates/FakePlate/adv_fake_plate_surprize.png");

                LoadModTexture("adv_safety_trapdoor_deactivated", "Objects/SafetyTrapdoor/adv_safety_trapdoor_deactivated.png");
                LoadModTexture("adv_safety_trapdoor_activated", "Objects/SafetyTrapdoor/adv_safety_trapdoor_activated.png");

                LoadModTexture("adv_kitchen_stove", "Objects/KitchenStove/adv_kitchen_stove.png");

                LoadModTexture("adv_gum_dispenser", "Objects/GumDispenser/adv_gum_dispenser.png");

                for (int i = 0; i < 5; i++)
                {
                    LoadModTexture("adv_pulley_base" + (i + 1), $"Objects/Pulley/adv_pulley_base{i + 1}.png");
                }

                LoadModSprite("adv_tip_screen_forward", "UI/SwingingTipsScreen/adv_tip_screen_forward.png");

                spriteSheets.Add("adv_tip_screen_forward_static_sheet",
                    AssetLoader.SpritesFromSpritesheet(2, 1, 1f, Vector2.one * 0.5f,
                    AssetsHelper.TextureFromFile("Textures/UI/SwingingTipsScreen/adv_tip_screen_forward_static_sheet.png")));

                spriteSheets.Add("adv_tips_screen", AssetLoader.SpritesFromSpritesheet(3, 2, 1f, Vector2.one * 0.5f,
                    AssetsHelper.TextureFromFile("Textures/UI/SwingingTipsScreen/adv_swinging_tip_screen_sheet.png")));
                spriteSheets["adv_tips_screen"] =
                    spriteSheets["adv_tips_screen"].Take(spriteSheets["adv_tips_screen"].Length - 1).ToArray();

                LoadModSprite("adv_pulley", "Objects/Pulley/adv_pulley_handle.png", 25f);

                LoadModSprite("adv_gauge_protection", "Gauges/adv_gauge_protection.png");
                LoadModSprite("adv_gauge_invisibility", "Gauges/adv_gauge_invisibility.png");

                LoadModSprite("adv_obstacle_trick", "Objects/Plates/FakePlate/adv_obstacle_trick.png", 20f);
                LoadModSprite("adv_boxing_glove_trick", "Objects/Plates/FakePlate/adv_boxing_glove_trick.png", 20f);
                LoadModSprite("adv_exit", "UI/Buttons/adv_button_exit.png");
                LoadModSprite("adv_exit_transparent", "UI/Buttons/adv_button_exit_transparent.png");

                LoadModSprite("adv_mysterious_teleporter", "Items/LargeSprites/adv_mysterious_teleporter_large.png", 50f);
                LoadModSprite("adv_teleportation_bomb", "Items/LargeSprites/adv_teleportation_bomb_large.png", 50f);

                //Projectiles!!1!
                LoadModSprite("adv_anvil_projectile", "Objects/Projectiles/adv_anvil_projectile.png", 25f);

                LoadModSprite("adv_frozen_overlay", "UI/adv_frozen_overlay.png");
                LoadModSprite("adv_protected_overlay", "UI/adv_protected_overlay.png");

                LoadModSprite("adv_frozen_enemy", "Npcs/adv_frozen_enemy.png", 20f);
                LoadModSprite("adv_portal", "Objects/MysteriousPortal/adv_portal.png", 15f);
                LoadModSprite("adv_portal_opened", "Objects/MysteriousPortal/adv_portal_opened.png", 15f);
                LoadModSprite("adv_elephant_overlay", "UI/adv_elephant_overlay.png");
                LoadModSprite("adv_expel_hammer", "Items/adv_the_hammer_of_force.png", 40f);
                LoadModSprite("adv_arrows", "UI/adv_arrows.png", 70f);

                LoadModSprite("adv_dough", "Items/LargeSprites/adv_dough_large.png", 50f);

                //Lamps!!!
                LoadModSprite("adv_advanced_class_lamp", "Rooms/AdvancedClass/adv_advanced_class_lamp.png", 50f);

                /*for (int i = 1; i <= 4; i++)
                {
                    LoadModSprite("adv_gum_dispenser_" + i, "Objects/GumDispenser/adv_gum_dispenser_" + i + ".png", 25f);
                }*/

                string alphabet = "abcdefghijklmnopqrstuvwxyz";
                for (int i = 0; i < alphabet.Length; i++)
                {
                    string symbol = alphabet.ElementAt(i).ToString();
                    LoadModSprite("adv_balloon_" + symbol, "Objects/Spelloons/adv_balloon_" + symbol + ".png", 30f);
                }

                LoadModSprite("adv_reaper", "Npcs/GottaReap/adv_reaper.png", 1.5f);
                LoadModSprite("adv_farm_flag", "Objects/Flags/adv_farm_flag.png", 5f, new Vector2(0.08f, 0.88f));
                LoadModSprite("adv_corn_sign1", "Objects/Signs/adv_corn_sign1.png", 25f);

                LoadModSound("adv_mysterious_machine", "Sounds/Adv_Mysterious_Machine.wav", SoundType.Effect, "Adv_Sub_Machine", 5f);
                LoadModSound("adv_pah", "Sounds/Adv_PAH.ogg", SoundType.Effect, "Adv_Sub_Pah", 1f);
                LoadModSound("adv_inhale", "Sounds/Adv_Inhale.ogg", SoundType.Effect, "Adv_Sub_Inhale", 1f);
                LoadModSound("adv_boing", "Sounds/Adv_Boing.ogg", SoundType.Effect, "Adv_Sub_Boing", 1f);
                LoadModSound("adv_appearing", "Sounds/Adv_Appearing.ogg", SoundType.Effect, "Adv_Sub_Appearing", 2f);
                LoadModSound("adv_disappearing", "Sounds/Adv_Disappearing.ogg", SoundType.Effect, "Adv_Sub_Disappearing", 3f);
                LoadModSound("adv_emergency", "Sounds/Adv_Emergency.wav", SoundType.Effect, "Adv_Sub_Emergency", 1f);
                LoadModSound("adv_frozen", "Sounds/Adv_Frozen.ogg", SoundType.Effect, "Adv_Sub_Frozen", Color.blue, 3f);
                LoadModSound("adv_elephant_hit", "Sounds/Adv_Elephant_Hit.ogg", SoundType.Effect, "Adv_Sub_ElephantHit", 1f);
                LoadModSound("adv_metal_blow", "Sounds/Adv_Metal_Blow.ogg", SoundType.Effect, "Adv_Sub_MetalBlow", 1f);
                LoadModSound("adv_boost", "Sounds/Adv_Boost.ogg", SoundType.Effect, "Adv_Sub_Boost", 1f);
                LoadModSound("adv_symbol_machine_reinit", "Sounds/Adv_Symbol_Machine_ReInit.wav", SoundType.Effect, "Adv_Sub_SymbolMachineReInit", 5f);
                LoadModSound("adv_beep", "Sounds/Adv_Beep.wav", SoundType.Effect, "Adv_Sub_Beep", 0.5f);
                LoadModSound("adv_magical_appearing", "Sounds/Adv_Magical_Appearing.wav", SoundType.Effect, "Adv_Sub_MagicalAppearing", 5f);
                LoadModSound("adv_activation_start", "Sounds/Adv_Activation_Start.wav", SoundType.Effect, "Adv_Sub_Activation", 1f);
                LoadModSound("adv_activation_loop", "Sounds/Adv_Activation_Loop.wav", SoundType.Effect, "Adv_Sub_Activation", 1f);
                LoadModSound("adv_activation_end", "Sounds/Adv_Activation_End.wav", SoundType.Effect, "Adv_Sub_Activation_End", 2f);
                LoadModSound("adv_explosion", "Sounds/Adv_Explosion.wav", SoundType.Effect, "Adv_Sub_Explosion", 4f);
                LoadModSound("adv_protection", "Sounds/Adv_Protection.wav", SoundType.Effect, "Adv_Sub_Protection", 3f);
                LoadModSound("adv_protected", "Sounds/Adv_Protected.wav", SoundType.Effect, "Adv_Sub_Protected", 2f);
                LoadModSound("adv_refresh", "Sounds/Adv_Refresh.wav", SoundType.Effect, "", 0f);
                LoadModSound("adv_turning_start", "Sounds/Adv_Turning_Start.wav", SoundType.Effect, "Adv_Sub_Turning_Loop", 3f);
                LoadModSound("adv_turning_loop", "Sounds/Adv_Turning_Loop.wav", SoundType.Effect, "Adv_Sub_Turning_Loop", 2f);
                LoadModSound("adv_turning_end", "Sounds/Adv_Turning_End.wav", SoundType.Effect, "Adv_Sub_Turning_End", 2f);
                LoadModSound("adv_burning_start", "Sounds/Adv_Burning_Start.wav", SoundType.Effect, "Adv_Sub_Burning_Loop", 3.2f);
                LoadModSound("adv_burning_loop", "Sounds/Adv_Burning_Loop.wav", SoundType.Effect, "Adv_Sub_Burning_Loop", 2f);
                LoadModSound("adv_burning_end", "Sounds/Adv_Burning_End.wav", SoundType.Effect, "Adv_Sub_Burning_End", 1f);
                LoadModSound("adv_throwing_vote", "Sounds/Adv_Throwing_Vote.wav", SoundType.Effect, "Adv_Sub_ThrowingVote", 1f);
                LoadModSound("adv_time_stops", "Sounds/Adv_Time_Stops.wav", SoundType.Effect, "Adv_Sub_TimeStops", 14f);
                LoadModSound("adv_time_starts", "Sounds/Adv_Time_Starts.wav", SoundType.Effect, "Adv_Sub_TimeStarts", 9f);
                LoadModSound("adv_bell", "Sounds/Adv_Bell.wav", SoundType.Effect, "Adv_Sub_Bell", 3f);
                LoadModSound("adv_suction_start", "Sounds/Adv_Suction_Start.wav", SoundType.Effect, "")
                    .volumeMultiplier = 0.25f;
                LoadModSound("adv_suction_loop", "Sounds/Adv_Suction_Loop.wav", SoundType.Effect, "")
                    .volumeMultiplier = 0.25f;
                LoadModSound("adv_suction_end", "Sounds/Adv_Suction_End.wav", SoundType.Effect, "")
                    .volumeMultiplier = 0.25f;
                LoadModSound("adv_laser_start", "Sounds/Adv_Laser_Start.wav", SoundType.Effect, "Adv_Sub_Laser_Loop", 2f);
                LoadModSound("adv_laser_loop", "Sounds/Adv_Laser_Loop.wav", SoundType.Effect, "Adv_Sub_Laser_Loop", 1f);
                LoadModSound("adv_laser_end", "Sounds/Adv_Laser_End.wav", SoundType.Effect, "Adv_Sub_Laser_End", 2f);
                LoadModSound("adv_balloon_inflation", "Sounds/Adv_Balloon_Inflation.wav", SoundType.Effect, "");
                LoadModSound("adv_magic_1", "Sounds/Adv_Magic_1.wav", SoundType.Effect, "");
                LoadModSound("adv_wood_1", "Sounds/Adv_Wood_1.wav", SoundType.Effect, "");
                LoadModSound("adv_wood_2", "Sounds/Adv_Wood_2.wav", SoundType.Effect, "");
                LoadModSound("adv_wood_3", "Sounds/Adv_Wood_3.wav", SoundType.Effect, "");
                LoadModSound("adv_yum", "Sounds/Adv_Yum.wav", SoundType.Effect, "");
                LoadModSound("adv_motor_loop", "Sounds/Adv_Motor_Loop.wav", SoundType.Effect, "");
                LoadModSound("adv_pulley_click", "Sounds/Adv_Pulley_Click.wav", SoundType.Effect, "");
                LoadModSound("adv_super_break", "Sounds/Adv_Super_Break.wav", SoundType.Effect, "", 4f);
                LoadModSound("adv_portal_starts_teleport", "Sounds/Adv_Portal_Preparing_Teleport.wav", SoundType.Effect, "Sfx_WormholeOpen");
                LoadModSound("adv_portal_teleports", "Sounds/Adv_Portal_Teleports.wav", SoundType.Effect, "Adv_Sub_Portal_Teleports");

                LoadModSound("adv_reaper_gotta_reap", "Voices/GottaReap/Adv_GottaReap_Reap.wav", SoundType.Voice,
                    "Adv_Sub_GottaReap_Reap1", Color.green, 7.257f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey() {
                            key = "Adv_Sub_GottaReap_Reap2",
                            time = 1.5f
                        },
                        new SubtitleTimedKey() {
                            key = "Adv_Sub_GottaReap_Reap3",
                            time = 2.5f
                        },
                        new SubtitleTimedKey() {
                            key = "Adv_Sub_GottaReap_Reap4",
                            time = 3.2f
                        }
                    };

                LoadModSound("adv_mus_win", "Music/Adv_Mus_Win.ogg", SoundType.Music, "");

                //event sounds
                LoadModSound("adv_bal_event_portals", "Voices/Baldi/Adv_Bal_Event_Portals.wav", SoundType.Voice, "Adv_Sub_Bal_Portals1", Color.green, 9.074f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Portals2",
                            time = 1.1f
                        },
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Portals3",
                            time = 5.1f
                        }
                    };

                LoadModSound("adv_bal_event_disappearing_characters", "Voices/Baldi/Adv_Bal_Event_Disappearing_Characters.wav",
                    SoundType.Voice, "Adv_Sub_Bal_Invisibility_Machine1", Color.green, 8.828f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Invisibility_Machine2",
                            time = 4.6f
                        }
                    };

                LoadModSound("adv_bal_event_cold_machine", "Voices/Baldi/Adv_Bal_Event_Cold_Machine.wav",
                    SoundType.Voice, "Adv_Sub_Bal_Cold_Machine1", Color.green, 10.282f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Cold_Machine2",
                            time = 3.4f
                        },
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Cold_Machine3",
                            time = 5.9f
                        }
                    };

                LoadModSound("adv_bal_event_voting", "Voices/Baldi/Adv_Bal_Event_Voting.wav", SoundType.Voice, "Adv_Sub_Bal_Voting1", Color.green, 10.598f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Voting2",
                            time = 3.1f
                        },
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Voting3",
                            time = 5.5f
                        }
                    };

                LoadModSound("adv_bal_great_job", "Voices/Baldi/Adv_Bal_Great_Job.wav",
                    SoundType.Voice, "Adv_Sub_Bal_Great_Job1", Color.green, 3.822f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Great_Job2",
                            time = 0.742f
                        }
                    };

                LoadModSound("adv_bal_fantastic", "Voices/Baldi/Adv_Bal_Fantastic.wav",
                    SoundType.Voice, "Adv_Sub_Bal_Fantastic1", Color.green, 4.979f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Fantastic2",
                            time = 1.36f
                        }
                    };

                LoadModSound("adv_bal_incredible", "Voices/Baldi/Adv_Bal_Incredible.wav",
                    SoundType.Voice, "Adv_Sub_Bal_Incredible1", Color.green, 3.76f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "Adv_Sub_Bal_Incredible2",
                            time = 0.8f
                        }
                    };

                LoadModSound("adv_bal_surprize", "Voices/Baldi/Adv_Bal_Surprize.wav",
                    SoundType.Voice, "Adv_Sub_Bal_Surprize", Color.green, 4f);

                LoadModSound("adv_bal_super_wow", "Voices/Baldi/Adv_Bal_Super_Wow.mp3",
                    SoundType.Voice, "");

                CreateMaterial("adv_good_machine", "zesty_machine",
                    AssetsHelper.TextureFromFile("Textures/VendingMachines/adv_good_stuffs_machine.png"));
                CreateMaterial("adv_good_machine_out", "zesty_machine",
                    AssetsHelper.TextureFromFile("Textures/VendingMachines/adv_good_stuffs_machine_out.png"));

                //Shader Graphs/Standard
                //Sprites/Default
                CreateMaterialByShader("adv_white", "Shader Graphs/Standard", textures["white"]);
                CreateMaterialByShader("qmark_sheet", "Shader Graphs/Standard", textures["qmark_sheet"]);

                sounds["adv_emergency"].color = Color.red;

                SpriteRenderer advancedClassLamp = UnityEngine.Object.Instantiate(AssetsHelper.LoadAsset<Transform>("HangingLight"))
                    .GetComponentInChildren<SpriteRenderer>();
                advancedClassLamp.transform.parent.gameObject.AddComponent<RendererContainer>().renderers = new Renderer[]
                {
                    advancedClassLamp
                };
                advancedClassLamp.transform.parent.name = "AdvancedClassLampLight";
                advancedClassLamp.sprite = sprites["adv_advanced_class_lamp"];
                advancedClassLamp.transform.localPosition = Vector3.up * 8.95f;
                advancedClassLamp.transform.parent.gameObject.ConvertToPrefab(true);

                cached = true;

                if (Debugging)
                {
                    sw.Stop();
                    AdvancedCore.Logging.LogInfo("Assets loaded in: " + sw.ElapsedMilliseconds + " ms");
                }
            }
            catch (Exception e)
            {
                cached = false;
                exception = e;
            }
        }

        #endregion

        #region Assets' overriding

        public static void OverrideAssetsProperties()
        {
            if (!overridden)
            {
                foreach (Principal principal in AssetsHelper.LoadAssets<Principal>())
                {
                    ReflectionHelper.SetValue(principal, "ignorePlayerOnSpawn", true);
                }

                sounds["bal_game_over"].soundKey = "Adv_Sub_Bal_Game_Over";
                sounds["bal_game_over"].color = Color.green;
                sounds["error_maybe"].soundKey = "Adv_Sub_Error_Maybe";

                //Elevator screen overrides
                ElevatorScreen elvScreen = AssetsHelper.LoadAsset<ElevatorScreen>("ElevatorScreen");
                elvScreen.GetComponent<AudioManager>().positional = false;
                //Elevator ends

                LevelAsset pitStop = AssetsHelper.LoadAsset<LevelAsset>("Pitstop");
                RoomData hall = pitStop.rooms.Find(x => x.category == RoomCategory.Null);

                if (!PitOverrides.AccelerationPlateDisabled)
                {
                    hall.basicObjects.Add(
                        new BasicObjectData()
                        {
                            prefab = ObjectsStorage.Triggers["no_plates_cooldown"].transform
                        }
                    );

                    hall.basicObjects.Add(
                        new BasicObjectData()
                        {
                            prefab = ObjectsStorage.Triggers["pit_stop_overrides"].transform
                        }
                    );

                    hall.basicObjects.Add(
                        new BasicObjectData()
                        {
                            prefab = ObjectsStorage.Objects["acceleration_plate"].transform,
                            position = new Vector3(335f, 0f, 35f)
                        }
                    );
                }

                //PIT overrides
                if (!PitOverrides.EnglishClassDisabled)
                {
                    StandardDoorMats engDoorMats = Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet");

                    RoomData roomData = new RoomData()
                    {
                        name = "EnglishClass",
                        category = EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                        keepTextures = true,
                        wallTex = textures["adv_english_wall"],
                        ceilTex = textures["adv_english_ceiling"],
                        florTex = textures["adv_english_floor"],
                        lightPre = AssetsHelper.LoadAsset<Transform>("HangingLight"),
                        roomFunctionContainer = ObjectsStorage.RoomFunctionsContainers["EnglishClassTimerFunction"],
                        color = ObjectsStorage.RoomColors["English"]
                    };

                    pitStop.rooms.Add(roomData);

                    int roomId = pitStop.rooms.IndexOf(roomData);

                    roomData.basicObjects.Add(new BasicObjectData()
                    {
                        position = new Vector3(385, 0, 84),
                        prefab = ObjectsStorage.Objects["symbol_machine"].transform
                    });

                    roomData.standardLightCells.Add(new IntVector2(39, 7));
                    roomData.standardLightCells.Add(new IntVector2(39, 4));
                    roomData.doorMats = engDoorMats;
                    roomData.entitySafeCells = RoomHelper.CreateVector2Range(new IntVector2(37, 3), new IntVector2(40, 8));
                    roomData.eventSafeCells = RoomHelper.CreateVector2Range(new IntVector2(37, 3), new IntVector2(40, 8));

                    pitStop.doors.Add(new DoorData(roomId, classDoor, new IntVector2(36, 4), Direction.East));

                    pitStop.SetCellsRange(new IntVector2(37, 3), new IntVector2(40, 9), roomId);

                    pitStop.windows.Add(new WindowData()
                    {
                        window = AssetsHelper.LoadAsset<WindowObject>("WoodWindow"),
                        position = new IntVector2(36, 5),
                        direction = Direction.East
                    });

                    pitStop.posters.Add(new PosterData()
                    {
                        poster = ObjectCreators.CreatePosterObject(textures["adv_poster_extra_points"], new PosterTextData[0]),
                        position = new IntVector2(36, 3),
                        direction = Direction.East
                    });

                    pitStop.posters.Add(new PosterData()
                    {
                        poster = ObjectCreators.CreatePosterObject(textures["adv_poster_symbol_machine"], new PosterTextData[0]),
                        position = new IntVector2(39, 4),
                        direction = Direction.East
                    });

                    if (!AssetsHelper.ModInstalled(IntegrationManager.recommendedCharactersId))
                    {
                        pitStop.posters.Add(new PosterData()
                        {
                            poster = PosterSerializableData.
                            GetPosterFromFile("Textures/Posters/Advertisement/Adv_Poster_RecommendedCharacters_Ad.png"),
                            position = new IntVector2(39, 5),
                            direction = Direction.East
                        });
                    }

                    if (!(AssetsHelper.ModInstalled(IntegrationManager.carnivalPackId) ||
                        AssetsHelper.ModInstalled(IntegrationManager.criminalPackId) ||
                        AssetsHelper.ModInstalled(IntegrationManager.piratePackId)))
                    {
                        pitStop.posters.Add(new PosterData()
                        {
                            poster = PosterSerializableData.
                                GetPosterFromFile("Textures/Posters/Advertisement/Adv_Poster_ContentPacks_Ad.png"),
                            position = new IntVector2(39, 6),
                            direction = Direction.East
                        });
                    }

                    //FluorescentLight
                    //HangingLight
                    pitStop.lights.Add(new LightSourceData()
                    {
                        prefab = AssetsHelper.LoadAsset<Transform>("HangingLight"),
                        color = Color.white,
                        position = new IntVector2(38, 7),
                        strength = 10
                    });

                    pitStop.lights.Add(new LightSourceData()
                    {
                        prefab = AssetsHelper.LoadAsset<Transform>("HangingLight"),
                        color = Color.white,
                        position = new IntVector2(38, 4),
                        strength = 10
                    });
                }

                if (!PitOverrides.KitchenStoveDisabled)
                {
                    hall.basicObjects.Add(new BasicObjectData()
                    {
                        prefab = ObjectsStorage.Objects["johnny_kitchen_stove"].transform,
                        position = new Vector3(365f, 0f, 145f)
                    });

                    pitStop.posters.Add(new PosterData()
                    {
                        poster = ObjectCreators.CreatePosterObject(
                            AssetsHelper.TextureFromFile("Textures/Posters/adv_poster_kitchen_stove.png"),
                            PosterSerializableData.GetFromFile(
                                AssetsHelper.modPath + "Textures/Posters/adv_poster_kitchen_stove.json").Texts),
                        position = new IntVector2(36, 13),
                        direction = Direction.East
                    });

                }

                overridden = true;
            }

        }
        #endregion 

        public static void Clear()
        {
            textures.Clear();
            sprites.Clear();
            sounds.Clear();
            meshes.Clear();
            materials.Clear();
            itemObjects.Clear();
            texts.Clear();
            gameObjects.Clear();
        }

        public static void CreateDoorMats(string nameBase, string texNameBase)
        {
            CreateMaterial(nameBase + "_open", "class_standard_open", textures[texNameBase + "_open"]);
            CreateMaterial(nameBase + "_closed", "class_standard_closed", textures[texNameBase + "_closed"]);
        }

        internal static Material CreateMaterialByShader(string key, string shaderNameBase, Texture2D tex)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Creating material by texture: " + tex.name);
            materials.Add(key, new Material(Shader.Find(shaderNameBase)));
            materials[key].mainTexture = tex;
            return materials[key];
        }

        internal static Material CreateMaterial(string key, string matNameBase, Texture2D tex)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Creating material by texture: " + tex.name);
            materials.Add(key, new Material(materials[matNameBase]));
            materials[key].mainTexture = tex;
            return materials[key];
        }

        internal static SoundObject LoadModSound(string key, string path, SoundType soundType, string subKey, float subDuration = -1f)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path);
            if (string.IsNullOrEmpty(subKey)) subDuration = 0f;
            SoundObject sound = ObjectCreators.CreateSoundObject(AssetsHelper.GetAudioFromFile("Audio/" + path), subKey, soundType, Color.white, subDuration);
            sounds.Add(key, sound);
            return sound;
        }

        internal static SoundObject LoadModSound(string key, string path, SoundType soundType, string subKey, Color color, float subDuration = 2f)
        {
            //if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path); //because used in other method
            SoundObject sound = LoadModSound(key, path, soundType, subKey, subDuration);
            sound.color = color;
            return sound;
        }

        internal static void LoadModSprite(string key, string path, float pixelsPerUnit = 1f, Vector2? center = null)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path);

            Sprite sprite = null;

            if (center != null)
            {
                sprite = AssetsHelper.SpriteFromFile("Textures/" + path, pixelsPerUnit, center);
            } else
                sprite = AssetsHelper.SpriteFromFile("Textures/" + path, pixelsPerUnit);

            sprites.Add(key, sprite);
        }

        internal static void LoadModTexture(string key, string path, bool doNotUnload = false)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path);
            Texture2D texture = AssetsHelper.TextureFromFile("Textures/" + path);
            textures.Add(key, texture);
        }

        internal static void LoadGameObject(string key, string name, bool doNotUnload = false)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            GameObject _object = AssetsHelper.LoadAsset<GameObject>(name);
            gameObjects.Add(key, _object);
        }

        internal static void LoadSprite(string key, string name, bool doNotUnload = false)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            Sprite sprite = AssetsHelper.LoadAsset<Sprite>(name);
            sprites.Add(key, sprite, doNotUnload);
        }

        internal static void LoadTexture(string key, string name, bool doNotUnload = false)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            Texture2D sprite = AssetsHelper.LoadAsset<Texture2D>(name);
            textures.Add(key, sprite, doNotUnload);
        }

        internal static void LoadMaterial(string key, string name, bool doNotUnload = false)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            Material material = AssetsHelper.LoadAsset<Material>(name);
            materials.Add(key, material, doNotUnload);
        }

        internal static void LoadMesh(string key, string name, bool doNotUnload = false)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            Mesh material = AssetsHelper.LoadAsset<Mesh>(name);
            meshes.Add(key, material, doNotUnload);
        }

        internal static void LoadSound(string key, string name, bool doNotUnload = false)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            SoundObject sound = AssetsHelper.LoadAsset<SoundObject>(name);
            sounds.Add(key, sound, doNotUnload);
        }
    }
}
