using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Cache
{
    public class AssetsStorage
    {
        private static bool cached = false;

        private static bool overridden = false;

        public static Exception exception;

        private static bool Debugging => false;

        public static bool Cached => cached;

        public static bool Overridden => overridden;

        //game assets new standard

        public static readonly Dictionary<string, SoundObject> sounds = new Dictionary<string, SoundObject>();

        public static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        public static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static readonly Dictionary<string, ItemObject> itemObjects = new Dictionary<string, ItemObject>();

        public static readonly Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();

        public static readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();

        public static readonly Dictionary<string, TMP_Text> texts = new Dictionary<string, TMP_Text>();

        public static readonly Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();


        //game assets

        public static readonly BeltManager windManager;

        public static readonly Pickup pickup;

        public static readonly Transform windGraphicsParent;

        public static readonly AudioClip weirdErrorSound;

        //public static readonly GameObject exitConfirmChalkboardStore;

        public static readonly GameButton gameButton;

        public static readonly ChalkEraser chalkEraser; //ChalkCloud

        public static readonly SoundObject[] bullyTakeouts;

        public static readonly StandardDoor classDoor;

        public static readonly CursorController cursor;


        //mod assets


        static AssetsStorage()
        {
            try
            {
                //priority game assets

                Stopwatch sw = null;

                if (Debugging)
                {
                    sw = new Stopwatch();
                    sw.Start();
                }

                weirdErrorSound = AssetsHelper.LoadAsset<AudioClip>("WeirdError"); //this moved

                //game assets

                //smallComicFontAsset = AssetsHelper.loadAsset<TMP_FontAsset>("COMIC_12_Pro");
                windManager = AssetsHelper.LoadAsset<BeltManager>("BeltManager");
                windGraphicsParent = AssetsHelper.LoadAsset<Transform>("WindGraphicsParent");
                //exitConfirmChalkboardStore = AssetsHelper.loadAsset<GameObject>("ExitConfirm"); //Obsolete
                chalkEraser = AssetsHelper.LoadAsset<ChalkEraser>("ChalkCloud");
                pickup = Array.Find(AssetsHelper.LoadAssets<Pickup>("Pickup"), x => x.gameObject.activeSelf);
                classDoor = AssetsHelper.LoadAsset<StandardDoor>("ClassDoor_Standard");
                cursor = AssetsHelper.LoadAsset<CursorController>("CursorOrigin");
                gameButton = Array.Find(AssetsHelper.LoadAssets<GameButton>("GameButton"), x => x.gameObject.activeSelf);

                texts.Add("total_display", AssetsHelper.LoadAsset<TextMeshPro>("TotalDisplay"));

                bullyTakeouts = ReflectionHelper.GetValue<SoundObject[]>(AssetsHelper.LoadAsset<Bully>("Bully"), "takeouts");

                LoadSound("blowing", "Cmlo_Blowing");
                LoadSound("buzz_lose", "Lose_Buzz");
                LoadSound("buzz_elv", "Elv_Buzz");
                LoadSound("teleport", "Teleport");
                LoadSound("scissors", "Scissors");
                LoadSound("hammer", "DrR_Hammer");
                LoadSound("water_slurp", "WaterSlurp");
                LoadSound("bang", "Bang");
                LoadSound("button_press", "Sfx_Button_Press");
                LoadSound("button_unpress", "Sfx_Button_Unpress");
                LoadSound("bal_break", "BAL_Break");
                LoadSound("bal_wow", "BAL_Wow");
                LoadSound("grapple_launch", "GrappleLaunch");
                LoadSound("grapple_clang", "GrappleClang");
                LoadSound("nana_sput", "Nana_Sput");
                LoadSound("lock_door_stop", "LockDoorStop");
                LoadSound("clock_wind", "ClockWind");
                //loadSound("xylophone", "Xylophone");
                LoadSound("bell", "CashBell");
                LoadSound("pop", "Gen_Pop");
                LoadSound("elv_buzz", "Elv_Buzz");
                LoadSound("activity_correct", "Activity_Correct");
                LoadSound("activity_incorrect", "Activity_Incorrect");
                LoadSound("ytp_pickup_0", "YTPPickup_0");
                LoadSound("error_maybe", "ErrorMaybe");
                LoadSound("spit", "Ben_Spit");
                LoadSound("whoosh", "Ben_Gum_Whoosh");
                LoadSound("splat", "Ben_Splat");

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

                //QMarkSheet_0

                for (int i = 0; i <= 5; i++)
                {
                    LoadSprite("exclamation_point_sheet" + i, "ExclamationPoint_Sheet_" + i);
                }

                LoadTexture("qmark_sheet", "QMarkSheet");
                LoadTexture("regular_wall", "Wall");
                LoadTexture("regular_ceiling", "CeilingNoLight");
                LoadTexture("carpet", "Carpet");

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

                LoadGameObject("line_renderer_hook", "LineRenderer");
                LoadGameObject("cracks_hook", "Cracks");

                //mod assets

                
                LoadModTexture("adv_ballot_front", "Objects/VotingBallot/adv_ballot_front.png");
                LoadModTexture("adv_ballot_front_voting", "Objects/VotingBallot/adv_ballot_front_voting.png");
                LoadModTexture("adv_ballot_empty_top", "Objects/VotingBallot/adv_ballot_empty_top.png");
                LoadModTexture("adv_ballot_top", "Objects/VotingBallot/adv_ballot_top.png");

                LoadModTexture("adv_poster_symbol_machine", "Posters/adv_poster_symbol_machine.png");
                LoadModTexture("adv_poster_extra_points", "Posters/adv_poster_extra_points.png");
                LoadModTexture("adv_english_ceiling", "Cells/adv_english_ceiling.png");
                LoadModTexture("adv_english_floor", "Cells/adv_english_floor.png");
                LoadModTexture("adv_english_wall", "Cells/adv_english_wall.png");
                LoadModTexture("adv_english_class_door_closed", "Doors/adv_EnglishClass_door_closed.png");
                LoadModTexture("adv_english_class_door_open", "Doors/adv_EnglishClass_door_open.png");

                LoadModTexture("adv_rusty_rotohall", "Objects/RotoHall/adv_rusty_RotoHall.png");
                LoadModTexture("adv_rusty_rotohall_blank", "Objects/RotoHall/adv_rusty_RotoHall_blank.png");
                LoadModTexture("adv_rusty_rotohall_floor", "Objects/RotoHall/adv_rusty_RotoHall_floor.png");
                LoadModTexture("adv_rusty_rotohall_sign_left", "Objects/RotoHall/adv_rusty_RotoHall_sign_left.png");
                LoadModTexture("adv_rusty_rotohall_sign_right", "Objects/RotoHall/adv_rusty_RotoHall_sign_right.png");

                LoadModTexture("adv_advanced_math_machine_face", "Objects/AdvancedMathMachine/adv_advanced_math_machine_front.png");
                LoadModTexture("adv_advanced_math_machine_face_right", "Objects/AdvancedMathMachine/adv_advanced_math_machine_front_right.png");
                LoadModTexture("adv_advanced_math_machine_face_wrong", "Objects/AdvancedMathMachine/adv_advanced_math_machine_front_wrong.png");

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

                //EDITOR SPRITES
                LoadModSprite("adv_editor_invisibility_plate", "Compats/LevelEditor/Objects/adv_editor_invisibility_plate.png");
                LoadModSprite("adv_editor_acceleration_plate", "Compats/LevelEditor/Objects/adv_editor_acceleration_plate.png");
                LoadModSprite("adv_editor_noisy_plate", "Compats/LevelEditor/Objects/adv_editor_noisy_plate.png");
                LoadModSprite("adv_editor_stealing_plate", "Compats/LevelEditor/Objects/adv_editor_stealing_plate.png");
                LoadModSprite("adv_editor_bully_plate", "Compats/LevelEditor/Objects/adv_editor_bully_plate.png");
                LoadModSprite("adv_editor_present_plate", "Compats/LevelEditor/Objects/adv_editor_present_plate.png");
                LoadModSprite("adv_editor_sugar_addiction_plate", "Compats/LevelEditor/Objects/adv_editor_sugar_addiction_plate.png");
                LoadModSprite("adv_editor_slowdown_plate", "Compats/LevelEditor/Objects/adv_editor_slowdown_plate.png");
                LoadModSprite("adv_editor_protection_plate", "Compats/LevelEditor/Objects/adv_editor_protection_plate.png");
                LoadModSprite("adv_editor_teleportation_plate", "Compats/LevelEditor/Objects/adv_editor_teleportation_plate.png");
                LoadModSprite("adv_editor_gum_dispenser", "Compats/LevelEditor/Objects/adv_editor_gum_dispenser.png");
                LoadModSprite("adv_editor_voting_ballot", "Compats/LevelEditor/Objects/adv_editor_voting_ballot.png");
                LoadModSprite("adv_editor_advanced_math_machine", "Compats/LevelEditor/Objects/adv_editor_activity_advanced_math_machine.png");

                LoadModSprite("adv_editor_symbol_machine", "Compats/LevelEditor/Objects/adv_editor_symbol_machine.png");
                LoadModSprite("adv_editor_english_floor", "Compats/LevelEditor/Rooms/adv_room_english.png");
                LoadModSprite("adv_editor_english_floor_timer", "Compats/LevelEditor/Rooms/adv_room_english_timer.png");
                //loadModSprite("adv_editor_plate", "Compats/LevelEditor/Objects/adv_editor_pressure_plate.png");

                LoadModSprite("adv_exit", "UI/adv_exit.png");
                LoadModSprite("adv_exit_transparent", "UI/adv_exit_transparent.png");
                LoadModSprite("adv_mysterious_teleporter", "Items/LargeSprites/adv_mysterious_teleporter_large.png", 50f);
                LoadModSprite("adv_teleportation_bomb", "Items/LargeSprites/adv_teleportation_bomb_large.png", 50f);

                LoadModSprite("adv_frozen_overlay", "UI/adv_frozen_overlay.png");
                LoadModSprite("adv_protected_overlay", "UI/adv_protected_overlay.png");

                LoadModSprite("adv_frozen_enemy", "Npcs/adv_frozen_enemy.png", 20f);
                LoadModSprite("adv_portal", "Objects/adv_portal.png", 15f);
                LoadModSprite("adv_portal_opened", "Objects/adv_portal_opened.png", 15f);
                LoadModSprite("adv_elephant_overlay", "UI/adv_elephant_overlay.png");
                LoadModSprite("adv_bully_overlay", "UI/adv_bully_overlay.png");
                LoadModSprite("adv_expel_hammer", "Items/adv_the_hammer_of_force.png", 40f);
                LoadModSprite("adv_arrows", "UI/adv_arrows.png", 70f);

                for (int i = 1; i <= 4; i++)
                {
                    LoadModSprite("adv_gum_dispenser_" + i, "Objects/GumDispenser/adv_gum_dispenser_" + i + ".png", 25f);
                }

                string alphabet = "abcdefghijklmnopqrstuvwxyz";
                for (int i = 0; i < alphabet.Length; i++)
                {
                    string symbol = alphabet.ElementAt(i).ToString();
                    LoadModSprite("adv_balloon_" + symbol, "Objects/Spelloons/adv_balloon_" + symbol + ".png", 30f);
                }

                LoadModSprite("adv_fan_face_1", "Objects/Fan/adv_fan_face_1.png", 27f);
                LoadModSprite("adv_fan_face_2", "Objects/Fan/adv_fan_face_2.png", 27f);
                LoadModSprite("adv_fan_face_side_1", "Objects/Fan/adv_fan_face_side_1.png", 27f);
                LoadModSprite("adv_fan_face_side_2", "Objects/Fan/adv_fan_face_side_2.png", 27f);
                LoadModSprite("adv_fan_face_side_3", "Objects/Fan/adv_fan_face_side_3.png", 27f);
                LoadModSprite("adv_fan_face_side_4", "Objects/Fan/adv_fan_face_side_4.png", 27f);
                LoadModSprite("adv_fan_side_1", "Objects/Fan/adv_fan_side_1.png", 27f);
                LoadModSprite("adv_fan_side_2", "Objects/Fan/adv_fan_side_2.png", 27f);
                LoadModSprite("adv_fan_rear_side_1", "Objects/Fan/adv_fan_rear_side_1.png", 27f);
                LoadModSprite("adv_fan_rear_side_2", "Objects/Fan/adv_fan_rear_side_2.png", 27f);
                LoadModSprite("adv_fan_backside", "Objects/Fan/adv_fan_backside.png", 27f);

                LoadModSound("adv_mysterious_machine", "Sounds/Adv_Mysterious_Machine.wav", SoundType.Effect, "SubAdv_Machine", 4f);
                LoadModSound("adv_pah", "Sounds/Adv_PAH.ogg", SoundType.Effect, "SubAdv_Pah", 1f);
                LoadModSound("adv_inhale", "Sounds/Adv_Inhale.ogg", SoundType.Effect, "SubAdv_Inhale", 1f);
                LoadModSound("adv_boing", "Sounds/Adv_Boing.ogg", SoundType.Effect, "SubAdv_Boing", 1f);
                LoadModSound("adv_appearing", "Sounds/Adv_Appearing.ogg", SoundType.Effect, "SubAdv_Appearing", 2f);
                LoadModSound("adv_disappearing", "Sounds/Adv_Disappearing.ogg", SoundType.Effect, "SubAdv_Disappearing", 3f);
                LoadModSound("adv_emergency", "Sounds/Adv_Emergency.wav", SoundType.Effect, "SubAdv_Emergency", 1f);
                LoadModSound("adv_frozen", "Sounds/Adv_Frozen.ogg", SoundType.Effect, "SubAdv_Frozen", Color.blue, 3f);
                LoadModSound("adv_elephant_hit", "Sounds/Adv_Elephant_Hit.ogg", SoundType.Effect, "SubAdv_ElephantHit", 1f);
                LoadModSound("adv_metal_blow", "Sounds/Adv_Metal_Blow.ogg", SoundType.Effect, "SubAdv_MetalBlow", 1f);
                LoadModSound("adv_boost", "Sounds/Adv_Boost.ogg", SoundType.Effect, "SubAdv_Boost", 1f);
                LoadModSound("adv_balloon_inflation", "Sounds/Adv_Balloon_Inflation.wav", SoundType.Effect, "SubAdv_BalloonInflation", 2.5f)
                    .volumeMultiplier = 0.5f;
                LoadModSound("adv_symbol_machine_reinit", "Sounds/Adv_Symbol_Machine_ReInit.wav", SoundType.Effect, "SubAdv_SymbolMachineReInit", 5f);
                LoadModSound("adv_beep", "Sounds/Adv_Beep.wav", SoundType.Effect, "SubAdv_Beep", 0.5f);
                LoadModSound("adv_magical_appearing", "Sounds/Adv_Magical_Appearing.wav", SoundType.Effect, "SubAdv_MagicalAppearing", 5f);
                LoadModSound("adv_activation_start", "Sounds/Adv_Activation_Start.wav", SoundType.Effect, "SubAdv_Activation", 1f);
                LoadModSound("adv_activation_loop", "Sounds/Adv_Activation_Loop.wav", SoundType.Effect, "SubAdv_Activation", 1f);
                LoadModSound("adv_activation_end", "Sounds/Adv_Activation_End.wav", SoundType.Effect, "SubAdv_Activation_End", 2f);
                LoadModSound("adv_explosion", "Sounds/Adv_Explosion.wav", SoundType.Effect, "SubAdv_Explosion", 4f);
                LoadModSound("adv_protection", "Sounds/Adv_Protection.wav", SoundType.Effect, "SubAdv_Protection", 3f);
                LoadModSound("adv_protection_ended", "Sounds/Adv_Protection_Ended.wav", SoundType.Effect, "SubAdv_Protection_Ended", 1f);
                LoadModSound("adv_saved", "Sounds/Adv_Saved.wav", SoundType.Effect, "SubAdv_Saved", 2f);
                LoadModSound("adv_refresh", "Sounds/Adv_Refresh.wav", SoundType.Effect, "", 0f);
                LoadModSound("adv_rotohall_start", "Sounds/Adv_RotoHall_Start.wav", SoundType.Effect, "SubAdv_RotoHall_Start", 2f);
                LoadModSound("adv_rotohall_loop", "Sounds/Adv_RotoHall_Loop.wav", SoundType.Effect, "SubAdv_RotoHall_Loop", 2f);
                LoadModSound("adv_rotohall_end", "Sounds/Adv_RotoHall_End.wav", SoundType.Effect, "SubAdv_RotoHall_End", 2f);
                LoadModSound("adv_throwing_vote", "Sounds/Adv_Throwing_Vote.wav", SoundType.Effect, "SubAdv_ThrowingVote", 1f);
                LoadModSound("adv_bell", "Sounds/Adv_Bell.wav", SoundType.Effect, "SubAdv_Bell", 3f);

                //event sounds
                LoadModSound("adv_bal_event_portals", "Voices/Adv_Bal_Event_Portals.wav", SoundType.Voice, "SubAdv_Bal_Portals1", Color.green, 8.48f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Portals2",
                            time = 5.5f
                        }
                    };

                LoadModSound("adv_bal_event_disappearing_characters", "Voices/Adv_Bal_Event_Disappearing_Characters.wav",
                    SoundType.Voice, "SubAdv_Bal_Invisibility_Machine1", Color.green, 7f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Invisibility_Machine2",
                            time = 4.45f
                        }
                    };

                LoadModSound("adv_bal_event_cold_machine", "Voices/Adv_Bal_Event_Cold_Machine.wav",
                    SoundType.Voice, "SubAdv_Bal_Cold_Machine1", Color.green, 8.48f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Cold_Machine2",
                            time = 5.16f
                        }
                    };

                LoadModSound("adv_bal_event_voting", "Voices/Adv_Bal_Event_Voting.wav", SoundType.Voice, "SubAdv_Bal_Voting1", Color.green, 8.36f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Voting2",
                            time = 2.7f
                        },
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Voting3",
                            time = 4.3f
                        }
                    };

                LoadModSound("adv_bal_event_voting_ended", "Voices/Adv_Bal_Event_Voting_Ended.wav",
                    SoundType.Voice, "SubAdv_Bal_Voting_Ended1", Color.green, 6.438f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Voting_Ended2",
                            time = 1.89f
                        }
                    };

                LoadModSound("adv_bal_great_job", "Voices/Adv_Bal_Great_Job.wav",
                    SoundType.Voice, "SubAdv_Bal_Great_Job1", Color.green, 3.822f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Great_Job2",
                            time = 0.742f
                        }
                    };

                LoadModSound("adv_bal_fantastic", "Voices/Adv_Bal_Fantastic.wav",
                    SoundType.Voice, "SubAdv_Bal_Fantastic1", Color.green, 4.979f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Fantastic2",
                            time = 1.36f
                        }
                    };

                LoadModSound("adv_bal_incredible", "Voices/Adv_Bal_Incredible.wav",
                    SoundType.Voice, "SubAdv_Bal_Incredible1", Color.green, 3.76f)
                    .additionalKeys = new SubtitleTimedKey[]
                    {
                        new SubtitleTimedKey()
                        {
                            key = "SubAdv_Bal_Incredible2",
                            time = 0.8f
                        }
                    };

                materials.Add("adv_good_machine", new Material(materials["zesty_machine"]));
                materials["adv_good_machine"].mainTexture = AssetsHelper.TextureFromFile("Textures/VendingMachines/adv_GoodStuffsMachine.png");

                materials.Add("adv_good_machine_out", new Material(materials["zesty_machine"]));
                materials["adv_good_machine_out"].mainTexture = AssetsHelper.TextureFromFile("Textures/VendingMachines/adv_GoodStuffsMachine_Out.png");

                //door mats
                materials.Add("adv_english_class_open", new Material(materials["class_standard_open"]));
                materials["adv_english_class_open"].SetMainTexture(textures["adv_english_class_door_open"]);

                materials.Add("adv_english_class_closed", new Material(materials["class_standard_closed"]));
                materials["adv_english_class_closed"].SetMainTexture(textures["adv_english_class_door_closed"]);
                //door mats end.

                //Shader Graphs/Standard
                //Sprites/Default
                materials.Add("qmark_sheet", new Material(Shader.Find("Shader Graphs/Standard"))); //sprite_standard_billboard
                materials["qmark_sheet"].SetMainTexture(textures["qmark_sheet"]);

                CreateScriptableObjects();

                sounds["adv_emergency"].color = Color.red;

                cached = true;

                if (Debugging)
                {
                    sw.Stop();
                    AdvancedCore.Logging.LogInfo("Assets loaded in: " + sw.ElapsedMilliseconds + " ms");
                }

            } catch (Exception e)
            {
                exception = e;
                cached = false;
            }

        }

        private static void CreateScriptableObjects()
        {
            StandardDoorMats engDoorMats = ScriptableObject.CreateInstance<StandardDoorMats>();
            engDoorMats.name = "EnglishDoorSet";
            engDoorMats.open = materials["adv_english_class_open"];
            engDoorMats.shut = materials["adv_english_class_closed"];
        }

        public static void OverrideAssetsProperties()
        {
            if (!overridden)
            {
                //Elevator screen overrides
                ElevatorScreen elvScreen = AssetsHelper.LoadAsset<ElevatorScreen>("ElevatorScreen");
                elvScreen.GetComponent<AudioManager>().positional = false;

                //PIT overrides
                if (!PitOverrides.EnglishClassDisabled)
                {
                    StandardDoorMats engDoorMats = Array.Find(ScriptableObject.FindObjectsOfType<StandardDoorMats>(),
                    x => x.name == "EnglishDoorSet");

                    LevelAsset pitStop = AssetsHelper.LoadAsset<LevelAsset>("Pitstop");

                    //Adding special trigger & Acceleration Plate
                    RoomData hall = pitStop.rooms.Find(x => x.category == RoomCategory.Null);

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
                            prefab = ObjectsStorage.GameButtons["acceleration_plate"].transform,
                            position = new Vector3(335f, 0f, 35f)
                        }
                    );
                    //End


                    RoomData roomData = new RoomData()
                    {
                        name = "EnglishClass",
                        category = EnumExtensions.GetFromExtendedName<RoomCategory>("EnglishClass"),
                        keepTextures = true,
                        wallTex = textures["adv_english_wall"],
                        ceilTex = textures["adv_english_ceiling"],
                        florTex = textures["adv_english_floor"],
                        lightPre = AssetsHelper.LoadAsset<Transform>("HangingLight"),
                        roomFunctionContainer = ObjectsStorage.RoomFunctionsContainers["EnglishClassTimerFunction"]
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

                    pitStop.posters.Add(new PosterData()
                    {
                        poster = ObjectCreators.CreatePosterObject(textures["adv_poster_extra_points"], new PosterTextData[0]),
                        position = new IntVector2(36, 5),
                        direction = Direction.East
                    });

                    pitStop.posters.Add(new PosterData()
                    {
                        poster = ObjectCreators.CreatePosterObject(textures["adv_poster_symbol_machine"], new PosterTextData[0]),
                        position = new IntVector2(39, 4),
                        direction = Direction.East
                    });

                    //FluorescentLight
                    pitStop.lights.Add(new LightSourceData()
                    {
                        prefab = AssetsHelper.LoadAsset<Transform>("FluorescentLight"),
                        color = Color.white,
                        position = new IntVector2(38, 7),
                        strength = 10
                    });

                    pitStop.lights.Add(new LightSourceData()
                    {
                        prefab = AssetsHelper.LoadAsset<Transform>("FluorescentLight"),
                        color = Color.white,
                        position = new IntVector2(38, 4),
                        strength = 10
                    });
                }

                overridden = true;
            }

        }

        private static SoundObject LoadModSound(string key, string path, SoundType soundType, string subKey, float subDuration = 2f)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path);
            SoundObject sound = ObjectCreators.CreateSoundObject(AssetsHelper.GetAudioFromFile("Audio/" + path), subKey, soundType, Color.white, subDuration);
            sounds.Add(key, sound);
            return sound;
        }

        private static SoundObject LoadModSound(string key, string path, SoundType soundType, string subKey, Color color, float subDuration = 2f)
        {
            //if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path); //because used in other method
            SoundObject sound = LoadModSound(key, path, soundType, subKey, subDuration);
            sound.color = color;
            return sound;
        }

        private static void LoadModSprite(string key, string path, float pixelsPerUnit = 1f)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path);
            Sprite sprite = AssetsHelper.SpriteFromFile("Textures/" + path, pixelsPerUnit);
            sprites.Add(key, sprite);
        }

        private static void LoadModTexture(string key, string path, float pixelsPerUnit = 1f)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading: " + path);
            Texture2D texture = AssetsHelper.TextureFromFile("Textures/" + path, pixelsPerUnit);
            textures.Add(key, texture);
        }

        private static void LoadGameObject(string key, string name)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            GameObject _object = AssetsHelper.LoadAsset<GameObject>(name);
            gameObjects.Add(key, _object);
        }

        /*private static void cloneGameObject(string key, string name, bool toPrefab = true, bool setActive = true)
        {
            GameObject _object = UnityEngine.Object.Instantiate(AssetsHelper.loadAsset<GameObject>(name));
            gameObjects.Add(key, _object);
            if (toPrefab) _object.ConvertToPrefab(true);
        }*/

        private static void LoadSprite(string key, string name)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            Sprite sprite = AssetsHelper.LoadAsset<Sprite>(name);
            sprites.Add(key, sprite);
        }

        private static void LoadTexture(string key, string name)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            Texture2D sprite = AssetsHelper.LoadAsset<Texture2D>(name);
            textures.Add(key, sprite);
        }

        private static void LoadMaterial(string key, string name)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            Material material = AssetsHelper.LoadAsset<Material>(name);
            materials.Add(key, material);
        }

        private static void LoadMesh(string key, string name)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            Mesh material = AssetsHelper.LoadAsset<Mesh>(name);
            meshes.Add(key, material);
        }

        private static void LoadSound(string key, string name)
        {
            if (Debugging) AdvancedCore.Logging.LogInfo("Loading from assets: " + name);
            SoundObject sound = AssetsHelper.LoadAsset<SoundObject>(name);
            sounds.Add(key, sound);
        }

        internal static void Cache()
        {
            if (!cached)
            {
                new AssetsStorage(); //made to call static constructor
            }
        }
    }
}
