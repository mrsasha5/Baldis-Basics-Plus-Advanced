﻿using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Components;
using BaldisBasicsPlusAdvanced.Game.Systems.BaseControllers;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.SaveSystem;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.PlusExtensions;
using System.Collections;
using UnityEngine;
namespace BaldisBasicsPlusAdvanced.Game.Systems.Controllers
{
    public class InflatableBalloonController : BaseController
    {
        private bool active;

        private object zeroMoveMod;

        private MovementModifier moveMod;

        private EntityOverrider entityOverrider;

        private SpriteRenderer renderer;

        private AudioManager audMan;

        private bool updateAllowed;

        public override int MaxCount => 1;

        public override bool Initializable {
            get {
                entityOverrider = new EntityOverrider();
                return entity.Override(entityOverrider);
            }
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            entity.StartCoroutine(Inflate());
            moveMod = new MovementModifier(Vector3.zero, 1f);
            entity.ExternalActivity.moveMods.Add(moveMod);

            if (owner == ControllerOwner.Player)
            {
                zeroMoveMod = new ValueModifier(0f, 0f);
                pm.GetMovementStatModifier().AddModifier("walkSpeed", (ValueModifier)zeroMoveMod);
                pm.GetMovementStatModifier().AddModifier("runSpeed", (ValueModifier)zeroMoveMod);
            } else
            {
                zeroMoveMod = new MovementModifier(Vector3.zero, 0f);
                entity.ExternalActivity.moveMods.Add((MovementModifier)zeroMoveMod);
            }
        }

        public override void VirtualUpdate()
        {
            if (!updateAllowed) return;
            base.VirtualUpdate();
            if (active)
            {
                moveMod.movementAddend = entity.transform.forward * 20f;
            } else
            {
                moveMod.movementAddend = Vector3.zero;
            }

            if (Input.GetKeyDown(OptionsDataManager.KeyBindings["balloon_pop_action"].Button))
            {
                SetToDestroy();
            }
        }

        public override void OnPostDestroying()
        {
            base.OnPostDestroying();
            GameObject.Destroy(renderer.transform.parent.gameObject);
            entityOverrider.Release();

            entity.ExternalActivity.moveMods.Remove(moveMod);

            if (owner == ControllerOwner.Player)
            {
                PlayerInteractionController.Instance.SetIgnorePlayerEntitiesInteraction(false);
                //PlayerInteractionController.Instance.SetPlayerClick(true);

                pm.GetMovementStatModifier().RemoveModifier((ValueModifier)zeroMoveMod);
                pm.GetMovementStatModifier().RemoveModifier((ValueModifier)zeroMoveMod);
            }
            else
            {
                entity.ExternalActivity.moveMods.Remove((MovementModifier)zeroMoveMod);
            }
            audMan.PlaySingle(AssetsStorage.sounds["pop"]);
            GameObject.Destroy(audMan.gameObject, 3f);
        }

        private IEnumerator Inflate()
        {
            float size = 0.15f;
            float speed = 1f;

            renderer = ObjectsCreator.CreateSpriteRendererBase(AssetsStorage.sprites["balloon_orange"], true);
            renderer.transform.parent.SetParent(entity.transform, false);

            //Physics.Raycast(entity.transform.position, entity.transform.forward, out RaycastHit hit, float.PositiveInfinity,
               // LayersHelper.ignorableCollidableObjects, QueryTriggerInteraction.Ignore);

            renderer.transform.localPosition = Vector3.forward * 4f + Vector3.up * -3.4f + Vector3.right * 0.3f; //* (hit.distance >= 4f ? 4f : 0f) + Vector3.up * -3.4f + Vector3.right * 0.3f;
            renderer.transform.localScale = Vector3.one * size;
            if (owner == ControllerOwner.Player) renderer.gameObject.layer = LayersHelper.takenBalloonLayer;

            audMan = ObjectsCreator.CreatePropagatedAudMan(entity.transform.position);
            audMan.transform.SetParent(entity.transform);
            audMan.PlaySingle(AssetsStorage.sounds["adv_balloon_inflation"]);
            
            while (size < 1f)
            {
                if (renderer == null) break;

                size += Time.deltaTime * ec.EnvironmentTimeScale * speed;
                if (size > 0.5f)
                {
                    speed -= Time.deltaTime * ec.EnvironmentTimeScale * 1.1f;

                    if (speed <= 0.047f) speed = 0.047f;
                }

                renderer.transform.localScale = Vector3.one * size;

                if (size > 1f)
                {
                    size = 1f;
                }
                
                yield return null;
            }

            if (owner == ControllerOwner.Player)
            {
                PlayerInteractionController.Instance.SetIgnorePlayerEntitiesInteraction(true);
                //PlayerInteractionController.Instance.SetPlayerClick(false);
                renderer.gameObject.layer = 0;
            }
            
            entityOverrider.SetHeight(8f);
            entityOverrider.SetGrounded(false);

            active = true;
            updateAllowed = true;
            if (renderer != null) renderer.transform.localPosition = Vector3.zero;

            yield break;
        }

    }
}
