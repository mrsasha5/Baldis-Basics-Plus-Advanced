using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Builders;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches.UI.Elevator;
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

    internal class AssetsStorage
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

            public bool Contains(string key)
            {
                return instances.ContainsKey(key);
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

        private static bool overridden = false;

        public static bool Overridden => overridden;

        public static AssetDictionary<SoundObject> sounds = new AssetDictionary<SoundObject>();

        public static AssetDictionary<Sprite> sprites = new AssetDictionary<Sprite>();

        public static AssetDictionary<Sprite[]> spriteSheets = new AssetDictionary<Sprite[]>();

        public static AssetDictionary<Texture2D> textures = new AssetDictionary<Texture2D>();

        public static AssetDictionary<ItemObject> itemObjects = new AssetDictionary<ItemObject>();

        public static AssetDictionary<GameObject> gameObjects = new AssetDictionary<GameObject>();

        public static AssetDictionary<Material> materials = new AssetDictionary<Material>();

        public static AssetDictionary<TMP_Text> texts = new AssetDictionary<TMP_Text>();

        public static AssetDictionary<Mesh> meshes = new AssetDictionary<Mesh>();

        //game assets

        public static Shader graphsStandardShader;

        public static Baldi genericBaldi;

        public static Structure_EnvironmentObjectPlacer weightedPlacer;

        public static Structure_EnvironmentObjectPlacer individualPlacer;

        public static BeltManager windManager;

        public static Pickup pickup;

        public static Transform windGraphicsParent;

        public static AudioClip weirdErrorSound;

        public static TextAsset campingMidi;

        public static GameButton gameButton;

        public static CoverCloud coverCloud;

        public static SoundObject[] bullyTakeouts;

        public static StandardDoor classDoor;

        public static CursorController cursor;

        #region Critical Assets for Systems (Notifications System, Invoking Crash Screen with Sound...)

        public static void InitializeCriticalResources()
        {
            weirdErrorSound = AssetsHelper.LoadAsset<AudioClip>("WeirdError");
            LoadSound("buzz_elv", "Elv_Buzz");
            LoadSprite("tooltip_bg", "TooltipBG");
        }

        #endregion

        #region Main Assets

        public static void Initialize()
        {
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

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
            LoadSound("explosion", "Explosion");
            LoadSound("power_breaker_lights_on", "PowerBreaker_LightsOn");
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
            LoadSprite("balloon_orange", "BalloonBuster_Balloons_Sheet_3"); //Orange
            LoadSprite("plant", "Plant");
            LoadSprite("food_plate_cover", "Cover_Sprite");
            LoadSprite("food_plate", "Plate_Sprite");

            for (int i = 0; i < ElevatorAdditionsPatch.explosionSprites.Length; i++)
            {
                ElevatorAdditionsPatch.explosionSprites[i] = AssetsHelper.LoadAsset<Sprite>($"Explostion_Sheet_{i + 1}");
            }

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

            LoadGameObject("math_num_0", "MathNum_0");
            //LoadGameObject("line_renderer_hook", "LineRenderer");
            //LoadGameObject("cracks_hook", "Cracks");

            //mod assets

            if (IntegrationManager.IsActive<LevelStudioIntegration>()) LevelStudioIntegration.LoadEditorAssets();
                
            LoadModTexture("adv_criss_the_crystal", "Npcs/CrissTheCrystal/adv_criss_the_crystal.png");
            LoadModTexture("adv_criss_the_crystal_crazy", "Npcs/CrissTheCrystal/adv_criss_the_crystal_crazy.png");
            LoadModTexture("adv_poster_criss_the_crystal", "Npcs/CrissTheCrystal/adv_poster_criss_the_crystal.png");

            //ROOMS AND POSTERS
            LoadModTexture("adv_poster_recipe_example", "Posters/Adv_Poster_Recipe_Example.png");

            LoadModTexture("adv_english_ceiling", "Rooms/EnglishClass/Adv_English_Ceiling.png");
            LoadModTexture("adv_english_floor", "Rooms/EnglishClass/Adv_English_Floor.png");
            LoadModTexture("adv_english_wall", "Rooms/EnglishClass/Adv_English_Wall.png");
            LoadModTexture("adv_english_class_bg", "Rooms/EnglishClass/Adv_English_Class_Bg.png");
            LoadModTexture("adv_english_class_door_closed", "Rooms/EnglishClass/Adv_English_Door_Closed.png");
            LoadModTexture("adv_english_class_door_open", "Rooms/EnglishClass/Adv_English_Door_Open.png");

            LoadModTexture("adv_advanced_class_ceiling", "Rooms/AdvancedClass/Adv_Advanced_Class_Ceiling.png");
            LoadModTexture("adv_advanced_class_floor", "Rooms/AdvancedClass/Adv_Advanced_Class_Floor.png");
            LoadModTexture("adv_advanced_class_wall", "Rooms/AdvancedClass/Adv_Advanced_Class_Wall.png");
            LoadModTexture("adv_advanced_class_door_closed", "Rooms/AdvancedClass/Adv_Advanced_Class_Door_Closed.png");
            LoadModTexture("adv_advanced_class_door_open", "Rooms/AdvancedClass/Adv_Advanced_Class_Door_Open.png");
            LoadModTexture("adv_advanced_class_bg", "Rooms/AdvancedClass/Adv_Advanced_Class_Bg.png");
            LoadModTexture("adv_advanced_class_lamp", "Rooms/AdvancedClass/Adv_Advanced_Class_Lamp.png");

            LoadModTexture("adv_school_council_door_open", "Rooms/SchoolCouncil/Adv_School_Council_Door_Open.png");
            LoadModTexture("adv_school_council_door_closed", "Rooms/SchoolCouncil/Adv_School_Council_Door_Closed.png");
            LoadModTexture("adv_school_council_wall", "Rooms/SchoolCouncil/Adv_School_Council_Wall.png");
            LoadModTexture("adv_school_council_bg", "Rooms/SchoolCouncil/Adv_School_Council_Bg.png");
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

            LoadModTexture("adv_kitchen_stove", "Objects/KitchenStove/adv_kitchen_stove.png");

            LoadModTexture("adv_gum_dispenser", "Objects/GumDispenser/adv_gum_dispenser.png");

            for (int i = 0; i < 5; i++)
            {
                LoadModTexture("adv_pulley_base" + (i + 1), $"Objects/Pulley/adv_pulley_base{i + 1}.png");
            }

            LoadModSprite("adv_tip_screen_forward", "Textures/UI/SwingingTipsScreen/adv_tip_screen_forward.png");

            spriteSheets.Add("adv_tip_screen_forward_static_sheet",
                AssetLoader.SpritesFromSpritesheet(2, 1, 1f, Vector2.one * 0.5f,
                AssetsHelper.TextureFromFile("Textures/UI/SwingingTipsScreen/adv_tip_screen_forward_static_sheet.png")));

            spriteSheets.Add("adv_tips_screen", AssetLoader.SpritesFromSpritesheet(3, 2, 1f, Vector2.one * 0.5f,
                AssetsHelper.TextureFromFile("Textures/UI/SwingingTipsScreen/adv_swinging_tip_screen_sheet.png")));
            spriteSheets["adv_tips_screen"] =
                spriteSheets["adv_tips_screen"].Take(spriteSheets["adv_tips_screen"].Length - 1).ToArray();
            spriteSheets.Add("adv_tips_screen_reversed", spriteSheets["adv_tips_screen"].Reverse().ToArray());

            LoadModSprite("adv_pulley", "Textures/Objects/Pulley/adv_pulley_handle.png", 25f);

            for (int i = 1; i <= 3; i++)
            {
                LoadModSprite($"adv_elv_tubes_glow_{i}", $"Textures/UI/Elevator/Tubes/Elv_TubesGlow_{i}.png");
                LoadModSprite($"adv_elv_tube_mask_{i}", $"Textures/UI/Elevator/Tubes/Elv_Tube_Mask_{i}.png");
            }

            LoadModSprite("adv_gauge_protection", "Textures/Gauges/adv_gauge_protection.png");

            LoadModSprite("adv_obstacle_trick", "Textures/Objects/Plates/FakePlate/adv_obstacle_trick.png", 20f);
            LoadModSprite("adv_boxing_glove_trick", "Textures/Objects/Plates/FakePlate/adv_boxing_glove_trick.png", 20f);
            LoadModSprite("adv_exit", "Textures/UI/Buttons/adv_button_exit.png");
            LoadModSprite("adv_exit_transparent", "Textures/UI/Buttons/adv_button_exit_transparent.png");

            LoadModSprite("adv_mysterious_teleporter", "Textures/Items/LargeSprites/adv_mysterious_teleporter_large.png", 50f);
            LoadModSprite("adv_teleportation_bomb", "Textures/Items/LargeSprites/adv_teleportation_bomb_large.png", 50f);

            //Projectiles!!1!
            LoadModSprite("adv_anvil_projectile", "Textures/Objects/Projectiles/adv_anvil_projectile.png", 25f);

            LoadModSprite("adv_frozen_overlay", "Textures/UI/adv_frozen_overlay.png");
            LoadModSprite("adv_protected_overlay", "Textures/UI/adv_protected_overlay.png");

            LoadModSprite("adv_frozen_enemy", "Textures/Npcs/adv_frozen_enemy.png", 20f);
            LoadModSprite("adv_portal", "Textures/Objects/MysteriousPortal/adv_portal.png", 15f);
            LoadModSprite("adv_portal_opened", "Textures/Objects/MysteriousPortal/adv_portal_opened.png", 15f);
            LoadModSprite("adv_elephant_overlay", "Textures/UI/adv_elephant_overlay.png");
            LoadModSprite("adv_expel_hammer", "Textures/Items/adv_the_hammer_of_force.png", 40f);
            LoadModSprite("adv_arrows", "Textures/UI/adv_arrows.png", 70f);

            LoadModSprite("adv_dough", "Textures/Items/LargeSprites/adv_dough_large.png", 50f);

            //Lamps!!!
            LoadModSprite("adv_advanced_class_lamp", "Textures/Rooms/AdvancedClass/Adv_Advanced_Class_Lamp.png", 50f);

            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < alphabet.Length; i++)
            {
                string symbol = alphabet.ElementAt(i).ToString();
                LoadModSprite("adv_balloon_" + symbol, "Textures/Objects/Spelloons/adv_balloon_" + symbol + ".png", 30f);
            }

            LoadModSprite("adv_reaper", "Textures/Npcs/GottaReap/adv_reaper.png", 1.5f);
            LoadModSprite("adv_farm_flag", "Textures/Objects/Flags/adv_farm_flag.png", 5f, new Vector2(0.08f, 0.88f));
            LoadModSprite("adv_corn_sign1", "Textures/Objects/Signs/adv_corn_sign1.png", 25f);

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

#if DEBUG
            sw.Stop();
            AdvancedCore.Logging.LogInfo("Assets loaded in: " + sw.ElapsedMilliseconds + " ms");
#endif
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
                        poster = ObjectsStorage.Posters.Find(x => x.name == "Adv_Poster_Extra_Points"),
                        position = new IntVector2(36, 3),
                        direction = Direction.East
                    });

                    pitStop.posters.Add(new PosterData()
                    {
                        poster = ObjectsStorage.Posters.Find(x => x.name == "Adv_Poster_Symbol_Machine"),
                        position = new IntVector2(39, 4),
                        direction = Direction.East
                    });

                    if (!AssetsHelper.ModInstalled(IntegrationManager.recommendedCharactersId))
                    {
                        pitStop.posters.Add(new PosterData()
                        {
                            poster = ObjectsStorage.Posters.Find(x => x.name == "Adv_Poster_Recommended_Characters_Ad"),
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
                            poster = ObjectsStorage.Posters.Find(x => x.name == "Adv_Poster_Content_Packs_Ad"),
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
                    Structure_PitStopLevelStove stoveBuilder = 
                        new GameObject("Structure_JohnnyKitchenStove").AddComponent<Structure_PitStopLevelStove>();
                    stoveBuilder.gameObject.ConvertToPrefab(true);
                    stoveBuilder.InitializePrefab(1);
                    pitStop.structures.Add(new StructureBuilderData
                    {
                        prefab = stoveBuilder
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

        public static Material CreateMaterialByShader(string key, string shaderNameBase, Texture2D tex)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nCreating material by texture: " + tex.name);
#endif
            materials.Add(key, new Material(Shader.Find(shaderNameBase)));
            materials[key].mainTexture = tex;
            return materials[key];
        }

        public static Material CreateMaterial(string key, string matNameBase, Texture2D tex)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nCreating material by texture: " + tex.name);
#endif
            materials.Add(key, new Material(materials[matNameBase]));
            materials[key].mainTexture = tex;
            return materials[key];
        }

        public static SoundObject LoadModSound(string key, string path, SoundType soundType, string subKey, float subDuration = -1f)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading: " + path);
#endif
            if (string.IsNullOrEmpty(subKey)) subDuration = 0f;
            SoundObject sound = ObjectCreators.CreateSoundObject(AssetsHelper.AudioFromFile("Audio/" + path), subKey, soundType, Color.white, subDuration);
            sounds.Add(key, sound);
            return sound;
        }

        public static SoundObject LoadModSound(string key, string path, SoundType soundType, string subKey, Color color, float subDuration = 2f)
        {
            //if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path); //because used in other method
            SoundObject sound = LoadModSound(key, path, soundType, subKey, subDuration);
            sound.color = color;
            return sound;
        }

        public static void LoadModSprite(string key, string path, float pixelsPerUnit = 1f, Vector2? center = null)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading: " + path);
#endif
            Sprite sprite = null;

            if (center != null)
            {
                sprite = AssetsHelper.SpriteFromFile(path, pixelsPerUnit, center);
            } else
                sprite = AssetsHelper.SpriteFromFile(path, pixelsPerUnit, center);

            sprites.Add(key, sprite);
        }

        public static void LoadModTexture(string key, string path, bool doNotUnload = false)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading: " + path);
#endif
            Texture2D texture = AssetsHelper.TextureFromFile("Textures/" + path);
            textures.Add(key, texture);
        }

        public static void LoadGameObject(string key, string name, bool doNotUnload = false)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading from assets: " + name);
#endif
            GameObject _object = AssetsHelper.LoadAsset<GameObject>(name);
            gameObjects.Add(key, _object);
        }

        public static void LoadSprite(string key, string name, bool doNotUnload = false)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading from assets: " + name);
#endif
            Sprite sprite = AssetsHelper.LoadAsset<Sprite>(name);
            sprites.Add(key, sprite, doNotUnload);
        }

        public static void LoadTexture(string key, string name, bool doNotUnload = false)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading from assets: " + name);
#endif
            Texture2D sprite = AssetsHelper.LoadAsset<Texture2D>(name);
            textures.Add(key, sprite, doNotUnload);
        }

        public static void LoadMaterial(string key, string name, bool doNotUnload = false)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading from assets: " + name);
#endif
            Material material = AssetsHelper.LoadAsset<Material>(name);
            materials.Add(key, material, doNotUnload);
        }

        public static void LoadMesh(string key, string name, bool doNotUnload = false)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading from assets: " + name);
#endif
            Mesh material = AssetsHelper.LoadAsset<Mesh>(name);
            meshes.Add(key, material, doNotUnload);
        }

        public static void LoadSound(string key, string name, bool doNotUnload = false)
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("\nAssetsStorage\nLoading from assets: " + name);
#endif
            SoundObject sound = AssetsHelper.LoadAsset<SoundObject>(name);
            sounds.Add(key, sound, doNotUnload);
        }
    }
}
