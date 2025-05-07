using BaldisBasicsPlusAdvanced.Cache;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{
    public class PlateBase : GameButton, IClickable<int>, IPrefab
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
        protected MeshRenderer meshRenderer;

        [SerializeField]
        protected Transform textBase;

        [SerializeField]
        protected TextMeshPro text;

        protected EnvironmentController ec;

        //[SerializeField]
        //THIS UNITY VERSION CANNOT SERIALIZE SERIALIZABLE STRUCTS! FUCK1!1!!
        protected PlateData plateData;

        protected List<Entity> entities = new List<Entity>();

        protected int usedCount;

        protected bool toPress;

        protected bool pressed;

        protected float time;

        public Sprite EditorToolSprite => editorToolSprite;

        protected virtual float Timescale => ec.EnvironmentTimeScale;

        private void Awake()
        {
            SetValues(ref plateData);
            if (Singleton<BaseGameManager>.Instance != null) ec = Singleton<BaseGameManager>.Instance.Ec;
        }

        public virtual void InitializePrefab()
        {
            BoxCollider plateCollider = gameObject.AddComponent<BoxCollider>();
            plateCollider.size = new Vector3(10f, 5f, 10f);
            plateCollider.isTrigger = true;

            audMan = gameObject.AddComponent<PropagatedAudioManager>();
            Destroy(audMan.audioDevice.gameObject); //because don't need this in prefabs

            activatedMaterial = new Material(AssetsStorage.materials["belt"]);
            deactivatedMaterial = new Material(AssetsStorage.materials["belt"]);

            SetTextures();

            //moved to awake
            //plateData = new PlateData();
            //setValues(ref plateData);

            InitializeRenderer();

            InitializeText();

            SetVisualUses(usedCount, plateData.uses);
        }

        protected virtual void SetValues(ref PlateData plateData)
        {
            plateData.timeToUnpress = 2f;
            plateData.targetPlayer = false;
            plateData.showUses = false;
            plateData.showCooldown = false;
            plateData.uses = 0;
            plateData.infinityUses = true;
            //plateData.setTargets(PlateTargets.AnyGroundedEntity);
        }

        private void InitializeRenderer()
        {
            GameObject childObj = new GameObject("Renderer");
            childObj.transform.SetParent(gameObject.transform, true);
            childObj.transform.localScale = new Vector3(10f, 10f, 1f);

            Quaternion rotation = childObj.transform.rotation;
            rotation.eulerAngles = new Vector3(90f, 0f, 0f); //90f 270f 0f
            childObj.transform.rotation = rotation;

            meshRenderer = childObj.AddComponent<MeshRenderer>();
            meshRenderer.material = deactivatedMaterial;
            MeshFilter meshFilter = childObj.AddComponent<MeshFilter>();
            meshFilter.mesh = AssetsStorage.meshes["quad"];
        }

        private void InitializeText()
        {
            if (plateData.showUses || plateData.showCooldown)
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

        protected void SetTexturesByBaseName(string name)
        {
            activatedMaterial.mainTexture = AssetsStorage.textures[name + "_activated"];
            deactivatedMaterial.mainTexture = AssetsStorage.textures[name + "_deactivated"];
        }

        protected void SetEditorSprite(string name)
        {
            editorToolSprite = AssetsStorage.sprites[name];
        }

        private void Update()
        {
            VirtualUpdate();
        }

        protected virtual void VirtualUpdate()
        {
            if (toPress && !pressed)
            {
                OnPress();
                pressed = true;
                time = plateData.timeToUnpress;
            }
            else if (!toPress && pressed)
            {
                time -= Time.deltaTime * Timescale;
                if (time < 0)
                {
                    OnUnpress();
                    pressed = false;
                }
            }

            //moved to more optimization
            //if (plateData.showUses) setVisualUses(usedCount, plateData.uses);
            toPress = false;
            entities.Clear();
        }

        protected void OnPress()
        {
            meshRenderer.material = activatedMaterial;
            audMan.PlaySingle(AssetsStorage.sounds["button_unpress"]);
            if (IsUsable())
            {
                if (!plateData.infinityUses) usedCount++;
                SetVisualUses(usedCount, plateData.uses);
                VirtualOnPress();
            }
        }

        protected void OnUnpress()
        {
            meshRenderer.material = deactivatedMaterial;
            audMan.PlaySingle(AssetsStorage.sounds["button_press"]);
            VirtualOnUnpress();
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
            if (plateData.targetPlayer && !(other.tag == "Player")) return;

            if (other.TryGetComponent(out Entity entity) && entity.Grounded && !entities.Contains(entity))
            {
                toPress = true;
                entities.Add(entity);
                if (!pressed) {
                    OnFirstTouch(entity);
                }
            }
        }

        protected virtual void OnFirstTouch(Entity entity)
        {

        }

        protected virtual bool IsUsable()
        {
            return plateData.infinityUses || usedCount < plateData.uses;
        }

        public virtual void ConnectTo(List<IButtonReceiver> receivers)
        {
            buttonReceivers = receivers;
        }

        protected virtual void ActivateReceivers()
        {
            for (int i = 0; i < buttonReceivers.Count; i++)
            {
                buttonReceivers[i].ButtonPressed(true);
            }
        }

        protected virtual bool SetVisualUses(int uses, int maxUses)
        {
            if (!plateData.showUses) return false;
            text.text = string.Join("", uses.ToString().Select(num => "<sprite=" + num + ">")) + "<sprite=10>" + string.Join("", maxUses.ToString().Select(num => "<sprite=" + num + ">"));
            return true;
        }

        bool IClickable<int>.ClickableHidden()
        {
            return true;
        }

        protected override void Pressed(int playerNumber)
        {

        }
    }
}
