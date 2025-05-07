using BaldisBasicsPlusAdvanced.Cache;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base
{
    public class BasePlate : GameButton, IClickable<int>, IPrefab
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
            setValues(ref plateData);
            if (Singleton<BaseGameManager>.Instance != null) ec = Singleton<BaseGameManager>.Instance.Ec;
        }

        public virtual void initializePrefab()
        {
            BoxCollider plateCollider = gameObject.AddComponent<BoxCollider>();
            plateCollider.size = new Vector3(10f, 5f, 10f);
            plateCollider.isTrigger = true;

            audMan = gameObject.AddComponent<PropagatedAudioManager>();
            Destroy(audMan.audioDevice.gameObject); //because don't need this in prefabs

            activatedMaterial = new Material(AssetsStorage.materials["belt"]);
            deactivatedMaterial = new Material(AssetsStorage.materials["belt"]);

            setTextures();

            //moved to awake
            //plateData = new PlateData();
            //setValues(ref plateData);

            initializeRenderer();

            initializeText();

            setVisualUses(usedCount, plateData.uses);
        }

        protected virtual void setValues(ref PlateData plateData)
        {
            plateData.timeToUnpress = 2f;
            plateData.targetPlayer = false;
            plateData.showUses = false;
            plateData.showCooldown = false;
            plateData.uses = 0;
            plateData.infinityUses = true;
            //plateData.setTargets(PlateTargets.AnyGroundedEntity);
        }

        private void initializeRenderer()
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

        private void initializeText()
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

        protected virtual void setTextures()
        {
            setTexturesByBaseName("adv_pressure_plate");
            setEditorSprite("adv_editor_acceleration_plate");
        }

        protected void setTexturesByBaseName(string name)
        {
            activatedMaterial.mainTexture = AssetsStorage.textures[name + "_activated"];
            deactivatedMaterial.mainTexture = AssetsStorage.textures[name + "_deactivated"];
        }

        protected void setEditorSprite(string name)
        {
            editorToolSprite = AssetsStorage.sprites[name];
        }

        private void Update()
        {
            virtualUpdate();
        }

        protected virtual void virtualUpdate()
        {
            if (toPress && !pressed)
            {
                onPress();
                pressed = true;
                time = plateData.timeToUnpress;
            }
            else if (!toPress && pressed)
            {
                time -= Time.deltaTime * Timescale;
                if (time < 0)
                {
                    onUnpress();
                    pressed = false;
                }
            }

            //moved to more optimization
            //if (plateData.showUses) setVisualUses(usedCount, plateData.uses);
            toPress = false;
            entities.Clear();
        }

        protected void onPress()
        {
            meshRenderer.material = activatedMaterial;
            audMan.PlaySingle(AssetsStorage.sounds["button_unpress"]);
            if (isUsable())
            {
                if (!plateData.infinityUses) usedCount++;
                setVisualUses(usedCount, plateData.uses);
                virtualOnPress();
            }
        }

        protected void onUnpress()
        {
            meshRenderer.material = deactivatedMaterial;
            audMan.PlaySingle(AssetsStorage.sounds["button_press"]);
            virtualOnUnpress();
        }

        protected virtual void virtualOnPress()
        {
            
        }

        protected virtual void virtualOnUnpress()
        {
            
        }

        private void OnTriggerStay(Collider other)
        {
            virtualTriggerStay(other);
        }

        protected virtual void virtualTriggerStay(Collider other)
        {
            if (plateData.targetPlayer && !(other.tag == "Player")) return;

            if (other.TryGetComponent(out Entity entity) && entity.Grounded && !entities.Contains(entity))
            {
                toPress = true;
                entities.Add(entity);
                if (!pressed) {
                    onFirstTouch(entity);
                }
            }
        }

        protected virtual void onFirstTouch(Entity entity)
        {

        }

        protected virtual bool isUsable()
        {
            return plateData.infinityUses || usedCount < plateData.uses;
        }

        public virtual void connectTo(List<IButtonReceiver> receivers)
        {
            buttonReceivers = receivers;
        }

        protected virtual void activateReceivers()
        {
            for (int i = 0; i < buttonReceivers.Count; i++)
            {
                buttonReceivers[i].ButtonPressed(true);
            }
        }

        protected virtual bool setVisualUses(int uses, int maxUses)
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
