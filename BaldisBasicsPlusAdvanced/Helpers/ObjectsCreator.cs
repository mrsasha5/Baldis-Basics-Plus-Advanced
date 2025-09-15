using BaldisBasicsPlusAdvanced.Game.Components;
using MTM101BaldAPI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Game.Objects.Texts;
using BaldisBasicsPlusAdvanced.Game.Objects.Pickups;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using MTM101BaldAPI.UI;
using BaldisBasicsPlusAdvanced.Patches;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class ObjectsCreator
    {

        public static TextMeshPro CreateTextMesh(BaldiFonts font, Vector2? size = null, Transform parent = null, 
            Vector3? position = null)
        {
            RectTransform rect = new GameObject("TextMeshPro").AddComponent<RectTransform>();

            TextMeshPro text = rect.gameObject.AddComponent<TextMeshPro>();
            text.alignment = TextAlignmentOptions.Center;
            text.font = font.FontAsset();
            text.fontSize = font.FontSize();

            if (parent != null)
                rect.SetParent(parent, false);
            if (position != null)
                rect.localPosition = (Vector3)position;
            if (size != null)
                rect.sizeDelta = (Vector2)size;
            

            return text;
        }

        public static StandardMenuButton CreateSpriteButton(Sprite sprite, Vector3? position = null, Transform parent = null, 
            Sprite highlightedSprite = null, Sprite heldSprite = null)
        {
            Image image = UIHelpers.CreateImage(sprite, parent, Vector3.zero, false);
            image.name = "Button";
            image.ToCenter();
            if (position != null) image.transform.localPosition = (Vector3)position;

            GameObject button = image.gameObject;

            button.layer = LayersHelper.ui;
            button.tag = "Button";
            
            image.sprite = sprite;

            StandardMenuButton standardButton = button.gameObject.AddComponent<StandardMenuButton>();
            standardButton.image = image;
            standardButton.InitializeAllEvents();
            standardButton.unhighlightedSprite = sprite;
            if (highlightedSprite != null)
            {
                standardButton.swapOnHigh = true;
                standardButton.image = image;
                standardButton.highlightedSprite = highlightedSprite;
            }
            if (heldSprite != null)
            {
                standardButton.swapOnHold = true;
                standardButton.heldSprite = heldSprite;
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
            standardButton.InitializeAllEvents();
            standardButton.text = text;
            text.tag = "Button";

            if (underlineOnHighlight) standardButton.underlineOnHigh = true;

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
            standardButton.text = text;
            standardButton.InitializeAllEvents();
            text.tag = "Button";

            if (underlineOnHighlight) standardButton.underlineOnHigh = true;

            return standardButton;
        }

        public static StandardMenuButton AddButtonProperties(Image image)
        {
            StandardMenuButton standardButton = image.gameObject.AddComponent<StandardMenuButton>();
            standardButton.InitializeAllEvents();
            standardButton.image = image;
            standardButton.tag = "Button";
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
            spriteRenderer.gameObject.layer = LayersHelper.billboard;

            if (isBillboard)
            {
                spriteRenderer.material = new Material(AssetsStorage.materials["sprite_standard_billboard"]);
            }
            else
                spriteRenderer.material = new Material(AssetsStorage.materials["sprite_standard_no_billboard"]);

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
            GameObject audMan = new GameObject("AudioManager");
            audMan.transform.position = pos;
            return CreateAudMan(audMan);
        }

        public static AudioManager CreateAudMan(GameObject obj)
        {
            AudioSource audDevice = obj.AddComponent<AudioSource>();
            AudioManager audMan = obj.AddComponent<AudioManager>();
            audMan.audioDevice = audDevice;
            return audMan;
        }

        public static PropagatedAudioManager CreatePropagatedAudMan(Vector3 pos, bool destroyWhenAudioEnds = false)
        {
            GameObject gm = new GameObject("PropagatedAudioManager");
            gm.transform.position = pos;
            return CreatePropagatedAudMan(gm, destroyWhenAudioEnds);
        }

        public static PropagatedAudioManager CreatePropagatedAudMan(GameObject obj, bool destroyWhenAudioEnds = false)
        {
            PropagatedAudioManager audMan = obj.AddComponent<PropagatedAudioManager>();

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
            GameObject parent = new GameObject("WindObject");
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
            gameObj.layer = LayersHelper.ui;
            Canvas canvas = gameObj.AddComponent<Canvas>();
            CanvasScaler canvasScaler = gameObj.AddComponent<CanvasScaler>();
            gameObj.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal
                | AdditionalCanvasShaderChannels.Tangent;

            canvasScaler.referencePixelsPerUnit = 1f;
            canvasScaler.referenceResolution = new Vector2(480, 360);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            canvasScaler.scaleFactor = 1f;

            //Huh?
            ReflectionHelper.SetValue<float>(gameObj.AddComponent<PlaneDistance>(), "planeDistance", planeDistance); //2f || 100f || 0.31f?
            canvas.planeDistance = planeDistance;

            //NOT UI CAMERA!!!
            //canvas.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;

            //Global cam!!!1
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
                AudioClip clip = AssetsHelper.LoadAsset<AudioClip>("WeirdError");
                GameObject gm = new GameObject("99");
                AudioSource audDevice = gm.AddComponent<AudioSource>();
                audDevice.PlayOneShot(AssetsStorage.weirdErrorSound);
            }
            
            MTM101BaldiDevAPI.CauseCrash(AdvancedCore.Instance.Info, e);
        }
    }
}
