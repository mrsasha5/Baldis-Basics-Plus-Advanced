using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Compats;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{

    public class BasePlate : MonoBehaviour, IPrefab
    {
        [SerializeField]
        protected AudioManager audMan;

        [SerializeField]
        protected SoundObject audCooldownEnds;

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
        protected TextMeshPro indicator;

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

        [SerializeReference]
        protected PlateData data;

        [SerializeField]
        protected int cooldownIgnores;

        protected float? overriddenCooldown;

        protected float cooldownTime;

        protected bool lockedByCooldown;

        //protected Cell cell;

        protected int turnOffs;

        protected EnvironmentController ec;

        protected List<Entity> entities = new List<Entity>();

        protected IEnumerator colorAnimator;

        protected int uses;

        protected bool toPress;

        protected bool pressed;

        protected float time;

        protected bool visualActiveState;

        protected virtual bool VisualPressedStateOverridden => false;

        protected virtual bool VisualActiveStateOverridden => false;

        protected virtual bool UsesSystemOverridden => false;

        protected virtual bool UnpressesItself => true;

        protected virtual bool UnpressesWithNoReason => false;

        protected virtual bool UpdatesPressedState => true;

        public virtual bool IsUsable => turnOffs <= 0 && (data.hasInfinityUses || uses < data.maxUses) && !lockedByCooldown;

        public virtual float Timescale => ec.EnvironmentTimeScale;

        public Texture UnpressedTex => deactivatedMaterial.mainTexture;

        public Texture PressedTex => activatedMaterial.mainTexture;

        public PlateData Data => data;

        public AudioManager AudMan => audMan;

        public EnvironmentController Ec => ec;

        public List<Entity> Entities => entities;

        public MeshRenderer[] Renderers => Renderers;

        public TextMeshPro Indicator => indicator;

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

        public virtual void InitializePrefab(int variant)
        {
            audCooldownEnds = AssetsStorage.sounds["bell"];
            audPress = AssetsStorage.sounds["button_unpress"];
            audUnpress = AssetsStorage.sounds["button_press"];

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(10f, 10f, 10f);
            collider.isTrigger = true;
            collider.center = Vector3.up * 5f;

            gameObject.layer = LayersHelper.ignoreRaycastB;

            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            activatedMaterial = new Material(AssetsStorage.materials["belt"]);
            deactivatedMaterial = new Material(AssetsStorage.materials["belt"]);

            disabledColorVal = 0.5f;
            activeColorVal = 1f;
            colorTransitionSpeed = 0.5f;
            colorValue = activeColorVal;

            data = new PlateData();
            SetValues(data);

            InitializeRenderer();
            meshRenderers.Last().material = deactivatedMaterial;

            SetTextures();

            InitializeText();

            SetVisualUses(uses, data.maxUses);
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

        protected virtual void SetValues(PlateData data)
        {
            data.timeToUnpress = 2f;
            data.showsUses = false;
            data.showsCooldown = false;
            data.maxUses = 0;
            data.hasInfinityUses = true;
            data.allowsToCopyTextures = true;
            //plateData.lightStrength = 16;
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
            textBase = new GameObject("TextBase").transform;
            textBase.SetParent(gameObject.transform, true);
            textBase.localPosition = Vector3.up * 5f;

            indicator = Instantiate((TextMeshPro)AssetsStorage.texts["total_display"], textBase);
            indicator.text = "";
            indicator.gameObject.SetActive(true);
            indicator.rectTransform.sizeDelta = new Vector2(10f, 4f);
        }

        protected virtual void SetTextures()
        {
            SetTexturesByBaseName("adv_pressure_plate");
        }

        internal protected void SetTexturesByBaseName(string name)
        {
            activatedMaterial.mainTexture = AssetsStorage.textures[name + "_activated"];
            deactivatedMaterial.mainTexture = AssetsStorage.textures[name + "_deactivated"];
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
                    time = data.timeToUnpress;
                }
                else if (pressed && (!toPress || UnpressesWithNoReason))
                {
                    if (time > 0f) 
                        time -= Time.deltaTime * Timescale;

                    if (time <= 0f && UnpressesItself)
                    {
                        OnUnpress();
                        pressed = false;
                    }
                }
            }

            if (lockedByCooldown && turnOffs <= 0 && cooldownTime > 0f)
            {
                cooldownTime -= Time.deltaTime * Timescale;

                SetVisualCooldown((int)cooldownTime + 1);

                if (cooldownTime <= 0f)
                {
                    OnCooldownEnded();
                }
            }

            toPress = false;
            entities.Clear();
        }

        protected virtual void UpdateVisualActiveState(bool? setState = null)
        {
            if (setState == null) setState = IsUsable;

            if (VisualActiveStateOverridden || visualActiveState == setState) return;

            colorAnimator = ColorAnimator((bool)setState);

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
                if (!UsesSystemOverridden) LoseOneUse();
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
            if (other.TryGetComponent(out Entity entity) && IsPressable(entity))
            {
                if (!entities.Contains(entity))
                {
                    toPress = true;
                    entities.Add(entity);
                }
            }
            
        }

        public void SetIgnoreCooldown(bool state)
        {
            if (state)
            {
                cooldownIgnores++;
            }
            else
            {
                cooldownIgnores--;
            }

            if (cooldownIgnores < 0) cooldownIgnores = 0;
        }

        public virtual void OnCooldownEnded()
        {
            lockedByCooldown = false;
            cooldownTime = 0f;
            if (indicator != null) indicator.text = "";
            if (audCooldownEnds != null) audMan.PlaySingle(audCooldownEnds);
            SetVisualUses(uses, data.maxUses);
            UpdateVisualActiveState();
        }

        public void ForcefullyPatchCooldown(float newCooldown)
        {
            overriddenCooldown = newCooldown;
        }

        public void CancelCooldownPatch()
        {
            overriddenCooldown = null;
        }

        public void SetCooldown(float cooldown)
        {
            if (cooldownIgnores > 0) return;
            if (cooldown <= 0f && overriddenCooldown == null || overriddenCooldown == 0f) return;

            if (overriddenCooldown != null)
                cooldown = overriddenCooldown.Value;

            cooldownTime = cooldown;
            lockedByCooldown = true;

            SetVisualCooldown((int)cooldown + 1);
            UpdateVisualActiveState();
        }

        public void SetMaxUses(int uses)
        {
            data.SetUses(uses);
            SetVisualUses(this.uses, data.maxUses);
            UpdateVisualActiveState();
        }

        protected void LoseOneUse()
        {
            if (!data.hasInfinityUses) uses++;
            SetVisualUses(uses, data.maxUses);
        }

        public void SetUses(int uses)
        {
            if (uses <= data.maxUses)
            {
                this.uses = uses;
                SetVisualUses(uses, data.maxUses);
                UpdateVisualActiveState();
            }
        }

        protected bool SetVisualCooldown(int cooldown)
        {
            if (!Data.showsCooldown) return false;
            indicator.text = string.Join("", cooldown.ToString().Select(num => "<sprite=" + num + ">"));
            return true;
        }

        public virtual void TurnOff(bool state)
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
            if (indicator != null) indicator.text = "";
        }

        protected virtual void OnTurnOn()
        {

        }

        protected virtual bool IsPressable(Entity target)
        {
            return target.Grounded;
        }

        protected virtual bool SetVisualUses(int uses, int maxUses)
        {
            if (!Data.showsUses || lockedByCooldown) return false;
            indicator.text = string.Join("", 
                uses.ToString().Select(num => "<sprite=" + num + ">")) + "<sprite=10>" + 
                    string.Join("", maxUses.ToString().Select(num => "<sprite=" + num + ">"));
            return true;
        }

        protected virtual void UpdateMeshColors(ref Color color)
        {
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                meshRenderers[i].material.SetColor(color);
            }
        }



    }
}
