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

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class ObjectsCreator
    {
        public static Projectile createProjectile<T>(string name, Sprite sprite, Vector3 pos, EnvironmentController ec, Vector3 forward, float radius) where T : Projectile
        {
            GameObject gm = new GameObject(name);
            gm.layer = 0;// LayerMask.NameToLayer("StandardEntities");

            T projectile = gm.AddComponent<T>();

            ActivityModifier activity = gm.AddComponent<ActivityModifier>();

            Transform spriteBase = createSpriteRendererBillboard(sprite).transform;
            spriteBase.SetParent(gm.transform);
            spriteBase.localPosition = Vector3.zero;

            SphereCollider sphereCollider = gm.AddComponent<SphereCollider>();
            sphereCollider.radius = radius;
            sphereCollider.isTrigger = true;

            projectile.Initialize(ec, pos, forward);

            Rigidbody rb = gm.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.angularDrag = 0;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.freezeRotation = true;
            rb.inertiaTensor = Vector3.one;
            rb.isKinematic = true;
            rb.mass = 0;

            return projectile;
        }

        public static StandardMenuButton createSpriteButton(Sprite sprite, Sprite spriteOnHighlight = null, Transform parent = null)
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

        public static StandardMenuButton createSpriteButton(Sprite sprite, out Image image, Sprite spriteOnHighlight = null, bool changeSpriteOnHightlight = false, Transform parent = null)
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

        public static StandardMenuButton addButtonProperties(TMP_Text text, bool underlineOnHighlight = false)
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

        public static StandardMenuButton addButtonProperties(TMP_Text text, Vector2 sizeDelta, bool underlineOnHighlight = false)
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

        public static StandardMenuButton addButtonProperties(TMP_Text text, UnityAction call, bool underlineOnHighlight = false)
        {
            StandardMenuButton standardMenuButton = addButtonProperties(text, underlineOnHighlight);
            standardMenuButton.OnPress.AddListener(call);
            return standardMenuButton;
        }

        public static SpriteRenderer createSpriteRenderer(Sprite sprite)
        {
            SpriteRenderer spriteRenderer = new GameObject("Sprite").AddComponent<SpriteRenderer>();
            spriteRenderer.gameObject.layer = 9; //billboard
            spriteRenderer.sprite = sprite;
            return spriteRenderer;
        }

        public static GameObject createSpriteRendererBillboard(Sprite sprite)
        {
            GameObject baseRenderer = new GameObject("RendererBase");
            SpriteRenderer spriteRenderer = createSpriteRenderer(sprite);
            spriteRenderer.gameObject.layer = 9; //billboard
            spriteRenderer.transform.parent = baseRenderer.transform;
            spriteRenderer.material = new Material(AssetsStorage.materials["sprite_standard_billboard"]);
            return baseRenderer;
        }

        public static AudioManager createAudMan(Vector3 pos)
        {
            GameObject gm = new GameObject("AudioManager");
            gm.transform.position = pos;

            AudioSource audDevice = gm.AddComponent<AudioSource>();
            AudioManager audMan = gm.AddComponent<AudioManager>();
            audMan.audioDevice = audDevice;
            return audMan;
        }

        public static PropagatedAudioManager createPropagatedAudMan(Vector3 pos)
        {
            GameObject gm = new GameObject("PropagatedAudioManager");
            gm.transform.position = pos;

            PropagatedAudioManager audMan = gm.AddComponent<PropagatedAudioManager>();
            return audMan;
        }

        public static PropagatedAudioManager createPropagatedAudMan(Vector3 pos, float aliveTime)
        {
            GameObject gm = new GameObject("PropagatedAudioManager");
            gm.transform.position = pos;

            PropagatedAudioManager audMan = gm.AddComponent<PropagatedAudioManager>();
            gm.AddComponent<TemporaryGameObject>().initialize(GameCamera.dijkstraMap.EnvironmentController, gm, aliveTime);

            return audMan;
        }

        public static void addChalkCloudEffect(NPC npc, float time, EnvironmentController ec)
        {
            GameObject gm = createChalkCloud(npc.transform.position, ec, npc.transform);

            TemporaryGameObject temporaryGm = gm.AddComponent<TemporaryGameObject>();
            temporaryGm.initialize(ec, gm, time);
            
        }

        public static void addChalkCloudEffect(PlayerManager pm, float time, EnvironmentController ec)
        {
            GameObject gm = createChalkCloud(pm.transform.position, ec, pm.transform);

            TemporaryGameObject temporaryGm = gm.AddComponent<TemporaryGameObject>();
            temporaryGm.initialize(ec, gm, time);

        }

        public static GameObject createChalkCloud(Vector3 position, EnvironmentController ec, Transform parent = null)
        {
            ChalkEraser chalkCloud = UnityEngine.Object.Instantiate(AssetsStorage.chalkEraser);
            position.y = ec.transform.position.y + 5f;
            chalkCloud.transform.localPosition = position;
            if (parent != null) chalkCloud.transform.SetParent(parent);
            UnityEngine.Object.Destroy(chalkCloud); //destroy component
            return chalkCloud.gameObject;
        }

        /*public static WindObject createWindObject(float windSize, float speed = 12f, bool dependsOnObstacles = true)
        {
            GameObject parent = new GameObject("Wind Object");
            PropagatedAudioManager audMan = parent.AddComponent<PropagatedAudioManager>();
            WindObject wind = parent.AddComponent<WindObject>();
            wind.initialize(windSize, speed, parent.transform, dependsOnObstacles);
            return wind;
        }*/

        public static void causeCrash(Exception e)
        {
            if (AssetsStorage.weirdErrorSound != null)
            {
                GameObject gm = new GameObject("99");
                AudioSource audDevice = gm.AddComponent<AudioSource>();
                audDevice.PlayOneShot(AssetsStorage.weirdErrorSound);
            }
            MTM101BaldiDevAPI.CauseCrash(AdvancedCore.Instance.Info, e);
        }

        public static void causeCrash(PluginInfo info, Exception e)
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
