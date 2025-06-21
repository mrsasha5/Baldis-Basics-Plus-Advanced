using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.LevelEditor;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{
    public class BasePlate : MonoBehaviour, IPrefab
    {
        [SerializeField]
        protected AudioManager audMan;

        [SerializeField]
        protected Material activatedMaterial;

        [SerializeField]
        protected Material deactivatedMaterial;

        [SerializeField]
        protected Sprite editorToolSprite;

        [SerializeField]
        protected List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        [SerializeField]
        protected Transform textBase;

        [SerializeField]
        protected TextMeshPro text;

        [SerializeField]
        protected SoundObject audPress;

        [SerializeField]
        protected SoundObject audUnpress;

        [SerializeField]
        protected float activeColorVal;

        [SerializeField]
        protected float disabledColorVal;

        [SerializeField]
        protected float colorTransitionSpeed;

        [SerializeField]
        protected float colorValue;

        //[SerializeField]
        [SerializeReference]
        protected PlateData plateData;

        //protected Cell cell;

        protected int turnOffs;

        protected EnvironmentController ec;

        protected List<Entity> entities = new List<Entity>();

        protected IEnumerator colorAnimator;

        protected int usedCount;

        protected bool toPress;

        protected bool pressed;

        protected float time;

        protected bool visualActiveState;

        public Sprite EditorToolSprite => editorToolSprite;

        public Texture UnpressedTex => deactivatedMaterial.mainTexture;

        public Texture PressedTex => activatedMaterial.mainTexture;

        public PlateData Data => plateData;

        public AudioManager AudMan => audMan;

        public EnvironmentController Ec => ec;

        public List<Entity> Entities => entities;

        protected virtual bool VisualPressedStateOverridden => false;

        protected virtual bool VisualActiveStateOverridden => false;

        protected virtual bool UnpressesItself => true;

        protected virtual bool UnpressesWithNoReason => false;

        protected virtual bool UpdatesPressedState => true;

        public virtual bool IsUsable => turnOffs <= 0 && (plateData.hasInfinityUses || usedCount < plateData.uses);

        public virtual float Timescale => ec.EnvironmentTimeScale;

        /*protected virtual void SetupLight()
        {
            if (Data.hasLight)
            {
                cell = ec.CellFromPosition(transform.position);
                if (cell != null)
                {
                    if (!cell.hasLight)
                    {
                        ec.GenerateLight(cell, Data.lightColor, Data.lightStrength);
                    }
                    else
                    {
                        cell.lightColor = Data.lightColor;
                        cell.lightStrength = Data.lightStrength;
                        cell.SetLight(true);
                    }
                }
            }
        }*/

        public virtual void SetTurnOff(bool state)
        {
            if (state && turnOffs == 0) OnTurnOff();
            else if (!state && turnOffs == 1) OnTurnOn();

            if (state)
                turnOffs++;
            else
                turnOffs--;

            if (turnOffs < 0) turnOffs = 0;

            UpdateVisualActiveState();
        }

        protected virtual void OnTurnOff()
        {
            
        }

        protected virtual void OnTurnOn()
        {
            
        }

        private void Awake()
        {
            ec = Singleton<BaseGameManager>.Instance.Ec;
            visualActiveState = IsUsable;
            VirtualAwake();
        }

        private void Start()
        {
            VirtualStart();
        }

        protected virtual void VirtualStart()
        {
            //SetupLight();
        }

        protected virtual void VirtualAwake()
        {
            
        }

        public virtual void InitializePrefab(int variant)
        {
            BoxCollider plateCollider = gameObject.AddComponent<BoxCollider>();
            plateCollider.size = new Vector3(10f, 10f, 10f);
            plateCollider.isTrigger = true;
            plateCollider.center = Vector3.up * 5f;

            gameObject.layer = LayersHelper.ignoreRaycastB;

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            audPress = AssetsStorage.sounds["button_unpress"];
            audUnpress = AssetsStorage.sounds["button_press"];

            activatedMaterial = new Material(AssetsStorage.materials["belt"]);
            deactivatedMaterial = new Material(AssetsStorage.materials["belt"]);

            disabledColorVal = 0.5f;
            activeColorVal = 1f;
            colorTransitionSpeed = 0.5f;
            colorValue = activeColorVal;

            plateData = new PlateData();
            SetValues(plateData);

            InitializeRenderer();
            meshRenderers.Last().material = deactivatedMaterial;

            SetTextures();

            InitializeText();

            SetVisualUses(usedCount, plateData.uses);            
        }

        protected virtual void SetValues(PlateData plateData)
        {
            plateData.timeToUnpress = 2f;
            plateData.targetsPlayer = false;
            plateData.showsUses = false;
            plateData.showsCooldown = false;
            plateData.uses = 0;
            plateData.hasInfinityUses = true;
            plateData.allowsToCopyTextures = true;
            //plateData.lightStrength = 16;
            //plateData.SetTargets(PlateTargets.AnyGroundedEntity);
        }

        protected virtual void InitializeRenderer()
        {
            MeshRenderer renderer = ObjectsCreator.CreateQuadRenderer();
            renderer.transform.SetParent(gameObject.transform, false);

            Quaternion rotation = renderer.transform.rotation;
            rotation.eulerAngles = new Vector3(90f, 0f, 0f);
            renderer.transform.rotation = rotation;

            meshRenderers.Add(renderer);
        }

        private void InitializeText()
        {
            if (plateData.showsUses || plateData.showsCooldown)
            {
                textBase = new GameObject("TextBase").transform;
                textBase.SetParent(gameObject.transform, true);
                textBase.localPosition = Vector3.up * 5f;

                text = Instantiate((TextMeshPro)AssetsStorage.texts["total_display"], textBase);
                text.text = "";
                text.gameObject.SetActive(true);
            }
        }

        protected virtual void SetTextures()
        {
            SetTexturesByBaseName("adv_pressure_plate");
            SetEditorSprite("adv_editor_acceleration_plate");
        }

        internal protected void SetTexturesByBaseName(string name)
        {
            activatedMaterial.mainTexture = AssetsStorage.textures[name + "_activated"];
            deactivatedMaterial.mainTexture = AssetsStorage.textures[name + "_deactivated"];
        }

        internal void SetEditorSprite(string name)
        {
            if (!IntegrationManager.IsActive<LevelEditorIntegration>()) return;
            editorToolSprite = AssetsStorage.sprites[name];
        }

        private void Update()
        {
            VirtualUpdate();
            if (colorAnimator != null && !colorAnimator.MoveNext())
            {
                colorAnimator = null;
            }
        }

        protected virtual void VirtualUpdate()
        {
            if (UpdatesPressedState)
            {
                if (toPress && !pressed)
                {
                    OnPress();
                    pressed = true;
                    time = plateData.timeToUnpress;
                }
                else if (pressed && (!toPress || UnpressesWithNoReason))
                {
                    time -= Time.deltaTime * Timescale;
                    if (time <= 0f && UnpressesItself)
                    {
                        OnUnpress();
                        pressed = false;
                    }
                }
            }

            toPress = false;
            entities.Clear();
        }

        protected virtual void UpdateVisualActiveState(bool? setState = null)
        {
            if (setState == null) setState = IsUsable;

            if (VisualActiveStateOverridden || visualActiveState == setState) return;

            //if (colorAnimator != null) StopCoroutine(colorAnimator);
            colorAnimator = ColorAnimator((bool)setState);
            //StartCoroutine(colorAnimator);

            visualActiveState = (bool)setState;
        }

        protected virtual IEnumerator ColorAnimator(bool setState)
        {
            colorValue = setState ? disabledColorVal : activeColorVal;

            Color color = new Color(colorValue, colorValue, colorValue, 1f);

            if (setState)
            {
                while (colorValue < activeColorVal)
                {
                    colorValue += Time.deltaTime * Timescale * colorTransitionSpeed;
                    color.r = colorValue;
                    color.g = colorValue;
                    color.b = colorValue;

                    UpdateMeshColors(ref color);
                    yield return null;
                }
                colorValue = activeColorVal;
                color.r = colorValue;
                color.g = colorValue;
                color.b = colorValue;
                UpdateMeshColors(ref color);
            } else
            {
                while (colorValue > disabledColorVal)
                {
                    colorValue -= Time.deltaTime * Timescale * colorTransitionSpeed;
                    color.r = colorValue;
                    color.g = colorValue;
                    color.b = colorValue;

                    UpdateMeshColors(ref color);
                    yield return null;
                }
                colorValue = disabledColorVal;
                color.r = colorValue;
                color.g = colorValue;
                color.b = colorValue;
                UpdateMeshColors(ref color);
            }
            
        }

        protected virtual void UpdateVisualPressedState(bool active, bool playSound = true)
        {
            if (VisualPressedStateOverridden) return;

            if (active)
            {
                meshRenderers[0].material = activatedMaterial;
                if (playSound) audMan.PlaySingle(audPress);
            } else
            {
                meshRenderers[0].material = deactivatedMaterial;
                if (playSound) audMan.PlaySingle(audUnpress);
            }

            if (!VisualActiveStateOverridden)
            {
                Color color = new Color(colorValue, colorValue, colorValue, 1f);
                UpdateMeshColors(ref color);
            }
        }

        public void UnpressImmediately(bool invokeEventOnUnpress = true, bool playSound = true)
        {
            time = 0f;
            toPress = false;
            pressed = false;
            UpdateVisualPressedState(false, playSound);
            if (invokeEventOnUnpress) VirtualOnUnpress();
            UpdateVisualActiveState();
        }

        protected void OnPress()
        {
            UpdateVisualPressedState(true);
            if (IsUsable)
            {
                if (!plateData.hasInfinityUses) usedCount++;
                SetVisualUses(usedCount, plateData.uses);
                VirtualOnPress();
            }
            UpdateVisualActiveState();
        }

        protected void OnUnpress()
        {
            UpdateVisualPressedState(false);
            VirtualOnUnpress();
            UpdateVisualActiveState();
        }

        protected virtual void VirtualOnPress()
        {
            
        }

        protected virtual void VirtualOnUnpress()
        {
            
        }

        private void OnTriggerStay(Collider other)
        {
            VirtualTriggerStay(other);
        }

        protected virtual void VirtualTriggerStay(Collider other)
        {
            if (plateData.targetsPlayer && !(other.CompareTag("Player"))) return;

            if (other.TryGetComponent(out Entity entity) && IsPressable(entity))
            {
                if (!entities.Contains(entity))
                {
                    toPress = true;
                    entities.Add(entity);
                    if (!pressed)
                    {
                        OnFirstTouch(entity);
                    }
                }
            }
            
        }

        protected virtual void OnFirstTouch(Entity entity)
        {

        }

        protected virtual bool IsPressable(Entity target)
        {
            return target.Grounded;
        }

        protected virtual bool SetVisualUses(int uses, int maxUses)
        {
            if (!Data.showsUses) return false;
            text.text = string.Join("", uses.ToString().Select(num => "<sprite=" + num + ">")) + "<sprite=10>" + string.Join("", maxUses.ToString().Select(num => "<sprite=" + num + ">"));
            return true;
        }

        protected virtual void UpdateMeshColors(ref Color color)
        {
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                meshRenderers[i].material.SetColor(color);
            }
        }

        /*bool IClickable<int>.ClickableHidden()
        {
            return true;
        }

        protected override void Pressed(int playerNumber)
        {

        }*/
    }
}
