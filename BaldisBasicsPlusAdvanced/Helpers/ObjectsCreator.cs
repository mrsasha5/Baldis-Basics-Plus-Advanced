using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.GameItems;
using BepInEx;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Game.Objects.Projectiles;
using BaldisBasicsPlusAdvanced.Game.Objects.Texts;
using BaldisBasicsPlusAdvanced.Game.Objects.Pickups;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class ObjectsCreator
    {
        public static StandardMenuButton CreateSpriteButton(Sprite sprite, Sprite spriteOnHighlight = null, Transform parent = null)
        {
            GameObject button = new GameObject("Button");
            if (parent != null)
            {
                button.transform.SetParent(parent);
                button.transform.localPosition = Vector3.zero;
            }

            button.layer = 5; //UI
            button.tag = "Button";

            Image image = button.gameObject.AddComponent<Image>();
            image.sprite = sprite;

            StandardMenuButton standardButton = button.gameObject.AddComponent<StandardMenuButton>();
            standardButton.OnPress = new UnityEvent();
            standardButton.OnRelease = new UnityEvent();
            if (spriteOnHighlight != null)
            {
                standardButton.OnHighlight = new UnityEvent();
                standardButton.swapOnHigh = true;
                standardButton.image = image;
                standardButton.highlightedSprite = spriteOnHighlight;
                standardButton.unhighlightedSprite = sprite;
            }
            

            return standardButton;
        }

        public static StandardMenuButton CreateSpriteButton(Sprite sprite, out Image image, Sprite spriteOnHighlight = null, bool changeSpriteOnHightlight = false, Transform parent = null)
        {
            GameObject button = new GameObject("Button");
            if (parent != null)
            {
                button.transform.SetParent(parent);
                button.transform.localPosition = Vector3.zero;
            }

            button.layer = 5; //UI
            button.tag = "Button";

            image = button.gameObject.AddComponent<Image>();
            image.sprite = sprite;

            StandardMenuButton standardButton = button.gameObject.AddComponent<StandardMenuButton>();
            standardButton.image = image;
            standardButton.unhighlightedSprite = sprite;
            standardButton.OnPress = new UnityEvent();
            standardButton.OnRelease = new UnityEvent();
            if (changeSpriteOnHightlight)
            {
                standardButton.OnHighlight = new UnityEvent();
                standardButton.swapOnHigh = true;
                standardButton.highlightedSprite = spriteOnHighlight;
            }


            return standardButton;
        }

        public static StandardMenuButton AddButtonProperties(TMP_Text text, bool underlineOnHighlight = false)
        {
            GameObject parent = new GameObject("Button");
            parent.layer = 5; //UI
            parent.tag = "Button";
            parent.transform.SetParent(text.transform.parent);
            parent.transform.position = text.transform.position;

            parent.gameObject.AddComponent<Image>().sprite = AssetsStorage.sprites["transparent"];
            text.transform.SetParent(parent.transform);
            text.transform.localPosition = Vector3.zero;

            StandardMenuButton standardButton = parent.gameObject.AddComponent<StandardMenuButton>();
            standardButton.OnPress = new UnityEvent();
            text.tag = "Button";

            if (underlineOnHighlight)
            {
                standardButton.OnHighlight = new UnityEvent();
                standardButton.underlineOnHigh = true;
                standardButton.text = text;
            }
            

            return standardButton;
        }

        public static StandardMenuButton AddButtonProperties(TMP_Text text, Vector2 sizeDelta, bool underlineOnHighlight = false)
        {
            GameObject parent = new GameObject("Button");
            parent.layer = 5; //UI
            parent.tag = "Button";
            parent.transform.SetParent(text.transform.parent);
            parent.transform.position = text.transform.position;

            Image image = parent.gameObject.AddComponent<Image>();
            image.sprite = AssetsStorage.sprites["transparent"];
            image.rectTransform.sizeDelta = sizeDelta;
            text.transform.SetParent(parent.transform);
            text.transform.localPosition = Vector3.zero;

            StandardMenuButton standardButton = parent.gameObject.AddComponent<StandardMenuButton>();
            standardButton.OnPress = new UnityEvent();
            text.tag = "Button";

            if (underlineOnHighlight)
            {
                standardButton.OnHighlight = new UnityEvent();
                standardButton.underlineOnHigh = true;
                standardButton.text = text;
            }


            return standardButton;
        }

        public static StandardMenuButton AddButtonProperties(TMP_Text text, UnityAction call, bool underlineOnHighlight = false)
        {
            StandardMenuButton standardMenuButton = AddButtonProperties(text, underlineOnHighlight);
            standardMenuButton.OnPress.AddListener(call);
            return standardMenuButton;
        }

        public static SpriteRenderer CreateSpriteRenderer(Sprite sprite, bool isBillboard)
        {
            SpriteRenderer spriteRenderer = new GameObject("Sprite").AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            if (isBillboard)
            {
                spriteRenderer.gameObject.layer = 9; //billboard
                spriteRenderer.material = new Material(AssetsStorage.materials["sprite_standard_billboard"]);
            }

            return spriteRenderer;
        }

        public static SpriteRenderer CreateSpriteRendererBase(Sprite sprite, bool isBillboard = true)
        {
            GameObject baseRenderer = new GameObject("RendererBase");
            SpriteRenderer spriteRenderer = CreateSpriteRenderer(sprite, isBillboard);
            spriteRenderer.transform.parent = baseRenderer.transform;
            return spriteRenderer;
        }

        public static AudioManager CreateAudMan(Vector3 pos)
        {
            GameObject gm = new GameObject("AudioManager");
            gm.transform.position = pos;

            AudioSource audDevice = gm.AddComponent<AudioSource>();
            AudioManager audMan = gm.AddComponent<AudioManager>();
            audMan.audioDevice = audDevice;
            return audMan;
        }

        public static PropagatedAudioManager CreatePropagatedAudMan(Vector3 pos)
        {
            GameObject gm = new GameObject("PropagatedAudioManager");
            gm.transform.position = pos;

            PropagatedAudioManager audMan = gm.AddComponent<PropagatedAudioManager>();
            return audMan;
        }

        public static PropagatedAudioManager CreatePropagatedAudMan(Vector3 pos, float aliveTime)
        {
            GameObject gm = new GameObject("PropagatedAudioManager");
            gm.transform.position = pos;

            PropagatedAudioManager audMan = gm.AddComponent<PropagatedAudioManager>();
            gm.AddComponent<TemporaryGameObject>().Initialize(GameCamera.dijkstraMap.EnvironmentController, gm, aliveTime);

            return audMan;
        }

        public static void AddChalkCloudEffect(NPC npc, float time, EnvironmentController ec)
        {
            GameObject gm = CreateChalkCloud(npc.transform.position, ec, npc.transform);

            TemporaryGameObject temporaryGm = gm.AddComponent<TemporaryGameObject>();
            temporaryGm.Initialize(ec, gm, time);
            
        }

        public static void AddChalkCloudEffect(Transform transform, float time, EnvironmentController ec)
        {
            GameObject gm = CreateChalkCloud(transform.position, ec, transform);

            TemporaryGameObject temporaryGm = gm.AddComponent<TemporaryGameObject>();
            temporaryGm.Initialize(ec, gm, time);

        }

        public static GameObject CreateChalkCloud(Vector3 position, EnvironmentController ec, Transform parent = null)
        {
            ChalkEraser chalkCloud = UnityEngine.Object.Instantiate(AssetsStorage.chalkEraser);
            position.y = ec.transform.position.y + 5f;
            chalkCloud.transform.localPosition = position;
            if (parent != null) chalkCloud.transform.SetParent(parent);
            UnityEngine.Object.Destroy(chalkCloud); //destroy component
            return chalkCloud.gameObject;
        }

        public static WindObject CreateWindObject(float windSize, float speed = 12f, bool dependsOnObstacles = true)
        {
            GameObject parent = new GameObject("Wind Object");
            PropagatedAudioManager audMan = parent.AddComponent<PropagatedAudioManager>();
            WindObject wind = parent.AddComponent<WindObject>();
            wind.Initialize(windSize, speed, parent.transform, dependsOnObstacles);
            return wind;
        }

        public static Canvas CreateCanvas(bool setGlobalCam, float planeDistance = 0.31f)
        {
            GameObject gameObj = new GameObject("Canvas");
            gameObj.layer = 5; //UI
            Canvas canvas = gameObj.AddComponent<Canvas>();
            CanvasScaler canvasScaler = gameObj.AddComponent<CanvasScaler>();
            gameObj.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal
                | AdditionalCanvasShaderChannels.Tangent;

            canvasScaler.referencePixelsPerUnit = 1f;
            canvasScaler.referenceResolution = new Vector2(480, 360);//480 360
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            canvasScaler.scaleFactor = 1f;

            ReflectionHelper.SetValue<float>(gameObj.AddComponent<PlaneDistance>(), "planeDistance", planeDistance); //2f || 100f || 0.31f?

            //NOT UI CAMERA!!!
            //canvas.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;

            //global cam!!!1
            if (setGlobalCam) canvas.worldCamera = Singleton<GlobalCam>.Instance.Cam;
            return canvas;
        }

        public static TimerText CreateBobTimerText(Transform parent)
        {
            Transform textBase = new GameObject("TextBase").transform;

            TextMeshPro text = GameObject.Instantiate((TextMeshPro)AssetsStorage.texts["total_display"], textBase);
            text.text = "";
            text.gameObject.SetActive(true);

            if (parent != null) textBase.parent = parent;

            TimerText bobTimer = text.gameObject.AddComponent<TimerText>();

            bobTimer.Setup(text);

            return bobTimer;
        }

        public static T CreateCustomPickup<T>(PriceTag tag, int price, Vector3 pos) where T : PickupBase
        {
            Pickup pickupComp = GameObject.Instantiate(AssetsStorage.pickup);

            T pickup = pickupComp.gameObject.AddComponent<T>();
            pickup.name = "Pickup";
            pickup.Initialize(pickup.GetComponentInChildren<SpriteRenderer>(), tag, price);
            pickup.transform.position = pos;

            pickup.SetSaleState(false);

            GameObject.Destroy(pickupComp);

            return pickup;
        }

        public static void CauseCrash(Exception e)
        {
            if (AssetsStorage.weirdErrorSound != null)
            {
                GameObject gm = new GameObject("99");
                AudioSource audDevice = gm.AddComponent<AudioSource>();
                audDevice.PlayOneShot(AssetsStorage.weirdErrorSound);
            }
            MTM101BaldiDevAPI.CauseCrash(AdvancedCore.Instance.Info, e);
        }

        public static void CauseCrash(PluginInfo info, Exception e)
        {
            if (AssetsStorage.weirdErrorSound != null)
            {
                GameObject gm = new GameObject("99");
                AudioSource audDevice = gm.AddComponent<AudioSource>();
                audDevice.PlayOneShot(AssetsStorage.weirdErrorSound);
            }
            MTM101BaldiDevAPI.CauseCrash(info, e);
        }
    }
}
