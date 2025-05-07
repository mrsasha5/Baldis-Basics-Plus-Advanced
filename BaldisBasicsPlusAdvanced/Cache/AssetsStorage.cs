using BaldisBasicsPlusAdvanced.API;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.UI;
using Rewired.Integration.UnityUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Cache
{
    public class AssetsStorage
    {
        private static bool cached = false;

        private static bool overridden = false;

        public static Exception exception;

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

        public static readonly AudioClip weirdErrorSound = AssetsHelper.loadAsset<AudioClip>("WeirdError");

        public static readonly GameObject exitConfirmChalkboardStore;

        public static readonly ChalkEraser chalkEraser; //ChalkCloud

        public static readonly SoundObject[] bullyTakeouts;


        //mod assets


        static AssetsStorage()
        {
            try
            {
                //priority game assets

                //weirdErrorSound = AssetsHelper.loadAsset<AudioClip>("WeirdError"); //this moved

                //game assets

                //smallComicFontAsset = AssetsHelper.loadAsset<TMP_FontAsset>("COMIC_12_Pro");
                windManager = AssetsHelper.loadAsset<BeltManager>("BeltManager");
                windGraphicsParent = AssetsHelper.loadAsset<Transform>("WindGraphicsParent");
                exitConfirmChalkboardStore = AssetsHelper.loadAsset<GameObject>("ExitConfirm");
                chalkEraser = AssetsHelper.loadAsset<ChalkEraser>("ChalkCloud");
                pickup = AssetsHelper.loadAsset<Pickup>("Pickup");

                texts.Add("total_display", AssetsHelper.loadAsset<TextMeshPro>("TotalDisplay"));

                bullyTakeouts = ReflectionHelper.getValue<SoundObject[]>(AssetsHelper.loadAsset<Bully>("Bully"), "takeouts");

                loadSound("blowing", "Cmlo_Blowing");
                loadSound("Jon_InfoExpel", "Jon_InfoExpel");
                loadSound("buzz_lose", "Lose_Buzz");
                loadSound("buzz_elv", "Elv_Buzz");
                loadSound("teleport", "Teleport");
                loadSound("scissors", "Scissors");
                loadSound("hammer", "DrR_Hammer");
                loadSound("whoosh", "Ben_Gum_Whoosh");
                loadSound("water_slurp", "WaterSlurp");
                loadSound("bang", "Bang");
                loadSound("button_press", "Sfx_Button_Press");
                loadSound("button_unpress", "Sfx_Button_Unpress");
                loadSound("bal_break", "BAL_Break");
                loadSound("bal_wow", "BAL_Wow");
                loadSound("grapple_launch", "GrappleLaunch");
                loadSound("grapple_clang", "GrappleClang");
                loadSound("nana_sput", "Nana_Sput");
                loadSound("lock_door_stop", "LockDoorStop");
                loadSound("clock_wind", "ClockWind");
                loadSound("xylophone", "Xylophone");
                loadSound("bell", "BellGeneric");
                loadSound("pop", "Gen_Pop");
                loadSound("elv_buzz", "Elv_Buzz");

                loadSprite("elv_button_up", "Button_Up");
                loadSprite("elv_button_down", "Button_Down");
                loadSprite("transparent", "Transparent");
                loadSprite("menuArrow0", "MenuArrowSheet_0");
                loadSprite("menuArrow1", "MenuArrowSheet_1");
                loadSprite("menuArrow2", "MenuArrowSheet_2");
                loadSprite("menuArrow3", "MenuArrowSheet_3");
                loadSprite("grappling_hook", "GrapplingHookSprite");

                //Quad Instance
                loadMesh("quad", "Quad");

                // SpriteStandard_Billboard (Instance)
                loadMaterial("wind", "Wind");
                loadMaterial("sprite_standard_billboard", "SpriteStandard_Billboard");
                loadMaterial("zesty_machine", "ZestyMachine");
                loadMaterial("belt", "Belt");
                loadMaterial("math_front_normal", "math_front_normal");
                loadMaterial("math_side", "math_side");

                loadGameObject("line_renderer_hook", "LineRenderer");
                loadGameObject("cracks_hook", "Cracks");

                //mod assets

                loadModTexture("adv_symbol_machine_face", "Objects/adv_symbol_machine_front.png");
                loadModTexture("adv_symbol_machine_side", "Objects/adv_symbol_machine_side.png");

                loadModTexture("adv_pressure_plate_activated", "Objects/plate/adv_pressure_plate_activated.png");
                loadModTexture("adv_pressure_plate_deactivated", "Objects/plate/adv_pressure_plate_deactivated.png");

                loadModTexture("adv_invisibility_plate_activated", "Objects/plate/adv_invisibility_plate_activated.png");
                loadModTexture("adv_invisibility_plate_deactivated", "Objects/plate/adv_invisibility_plate_deactivated.png");

                loadModTexture("adv_acceleration_plate_activated", "Objects/plate/adv_acceleration_plate_activated.png");
                loadModTexture("adv_acceleration_plate_deactivated", "Objects/plate/adv_acceleration_plate_deactivated.png");

                loadModTexture("adv_noisy_plate_activated", "Objects/plate/adv_noisy_plate_activated.png");
                loadModTexture("adv_noisy_plate_deactivated", "Objects/plate/adv_noisy_plate_deactivated.png");

                loadModTexture("adv_stealing_plate_activated", "Objects/plate/adv_stealing_plate_activated.png");
                loadModTexture("adv_stealing_plate_deactivated", "Objects/plate/adv_stealing_plate_deactivated.png");

                loadModTexture("adv_bully_plate_activated", "Objects/plate/adv_bully_plate_activated.png");
                loadModTexture("adv_bully_plate_deactivated", "Objects/plate/adv_bully_plate_deactivated.png");

                loadModTexture("adv_present_plate_activated", "Objects/plate/adv_present_plate_activated.png");
                loadModTexture("adv_present_plate_deactivated", "Objects/plate/adv_present_plate_deactivated.png");

                loadModTexture("adv_sugar_addiction_plate_activated", "Objects/plate/adv_sugar_addiction_plate_activated.png");
                loadModTexture("adv_sugar_addiction_plate_deactivated", "Objects/plate/adv_sugar_addiction_plate_deactivated.png");

                loadModTexture("adv_slowdown_plate_activated", "Objects/plate/adv_slowdown_plate_activated.png");
                loadModTexture("adv_slowdown_plate_deactivated", "Objects/plate/adv_slowdown_plate_deactivated.png");

                loadModSprite("adv_editor_invisibility_plate", "Compats/LevelEditor/Objects/adv_editor_invisibility_plate.png");
                loadModSprite("adv_editor_acceleration_plate", "Compats/LevelEditor/Objects/adv_editor_acceleration_plate.png");
                loadModSprite("adv_editor_noisy_plate", "Compats/LevelEditor/Objects/adv_editor_noisy_plate.png");
                loadModSprite("adv_editor_stealing_plate", "Compats/LevelEditor/Objects/adv_editor_stealing_plate.png");
                loadModSprite("adv_editor_bully_plate", "Compats/LevelEditor/Objects/adv_editor_bully_plate.png");
                loadModSprite("adv_editor_present_plate", "Compats/LevelEditor/Objects/adv_editor_present_plate.png");
                loadModSprite("adv_editor_sugar_addiction_plate", "Compats/LevelEditor/Objects/adv_editor_sugar_addiction_plate.png");
                loadModSprite("adv_editor_slowdown_plate", "Compats/LevelEditor/Objects/adv_editor_slowdown_plate.png");
                //loadModSprite("adv_editor_plate", "Compats/LevelEditor/Objects/adv_editor_pressure_plate.png");

                loadModSprite("adv_exit", "UI/adv_exit.png");
                loadModSprite("adv_exit_transparent", "UI/adv_exit_transparent.png");
                loadModSprite("adv_mysterious_teleporter", "Items/LargeSprites/adv_mysterious_teleporter_large.png", 50f);
                loadModSprite("adv_frozen_overlay", "UI/adv_frozen_overlay.png");
                loadModSprite("adv_frozen_enemy", "Npcs/adv_frozen_enemy.png", 20f);
                loadModSprite("adv_portal", "Objects/adv_portal.png", 15f);
                loadModSprite("adv_portal_opened", "Objects/adv_portal_opened.png", 15f);
                loadModSprite("adv_authentic_border", "UI/adv_authentic_border.png");
                loadModSprite("adv_item_discount", "UI/adv_item_discount.png");
                loadModSprite("adv_authentic_label", "Compats/LevelEditor/adv_editor_authentic_mode.png", 5f);
                loadModSprite("adv_authentic_label_slot", "Compats/LevelEditor/adv_editor_authentic_mode_slot.png");
                loadModSprite("adv_elephant_overlay", "UI/adv_elephant_overlay.png");
                loadModSprite("adv_bully_overlay", "UI/adv_bully_overlay.png");

                string alphabet = "abcdefghijklmnopqrstuvwxyz";
                for (int i = 0; i < alphabet.Length; i++)
                {
                    string symbol = alphabet.ElementAt(i).ToString();
                    loadModSprite("adv_balloon_" + symbol, "Objects/spelloons/adv_balloon_" + symbol + ".png", 30f);
                }

                /*loadModSprite("adv_fan_face_1", "Objects/fan/adv_fan_face_1.png", 27f);
                loadModSprite("adv_fan_face_2", "Objects/fan/adv_fan_face_2.png", 27f);
                loadModSprite("adv_fan_face_side_1", "Objects/fan/adv_fan_face_side_1.png", 27f); //28
                loadModSprite("adv_fan_face_side_2", "Objects/fan/adv_fan_face_side_2.png", 27f); //28
                loadModSprite("adv_fan_face_side_3", "Objects/fan/adv_fan_face_side_3.png", 27f); //28
                loadModSprite("adv_fan_face_side_4", "Objects/fan/adv_fan_face_side_4.png", 27f); //28
                loadModSprite("adv_fan_side_1", "Objects/fan/adv_fan_side_1.png", 27f); //28
                loadModSprite("adv_fan_side_2", "Objects/fan/adv_fan_side_2.png", 27f); //28
                loadModSprite("adv_fan_rear_side_1", "Objects/fan/adv_fan_rear_side_1.png", 27f); //28
                loadModSprite("adv_fan_rear_side_2", "Objects/fan/adv_fan_rear_side_2.png", 27f); //28
                loadModSprite("adv_fan_backside", "Objects/fan/adv_fan_backside.png", 27f);*/

                loadModSound("adv_mysterious_machine", "adv_Mysterious_Machine.wav", SoundType.Effect, "SubAdv_Machine", 4f);
                loadModSound("adv_pah", "adv_PAH.ogg", SoundType.Effect, "SubAdv_Pah", 1f);
                loadModSound("adv_inhale", "adv_Inhale.ogg", SoundType.Effect, "SubAdv_Inhale", 1f);
                loadModSound("adv_boing", "adv_Boing.ogg", SoundType.Effect, "SubAdv_Boing", 1f);
                loadModSound("adv_appearing", "adv_Appearing.ogg", SoundType.Effect, "SubAdv_Appearing", 2f);
                loadModSound("adv_disappearing", "adv_Disappearing.ogg", SoundType.Effect, "SubAdv_Disappearing", 3f);
                loadModSound("adv_emergency", "adv_Emergency.wav", SoundType.Effect, "SubAdv_Emergency", 2f);
                loadModSound("adv_frozen", "adv_Frozen.ogg", SoundType.Effect, "SubAdv_Frozen", Color.blue, 3f);
                loadModSound("adv_elephant_hit", "adv_Elephant_Hit.ogg", SoundType.Effect, "SubAdv_ElephantHit", 1f);
                loadModSound("adv_metal_blow", "adv_Metal_Blow.ogg", SoundType.Effect, "SubAdv_MetalBlow", 1f);
                loadModSound("adv_boost", "adv_Boost.ogg", SoundType.Effect, "SubAdv_Boost", 1f);

                materials.Add("adv_good_machine", new Material(materials["zesty_machine"]));
                materials["adv_good_machine"].mainTexture = AssetsHelper.textureFromFile("Textures/VendingMachines/adv_GoodStuffsMachine.png");

                materials.Add("adv_good_machine_out", new Material(materials["zesty_machine"]));
                materials["adv_good_machine_out"].mainTexture = AssetsHelper.textureFromFile("Textures/VendingMachines/adv_GoodStuffsMachine_Out.png");

                sounds["adv_emergency"].color = Color.red;

                cached = true;

            } catch (Exception e)
            {
                exception = e;
                cached = false;
            }

        }

        public static void overrideAssetsProperties()
        {
            if (!overridden)
            {
                //authentic border
                if (ApiManager.AuthenticModeExtended)
                {
                    Vector2[] positions = new Vector2[] {
                    new Vector2(28f, 139f),
                    new Vector2(74f, 139f),
                    new Vector2(118f, 139f),
                    new Vector2(164f, 139f),
                    new Vector2(210f, 139f),
                    new Vector2(99f, 99f) //99
                    };

                    Canvas authenticCanvas = AssetsHelper.loadAsset<Canvas>("AuthenticModeCanvas");
                    AuthenticScreenManager authenticManager = authenticCanvas.GetComponent<AuthenticScreenManager>();
                    Image authenticBorder = Array.Find(authenticCanvas.GetComponentsInChildren<Image>(), x => x.name == "Border");
                    RectTransform itemsParent = Array.Find(authenticCanvas.GetComponentsInChildren<RectTransform>(), x => x.name == "Items");
                    HudManager authenticHudManager = authenticCanvas.GetComponent<HudManager>();

                    Array.Find(itemsParent.GetComponentsInChildren<RectTransform>(), x => x.name == "BG").sizeDelta =
                        new Vector2(236, 64); //changing bg size

                    Array.Find(authenticCanvas.GetComponentsInChildren<RectTransform>(), x => x.name == "NotebookText").
                        localScale = Vector3.one * 0.75f; // changing text scale

                    authenticBorder.sprite = sprites["adv_authentic_border"];

                    StandardMenuButton basePrefabButtonInstance = Array.Find(authenticCanvas.GetComponentsInChildren<StandardMenuButton>(), x => x.name == "ItemButton_0");

                    //items rendering
                    for (int n = 3; n <= 4; n++)
                    {
                        Image itemImage = new GameObject("Item_" + n).AddComponent<Image>();
                        itemImage.MarkAsNeverUnload();
                        itemImage.gameObject.layer = LayerMask.NameToLayer("UI");
                        itemImage.rectTransform.sizeDelta = Vector2.one * 32;
                        itemImage.transform.SetParent(itemsParent.transform, false);
                        itemImage.transform.localScale = Vector3.one;
                        itemImage.transform.SetSiblingIndex(n + 1);
                    }

                    int counter = 0;
                    int buttonCounter = 0;

                    Image[] itemSprites = ReflectionHelper.getValue<Image[]>(authenticHudManager, "itemSprites");

                    foreach (Image image in itemsParent.GetComponentsInChildren<Image>())
                    {
                        if (image.name == "Item_" + counter)
                        {
                            image.transform.localPosition = positions[counter];
                            itemSprites[counter] = image;
                            counter++;
                        }

                        if (image.name == "ItemButton_" + buttonCounter)
                        {
                            image.transform.localPosition = positions[buttonCounter];
                            buttonCounter++;
                        }
                    }

                    ReflectionHelper.setValue<Image[]>(authenticHudManager, "itemSprites", itemSprites);
                }

                //authentic border ends

                overridden = true;
            }

        }

        private static SoundObject loadModSound(string key, string path, SoundType soundType, string subKey = "somesound", float subDuration = 2f)
        {
            SoundObject sound = ObjectCreators.CreateSoundObject(AssetsHelper.getAudioFromFile("Audio/Sounds/" + path), subKey, soundType, Color.white, subDuration);
            sounds.Add(key, sound);
            return sound;
        }

        private static SoundObject loadModSound(string key, string path, SoundType soundType, string subKey, Color color, float subDuration = 2f)
        {
            SoundObject sound = loadModSound(key, path, soundType, subKey, subDuration);
            sound.color = color;
            return sound;
        }

        private static void loadModSprite(string key, string path, float pixelsPerUnit = 1f)
        {
            Sprite sprite = AssetsHelper.spriteFromFile("Textures/" + path, pixelsPerUnit);
            sprites.Add(key, sprite);
        }

        private static void loadModTexture(string key, string path, float pixelsPerUnit = 1f)
        {
            Texture2D texture = AssetsHelper.textureFromFile("Textures/" + path, pixelsPerUnit);
            textures.Add(key, texture);
        }

        private static void loadGameObject(string key, string name)
        {
            GameObject _object = AssetsHelper.loadAsset<GameObject>(name);
            gameObjects.Add(key, _object);
        }

        /*private static void cloneGameObject(string key, string name, bool toPrefab = true, bool setActive = true)
        {
            GameObject _object = UnityEngine.Object.Instantiate(AssetsHelper.loadAsset<GameObject>(name));
            gameObjects.Add(key, _object);
            if (toPrefab) _object.ConvertToPrefab(true);
        }*/

        private static void loadSprite(string key, string name)
        {
            Sprite sprite = AssetsHelper.loadAsset<Sprite>(name);
            sprites.Add(key, sprite);
        }

        private static void loadMaterial(string key, string name)
        {
            Material material = AssetsHelper.loadAsset<Material>(name);
            materials.Add(key, material);
        }

        private static void loadMesh(string key, string name)
        {
            Mesh material = AssetsHelper.loadAsset<Mesh>(name);
            meshes.Add(key, material);
        }

        private static void loadSound(string key, string name)
        {
            SoundObject sound = AssetsHelper.loadAsset<SoundObject>(name);
            sounds.Add(key, sound);
        }

        internal static void cache()
        {
            if (!cached)
            {
                new AssetsStorage(); //made to call static constructor
            }
        }
    }
}
