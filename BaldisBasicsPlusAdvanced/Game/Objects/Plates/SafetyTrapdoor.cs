using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using BaldisBasicsPlusAdvanced.SaveSystem.Managers;
using MTM101BaldAPI.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Plates
{
    public class SafetyTrapdoor : BaseCooldownPlate
    {
        [SerializeField]
        private float sinkSpeed;

        [SerializeField]
        private float hidingCooldown;

        [SerializeField]
        private float minSinkPercent;

        [SerializeField]
        private float maxFogDistance;

        [SerializeField]
        private InteractionObject interactionObject;

        private EntityOverrider overrider;

        private Fog fog;

        private Canvas[] canvases = new Canvas[2];

        private TMP_Text canvasText;

        private float hidingTime;

        private bool entering;

        private bool releasing;

        protected override bool UpdatesPressedState => false;

        protected override bool VisualActiveStateOverridden => true;

        public override void InitializePrefab(int variant)
        {
            base.InitializePrefab(1);
            GameObject gm = new GameObject("InteractionObject");
            gm.transform.parent = transform;
            gm.transform.localPosition = Vector3.up * 5f;
            gm.SetActive(false); //hide for the editor

            interactionObject = gm.AddComponent<InteractionObject>();
            interactionObject.trapdoor = this;

            SphereCollider collider = gm.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 5f;

            minSinkPercent = 0.04f;
            sinkSpeed = 0.5f;
            hidingCooldown = 20f;
            maxFogDistance = 1000f;

            audPress = AssetsStorage.sounds["adv_wood_1"];
            audUnpress = AssetsStorage.sounds["adv_wood_2"];
        }

        protected override void VirtualStart()
        {
            base.VirtualStart();
            interactionObject.gameObject.SetActive(true);
            //transform.position = transform.position.CorrectForCell(y: 0f); //Haha, no map creators.
            //if I will invent how to create a halls for the cell models, then I will return it.
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (overrider != null)
            {
                if (!entering && hidingTime > 0f)
                {
                    hidingTime -= Time.deltaTime * Timescale;
                    UpdateText();
                }

                if (overrider.entity == null)
                {
                    overrider = null;
                    return;
                }

                if (!releasing && Time.deltaTime != 0f &&
                    !entering && (hidingTime <= 0f || (Input.GetKeyDown(KeyBindingsManager.Keys["exit_from_trapdoor"].Button) &&
                    overrider.entity.CompareTag("Player"))))
                {
                    ReleaseEntity();
                }
            }
        }

        protected override void SetValues(PlateData plateData)
        {
            base.SetValues(plateData);

            plateData.showsCooldown = true;
            plateData.allowsToCopyTextures = false;
        }

        protected override void SetTextures()
        {
            SetTexturesByBaseName("adv_safety_trapdoor");
            SetEditorSprite("adv_editor_safety_trapdoor");
        }

        private void ReleaseEntity()
        {
            StartCoroutine(Releasing());
        }

        private IEnumerator Releasing()
        {
            float sinkPercent = minSinkPercent;

            releasing = true;

            OnPress();

            if (canvases[0] != null) Destroy(canvases[0].gameObject);
            if (canvases[1] != null) Destroy(canvases[1].gameObject);

            while (sinkPercent < 1f)
            {
                if (overrider.entity == null)
                {
                    overrider = null;
                    UnpressImmediately();
                    if (fog != null)
                    {
                        ec.RemoveFog(fog);
                        fog = null;
                    }
                    yield break;
                }

                if (fog != null)
                {
                    if (sinkPercent >= 1f) fog.maxDist = maxFogDistance;
                    else fog.maxDist = (sinkPercent - minSinkPercent) * maxFogDistance;
                    ec.UpdateFog();
                }

                sinkPercent += Time.deltaTime * Timescale * sinkSpeed;
                overrider.SetHeight(overrider.entity.InternalHeight * sinkPercent);
                yield return null;
            }

            if (overrider.entity != null && overrider.entity.CompareTag("Player"))
                overrider.entity.GetComponent<PlayerManager>().SetHidden(false);

            overrider.Release();
            overrider = null;
            ec.RemoveFog(fog); //it already invokes UpdateFog so I don't need to update it
            fog = null;
            UnpressImmediately();
            SetCooldown(100f);

            releasing = false;

            yield break;
        }

        private IEnumerator Entering(Entity entity)
        {
            float sinkPercent = 1f;

            overrider.SetHeight(entity.InternalHeight * sinkPercent);
            if (entity.CompareTag("Player"))
            {
                fog = new Fog();

                fog.priority = int.MaxValue;
                fog.color = Color.black;
                fog.strength = 1f;
                fog.maxDist = maxFogDistance;

                ec.AddFog(fog);
                overrider.entity.GetComponent<PlayerManager>().SetHidden(true);

                //TODO: SOLVE ALL UI PROBLEMS
                //Did I? Don't remember.
                canvases[0] = ObjectsCreator.CreateCanvas(setGlobalCam: true, planeDistance: 2f); 
                canvases[0].worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;
                CanvasScaler scaler = canvases[0].GetComponent<CanvasScaler>();
                scaler.screenMatchMode = ScreenMatchMode.MatchWidthOrHeight;

                Image image = UIHelpers.CreateImage(AssetsStorage.sprites["adv_white"], canvases[0].transform, Vector3.zero,
                    correctPosition: false);
                image.ToCenter();
                image.rectTransform.sizeDelta = new Vector2(480f, 360f);
                image.color = Color.black;

                canvases[1] = ObjectsCreator.CreateCanvas(setGlobalCam: true);
                canvases[1].worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(0).canvasCam;

                canvasText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "", canvases[1].transform, Vector3.zero, false);
                canvasText.alignment = TextAlignmentOptions.Center;
                canvasText.rectTransform.sizeDelta = new Vector2(300, 50);

                canvases[0].gameObject.SetActive(false);
                canvases[1].gameObject.SetActive(false);
            }

            entering = true;
            while (sinkPercent > minSinkPercent)
            {
                if (entity == null)
                {
                    overrider = null;
                    entering = false;
                    if (fog != null)
                    {
                        ec.RemoveFog(fog);
                    }
                    yield break;
                }

                sinkPercent -= Time.deltaTime * Timescale * sinkSpeed;

                if (fog != null)
                {
                    if (sinkPercent <= minSinkPercent) fog.maxDist = 0f;
                    else fog.maxDist = (sinkPercent - minSinkPercent) * maxFogDistance;
                    ec.UpdateFog();
                }

                if (sinkPercent < minSinkPercent) overrider.SetHeight(entity.InternalHeight * minSinkPercent);
                else overrider.SetHeight(entity.InternalHeight * sinkPercent);

                yield return null;
            }

            overrider.SetBlinded(true);
            overrider.SetInBounds(false);
            overrider.SetVisible(false);
            canvases[0].gameObject.SetActive(true);
            canvases[1].gameObject.SetActive(true);

            UnpressImmediately();

            /*if (fog != null)
            {
                fog.maxDist = 0f;
                ec.UpdateFog();
            }*/

            entering = false;
            hidingTime = hidingCooldown;

            yield break;
        }

        private void UpdateText()
        {
            if (canvasText != null && hidingTime >= 0f) canvasText.text = string.Format("Adv_Safety_Trapdoor_Time_Remains_In".Localize(), (int)hidingTime + 1,
                KeyBindingsManager.Keys["exit_from_trapdoor"].Button.ToString());
        }

        public bool CatchEntity(Entity entity)
        {
            EntityOverrider overrider = new EntityOverrider();

            if (entity.Override(overrider))
            {
                this.overrider = overrider;
                overrider.SetFrozen(true);
                overrider.SetGrounded(true);
                overrider.SetInteractionState(false);

                entity.Teleport(transform.position);

                hidingTime = hidingCooldown;

                StartCoroutine(Entering(entity));

                OnPress();

                return true;
            }

            return false;
        }

        protected override bool IsPressable(Entity target)
        {
            return false;
        }

        public class InteractionObject : MonoBehaviour, IClickable<int>
        {

            public SafetyTrapdoor trapdoor;

            public void Clicked(int player)
            {
                if (!ClickableHidden())
                {
                    Entity entity = Singleton<CoreGameManager>.Instance.GetPlayer(0).GetComponent<Entity>();
                    trapdoor.CatchEntity(entity);
                    PlayerInteractionController.Instance.SetGameTip(player);
                }
            }

            public bool ClickableHidden()
            {
                return trapdoor.cooldownTime > 0f;
            }

            public bool ClickableRequiresNormalHeight()
            {
                return true;
            }

            public void ClickableSighted(int player)
            {
                if (!ClickableHidden()) PlayerInteractionController.Instance.SetGameTip(player, "Adv_Tip_SafetyTrapdoor");
            }

            public void ClickableUnsighted(int player)
            {
                PlayerInteractionController.Instance.SetGameTip(player);
            }
        }

    }
}
