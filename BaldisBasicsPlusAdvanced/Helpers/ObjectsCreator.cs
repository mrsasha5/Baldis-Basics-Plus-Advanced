using BaldisBasicsPlusAdvanced.Game.Components;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Game.Objects.Texts;
using BaldisBasicsPlusAdvanced.Game.Objects.Pickups;
using Rewired.Demos;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using MTM101BaldAPI.UI;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class ObjectsCreator
    {

        public static StandardMenuButton CreateSpriteButton(Sprite sprite, Vector3? position = null, Transform parent = null, 
            Sprite spriteOnHighlight = null, Sprite spriteOnPress = null)
        {
            Image image = UIHelpers.CreateImage(sprite, parent, position != null ? (Vector3)position : Vector3.zero, false);
            image.name = "Button";

            GameObject button = image.gameObject;

            button.layer = LayersHelper.ui;
            button.tag = "Button";
            
            image.sprite = sprite;

            StandardMenuButton standardButton = button.gameObject.AddComponent<StandardMenuButton>();
            standardButton.image = image;
            standardButton.InitializeAllEvents();
            standardButton.unhighlightedSprite = sprite;
            if (spriteOnHighlight != null)
            {
                standardButton.swapOnHigh = true;
                standardButton.image = image;
                standardButton.highlightedSprite = spriteOnHighlight;
            }
            if (spriteOnPress != null)
            {
                standardButton.swapOnHold = true;
                standardButton.heldSprite = spriteOnPress;
            }

            return standardButton;
        }

        public static StandardMenuButton AddButtonProperties(TMP_Text text, bool underlineOnHighlight = false)
        {
            GameObject parent = new GameObject("Button");
            parent.layer = LayersHelper.ui;
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
            parent.layer = LayersHelper.ui;
            parent.tag = "Button";
            parent.transform.SetParent(text.transform.parent);
            parent.transform.position = text.transform.position;

            Image image = parent.gameObject.AddComponent<Image>();
            image.sprite = AssetsStorage.sprites["transparent"];
            image.rectTransform.sizeDelta = sizeDelta;
            text.transform.SetParent(parent.transform);
            text.transform.localPosition = Vector3.zero;

            StandardMenuButton standardButton = parent.gameObject.AddComponent<StandardMenuButton>();
            standardButton.InitializeAllEvents();
            text.tag = "Button";

            if (underlineOnHighlight)
            {
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

        public static SpriteRenderer CreateSpriteRenderer(Sprite sprite, bool isBillboard = true)
        {
            SpriteRenderer spriteRenderer = new GameObject("Sprite").AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.gameObject.layer = LayersHelper.billboard; //always needed I guess?

            if (isBillboard)
            {
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

        public static PropagatedAudioManager CreatePropagatedAudMan(Vector3 pos, bool destroyWhenAudioEnds = false)
        {
            GameObject gm = new GameObject("PropagatedAudioManager");
            gm.transform.position = pos;

            PropagatedAudioManager audMan = gm.AddComponent<PropagatedAudioManager>();

            if (destroyWhenAudioEnds)
            {
                audMan.gameObject.AddComponent<DestroyAudioOnEnd>().Assign(audMan, destroyWithObject: true);
            }

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
            CoverCloud chalkCloud = UnityEngine.Object.Instantiate(AssetsStorage.coverCloud);
            chalkCloud.StartDelay(0f);
            position.y = ec.transform.position.y + 5f;
            chalkCloud.transform.localPosition = position;
            if (parent != null) chalkCloud.transform.SetParent(parent);
            UnityEngine.Object.Destroy(chalkCloud); //destroy component
            return chalkCloud.gameObject;
        }

        public static MeshRenderer CreateQuadRenderer()
        {
            GameObject childObj = new GameObject("MeshRenderer");
            childObj.transform.localScale = new Vector3(10f, 10f, 1f);

            MeshRenderer meshRenderer = childObj.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(AssetsStorage.materials["belt"]);

            MeshFilter meshFilter = childObj.AddComponent<MeshFilter>();
            meshFilter.mesh = AssetsStorage.meshes["quad"];
            return meshRenderer;
        }

        public static WindObject CreateWindObject(float windSize, float speed = 12f, bool dependsOnObstacles = true)
        {
            GameObject parent = new GameObject("Wind Object");
            WindObject wind = parent.AddComponent<WindObject>();
            wind.Initialize(windSize, speed, parent.transform, dependsOnObstacles);
            return wind;
        }

        public static Canvas CreateCanvas(bool setGlobalCam, float planeDistance = 0.31f)
        {
            GameObject gameObj = new GameObject("Canvas");
            return CreateCanvas(gameObj, setGlobalCam, planeDistance);
        }

        public static Canvas CreateCanvas(GameObject gameObj, bool setGlobalCam, float planeDistance = 0.31f)
        {
            gameObj.layer = LayersHelper.ui; //UI
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

            //huh?
            ReflectionHelper.SetValue<float>(gameObj.AddComponent<PlaneDistance>(), "planeDistance", planeDistance); //2f || 100f || 0.31f?
            canvas.planeDistance = planeDistance;

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

        public static T CreateCustomPickup<T>(PriceTag tag, int price, Vector3 pos) where T : BasePickup
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

        public static TextMeshProUGUI CauseCrash(string text, AudioClip sound = null)
        {
            Canvas canvas = AssetsHelper.LoadAsset<Canvas>("EndingError");
            if (canvas == null)
            {
                AdvancedCore.Logging.LogError("Attempted to cause a crash before the EndingError was found!");
                return null;
            }

            GameObject obj = UnityEngine.Object.Instantiate(canvas).gameObject;
            obj.GetComponent<Canvas>().sortingOrder = 99;
            TextMeshProUGUI componentInChildren = obj.GetComponentInChildren<TextMeshProUGUI>();
            componentInChildren.text = text;
            componentInChildren.color = Color.white;
            //componentInChildren.transform.localPosition += Vector3.up * 32f;
            if (Singleton<BaseGameManager>.Instance != null)
            {
                Singleton<BaseGameManager>.Instance.Ec.PauseEnvironment(true);
            }

            if (CursorController.Instance != null)
            {
                CursorController.Instance.enabled = false;
            }

            Time.timeScale = 0f;
            obj.gameObject.SetActive(value: true);

            if (sound != null)
            {
                GameObject gm = new GameObject("99");
                AudioSource audDevice = gm.AddComponent<AudioSource>();
                audDevice.PlayOneShot(sound);
            }

            return componentInChildren;
        }
    }
}
